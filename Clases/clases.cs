
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using BibliotecaBackend;
using static BibliotecaBackend.BibliotecaBackend;
namespace clases
{

    // usamos double en x,y,z y en distancia recorrida ya que pi es un numero double

    // aqui pondremos las clases, usamos EFCORE

    







    internal class Program
    {

        






        // funciones para la BD de conectarse y desconectarse


        static void closeconnection(DBProyectoContext context)
        {
            context.Dispose();
            
        }





        static void poner_notas(Socket backend_service_socket, DBProyectoContext context)
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












        static void leer_notas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("estamos en leer notas");

            Console.WriteLine("Hacemos un bucle donde tendra este menu");
            Console.WriteLine("0. salir");
            Console.WriteLine("1. enviar estacion cercana");
            Console.WriteLine("2 enviar notas");

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
                else if(opcion == 2)
                {


                    // recibimos la estacion(parada) actual
                    parada_actual = saber_parada_seleccionada_frontend(backend_service_socket, context, parada_actual);

                    if (parada_actual != null)
                    {
                        
                        List<Incidencias> listaIncidencias = context.Incidencias.Where(i => i.ParadaId == parada_actual.Id).ToList();
                        // sacamos la lista de incidencias de esta estacion y enviamos un numero con cuantas incidencias hay 

                        Console.WriteLine("Incidencias: " + listaIncidencias.Count);
                        enviar_numero(listaIncidencias.Count, backend_service_socket);


                        // for que recorre las listas de incidencias y mira si hay notas de incidencia
                        for (int i = 0; i < listaIncidencias.Count; i = i + 1)
                        {
                            
                            // por cada incidencia vamos a buscar cuantas notas tiene
                            List<Nota_Incidencia> nota_incidencias = context.NotasIncidencias.Where(n => n.IncidenciaId == listaIncidencias[i].Id).ToList();
                            Console.WriteLine("Count: " + nota_incidencias.Count);
                            // vamos nota a nota en la incidencia 
                            for(int j = 0; j < nota_incidencias.Count; j = j + 1)
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


        static Paradas saber_parada_seleccionada_frontend(Socket backend_service_socket, DBProyectoContext context, Paradas parada_actual)
        {


            // recibimos la estacion(parada) actual
            string parada = recibir_texto(backend_service_socket);


            string[] nombre_parada = parada.Split("Estación: ");


            string[] partes = nombre_parada[1].Split("(");
            string estacion = partes[0].Trim();

            // La segunda parte es la línea, pero tiene el ')' al final
            string linea = partes[1].Replace(")", "").Trim();



            parada_actual = context.Paradas.Include(p => p.Estacion).Include(p => p.Linea)
           .FirstOrDefault(p => p.Estacion.nombre.Trim().ToLower() == estacion.ToLower() && p.Linea.nombre.Trim().ToLower() == linea.ToLower());

            return parada_actual;
        }
           

        











        // funciones para las estaciones
        public static string calcular_estacion_cercana(double x, double y, double z, DBProyectoContext context)
        {
            List<Estacion> lista_estaciones = context.Estaciones.ToList(); // cogemos todas las estaciones

            Estacion estacion_cercana = null; // vamos a crear una estacion como variable,
                                              // vamos recorriendo todas las estaciones y si la nueva estacion es mas cercana a la que hemos encontrado,
                                              // sera la nueva estacion_cercana

            Estacion nueva_estacion = null;

            double distancia = 0; // variable donde guardamos la distancia entre el usuario y las estaciones

            double nueva_distancia = 0; // otra variable que servira para el calculo con la nueva estacion


            for(int i = 0; i < lista_estaciones.Count; i = i + 1)
            {
                nueva_estacion = lista_estaciones[i];

                if (estacion_cercana == null)
                {
                    estacion_cercana = lista_estaciones[i]; // si es null, la estacion cercana es la primera estacion
                }
                if (distancia == 0) // si es 0 entonces cogemos de la estacion mas cercana la distancia
                {
                    distancia = Math.Pow(((x - estacion_cercana.x)* (x - estacion_cercana.x) + (y - estacion_cercana.y) * (y - estacion_cercana.y) + (z - estacion_cercana.z) * (z - estacion_cercana.z)), 0.5); 
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







        // funciones de inserts
        static void inserts_estaciones(DBProyectoContext context)
        {
            FileStream fichero = new FileStream("../../../estaciones_xyz.csv", FileMode.Open, FileAccess.Read);

            StreamReader reader = new StreamReader(fichero, Encoding.UTF8);

            int count = 0; // contador para que no se haga un insert de la primera linea si count != 1
            string linea = reader.ReadLine();

            // variables para guardar las estaciones 
            string nombre;
            double x;
            double y;
            double z;

            count = count + 1;

            // la flecha es una funcion lambda donde le pasas una variable y te devuelve el nombre
            HashSet<string> list_estaciones = context.Estaciones.Select(e => e.nombre).ToHashSet();
            // con esto sacamos todos los nombres de las estaciones y lo guardamos con hashset
            // el hashset es como un diccionario, no hace un for para recorrer sus elementos

            while (linea != null) // leemos el fichero hasta que sea null
            {
                string[] parts = linea.Split(",");
                if (count != 1)
                {
                    nombre = parts[0];

                    // sacamos de el fichero los datos que vamos a insertar
                    // no podemos hacer casting() hay que usar parse
                    // para que no haya problemas para guardar los doubles ponemos el cultureinfo invariant culture

                    x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    z = double.Parse(parts[3], CultureInfo.InvariantCulture);

                    // con el any nos devuele si existe ya una estacion con un campo en concreto, en este caso el nombre
                    bool existe = list_estaciones.Contains(nombre);


                    if (existe == false) // si no existe hacemos el insert
                    {
                        // para hacer el insert creamos un objeto de clase estacion donde guardamos las variables
                        Estacion estacion = new Estacion();

                        estacion.nombre = nombre;
                        estacion.x = x;
                        estacion.y = y;
                        estacion.z = z;
                        estacion.obras = false; // dejamos de momento por defecto las obras como false

                        context.Estaciones.Add(estacion); // añadimos la estacion



                    }


                }

                linea = reader.ReadLine(); // leemos la linea siguiente
                count = count + 1; // sumamos uno al contador
            }

            context.SaveChanges(); // guardamos los cambios fuera de el for para que no haya tanta lentitud
            // cerramos los ficheros
            reader.Close();
            fichero.Close();
        }







        static void inserts_lineas(DBProyectoContext context)
        {
            FileStream fichero = new FileStream("../../../lineas.csv", FileMode.Open, FileAccess.Read);

            StreamReader reader = new StreamReader(fichero, Encoding.UTF8);

            int count = 0; // contador para que no se haga un insert de la primera linea si count != 1
            string linea_leida = reader.ReadLine();

            // variables para guardar las estaciones 
            string linea;
            string estacion_inicio;
            string estacion_final;

            count = count + 1;


            Dictionary<string, int> estacionesDiccionario = context.Estaciones.ToDictionary(e => e.nombre.Trim(), e => e.Id);
            HashSet<string> list_lineas = context.Lineas.Select(l => l.nombre).ToHashSet();

            while (linea_leida != null) // leemos el fichero hasta que sea null
            {
                string[] parts = linea_leida.Split(";"); // se separa por ;

                if (count != 1)
                {

                    linea = parts[0].ToString(CultureInfo.InvariantCulture);
                    estacion_inicio = parts[1].ToString(CultureInfo.InvariantCulture);
                    estacion_final = parts[2].ToString(CultureInfo.InvariantCulture);


                    // para ahcer el insert creamos un objeto de clase estacion donde guardamos las variables
                    Linea lineas = new Linea();

                    lineas.nombre = parts[0];
                    // context.Estaciones.FirstOrDefault con esto podemos buscar por nombre si coincide
                    bool existe = list_lineas.Contains(linea);

                    if (existe == false)
                    {
                        estacionesDiccionario.TryGetValue(estacion_inicio, out int idInicio);
                        estacionesDiccionario.TryGetValue(estacion_final, out int idFinal);

                        // Asignamos los IDs ahora que sabemos que NO son null
                        lineas.EstacionInicioId = idInicio;
                        lineas.EstacionFinalId = idFinal;

                        context.Lineas.Add(lineas);



                    }


                }

                linea_leida = reader.ReadLine(); // leemos la linea siguiente
                count = count + 1; // sumamos uno al contador
            }

            context.SaveChanges();
            // cerramos los ficheros
            reader.Close();
            fichero.Close();

        }









        static void inserts_paradas(DBProyectoContext context)
        {

            FileStream fichero = new FileStream("../../../Paradas.csv", FileMode.Open, FileAccess.Read);

            StreamReader reader = new StreamReader(fichero, Encoding.UTF8);

            // para ahorrar consultas en el futuro
            // aqui usamos un diccionario ya que necesitamos comparar los nombres con los id

            Dictionary<string, int> estacionesDiccionario = context.Estaciones.ToDictionary(e => e.nombre.Trim(), e => e.Id); // value id, key nombre
            Dictionary<string, int> lineasDiccionario = context.Lineas.ToDictionary(l => l.nombre.Trim(), l => l.Id); // value id, key nombre

            // Para las paradas existentes, usamos un HashSet de "Claves combinadas" (EstacionId-LineaId)
            // con el $ se le dice que lo que esta entre {} es una variable no texto
            // es decir la funcion lambda recibe una parada yu devuelve este par de numeros
            // que hashset es capaz de interpretar como clave valor

            HashSet<string> paradasExistentes = context.Paradas
                .Select(p => $"{p.EstacionId}-{p.LineaId}")
                .ToHashSet();

            string linea = reader.ReadLine();
            string estacion;

            string lineas_estaciones;
            int count = 0;
            count = count + 1;

            while (linea != null)
            {

                if (count != 1)
                {

                    linea = linea.Trim('\"');
                    string[] parts = linea.Split(',');

                    estacion = parts[0].ToString(CultureInfo.InvariantCulture).Trim('\"').Trim();


                    // sacamos esto fuera de el for para no repetir codigo, value es el id,
                    // con el out guardamos el id que servira para despues hacer comprobaciones
                    bool existe = estacionesDiccionario.TryGetValue(estacion, out int estacionId);

                    if (existe == true)
                    {
                        for (int i = 1; i < parts.Length; i = i + 1) // es un for que recorre todas las estaciones que tienes y va haciendo inserts
                        {

                            string nombreLineaLimpio = parts[i].Trim('\"').Trim();

                            // hacemos lo mismo para la linea
                            existe = lineasDiccionario.TryGetValue(nombreLineaLimpio, out int lineaId);

                            if (existe == true)
                            {

                                // validacion pero para la parada
                                // comprovamos que haya tanto el id de la estacion como para la linea

                                string claveNueva = estacionId + "-" + lineaId;

                                existe = paradasExistentes.Contains(claveNueva);


                                if (existe == false)
                                {
                                    Paradas paradas = new Paradas();
                                    paradas.EstacionId = estacionId;
                                    paradas.LineaId = lineaId;

                                    context.Paradas.Add(paradas);




                                }

                            }

                        }
                    }




                }

                count = count + 1;
                linea = reader.ReadLine();
            }

            context.SaveChanges(); // guardamos los cambios
            reader.Close();
            fichero.Close();
        }
        public static void sql(DBProyectoContext context)
        {

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Console.WriteLine("BD + tablas creadas");

            // insertamos las estaciones
            inserts_estaciones(context);
            Console.WriteLine("estaciones insertadas en la BD");

            // insertamos las lineas
            inserts_lineas(context);
            Console.WriteLine("lineas insertadas en BD");

            // insertamos paradas
            inserts_paradas(context);
            Console.WriteLine("paradas insertadas en la BD");
        }
    

        





































        // funcion principal
        static void Main(string[] args)
        { 

            DBProyectoContext context = new DBProyectoContext();


            sql(context); // funcion de inserts... todo lo de sql 

            int try_except = 1; // variable que sirve para que si entras en el catch el while se repita todo el rato

            while (try_except == 1)
            {
                
               
                    // ip

                    // leemos el fichero_configuracion 

                    string ip = calculo_ip();

                    // sockets
                    // aqui se enviara el socket a el backend


                    if (ip == null)
                    {
                        
                        Console.WriteLine(ip);
                        
                    }

                    Console.WriteLine("IP: " + ip);

                    Socket backend_socket = crear_backend_socket(ip);


                    while (backend_socket.IsBound == true)
                    {
                       
                        Socket backend_service_socket = backend_socket.Accept();

                        Console.WriteLine("Conectado");

                        int codigo = recibir_numero(backend_service_socket);

                        if (codigo == 1)
                        {
                            
                            poner_notas(backend_service_socket, context);

                        }
                    else if (codigo == 2)
                    {
                        leer_notas(backend_service_socket, context);
                    }

                        backend_service_socket.Close(); // cerramos el socket




                    }
                    

                    backend_socket.Close();
                    try_except = 0; // salimos de el bucle de try_except
                }
                

                // cerrar conexion
                closeconnection(context);
            }
            

        }
    }

