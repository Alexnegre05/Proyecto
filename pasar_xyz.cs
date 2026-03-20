using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace clases
{
    internal class pasar_xyz
    {

        // pi usa double, todos los datos de ahora usaran double
        public static double grados_a_radianes(float grados)
        {
            double radianes = (grados*Math.PI)/180;

            return radianes;
        }
        public static void de_polares_a_xyz()
        {
            FileStream file = new FileStream("../../../fichero_estaciones.csv", FileMode.Open, FileAccess.Read);

            StreamReader reader = new StreamReader(file);

            FileStream fichero_resultado = new FileStream("../../../estaciones_xyz.csv", FileMode.OpenOrCreate, FileAccess.Write);

            StreamWriter writer = new StreamWriter(fichero_resultado);

            int count = 0; // este contador esta solo para que la primera vez la primera linea es solo texto
            // no son numeros(latitud y longitud) todos los otros si, se separa la informacion aqui por ,
            string linea = reader.ReadLine();


            // para pasar de latitud a longitud usamos un radio de la tierra(aproximacion), hay que usar la f de float
            // Esto esta en kilometros, lo pasamos a metros
            float R = 6371.00877f*1000;
            while(linea != null)
            {

                
                count = count + 1;

                if (count != 1)
                {
                    string[] parts = linea.Split(',');

                    // hay que usar culture info invariant culture tanto para el tema de comas y puntos
                    float latitud = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float longitud = float.Parse(parts[2], CultureInfo.InvariantCulture);

                    double latitud_radianes =  grados_a_radianes(latitud);
                    double longitud_radianes = grados_a_radianes(longitud);


                    // reconstruimos la linea
                    Console.WriteLine(latitud_radianes.ToString(CultureInfo.InvariantCulture) + "," + longitud_radianes.ToString(CultureInfo.InvariantCulture));
                    linea = parts[0] + ',' + parts[1] + ',' + parts[2];
                }
                else // si la linea es los titulos
                {
                    writer.WriteLine(linea);
                }
                    linea = reader.ReadLine();
            }
            reader.Close();
            file.Close();

            writer.Close();
            fichero_resultado.Close();
        }
    }
}
