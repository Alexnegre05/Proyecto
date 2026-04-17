using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Maui.Devices.Sensors; // para los sensores
using Microsoft.Maui.ApplicationModel; // para qeu sepa que es de maui 
using Microsoft.Maui.Graphics; // para Color
using Microsoft.Maui.Controls; // para label y colectionview
namespace BibliotecaFrontend
{
    public class BibliotecaFrontend
    {


        public class InfoLinea
        {
            public string Nombre { get; set; }
            public Color Color { get; set; }
        }


        // Diccionario de colores
        // aqui tenemos un diccionario que relaciona una linea con su color correspondiente
        public static Dictionary<string, Color> colores = new Dictionary<string, Color>
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



        // funciones de grados

        // funcion que pasa de grados a radianes
        public static double grados_a_radianes(double grados)
        {
            double radianes = (grados * Math.PI) / 180;

            return radianes;
        }


        // funciones de sockets


        // funcion para crear el socket 
        public static Socket crear_frontend_socket(int puerto)
        {
            // ip que se usa para conectarse en el movil
            string ip = "192.168.111.24";

            // Creamos el socket 
            IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
            IPEndPoint endpoint = new IPEndPoint(address, puerto); // el puerto es el 1000
            Socket frontend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            frontend_socket.Connect(endpoint); // es lo mimso que connect pero preparada para el async 

            return frontend_socket;

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


        // cosas de mainthread y calculo de estación cercana 


        // funcion que se encarga de el main thread de poner notas
        public static void mainthreadPonerNotas(string estacion, List<InfoLinea> paradas, Label LabelEstacion, CollectionView LineasView, Border BordePrincipal, Button guardar, Label Titulo, Button BtnFlecha, Border ContenedorIncidencias)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelEstacion.Text = "Estacion: " + estacion;
                LineasView.ItemsSource = paradas;
                // lineas view es como el id de collectionview y sirve para poder tenerlo en el backend y
                // modificar sus atributos igual a label estación que es el nombre de la estacion 


                // el selectionchanges es que cuando se cambie la estacion(el selector) que se cambie el nombre
                // el += no es un a= a + 1 sino aqui
                // es un añade también esta función a la lista de cosas por hacer cuando ocurra el evento SelectionChanged

                // sender el objeto que disparo el evento y la e es el objeto que tiene los datos sobre el evento

                LineasView.SelectionChanged += (s, e) =>
                {
                    //  Obtenemos el elemento seleccionado actual, el que es el primero y lo ponemos como dinamico
                    // lo tenemos que pasar con el as a InfoLinea

                    InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;
                    // dynamic salta la comprovacion de a la hora de crear un objeto 

                    if (seleccion != null) // miramos que no sea nulo
                    {
                        // Cambiamos el texto
                        LabelEstacion.Text = $"Estación: {estacion} ({seleccion.Nombre})";


                        // Usamos el color que viene guardado en el objeto seleccionado
                        LabelEstacion.TextColor = (Color)seleccion.Color;

                        BordePrincipal.Background = (Color)seleccion.Color;

                        guardar.Background = (Color)seleccion.Color;
                        Titulo.TextColor = Colors.White;

                        LineasView.IsVisible = false;
                        BtnFlecha.Text = "▼";

                        LineasView.SelectedItem = null;
                        // esto es para que el selector se pueda volver a clicar una segunda vez
                        // si esta en la version resumida ya que si es la misma letra no detectara el evento 


                        // si no hay nada seleccionado, que no salga la posibilidad de poner notas
                        if (seleccion == null)
                        {
                            ContenedorIncidencias.IsVisible = false;// para que se oculte 
                        }
                        else // caso contrario
                        {
                            ContenedorIncidencias.IsVisible = true;
                        }

                        
                    }
                };
            });

        }



    
    }
}
