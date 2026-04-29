using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.General;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
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
            Console.WriteLine("2 Recibir estaciones origen y destino y enviar ruta");
            

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
                else if (opcion == 2) // recibimos tanto la estacion origen como destino
                {
                    string origen = recibir_texto(backend_service_socket);

                    string destino = recibir_texto(backend_service_socket);

                    Estacion estacion_origen = context.Estaciones.FirstOrDefault(e => e.nombre == origen);

                    Estacion estacion_destino = context.Estaciones.FirstOrDefault(e => e.nombre == destino); // sacamos la estacion de origen y de destino

                    if (estacion_origen == null || estacion_destino == null)
                    {
                        Console.WriteLine("No deberia estar aqui");
                    }
                    else
                    {
                        // sacamos la lista de todas las paradas que tiene cada estación origen y destino 
                        List<Paradas> paradasOrigen = context.Paradas.Where(p => p.EstacionId == estacion_origen.Id).ToList();

                        List<Paradas> paradasDestino = context.Paradas.Where(p => p.EstacionId == estacion_destino.Id).ToList();



                        List<Paradas> Todasparadas = context.Paradas.Include(p => p.Estacion).Include(p => p.Linea).ThenInclude(l => l.ListaParadas).ToList();
                        List<Enlace> Todosenlaces = context.Enlaces.ToList(); // sacamos todas las paradas con todos sus enlaces


                        HashSet<int> visitados = new HashSet<int>(); // conjunto donde guardamos que id ya hemos pasado(paradas) en el algoritmo
                                                                     // es un conjunto porque asi no se repiten elementos 

                        Dictionary<int, int> distancia = new Dictionary<int, int>();
                        // vamos a usar un diccionario que sea Id -> num(distancia entre estaciones),


                        Dictionary<int, int?> previo = new Dictionary<int, int?>(); // un diccionario donde guardamos id parada actual + id de parada anterior
                                                                                    // como puede ser que sea la primera parada usamos un ? 


                        List<Paradas> ruta = new List<Paradas>(); // aqui guardamos la ruta mas optima 

                        // de momento como no la sabemos pondremos infinito, en nuestro caso int.maxvalue(el entero mas grande)
                        for (int i = 0; i < Todasparadas.Count; i = i + 1)
                        {

                            distancia[Todasparadas[i].Id] = int.MaxValue;
                            
                                
                        }

                        // ponemos las paradas de origen como distancia = 0

                        for (int i = 0; i < paradasOrigen.Count; i = i + 1)
                        {
                            distancia[paradasOrigen[i].Id] = 0;
                        }

                        foreach (var p in Todasparadas)
                        {
                            previo[p.Id] = null;
                        }

                        // bucle que mira si las paradas que hemos visitado son menores a el numero total de paradas, si es asi aun hay que recorrer el grafo

                        while (visitados.Count < Todasparadas.Count)
                        {
                            // sacamos la parada actual que estamos mirando, en concreto queremos saber el valor de el coste en el diccionario distancia
                            // [id, coste]

                            // visitados.Contains(d.Key) == false esto mira si no has visitado esta parada en concreto
                            int actual = distancia
                            .Where(d => visitados.Contains(d.Key) == false)
                            .OrderBy(d => d.Value) // ordena por distancia 
                            .First().Key; // extraemos solo el id


                            // sacamos los enlaces de todas las siguentes paradas poniendo como anterior a la actual en el where
                            List<Enlace> vecinos = Todosenlaces.Where(e => e.AnteriorParadaId == actual || e.SiguienteParadaId == actual).ToList();

                            // vamos a mirar vecino por vecino 
                            for (int i = 0; i < vecinos.Count; i = i + 1)
                            {
                                // cojemos uno de estos vecinos
                                Enlace siguiente = vecinos[i];

                                // miramos que parada es

                                Paradas paradaDestino = Todasparadas.First(p => p.Id == siguiente.SiguienteParadaId);

                                int costo_total = paradaDestino.Estacion.Incidencias.Sum(i => i.gravedad + i.nota_Incidencias.Count) + vecinos[i].Costo + distancia[actual];
                                // sacamos como costo total el numero de notas de incidencias que hay + costo entre cada trayecto y la distancia actual que ya hemos recorrido

                                // comparamos si el costo total es menor a la distancia de el siguiente

                                if (costo_total < distancia[siguiente.SiguienteParadaId])
                                {
                                    // si lo es la distancia de el siguiente parada sera el costo total
                                    distancia[siguiente.SiguienteParadaId] = costo_total;
                                    // y la estacion previa pasa a ser la actual 
                                    previo[siguiente.SiguienteParadaId] = actual;
                                }

                            }

                            visitados.Add(actual); // añadimos la parada en la lista de paradas visitadas





                        }

                        // con esto tenemos la parada actual 
                        Paradas destinoParada = paradasDestino.OrderBy(p => distancia[p.Id]).First(); // ordenamos por id 

                        int? nodo = destinoParada.Id;

                        while (nodo != null)
                        {
                            // añadimos la parada en la ruta, miramos si la lista de todas las paradas el id coincide con el id de el nodo destino parada (Id)
                            ruta.Insert(0, Todasparadas.First(p => p.Id == nodo));
                            // vamos a por el sigiuente nodo 

                            
                            nodo = previo[nodo.Value];
                        }

                        enviar_numero(ruta.Count, backend_service_socket); // enviamos la parada como estacion.nombre y linea por separado
                        for (int i = 0; i < ruta.Count; i = i + 1)
                        {
                            Console.WriteLine(ruta[i].Estacion.nombre + ruta[i].Linea.nombre);

                            enviar_texto(ruta[i].Estacion.nombre, backend_service_socket);
                            enviar_texto(ruta[i].Linea.nombre, backend_service_socket);
                        }



                    }
                }
            }
        }
    }
}
