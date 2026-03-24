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
        static void menu()
        {
            Console.WriteLine("0: iniciar programa");
            // al iniciar el programa se crea la BD y se hacen si es necesario los inserts
            Console.WriteLine("1: Cerrar programa");

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            int puerto_servidor_backend = 1000;
            int puerto_servidor_frontend = 1001;

            int try_except = 0; // variable que sera 0 si todo es correcto en el try catch, 1 en otro caso

            menu();

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
                        Console.WriteLine("Introduce un numero entre 0 y 1");
                    }


                    if (opcion == 0)
                    {
                        server = new Server();

                        server.run = true;
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
