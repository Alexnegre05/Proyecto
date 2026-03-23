
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
namespace clases
{
    // usamos double en x,y,z y en distancia recorrida ya que pi es un numero double

    // aqui pondremos las clases

    
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

            optionsBuilder.UseNpgsql("Host=localhost;Port=9000;Username=postgres;Password=ifphospi;Database=postgres");
            

        }
    }




    internal class Program
    {
        
        // funciones para la BD de conectarse y desconectarse
       

        static void closeconnection(DBProyectoContext context)
        {
            context.Dispose();
            
        }


        static void inserts_estaciones()
        {

        }
        // funcion principal
        static void Main(string[] args)
        { 

            DBProyectoContext context = new DBProyectoContext();

            context.Database.EnsureCreated();

            Console.WriteLine("BD + tablas creadas");

            inserts_estaciones();
            // cerrar conexion
            closeconnection(context);

            Console.WriteLine("conexion cerrada");
            

        }
    }
}
