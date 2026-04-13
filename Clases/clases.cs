
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
namespace clases
{

    // usamos double en x,y,z y en distancia recorrida ya que pi es un numero double

    // aqui pondremos las clases, usamos EFCORE

    
    public class Estacion
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public bool obras { get; set; }

        public HashSet<Incidencias> Incidencias { get; set; } = new HashSet<Incidencias>();

       
        public void estacion_obras(bool obras)
        {

        }

        
    }



    public class Incidencias
    {
        public int Id { get; set; }
        public DateTime fecha { get; set; }
        public int gravedad { get; set; }
        public bool solucionado { get; set; }
        

        // Relación con Paradas (FK)
        public int EstacionId { get; set; }
        public Paradas Paradas { get; set; }

        public HashSet<Nota_Incidencia> nota_Incidencias { get; set; } = new HashSet<Nota_Incidencia>();
        public void mostrar_listas_incidencias()
        {

        }

        public void solucionado_nosolucionado(bool solucionado, int id_incidencia)
        {

        }

        public void poner_nueva_incidencia(string title, string text, DateTime fecha, int gravedad)
        {

        }
    }

    public class Nota_Incidencia
    {
        public int Id { get; set; }
        public string titulo { get; set; }
        public string contenido_incidencia { get; set; }
        public int puntuacion { get; set; }

        public int IncidenciaId { get; set; }
        public Incidencias Incidencia { get; set; }
        public void mostrar_notas_incidencia(int id_incidencia)
        {

        }
        public void leer_notas(int id_notas_incidencia)
        {

        }

        public void modificar_notas(string title, string text, DateTime fecha, int gravedad, int id_notas_incidencia)
        {

        }
        // si puntuacion es true sumamos 1, en caso contrario restamos 1
        public void introducir_puntuacion(bool puntuacion, int id_notas_incidencia)
        {

        }
    }


    public class Linea
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public bool obras { get; set; }
        public int EstacionInicioId { get; set; }
        public Estacion EstacionInicio { get; set; }

        public int EstacionFinalId { get; set; }
        public Estacion EstacionFinal { get; set; }

        public HashSet<Paradas> ListaParadas { get; set; } = new HashSet<Paradas>();

        public void linea_obras(bool obras)
        {

        }
        public void mostrar_paradas_linea(List<int> id_paradas)
        {

        }
    }

    public class Paradas
    {
        public int Id { get; set; }
        public bool Obras { get; set; }

        // Conexión con la Línea (N:1)
        public int LineaId { get; set; }
        public Linea Linea { get; set; }

        // la posicion exacta de la parada
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        // Conexión con la Estación (N:1)
        public int EstacionId { get; set; }
        public Estacion Estacion { get; set; }
        public void parada_obras(bool obras)
        {

        }
        // dependiendo de si la estacion/linea esta en iobras tambien lo estara la parada, y si la parada esta en obras no se mostrara
        public void ocultar_parada(int id_estación, int id_tipo_linea, bool obras)
        {

        }
    }

   

    public class Enlace
    {
        public int Id { get; set; }

        // Conexión a la siguiente parada
        public int SiguienteParadaId { get; set; }
        public Paradas SiguienteParada { get; set; }

        // Conexión a la parada anterior
        public int AnteriorParadaId { get; set; }
        public Paradas AnteriorParada { get; set; }

        // Costo del trayecto (ej: tiempo en minutos o metros entre paradas)
        public int Costo { get; set; }
    }

    public class Dijkstra
    {
        public int Id { get; set; }
        public DateTime FechaGuardar { get; set; }
        public int Costo { get; set; }
        public double DistanciaTotalRecorrida { get; set; }

        // FKs hacia Paradas
        public int ParadaInicioId { get; set; }
        public Paradas ParadaInicio { get; set; }

        public int ParadaDestinoId { get; set; }
        public Paradas ParadaDestino { get; set; }

        // devuelve una lista de paradas que forman la ruta
        public void calcular_ruta_id(int id_parada_inicio, int id_parada_final)
        {

        }


        public void guardar_ruta(List<Paradas> list_paradas, DateTime fecha_guardar)
        {

        }

        public void sacar_rutas_fecha(DateTime fecha1, DateTime fecha2)
        {

        }
    }

    public class DBProyectoContext : DbContext
    {
        public DbSet<Estacion> Estaciones { get; set; }
        public DbSet<Linea> Lineas { get; set; }
        public DbSet<Paradas> Paradas { get; set; }
        public DbSet<Enlace> Enlaces { get; set; }
        public DbSet<Dijkstra> Dijkstras { get; set; }

        public DbSet<Incidencias> Incidencias { get; set; }
        public DbSet<Nota_Incidencia> NotasIncidencias { get; set; }

        // la funcion de efcore para hacer la configuracion(conexion string)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql("Host=localhost;Port=9000;Username=postgres;Password=ifphospi;Database=rodalies");
            

        }
    }











    internal class Program
    {

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






        // funciones para la BD de conectarse y desconectarse


        static void closeconnection(DBProyectoContext context)
        {
            context.Dispose();
            
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

            while(linea != null) // leemos el fichero hasta que sea null
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



        static void sql(DBProyectoContext context)
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





















        // funciones de ip

        static string calculo_ip()
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



        static int check_ip_manual_numeros(string[] parts)
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
            byte[] data;
            byte[] posicion;
            string estacion_cercana;

            List<Paradas> lista_paradas;
            List<string> paradas;
            Paradas parada_actual;

            while (opcion != 0)
            {
                // leemos la opcion 
                data = new byte[sizeof(int)];
                backend_service_socket.Receive(data);

                opcion = BitConverter.ToInt32(data);


                if (opcion == 1) // dependiendo de la opcion enviamos una cosa u otra
                {
                    posicion = new byte[sizeof(double)];
                    backend_service_socket.Receive(posicion);

                    x = BitConverter.ToDouble(posicion);

                    backend_service_socket.Receive(posicion);

                    y = BitConverter.ToDouble(posicion);

                    backend_service_socket.Receive(posicion);

                    z = BitConverter.ToDouble(posicion);

                    Console.WriteLine("x: " + x);
                    Console.WriteLine("y: " + y);
                    Console.WriteLine("z: " + z);

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
                    // recibimos la estacion(parada) actual
                    string parada = recibir_texto(backend_service_socket);

                    // recibimos el titulo y la incidencia
                    string titulo_incidencia = recibir_texto(backend_service_socket);
                    string incidencia = recibir_texto(backend_service_socket);

                    string[] nombre_parada =  parada.Split("Estación: ");


                    string[] partes = nombre_parada[1].Split("(");
                    string estacion = partes[0].Trim();

                    // La segunda parte es la línea, pero tiene el ')' al final
                    string linea = partes[1].Replace(")", "").Trim();

                   

                     parada_actual = context.Paradas.Include(p => p.Estacion).Include(p => p.Linea)
                    .FirstOrDefault(p => p.Estacion.nombre.Trim().ToLower() == estacion.ToLower() && p.Linea.nombre.Trim().ToLower() == linea.ToLower());
              
                    if(parada_actual != null)
                    {


                        Incidencias nuevaIncidencia = new Incidencias // creamos una incidencia
                        {
                            fecha = DateTime.UtcNow, // la fecha es la actual, nos pide que sea UTC sino peta
                            gravedad = 1, // Valor por defecto
                            solucionado = false, // Valor por defecto
                            EstacionId = parada_actual.Id, // FK a Parada
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

                        Console.WriteLine("Incidencia registrada");

                       
                    }
                    
                }   
               
            }
            

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








        static Socket crear_backend_socket(string ip)
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

                        // leemos el numero para saber que tiene que pasar(si es poner notas, leer...)
                        byte[] data = new byte[sizeof(int)];
                        backend_service_socket.Receive(data);

                        int codigo = BitConverter.ToInt32(data);

                        if (codigo == 1)
                        {
                            
                            poner_notas(backend_service_socket, context);

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

