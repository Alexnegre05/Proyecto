using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Frontend.Pages;

public partial class LeerNotasPage : ContentPage
{

    public class InfoLinea
    {
        public string Nombre { get; set; }
        public Color Color { get; set; }
    }


    public LeerNotasPage()
	{
		InitializeComponent();
	}


    // funcion que pasa de grados a radianes
    public static double grados_a_radianes(double grados)
    {
        double radianes = (grados * Math.PI) / 180;

        return radianes;
    }




    Socket frontend_socket;

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


    protected override void OnAppearing()
    {
        base.OnAppearing();
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    protected override void OnDisappearing()
    {

        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);



        // y cerramos los sockets
        frontend_socket.Dispose();
        frontend_socket.Close();
    }

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


    public static void enviar_texto(string text, Socket frontend_socket)
    {


        byte[] bytes = Encoding.UTF8.GetBytes(text);
        byte[] lenght = BitConverter.GetBytes(bytes.Length);
        frontend_socket.Send(lenght);
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


    // funcion para crear el socket 
    private static Socket crear_frontend_socket(int puerto)
    {
        // ip que se usa para conectarse en el movil
        string ip = "192.168.111.22";

        // Creamos el socket 
        IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
        IPEndPoint endpoint = new IPEndPoint(address, puerto); // el puerto es el 1000
        Socket frontend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        frontend_socket.Connect(endpoint); // es lo mimso que connect pero preparada para el async 

        return frontend_socket;

    }


    private void OnFlechaClicked(object sender, EventArgs e)
    {
        // Invierte la visibilidad: si está abierta se cierra, si está cerrada se abre,
        // esta propiedad es la que indica si la flecha muestra todas las estaciones o no lo muestra
        // se indica con un booleano 

        if (LineasView.IsVisible == true)
        {
            LineasView.IsVisible = false;
        }
        else
        {
            LineasView.IsVisible = true;
        }

        // Cambia la flecha según el estado 
        BtnFlecha.Text = LineasView.IsVisible ? "▲" : "▼";


    }
   


    private void mainthread(string estacion, List<InfoLinea> paradas)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelEstacion.Text = "Estacion: " + estacion;
            LineasView.ItemsSource = paradas;
        });

    }

    // repetimos la funcion de estacion cercana para que te salga por defecto la estacion mas cercana 

    private async void EstacionCercana()
    {

        try
        {

            frontend_socket = crear_frontend_socket(1000);

            // enviamos un 2 para decir que va a recibir algo de enviar notas 

            send_num(2, frontend_socket);

            // enviamos otro 1 para decirle que queremos que nos de la opcion de la estacion mas cercana en el backend
            send_num(1, frontend_socket);


            // enviamos el xyz a el servidor
            await send_xyz(frontend_socket);

            string estacion = recibir_texto(frontend_socket);




            int num = recibir_numero(frontend_socket); // numero que nos dice cuantas paradas hay 

            List<InfoLinea> paradas = new List<InfoLinea>();
            // Infolinea es una clase donde se guardan los colores y los nombres de las lineas

            // recorremos toda la lista
            for (int i = 0; i < num; i = i + 1)
            {
                string linea = recibir_texto(frontend_socket);
                // obtenemos una de las lineas de la estacion y lo añadimos a paradas

                //esto es un objeto de la clase InfoLinea que se añade a las paradas
                InfoLinea linea_actual = new InfoLinea
                {
                    Nombre = linea,
                    Color = colores.GetValueOrDefault(linea, Colors.Gray)
                };


                paradas.Add(linea_actual);

                //new Nombre = linea, Color = colores.GetValueOrDefault(linea, Colors.Gray)

            }


            // aqui es donde se cambia el nombre, y todo el tema de el color.

            // El MainThread es el que se encarga de dibujar por pantalla
            // le decimos a ese hilo que se invoque y que cambie el texto y que las lineas son las paradas que hemos cogido

            mainthread(estacion, paradas);




        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }
}