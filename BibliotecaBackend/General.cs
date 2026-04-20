using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.Sockets;

using Microsoft.EntityFrameworkCore;


namespace BibliotecaBackend
{

    // auqii tengo funciones de poner notas/leer...
    public class General
    {



        public static void poner_notas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("estamos en poner notas");

            Console.WriteLine("Hacemos un bucle donde tendra este menu");
            Console.WriteLine("0. salir");
            Console.WriteLine("1. enviar estacion cercana");
            Console.WriteLine("2 recibir nota");

            // como es un bucle while ponemos las variables afuera para no reservar memoria de mas
            int opcion = -1;

            double x;
            double y;
            double z;

            string estacion_cercana;

            List<Paradas> lista_paradas;
            List<string> paradas;
            Paradas parada_actual = null;

            while (opcion != 0)
            {
                // leemos la opcion 
                opcion = recibir_numero(backend_service_socket);


                if (opcion == 1) // dependiendo de la opcion enviamos una cosa u otra
                {


                    x = recibir_double(backend_service_socket);

                    y = recibir_double(backend_service_socket);

                    z = recibir_double(backend_service_socket);


                    // aqui buscamos qual es la estacion mas cercana, solo nuecesitamos el nombre
                    estacion_cercana = calcular_estacion_cercana(x, y, z, context);

                    Console.WriteLine(estacion_cercana);
                    enviar_texto(estacion_cercana, backend_service_socket);



                    // sacamos la parada con todas sus lineas

                    // el .include es para que no haya problemas con el tema de llamar a otras tablas
                    lista_paradas = context.Paradas
                    .Include(p => p.Linea)
                    .Include(p => p.Estacion)
                    .Where(p => p.Estacion != null &&
                                p.Estacion.nombre.Trim().ToLower() == estacion_cercana.Trim().ToLower()).ToList();
                    // el ToLower y trim es para que todos los nombres coincidan



                    paradas = lista_paradas.Select(p => p.Linea.nombre).Distinct().ToList();
                    //cogemos solo las lineas el nombre que tienen no su id Select(p => p.Linea.nombre)


                    // vamos a enviar en este orden las cosas, el numero de paradas y despues todas las paradas con formato R1,R2...
                    enviar_numero(paradas.Count, backend_service_socket);

                    for (int i = 0; i < paradas.Count; i = i + 1)
                    {
                        enviar_texto(paradas[i], backend_service_socket);
                    }
                }

                else if (opcion == 2)
                {
                    parada_actual = saber_parada_seleccionada_frontend(backend_service_socket, context, parada_actual);


                    // recibimos el titulo y la incidencia
                    string titulo_incidencia = recibir_texto(backend_service_socket);
                    string incidencia = recibir_texto(backend_service_socket);


                    if (parada_actual != null)
                    {


                        Incidencias nuevaIncidencia = new Incidencias // creamos una incidencia
                        {
                            fecha = DateTime.UtcNow, // la fecha es la actual, nos pide que sea UTC sino peta
                            gravedad = 1, // Valor por defecto
                            solucionado = false, // Valor por defecto
                            ParadaId = parada_actual.Id, // FK a Parada
                            Paradas = parada_actual               // Relación con Parada
                        };

                        context.Incidencias.Add(nuevaIncidencia);
                        context.SaveChanges();

                        // guardamos la nota de la incidencia
                        Nota_Incidencia nota_incidencia = new Nota_Incidencia
                        {

                            titulo = titulo_incidencia,
                            contenido_incidencia = incidencia,
                            puntuacion = 0,
                            IncidenciaId = nuevaIncidencia.Id // le pasamos el id de la incidsencia cxreada antes,
                                                              // por esto hacemos el savechanges antes de crear este objeto
                        };


                        context.NotasIncidencias.Add(nota_incidencia);
                        context.SaveChanges();

                        Console.WriteLine("Incidencia registrada");


                    }

                }

            }
        }












        public static void leer_notas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("estamos en leer notas");

            Console.WriteLine("Hacemos un bucle donde tendra este menu");
            Console.WriteLine("0. salir");
            Console.WriteLine("1 Listar todas estaciones");
            Console.WriteLine("2. enviar estacion cercana");
            Console.WriteLine("3 enviar notas");

            // como es un bucle while ponemos las variables afuera para no reservar memoria de mas
            int opcion = -1;

            double x;
            double y;
            double z;

            string estacion_cercana;

            List<Paradas> lista_paradas;
            List<string> paradas;
            Paradas parada_actual = null;
            List<Estacion> todas_estaciones;
            while (opcion != 0)
            {
                // leemos la opcion 
                opcion = recibir_numero(backend_service_socket);

                if(opcion == 1) // dependiendo de la opcion enviamos una cosa u otra, aqui se envia la lista de estaciones
                {
                    todas_estaciones = context.Estaciones.ToList();

                    // Enviamos primero la cantidad de estaciones que vamos a mandar
                    enviar_numero(todas_estaciones.Count, backend_service_socket);

                    for(int i = 0; i <  todas_estaciones.Count; i = i + 1)
                    {
                        enviar_texto(todas_estaciones[i].nombre, backend_service_socket); // enviamos solo el nombre
                    }
                }
                else if (opcion == 2) 
                {
                    x = recibir_double(backend_service_socket);

                    y = recibir_double(backend_service_socket);

                    z = recibir_double(backend_service_socket);


                    // aqui buscamos qual es la estacion mas cercana, solo nuecesitamos el nombre
                    estacion_cercana = calcular_estacion_cercana(x, y, z, context);


                    enviar_texto(estacion_cercana, backend_service_socket);

                    // sacamos la parada con todas sus lineas

                    // el .include es para que no haya problemas con el tema de llamar a otras tablas

                    lista_paradas = context.Paradas
                        .Include(p => p.Linea)
                        .Include(p => p.Estacion)
                        .Where(p => p.Estacion != null &&
                                    p.Estacion.nombre.Trim().ToLower() == estacion_cercana.Trim().ToLower()).ToList();

                    // el ToLower y trim es para que todos los nombres coincidan



                    paradas = lista_paradas.Select(p => p.Linea.nombre).Distinct().ToList();
                    //cogemos solo las lineas el nombre que tienen no su id Select(p => p.Linea.nombre)


                    // vamos a enviar en este orden las cosas, el numero de paradas y despues todas las paradas con formato R1,R2...
                    enviar_numero(paradas.Count, backend_service_socket);

                    for (int i = 0; i < paradas.Count; i = i + 1)
                    {
                        enviar_texto(paradas[i], backend_service_socket);
                    }
                }
                else if (opcion == 3)
                {


                    // recibimos la estacion(parada) actual
                    parada_actual = saber_parada_seleccionada_frontend(backend_service_socket, context, parada_actual);

                    if (parada_actual != null)
                    {

                        DateTime inicioDia = DateTime.UtcNow.Date;
                        DateTime finDia = inicioDia.AddDays(1);

                        List<Incidencias> listaIncidencias = context.Incidencias.Where(i => i.ParadaId == parada_actual.Id && i.fecha >= inicioDia && i.fecha < finDia).ToList();
                        // sacamos la lista de incidencias de esta estacion y enviamos un numero con cuantas incidencias hay 
                        // se coje solo las de el dia actual no las de dias antiguos, necesitamos usar el UTCNow, comparamos que la fecha este entre el dia actual y el siguiente
                        // para saber el dia siguiente se tiene que llamar a AddDays(numero de dias a sumar) si por ejemplo es 3 /4/26 0:0:0 y hacemos adddays llegamos a 4/4/26 0:0:0
                        // copmo acaba en 0:0:0 de el dia siguente y queremos las 23:59 en vez de <= usamos <

                        Console.WriteLine("Incidencias: " + listaIncidencias.Count);
                        enviar_numero(listaIncidencias.Count, backend_service_socket);


                        // for que recorre las listas de incidencias y mira si hay notas de incidencia
                        for (int i = 0; i < listaIncidencias.Count; i = i + 1)
                        {

                            // por cada incidencia vamos a buscar cuantas notas tiene
                            List<Nota_Incidencia> nota_incidencias = context.NotasIncidencias.Where(n => n.IncidenciaId == listaIncidencias[i].Id).ToList();
                            Console.WriteLine("Count: " + nota_incidencias.Count);
                            // vamos nota a nota en la incidencia 
                            for (int j = 0; j < nota_incidencias.Count; j = j + 1)
                            {
                                // enviamos el titulo y el contenido de la incidencia
                                enviar_texto(nota_incidencias[j].titulo, backend_service_socket);

                                Console.WriteLine("Titulo: " + nota_incidencias[j].titulo);

                                enviar_texto(nota_incidencias[j].contenido_incidencia, backend_service_socket);

                                Console.WriteLine("Descripcion: " + nota_incidencias[j].contenido_incidencia);
                            }
                        }
                    }
                }
            }
        }



        // funciones para las estaciones
        public static string calcular_estacion_cercana(double x, double y, double z, DBProyectoContext context)
        {
            if(context != null)
            {
                lock (context)
                {
                    List<Estacion> lista_estaciones = context.Estaciones.ToList(); // cogemos todas las estaciones

                    Estacion estacion_cercana = null; // vamos a crear una estacion como variable,
                                                      // vamos recorriendo todas las estaciones y si la nueva estacion es mas cercana a la que hemos encontrado,
                                                      // sera la nueva estacion_cercana

                    Estacion nueva_estacion = null;

                    double distancia = 0; // variable donde guardamos la distancia entre el usuario y las estaciones

                    double nueva_distancia = 0; // otra variable que servira para el calculo con la nueva estacion


                    for (int i = 0; i < lista_estaciones.Count; i = i + 1)
                    {
                        nueva_estacion = lista_estaciones[i];

                        if (estacion_cercana == null)
                        {
                            estacion_cercana = lista_estaciones[i]; // si es null, la estacion cercana es la primera estacion
                        }
                        if (distancia == 0) // si es 0 entonces cogemos de la estacion mas cercana la distancia
                        {
                            distancia = Math.Pow(((x - estacion_cercana.x) * (x - estacion_cercana.x) + (y - estacion_cercana.y) * (y - estacion_cercana.y) + (z - estacion_cercana.z) * (z - estacion_cercana.z)), 0.5);
                            // pitagoras en 3D
                        }

                        nueva_distancia = Math.Pow(((x - nueva_estacion.x) * (x - nueva_estacion.x) + (y - nueva_estacion.y) * (y - nueva_estacion.y) + (z - nueva_estacion.z) * (z - nueva_estacion.z)), 0.5);

                        if (nueva_distancia < distancia) // si la nueva distancia es menor, la estacione estara mas cerca
                        {
                            distancia = nueva_distancia;
                            estacion_cercana = nueva_estacion;
                        }

                    }


                    return estacion_cercana.nombre;
                }
            }
            return "";
            
            
        }
    }
}
