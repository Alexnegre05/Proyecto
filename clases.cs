
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
        void calcular_estacion_cercana(double x, double y, double z)
        {

        }

        void estacion_obras(bool obras)
        {

        }

        void calcular_posicion_estacion(List<int> lista_paradas)
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
        void mostrar_listas_incidencias()
        {

        }

        void solucionado_nosolucionado(bool solucionado, int id_incidencia)
        {

        }

        void poner_nueva_incidencia(string title, string text, DateTime fecha, int gravedad)
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
        void mostrar_notas_incidencia(int id_incidencia)
        {

        }
        void leer_notas(int id_notas_incidencia)
        {

        }

        void modificar_notas(string title, string text, DateTime fecha, int gravedad, int id_notas_incidencia)
        {

        }
        // si puntuacion es true sumamos 1, en caso contrario restamos 1
        void introducir_puntuacion(bool puntuacion, int id_notas_incidencia)
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

        void linea_obras(bool obras)
        {

        }
        void mostrar_paradas_linea(List<int> id_paradas)
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
        void parada_obras(bool obras)
        {

        }
        // dependiendo de si la estacion/linea esta en iobras tambien lo estara la parada, y si la parada esta en obras no se mostrara
        void ocultar_parada(int id_estación, int id_tipo_linea, bool obras)
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
        void calcular_ruta_id(int id_parada_inicio, int id_parada_final)
        {

        }
        

        void guardar_ruta(List<Paradas> list_paradas, DateTime fecha_guardar)
        {

        }

        void sacar_rutas_fecha(DateTime fecha1, DateTime fecha2)
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
        // la funcion de efcore paar hacer la confighuracion(conexion string)
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

            while(linea != null) // leemos el fichero hasta que sea null
            {
                string[] parts = linea.Split(","); 
                if (count != 1)
                {
                    nombre = parts[0]; 
                    
                    // sacamos de el fichero los datos que vamos a insertar
                    // no podemos hacer casting() hay que usar parse
                    x = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                    y = double.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                    z = double.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture); // para que no haya problemas para guardar los doubles
                
                    // para ahcer el insert creamos un objeto de clase estacion donde guardamos las variables
                    Estacion estacion = new Estacion();

                    estacion.nombre = nombre;
                    estacion.x = x;
                    estacion.y = y;
                    estacion.z = z;
                    estacion.obras = false; // dejamos de momento por defecto las obras como false

                    context.Estaciones.Add(estacion); // añadimos la estacion
                    context.SaveChanges();

                }

                linea = reader.ReadLine(); // leemos la linea siguiente
                count = count + 1; // sumamos uno al contador
            }
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

                    Estacion  Estacion_inicio = context.Estaciones.FirstOrDefault(e => e.nombre.Trim() == estacion_inicio);
                    Estacion  Estacion_final = context.Estaciones.FirstOrDefault(e => e.nombre.Trim() == estacion_final);

                    // Asignamos los IDs ahora que sabemos que NO son null
                    lineas.EstacionInicioId = Estacion_inicio.Id;
                    lineas.EstacionFinalId = Estacion_final.Id;

                    context.Lineas.Add(lineas);
                    context.SaveChanges();

                }

                linea_leida = reader.ReadLine(); // leemos la linea siguiente
                count = count + 1; // sumamos uno al contador
            }

            // cerramos los ficheros
            reader.Close();
            fichero.Close();

        }


        static void inserts_paradas(DBProyectoContext context)
        {

            FileStream fichero = new FileStream("../../../Paradas.csv", FileMode.Open, FileAccess.Read);

            StreamReader reader = new StreamReader(fichero, Encoding.UTF8);

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


                    // sacamos esto fuera de el for para no repetir codigo
                    Estacion primera_estacion_encontrada = context.Estaciones.FirstOrDefault(e => e.nombre.Trim() == estacion);


                    if(primera_estacion_encontrada != null)
                    {
                        for (int i = 1; i < parts.Length; i = i + 1) // es un for que recorre todas las estaciones que tienes y va haciendo inserts
                        {

                            string nombreLineaLimpio = parts[i].Trim('\"').Trim();
                            

                            Linea lineaBD = context.Lineas.FirstOrDefault(l => l.nombre.Trim() == nombreLineaLimpio);

                            if (lineaBD != null)
                            {
                                Paradas paradas = new Paradas();
                                paradas.EstacionId = primera_estacion_encontrada.Id;
                                paradas.LineaId = lineaBD.Id;

                                context.Paradas.Add(paradas);
                            }

                        }
                    }
                    

                }

                count = count + 1;
                linea = reader.ReadLine();
            }

            reader.Close();
            fichero.Close();
        }





        // funcion principal
        static void Main(string[] args)
        { 

            DBProyectoContext context = new DBProyectoContext();
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


            // cerrar conexion
            closeconnection(context);
            Console.WriteLine("conexion cerrada");
            

        }
    }
}
