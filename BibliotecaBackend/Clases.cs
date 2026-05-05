
using System.Net.Sockets;

using Microsoft.EntityFrameworkCore;
using static BibliotecaBackend.Sockets;
namespace BibliotecaBackend
{


    public class Clases
    {
        // aqui estan todas las clases con la infoermacion que necesitamos
        public class Estacion
        {
            public int Id { get; set; }
            public string nombre { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double z { get; set; }
            public bool obras { get; set; }

            public HashSet<Incidencias> Incidencias { get; set; } = new HashSet<Incidencias>();


            


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
            
        }

        public class Nota_Incidencia
        {
            public int Id { get; set; }
            public string titulo { get; set; }
            public string contenido_incidencia { get; set; }
            public int puntuacion { get; set; }

            public int IncidenciaId { get; set; }
            public Incidencias Incidencia { get; set; }
            
           
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
            
        }


        public class RankingEstacion
        {
            public string Nombre { get; set; }
            public int TotalIncidencias { get; set; }
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
