using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaBackend
{
    public class Sockets
    {


        static public Socket crear_backend_socket(string ip)
        {
            IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
            IPEndPoint endpoint = new IPEndPoint(address, 1000); // el puerto es el 1000
            Socket backend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // creamos el socket


            backend_socket.Bind(endpoint); // al usar bind el socket espera recibir que la ip sea de el formato que ha recibido 
                                           // ej si recibe 192.168.32.47 espera que se conecte usando 192.168.x.x o similar
            backend_socket.Listen(); // para que se escuche el socket

            return backend_socket;
        }


        // funciones de enviar/recibir cosas entre frontend y backend 

        // enviar texto
        public static void enviar_texto(string text, Socket backend_service_socket)
        {


            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] lenght = BitConverter.GetBytes(bytes.Length);
            backend_service_socket.Send(lenght);
            backend_service_socket.Send(bytes);
        }

        public static void enviar_numero(int num, Socket backend_service_socket)
        {


            byte[] bytes = BitConverter.GetBytes(num);
            backend_service_socket.Send(bytes);
        }


        public static string recibir_texto(Socket backend_socket)
        {
            byte[] data = new byte[sizeof(int)];

            // 1. LEER SOLO UNA VEZ el tamaño
            int bytesRecibidos = backend_socket.Receive(data);

            // Si por red no llegaron los 4 bytes de golpe, completamos la lectura
            while (bytesRecibidos < 4)
            {
                bytesRecibidos += backend_socket.Receive(data, bytesRecibidos, 4 - bytesRecibidos, SocketFlags.None);

            }

            int num = BitConverter.ToInt32(data);

            // 2. Leer la palabra usando ese tamaño
            byte[] palabra = new byte[num];
            int textoLeido = 0;
            while (textoLeido < num)
            {
                textoLeido += backend_socket.Receive(palabra, textoLeido, num - textoLeido, SocketFlags.None);
            }

            return Encoding.UTF8.GetString(palabra);
        }

        public static int recibir_numero(Socket backend_socket)
        {
            byte[] data = new byte[sizeof(int)];
            backend_socket.Receive(data);

            int num = BitConverter.ToInt32(data);

            return num;
        }

        public static double recibir_double(Socket backend_socket)
        {
            byte[] data = new byte[sizeof(double)];
            backend_socket.Receive(data);

            double num = BitConverter.ToDouble(data);

            return num;
        }
    }
}
