using System.Net.Sockets;
using System.Net;
using System.Text;



namespace Frontend.Pages;




public partial class PonerNotasPage : ContentPage
{

    // aqui tenemos un diccionario que relaciona una linea con su color correspondiente,
    // esta aqui arriba para poder reutilizarlo


    public Dictionary<string, Color> colores = new Dictionary<string, Color>
    {
                { "R1", Color.FromArgb("#4499D4") },
                { "R2", Color.FromArgb("#009900") },
                { "R2N", Color.FromArgb("#99C83E") },
                { "R2S", Color.FromArgb("#00642E") },
                { "R3", Color.FromArgb("#FF131A") },
                { "R4", Color.FromArgb("#FF9221") },
                { "R7", Color.FromArgb("#BD7DB5") },
                { "R8", Color.FromArgb("#9B1987") },
                { "RG1", Color.FromArgb("#007DC3") },
                { "R10", Color.FromArgb("#930030") },
                { "R11", Color.FromArgb("#0064A5") },
                { "R12", Color.FromArgb("#FFDC00") },
                { "R13", Color.FromArgb("#E52E87") },
                { "R14", Color.FromArgb("#675199") },
                { "R15", Color.FromArgb("#9A8A76") },
                { "R16", Color.FromArgb("#AF0036") },
                { "R17", Color.FromArgb("#E97300") }
    };

    public PonerNotasPage()
	{
		InitializeComponent();
	}

    // la funcion onAppearing sirve para indicarle a la aplicacion que cuando se abra la pagina  se llame a la funcion que quieras
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    // funcion que pasa de grados a radianes
    public static double grados_a_radianes(double grados)
    {
        double radianes = (grados * Math.PI) / 180;

        return radianes;
    }


    // funcion para enviar el x,y,z de el movil 

    public static async Task send_xyz(Socket frontend_socket)
    {
        // con geolocation sacamos el x,y,z de el movil, el await y el async es porque la funcion es asincrona 
        // esto le dice cuanta precision queremos que haya GeolocationAccuracy.Medium
        // usamos medium para no consumir muchos datos de el movil 

        // esto es latitud, longitud... hay que pasarlo a x,y,z
        Location location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

        double x = location.Longitude;
        double y = location.Latitude;
        double z = location.Altitude ?? 0;
        // la z puede ser 0 y ya esta en metros pero en 2D, se usa para sumarle a el radio ya que es la altura al nivel de el mar 


        // radio de la tierra
        float R = 6371.00877f * 1000;

        // lo pasamos a radianes
        x = grados_a_radianes(x);
        y = grados_a_radianes(y);


        // lo pasamos a metros, misma formula que el backend pero sumandole la altura de el mar 
        double final_x = (R + z) * Math.Cos(y) * Math.Cos(x);
        double final_y = (R + z) * Math.Cos(y) * Math.Sin(x);
        double final_z = (R + z) * Math.Sin(y);

        // enviamos el x,y,z a el backend

        send_parameter_xyz(final_x, frontend_socket);
        send_parameter_xyz(final_y, frontend_socket);
        send_parameter_xyz(final_z, frontend_socket);
    }

    // funciones que envian un numero o un xyz 
    public static void send_parameter_xyz(double var, Socket frontend_socket)
    {
        byte[] bytes = new byte[sizeof(double)];
        bytes = BitConverter.GetBytes(var);
        frontend_socket.Send(bytes);
    }

    public static void send_num(int num, Socket frontend_socket)
    {
        byte[] bytes = new byte[sizeof(int)];
        bytes = BitConverter.GetBytes(num);
        frontend_socket.Send(bytes);
    }


    public static string recibir_texto(Socket frontend_socket)
    {
        byte[] data = new byte[sizeof(int)];

        // 1. LEER SOLO UNA VEZ el tamaño
        int bytesRecibidos = frontend_socket.Receive(data);

        // Si por red no llegaron los 4 bytes de golpe, completamos la lectura
        while (bytesRecibidos < 4)
        {
            bytesRecibidos += frontend_socket.Receive(data, bytesRecibidos, 4 - bytesRecibidos, SocketFlags.None);

        }

        int num = BitConverter.ToInt32(data);

        // 2. Leer la palabra usando ese tamaño
        byte[] palabra = new byte[num];
        int textoLeido = 0;
        while (textoLeido < num)
        {
            textoLeido += frontend_socket.Receive(palabra, textoLeido, num - textoLeido, SocketFlags.None);
        }

        return Encoding.UTF8.GetString(palabra);
    }

    public static int recibir_numero(Socket frontend_socket)
    {
        byte[] data = new byte[sizeof(int)]; // 4 bytes
        int leidos = 0;

        // Forzamos a leer hasta que tengamos los 4 bytes exactos del int
        while (leidos < 4)
        {
            int r = frontend_socket.Receive(data, leidos, 4 - leidos, SocketFlags.None);
            if (r <= 0) return -1; // O maneja el error si la conexión se corta
            leidos += r;
        }
        int num = BitConverter.ToInt32(data);
        return num;
    }

    // COMO MOSTRAR TEXTO AQUI CONSOLE WRITELINE

    // await Shell.Current.DisplayAlert("Estación Encontrada", estacion, "Cerrar"); poner funcion como async 








    // funcion para saber el nombre de la estacion mas cercana
    // usamos de nuevo el async y el await pero esta vez en la conexion de el socket 
    private async void EstacionCercana()
    {

        try
        {
            // ip que se usa para conectarse en el movil
            string ip = "172.23.192.1";

            // Creamos el socket 
            IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
            IPEndPoint endpoint = new IPEndPoint(address, 1000); // el puerto es el 1000
            Socket frontend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            frontend_socket.Connect(endpoint); // es lo mimso que connect pero preparada para el async 

            Console.WriteLine("Conectado");

            // enviamos un 1 para decir que va a recibir algo de poner notas 

            send_num(1, frontend_socket);
            // enviamos el xyz a el servidor
            await send_xyz(frontend_socket);

            string estacion = recibir_texto(frontend_socket);


            // aqui es donde se cambia el nombre, el MainThread es el que se encarga de dibujar por pantalla
            // le decimos a ese hilo que se invoque y que cambie el texto 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelEstacion.Text = "Estacion: " + estacion;
            });

            int num = recibir_numero(frontend_socket); // numero que nos dice cuantas paradas hay 

            List<object> lineas = new List<object>();
            for (int i = 0; i < num; i++)
            {
                string parada = recibir_texto(frontend_socket);

                lineas.Add(new
                {
                    Nombre = parada,
                    Color = colores.GetValueOrDefault(parada, Colors.Gray)
                });
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LineasView.ItemsSource = lineas;
            });




            LineasView.SelectionChanged += (s, e) =>
            {
                // 1. Obtenemos el elemento seleccionado
                var seleccion = e.CurrentSelection.FirstOrDefault() as dynamic;

                if (seleccion != null)
                {
                    // 2. Cambiamos el texto
                    LabelEstacion.Text = $"Estación: {seleccion.Nombre}";

                    // 3. CAMBIAMOS EL COLOR (Esta es la parte que te falta)
                    // Usamos el color que viene guardado en el objeto seleccionado
                    LabelEstacion.TextColor = (Color)seleccion.Color;
                }
            };


            // cerramos el socket
            frontend_socket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }
}