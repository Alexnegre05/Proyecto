using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace BibliotecaBackend
{
    public class BibliotecaBackend
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
        // funciones de ip

        static public string calculo_ip()
        {


            string ip = ""; // string de la ip de el usuario 

            int ip_automatica = -1;
            int ip_correcta = 0;

            FileStream file = new FileStream("fichero_configuracion.txt", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(file, Encoding.UTF8);

            string linea = reader.ReadLine(); // con esto cogemos la linea, si es null sabemos de que ha acabado
            int count = 0; // 0 si es la primera linea que tenemos que leer  1 en caso de ser la siguiente

            while (linea != null)
            {
                if (count == 0) // si es la primera linea
                {
                    string[] parts = linea.Split("="); // separamos el = para solo sacar el dato que queremos

                    string tipo_ip = parts[1];
                    tipo_ip = tipo_ip.Trim().ToLower(); // quita posibles espacios y que todo este en minusculas

                    if (tipo_ip == "automatico")
                    {
                        ip_automatica = 1; // lo dejamos en 1 para indicar que es automatico
                    }
                    else if (tipo_ip == "manual")
                    {
                        ip_automatica = 0;
                    }
                    else
                    {
                        Console.WriteLine("Siga las instrucciones del fichero, tipo_ip mal introducida");
                    }
                }
                else if (count == 1)
                {
                    string[] parts = linea.Split('=');

                    ip = parts[1]; // sacamos la ip
                }

                linea = reader.ReadLine(); // volvemos a leer la siguiente linea
                count = count + 1; // sumamos 1 al contador
            }

            reader.Close();
            file.Close();


            if (ip_automatica == 1)
            {
                ip = calcular_ip_automatico();
                ip_automatica = 1;
                Console.WriteLine("IP detectada correctamente");



            }
            else if (ip_automatica == 0)// ip no automatica
            {

                // comprovamos la ip
                while (ip_correcta == 0)
                {
                    // si entra en el catch que se introduzca manualmente el ip 


                    string[] parts = ip.Split(".");

                    while (parts.Length != 4) // aqui comprobamos que el formato sea A.B.C.D y no haya ningun error
                    {
                        Console.Write("introduce la ip correctamente: ");
                        ip = Console.ReadLine();
                        parts = ip.Split(".");
                    }


                    ip_correcta = check_ip_manual_numeros(parts);
                    // funcion que mira que haya 4 numeros puestos en la ip manual de 3 digitos, si devuelve 0 entonces esta mal y se repite el bucle, 1 no lo hace

                    if (ip_correcta == 0)
                    {
                        Console.WriteLine("introduce la ip correctamente en el formato A.B.C.D\n donde las letras son numeros de 3 digitos");
                        Console.Write("ip: ");
                        ip = Console.ReadLine();
                    }

                }
            }
            else
            {
                Console.WriteLine("no deberias estar aqui");
            }

            return ip;
        }


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



        static public int check_ip_manual_numeros(string[] parts)
        {
            // usamos try parse para que si falla salte un error nos pide el out, no haremos nada con el numero 
            int numero;
            bool esValido;
            esValido = Int32.TryParse(parts[0], out numero);

            if (esValido == true && parts[0].Length <= 3) // si es valido comprovamos el segundo numero y asi hasta los 4
            {
                esValido = Int32.TryParse(parts[1], out numero);

                if (esValido == true && parts[1].Length <= 3)
                {
                    esValido = Int32.TryParse(parts[2], out numero);

                    if (esValido == true && parts[2].Length <= 3)
                    {
                        esValido = Int32.TryParse(parts[3], out numero);

                        if (esValido == true && parts[3].Length <= 3)
                        {
                            // aqui si que la ip es correcta y por eso devolvemos 1
                            return 1;
                        }
                        else
                        {
                            return 0; // devolvemos 0 para que se repita el bucle
                        }
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }


            }
            else
            {
                return 0;
            }
        }


    }



}
