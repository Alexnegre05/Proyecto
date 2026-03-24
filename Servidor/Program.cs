using System.Net.Sockets;
using System.Net;
using static Servidor.Program;

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

        static void autodetectar_cierre(Server server)
        {

            // esto es para detectar cuando se cierra la ventana se cierre el servidor 
            Console.CancelKeyPress += (sender, e) =>
            {

                Console.WriteLine("\n[!] Detectado cierre de ventana. Apagando servidor...");
                server.run = false;
            };


        }

        static int leer_opcion_menu_principal()
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

            return opcion;
        }


        static string calcular_ip_automatico()
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



        static void check_ip_manual(string[] parts)
        {
            // usamos try parse para que si falla salte un error nos pide el out, no haremos nada con el numero 
            int numero;
            bool esValido;
            esValido = Int32.TryParse(parts[0], out numero);

            if (esValido == true && parts[0].Length == 3) // si es valido comprovamos el segundo numero y asi hasta los 4
            {
                esValido = Int32.TryParse(parts[1], out numero);

                if (esValido == true && parts[1].Length == 3)
                {
                    esValido = Int32.TryParse(parts[2], out numero);

                    if (esValido == true && parts[2].Length == 3)
                    {
                        esValido = Int32.TryParse(parts[3], out numero);

                        if (esValido == true && parts[3].Length == 3)
                        {

                        }
                        else
                        {
                            return; // devolvemos nada para que se salga antes de la funcion en casod e que ya falle, ahorramos tiempo
                        }
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    return;
                }


            }
            else
            {
                return;
            }
            // que se repita el bucle y que pete por ip mal puesta, que vuelve al except
        }




        // menu principal 
        static void Main(string[] args)
        {
           
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

                    int opcion = leer_opcion_menu_principal();

                    // backend
                    if (opcion == 0)
                    {
                        server = new Server();

                        server.run = true;

                        while (server.run == true) // mientras este conectado el servidor 
                        {

                            autodetectar_cierre(server);

                            server.puerto_servidor = puerto_servidor_backend; // ponemos el servidor en este puerto 

                            menu_ip(); // tenemos un menu_ip, por defecto vamos a dejarlo en 0

                            int opcion_ip = 0;

                            
        
                            string ip;
                            int ip_automatica = 1; 
                            // simplemente esta aqui para saber si la ip automatica falla, si falla ya no te volvera a entrar a la automatica


                            try_except = 1; // volvemos a hacer lo mismo pero para si sale mal lo de la ip
                            while (try_except == 1)
                            {
                                // aqui lo que hacemos es que si el try except peta entonces que se introduzca manualmente el ip
                                try
                                {
                                    if (ip_automatica == 1)
                                    {
                                        ip = calcular_ip_automatico();
                                        ip_automatica = 1;

                                        // aqui se enviara el socket a el backend
                                    }

                                }
                                catch
                                {
                                    ip_automatica = 0; // desactivamos el que se use la ip automatica 

                                    // si entra en el catch que se introduzca manualmente el ip 
                                    Console.Write("introduce la ip: ");
                                    ip = Console.ReadLine();

                                    string[] parts = ip.Split(".");

                                    while (parts.Length != 4) // aqui comprobamos que el formato sea A.B.C.D y no haya ningun error
                                    {
                                        Console.Write("introduce la ip correctamente: ");
                                        ip = Console.ReadLine();
                                        parts = ip.Split(".");
                                    }


                                    check_ip_manual(parts); // funcion que mira que haya 4 numeros puestos en la ip manual




                                }
                            }
                        
                        } // fin server run true
                    }

                    // frontend
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
