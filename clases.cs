
using System;
using System.Collections.Generic;
namespace clases
{

    // aqui pondremos las clases
    class Estación
    {
        public int id;
        public string nombre;
        public float x;
        public float y;
        public float z;
        public bool obras;
       
        void calcular_estacion_cercana(float x, float y, float z)
        {

        }

        void estacion_obras(bool obras)
        {

        }

        void calcular_posicion_estacion(List<int> lista_paradas)
        {

        }
    }

    class Incidencias
    {
        public int id;
        public DateTime fecha;
        public int gravedad;
        public bool solucionado;
        public int id_estacion;

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

    class Nota_Incidencia
    {
        public int id;
        public int id_incidencia;
        public string titulo;
        public string incidencia;
        public int puntuacion;

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

    class Paradas
    {
        public int id;
        public int id_tipo_lineas;
        public int id_estacion;
        public bool obras;

        void parada_obras(bool obras)
        {

        }
        // dependiendo de si la estacion/linea esta en iobras tambien lo estara la parada, y si la parada esta en obras no se mostrara
        void ocultar_parada(int id_estación, int id_tipo_linea, bool obras)
        {

        }
    }

    class Linea
    {
        public int id;
        public string nombre;
        public bool obras;
        public int id_parada_inicio;
        public int id_parada_final;

        void linea_obras(bool obras)
        {

        }
        void mostrar_paradas_linea(List<int> id_paradas)
        {

        }
    }

    class Enlace
    {
        public int id;
        public int id_siguiente_parada;
        public int id_anterior_parada;
    }

    class Dijkstra
    {
        public int id;
        public int id_parada_inicio;
        public int id_parada_destino;
        public DateTime fecha_guardar;
        public int costo;
        public float distancia_total_recorrida;

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



    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello, World!");
        }
    }
}
