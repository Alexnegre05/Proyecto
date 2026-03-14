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
       
    }

    class Incidencias
    {
        public int id;
        public DateTime fecha;
        public int gravedad;
        public bool solucionado;
        public int id_estacion;
    }

    class Nota_Incidencia
    {
        public int id;
        public int id_incidencia;
        public string titulo;
        public string incidencia;
        public int puntuacion;
    }

    class Paradas
    {
        public int id;
        public int id_tipo_lineas;
        public int id_estacion;
        public bool obras;
    }

    class Linea
    {
        public int id;
        public string nombre;
        public bool obras;
        public int id_parada_inicio;
        public int id_parada_final;
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
    }



    internal class clases
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
