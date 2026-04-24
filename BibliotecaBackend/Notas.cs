using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.General;
using static BibliotecaBackend.Sockets;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaBackend
{
    // aqui se trata poner/leer/modificar notas
    public class Notas
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

            while (opcion != 0)
            {
                // leemos la opcion 
                opcion = recibir_numero(backend_service_socket);

                if (opcion == 1) // dependiendo de la opcion enviamos una cosa u otra
                {

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


                            // enviamos cuantas notas hay por cada incidencia
                            enviar_numero(nota_incidencias.Count, backend_service_socket);
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


        public static void modificar_notas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("estamos en modificar notas");

            Console.WriteLine("Hacemos un bucle donde tendra este menu");
            Console.WriteLine("0. salir");
            Console.WriteLine("1 Enviar estacion cercana");
            Console.WriteLine("2 enviar notas");
            Console.WriteLine("3 añadir notas");
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



                        enviar_numero(listaIncidencias.Count, backend_service_socket);


                        
                        // for que recorre las listas de incidencias y mira si hay notas de incidencia
                        for (int i = 0; i < listaIncidencias.Count; i = i + 1)
                        {
                            // le enviamos primero el id de cada incidencia, sirve para despues a la hora de guardar una incidencia que sepa que incidencia es la que hay que añadir una nota

                            enviar_numero(listaIncidencias[i].Id, backend_service_socket);

                            // por cada incidencia vamos a buscar cuantas notas tiene
                            List<Nota_Incidencia> nota_incidencias = context.NotasIncidencias.Where(n => n.IncidenciaId == listaIncidencias[i].Id).ToList();
                           
                            // vamos nota a nota en la incidencia 

                            // enviamos primero cuantas notas de incidencia hay 
                            enviar_numero(nota_incidencias.Count, backend_service_socket);

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
                else if (opcion == 3)
                {
                    parada_actual = saber_parada_seleccionada_frontend(backend_service_socket, context, parada_actual);


                    // recibimos el titulo y la incidencia
                    string titulo_incidencia = recibir_texto(backend_service_socket);
                    string incidencia = recibir_texto(backend_service_socket);


                    if (parada_actual != null)
                    {


                        

                        //// guardamos la nota de la incidencia
                        //Nota_Incidencia nota_incidencia = new Nota_Incidencia
                        //{

                        //    titulo = titulo_incidencia,
                        //    contenido_incidencia = incidencia,
                        //    puntuacion = 0,
                        //    IncidenciaId = nuevaIncidencia.Id // le pasamos el id de la incidsencia cxreada antes,
                        //                                      // por esto hacemos el savechanges antes de crear este objeto
                        //};


                        //context.NotasIncidencias.Add(nota_incidencia);
                        context.SaveChanges();

                        Console.WriteLine("Incidencia registrada");


                    }
                }
            }
        }

    }
}
