
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
        public void calcular_estacion_cercana(double x, double y, double z)
        {

        }

        public void estacion_obras(bool obras)
        {

        }

        public void calcular_posicion_estacion(List<int> lista_paradas)
        {

        }
    }

    public class Incidencias
    {
        public int Id { get; set; }
        public DateTime fecha { get; set; }
        public int gravedad { get; set; }
        public bool solucionado { get; set; }
        

        // Relación con Estación (FK)
        public int EstacionId { get; set; }
        public Estacion Estacion { get; set; }

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

                                    Console.WriteLine("A");

                                    
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

















        // funcion principal
        static void Main(string[] args)
        { 

            DBProyectoContext context = new DBProyectoContext();
            
            
            sql(context); // funcion de inserts... todo lo de sql 

            

            string ip = ""; // string de la ip de el usuario 
            int try_except = 1; // variable que sirve para que si entras en el catch el while se repita todo el rato
            int ip_automatica = -1;
            int ip_correcta = 0;

            while (try_except == 1)
            {
                
                try
                {
                    // ip

                    // leemos el fichero_configuracion 

                    FileStream file = new FileStream("fichero_configuracion.txt", FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(file, Encoding.UTF8);

                    string linea = reader.ReadLine(); // con esto cogemos la linea, si es null sabemos de que ha acabado
                    int count = 0; // 0 si es la primera linea que tenemos que leer  1 en caso de ser la siguiente
                        
                    while(linea != null)
                    {
                        if (count == 0) // si es la primera linea
                        {
                            string[] parts = linea.Split("="); // separamos el = para solo sacar el dato que queremos

                            string tipo_ip = parts[1];
                            tipo_ip = tipo_ip.Trim().ToLower(); // quita posibles espacios y que todo este en minusculas

                            if(tipo_ip == "automatico")
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

                            if(ip_correcta == 0)
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



                    // sockets
                    // aqui se enviara el socket a el backend

                    IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
                    IPEndPoint endpoint = new IPEndPoint(address, 1000); // el puerto es el 1000
                    Socket backend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    // creamos el socket

                    //backend_socket.Bind(endpoint);
                    //backend_socket.Listen(); // para que se escuche el socket

                    //while (backend_socket.IsBound == true)
                    //{
                    //    //Socket backend_service_socket = backend_socket.Accept();
                    //    //backend_service_socket.Close(); // cerramos el socket




                    //}

                    
                    backend_socket.Close();
                    try_except = 0; // salimos de el bucle de try_except
                }
                catch
                {
                    Console.WriteLine("Introduce una ip correcta");
                }

                // cerrar conexion
                closeconnection(context);
            }
            

        }
    }
}
