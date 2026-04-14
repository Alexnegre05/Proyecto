using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Maui.Devices.Sensors;
namespace BibliotecaFrontend
{
    public class BibliotecaFrontend
    {

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
            string ip = "192.168.111.22";

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





    }
}
