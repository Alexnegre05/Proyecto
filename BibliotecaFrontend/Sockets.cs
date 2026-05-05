using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaFrontend
{
    public class Sockets
    {
        static public string calcular_ip_automatico()
        {
            // calcular la ip de manera automatica 
            string hostName = Dns.GetHostName();
            string ip = "";



            IPAddress[] localIPs = Dns.GetHostAddresses(hostName);

            foreach (IPAddress ipaddress in localIPs)
            {
                // Filtra para obtener solo direcciones IPv4
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = ipaddress.ToString();
                }

            }

            return ip;
        }


        // funcion para crear el socket 
        public static Socket crear_frontend_socket(int puerto)
        {
            string ip;

            #if DEBUG
                // DeviceInfo.DeviceType == Virtual significa emulador
                ip = DeviceInfo.DeviceType == DeviceType.Virtual
                ? "10.0.2.2"           // Emulador Android
                : calcular_ip_automatico(); // Móvil físico
            #else
                ip = calcular_ip_automatico();
#           endif

                IPAddress address = IPAddress.Parse("192.168.111.37");
                IPEndPoint endpoint = new IPEndPoint(address, puerto);
                Socket frontend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                frontend_socket.Connect(endpoint);
                return frontend_socket;
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
