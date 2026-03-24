using System.Net.Sockets;
using System.Net;

namespace Servidor
{

    // este servidor solo envia datos o recibe datos, no calcula nada
    internal class Program
    {
        
        // variables publicas

        public class Server
        {
            public bool run = true;
            public int puerto_servidor;
        }
        static void menu_principal()
        {
            Console.WriteLine("0: iniciar programa");
            // al iniciar el programa se crea la BD y se hacen si es necesario los inserts
            Console.WriteLine("1: Cerrar programa");

        }

        static void menu_ip()
        {
            Console.WriteLine("0: autodetectar_ip");
            Console.WriteLine("1: ip manual");
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            int puerto_servidor_backend = 1000; // variables para cada puerto 
            int puerto_servidor_frontend = 1001;

            int try_except = 0; // variable que sera 0 si todo es correcto en el try catch, 1 en otro caso

            menu_principal();

            try_except = 1;
            Server server = new Server(); // creamos el servidor arriba

            while (try_except == 1) // bucle para que si hay un try except no falle nada
            {
                try
                {
                    string opcion_menu = Console.ReadLine(); // leemos la opcion 

                    int opcion = Int32.Parse(opcion_menu);

                    while (opcion < 0 || opcion > 1)
                    {
                        menu_principal();
                        Console.Write("Introduce un numero entre 0 y 1");
                        opcion_menu = Console.ReadLine();

                        opcion = Int32.Parse(opcion_menu);
                    }


                    if (opcion == 0)
                    {
                        server = new Server();

                        server.run = true;

                        while (server.run == true) // mientras este conectado el servidor 
                        {


                            // esto es para detectar cuando se cierra la ventana se cierre el servidor 
                            Console.CancelKeyPress += (sender, e) =>
                            {

                                 Console.WriteLine("\n[!] Detectado cierre de ventana. Apagando servidor...");
                                 server.run = false;

                            };

                            server.puerto_servidor = puerto_servidor_backend; // ponemos el servidor en este puerto 

                            menu_ip(); // tenemos un menu_ip, por defecto vamsoa  dejarlo en 0

                            int opcion_ip = 0;
                            string ip;
                            // aqui lo que hacemos es que si el try except peta entonces que se introduzca manualmente el ip
                            try
                            {

                                // calcular la ip de manera automatica 
                                string hostName = Dns.GetHostName();

                                IPAddress[] localIPs = Dns.GetHostAddresses(hostName);
                                foreach (IPAddress ipaddress in localIPs)
                                {
                                    // Filtra para obtener solo direcciones IPv4
                                    if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                                    {
                                        ip = ipaddress.ToString();
                                    }
                                }
                            }
                            catch
                            { // si entra en el catch que se introduzca manualmente el ip 
                                Console.Write("introduce la ip: ");
                                ip = Console.ReadLine();
                            }
                            

                        }
                    }
                    else
                    {
                        server.run = false;
                    }
                    try_except = 0; // ponemos a 0 para salir de aqui 
                }
                catch
                {
                    Console.WriteLine("Introduce un numero correcto");
                }
            }
            
        }
    }
}
