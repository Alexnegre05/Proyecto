using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clases
{
    internal class leerfichero
    {
        public static void leer_fichero()
        {
            // leemos el fichero 
            FileStream fichero_estaciones = new FileStream("listado-estaciones-rodalies-barcelona.csv", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fichero_estaciones, Encoding.UTF8);

            // creamos un nuevo archivo con la limpieza total, usamos el ../ para que se cree donde queremos 
            FileStream fichero_resultado = new FileStream("../../../fichero_estaciones.csv", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fichero_resultado, Encoding.UTF8);


            string linea = reader.ReadLine(); // vamos linea por linea en el fichero con informacion

            while (linea != null) // miramos si no es la ultima linea
            {

                string[] parts = linea.Split(";"); // lo separamos con ;
                string new_line = parts[1] + "," + parts[2] + "," + parts[3]; // sacamos la informacion que nos interesa y lo separamos con ,

                writer.WriteLine(new_line);
                linea = reader.ReadLine();
            }
            
            // cerramos los ficheros
            reader.Close();
            fichero_estaciones.Close();

            writer.Close();
            fichero_resultado.Close();
        }


        
    }
}
