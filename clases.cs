
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using Npgsql;
using static System.Net.Mime.MediaTypeNames;
namespace clases
{
    // usamos double en x,y,z y en distancia recorrida ya que pi es un numero double

    // aqui pondremos las clases

    
    class Estacion
    {
        public int id;
        public string nombre;
        public double x;
        public double y;
        public double z;
        public bool obras;
       
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


    class Linea
    {
        public int id;
        public string nombre;
        public bool obras;
        public int id_estacion_inicio;
        public int id_estacion_final;

        void linea_obras(bool obras)
        {

        }
        void mostrar_paradas_linea(List<int> id_paradas)
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
        public double distancia_total_recorrida;

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
        static string connectionString = "Host=localhost;Port=9000;Username=postgres;Password=ifphospi;Database=postgres";


        // funciones que simplifican el hacer consultas en BD postgre
        static void no_query(DbConnection connection,string text)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = text;
            command.ExecuteNonQuery();
        }

        static DbDataReader query(DbConnection connection, string text)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = text;

            DbDataReader reader = command.ExecuteReader();

            return reader;
        }

        // funciones para la BD de conectarse y desconectarse
        static void connect(string connectionString, out DbConnection connection, out DbDataSource source)
        {
            source = NpgsqlDataSource.Create(connectionString); // le pasamos el connectionstring

            connection = source.OpenConnection();// creamos la conexion
            
        }

        static void closeconnection(DbConnection connection, DbDataSource source)
        {
            // cerramos la conexion
            connection.Close();
            connection.Dispose();
            source.Dispose();
            connection = null;
            source = null;
        }

        static void comprovar_crear_BD(DbConnection connection)
        {
            DbDataReader reader = query(connection, $"SELECT 1 FROM pg_database WHERE datname = '{"proyecto"}'"); // esto sirve para mirar si la BD ya existe 

            if (reader.Read() == false) // si es false entonces no existe la BD y la tenemos que crear
            {
                string text = "CREATE DATABASE \"proyecto\" " + "WITH OWNER = \"postgres\" ENCODING = 'UTF8' CONNECTION LIMIT -1";

                reader.Dispose();
                reader.Close(); // cerramos los readers para que no falle el no_query, solo podemos hacer una cosa a la vez
                no_query(connection, text);

               
            }

            reader.Dispose();
            reader.Close();
        }


       
        static void comprovar_crear_tablas(DbConnection connection)
        {

            string sql;

            // estacion

            sql = "CREATE TABLE IF NOT EXISTS Estacion (\n" +
                                          "   id INTEGER PRIMARY KEY,\n" +
                                          "   nombre TEXT NOT NULL,\n" +
                                          "   x DOUBLE PRECISION NOT NULL,\n" +
                                          "   y DOUBLE PRECISION NOT NULL,\n" +
                                          "   z DOUBLE PRECISION NOT NULL,\n" +
                                          "   obras BOOLEAN NOT NULL)\n";

            no_query(connection, sql);


            // incidencias
            sql = "CREATE TABLE IF NOT EXISTS Incidencias (\n" +
                                              "   id INTEGER PRIMARY KEY,\n" +
                                              "   fecha TIMESTAMP NOT NULL,\n" +
                                              "   gravedad INTEGER NOT NULL,\n" +
                                              "   solucionado BOOLEAN NOT NULL,\n" +
                                              "   id_estacion INTEGER NOT NULL,\n" +
                                              "   CONSTRAINT fk_estacion\n" +
                                              "   FOREIGN KEY(id_estacion)\n" +
                                              "   REFERENCES Estacion(id)\n)";

            no_query(connection, sql);

            // nota_incidencia

            sql = "CREATE TABLE IF NOT EXISTS Nota_Incidencia (\n" +
                                              "   id INTEGER PRIMARY KEY,\n" +
                                              "   titulo TEXT NOT NULL,\n" + 
                                              "   id_incidencia INTEGER NOT NULL,\n" +
                                              "   puntuacion INTEGER NOT NULL, \n" +
                                              "   CONSTRAINT fk_incidencia\n" +
                                              "   FOREIGN KEY(id_incidencia)\n" +
                                              "   REFERENCES Incidencias(id)\n)";

            no_query(connection, sql);



            // lineas
            sql = "CREATE TABLE IF NOT EXISTS Linea (\n" +
                                                  "   id INTEGER PRIMARY KEY,\n" +
                                                  "   nombre TEXT NOT NULL,\n" +
                                                  "   id_estacion_inicio INTEGER NOT NULL,\n" +
                                                  "   id_estacion_final INTEGER NOT NULL,\n" +
                                                  "   obras BOOLEAN NOT NULL,\n" +

                                                  "   CONSTRAINT fk_estacion_inicio\n" +
                                                  "   FOREIGN KEY(id_estacion_inicio)\n" +
                                                  "   REFERENCES Estacion(id),\n" + 

                                                  "   CONSTRAINT fk_estacion_final\n" +
                                                  "   FOREIGN KEY(id_estacion_final)\n" +
                                                  "   REFERENCES Estacion(id)\n)";


            no_query(connection, sql);


            // paradas
            sql = "CREATE TABLE IF NOT EXISTS Paradas (\n" +
                                                  "   id INTEGER PRIMARY KEY,\n" +
                                                  "   id_tipo_lineas INTEGER NOT NULL,\n" +
                                                  "   id_estacion INTEGER NOT NULL,\n" +
                                                  "   obras BOOLEAN NOT NULL,\n" +

                                                  "   CONSTRAINT fk_linea_parada\n" +
                                                  "   FOREIGN KEY (id_tipo_lineas)\n" +
                                                  "   REFERENCES Linea(id),\n" +

                                                  "   CONSTRAINT fk_estacion_parada\n" +
                                                  "   FOREIGN KEY (id_estacion)\n" +
                                                  "   REFERENCES Estacion(id)\n)";



            no_query(connection, sql);

            // enlaces


            sql = "CREATE TABLE IF NOT EXISTS Enlaces (\n" +
                                                  "   id INTEGER PRIMARY KEY,\n" +
                                                  "   id_siguiente_parada INTEGER,\n" + // puede ser null
                                                  "   id_anterior_parada INTEGER,\n" +


                                                  "   CONSTRAINT fk_siguiente_parada\n" +
                                                  "   FOREIGN KEY (id_siguiente_parada)\n" +
                                                  "   REFERENCES Paradas(id),\n" +

                                                  "   CONSTRAINT fk_anterior_parada\n" +
                                                  "   FOREIGN KEY (id_anterior_parada)\n" +
                                                  "   REFERENCES Paradas(id)\n)";

            no_query(connection, sql);

        }




        // funcion principal
        static void Main(string[] args)
        {


            
            DbConnection connection = null;
            DbDataSource source = null;

            // crear conexion
            connect(connectionString, out connection, out source);

            // comprovacion y creacion de la BD
            comprovar_crear_BD(connection);

            // comprovacion y creacion de las tablas

            comprovar_crear_tablas(connection);

            // cerrar conexion
            closeconnection(connection, source);

            

        }
    }
}
