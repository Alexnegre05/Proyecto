
using System.Globalization;



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


                    // formulas que pasan de latitud/longitud a radianes
                    double x = R*Math.Cos(latitud_radianes)*Math.Cos(longitud_radianes);
                    double y = R * Math.Cos(latitud_radianes) * Math.Sin(longitud_radianes);
                    double z = R * Math.Sin(latitud_radianes);


                    // reconstruimos la linea
                    
                    linea = parts[0] + ',' + x.ToString(CultureInfo.InvariantCulture) + ',' + y.ToString(CultureInfo.InvariantCulture) + ',' + z.ToString(CultureInfo.InvariantCulture);
                    writer.WriteLine(linea);
                }
                else // si la linea es los titulos
                {
                    Console.Write(linea);
                    string[] parts = linea.Split(","); // de el titulo solo queremos la primera parte, descartamos lo otro para poner x,y,z

                    linea = parts[0] + "," + "x" + "," + "y" + "," + "z";
                    writer.WriteLine(linea);
                }
                
                // leemos la siguiente linea
                linea = reader.ReadLine();
            }

            // fuera del bucle cerramos los ficheros
            reader.Close();
            file.Close();

            writer.Close();
            fichero_resultado.Close();
        }
    }
}
