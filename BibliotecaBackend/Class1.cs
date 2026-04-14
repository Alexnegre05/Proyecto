using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace BibliotecaBackend
{


    public class BibliotecaBackend
    {

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
            public int ParadaId { get; set; }
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






        static public Socket crear_backend_socket(string ip)
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

        public static int recibir_numero(Socket backend_socket)
        {
            byte[] data = new byte[sizeof(int)];
            backend_socket.Receive(data);

            int num = BitConverter.ToInt32(data);

            return num;
        }

        public static double recibir_double(Socket backend_socket)
        {
            byte[] data = new byte[sizeof(double)];
            backend_socket.Receive(data);

            double num = BitConverter.ToDouble(data);

            return num;
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















        // funciones de sql 

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

        // funciones para la BD de conectarse y desconectarse


        public static void closeconnection(DBProyectoContext context)
        {
            context.Dispose();

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




        // funciones de ip

        static public string calculo_ip()
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


        static public string calcular_ip_automatico()
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



        static public int check_ip_manual_numeros(string[] parts)
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


        // otras cosas
        public static Paradas saber_parada_seleccionada_frontend(Socket backend_service_socket, DBProyectoContext context, Paradas parada_actual)
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

    }

     





}
