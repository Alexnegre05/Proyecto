using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.General;
using System.Net.Sockets;
namespace BibliotecaBackend
{
    public class Enlaces
    {
        public static void inserts_enlaces(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("Estamos en enlaces");
            Console.WriteLine("Menu");
            Console.WriteLine("0 Salir");
            Console.WriteLine("1 Enviar todas estaciones");
            Console.WriteLine("2 Recibir estaciones origen y destino");
            Console.WriteLine("3 Enviar ruta optima");

            int opcion = -1;

            while(opcion != 0)
            {
                opcion = recibir_numero(backend_service_socket);

                if (opcion == 1)
                {
                    List<string> nombres = context.Estaciones.Select(e => e.nombre).ToList(); // sacamos todas las estaciones


                    // enviamos cuantos nombres va a recibir
                    enviar_numero(nombres.Count, backend_service_socket);

                    // bucle que recorre toda la lista
                    for(int i = 0; i < nombres.Count; i = i + 1)
                    {
                        enviar_texto(nombres[i], backend_service_socket); // enviamos todos los textos de todas las estaciones
                    }
                }
            }
        }
    }
}
