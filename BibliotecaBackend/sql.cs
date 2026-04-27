using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Clases;

namespace BibliotecaBackend
{
    public class SQL
    {

        // funciones de inserts
        public static void inserts_estaciones(DBProyectoContext context)
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







        public static void inserts_lineas(DBProyectoContext context)
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

        public static void inserts_paradas(DBProyectoContext context)
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

        public static void inserts_enlaces(DBProyectoContext context)
        {
            FileStream fichero = new FileStream("../../../Enlaces.csv", FileMode.Open, FileAccess.Read); // leemos de el fichero
            StreamReader reader = new StreamReader(fichero, Encoding.UTF8);

            string linea = reader.ReadLine();
            int count = 0;
            count = count + 1;// sirve para saber si es la primera linea y quitar lo de parada, linea de arriba

            // Diccionarios para evitar consultas constantes
            Dictionary<string, int> estacionesDiccionario = context.Estaciones
                .ToDictionary(e => e.nombre.Trim(), e => e.Id);

            Dictionary<string, int> lineasDiccionario = context.Lineas
                .ToDictionary(l => l.nombre.Trim(), l => l.Id);

            Dictionary<string, int> paradasDiccionario = context.Paradas
                .ToDictionary(p => $"{p.EstacionId}-{p.LineaId}", p => p.Id);

            // Para evitar duplicados usamos un hashset
            HashSet<string> enlacesExistentes = context.Enlaces
                .Select(e => $"{e.AnteriorParadaId}-{e.SiguienteParadaId}")
                .ToHashSet();

            while (linea != null) // mientras la linea no sea null leemos el fichero
            {
                if (count != 1) // si no es la primera linea
                {
                    string[] parts = linea.Split(','); // separamos el fichero por , y sacamos la información

                    string nombreLinea = parts[0].Trim();
                    string estacionOrigen = parts[1].Trim();
                    string estacionDestino = parts[2].Trim();
                    int costo = int.Parse(parts[3]);

                    // miramos si no existe el id de las lineas
                    bool existe = lineasDiccionario.TryGetValue(nombreLinea, out int lineaId);
                    if (existe == false)
                    {
                        linea = reader.ReadLine();
                        count = count + 1;
                        Console.WriteLine($"FALLO PARADA: {estacionOrigen} - {estacionDestino} ({nombreLinea})");
                        continue;
                    }

                    bool existeOrigen = estacionesDiccionario.TryGetValue(estacionOrigen, out int estacionOrigenId);
                    bool existeDestino = estacionesDiccionario.TryGetValue(estacionDestino, out int estacionDestinoId);

                    if (existeOrigen == false || existeDestino == false) // miramos que la estacion sea correcta
                    {
                        linea = reader.ReadLine();
                        count = count + 1;
                        Console.WriteLine($"FALLO PARADA: {estacionOrigen} - {estacionDestino} ({nombreLinea})");
                        continue; // con continue saltamos a la sigueinte linea de el fichero(proxima vuelta de el bucle while)
                    }

                    // Obtener Paradas
                    string claveOrigen = $"{estacionOrigenId}-{lineaId}";
                    string claveDestino = $"{estacionDestinoId}-{lineaId}";

                    bool existeParadaOrigen = paradasDiccionario.TryGetValue(claveOrigen, out int paradaOrigenId);
                    bool existeParadaDestino = paradasDiccionario.TryGetValue(claveDestino, out int paradaDestinoId);

                    if (existeParadaOrigen == false || existeParadaDestino == false) // lo mismo pero para paradas
                    {
                        linea = reader.ReadLine();
                        count = count + 1;
                        Console.WriteLine($"FALLO PARADA: {estacionOrigen} - {estacionDestino} ({nombreLinea})");
                        continue;
                    }

                    // Validar duplicado, tenemos que poner los insert en ambos lados tanto de hospitalet a sants como viceversa
                    string claveEnlace = $"{paradaOrigenId}-{paradaDestinoId}";
                    if (enlacesExistentes.Contains(claveEnlace) == false) // con esto miramos si no existe
                    {
                        Enlace enlace = new Enlace(); // creamos un nuevo enlace
                        enlace.AnteriorParadaId = paradaOrigenId;
                        enlace.SiguienteParadaId = paradaDestinoId;
                        enlace.Costo = costo;

                        Console.WriteLine("Entra aqui");
                        context.Enlaces.Add(enlace); // añadimos en el contexto los enlaces
                        enlacesExistentes.Add(claveEnlace);
                    }
                    else
                    {
                        Console.WriteLine("No entra aqui");
                    }
                }

                linea = reader.ReadLine();
                count = count + 1;
            }

            context.SaveChanges();
            reader.Close();
            fichero.Close();
        }


        // funciones para la BD de conectarse y desconectarse


        public static void closeconnection(DBProyectoContext context)
        {
            context.Dispose();

        }

        public static void insert_ensurecreated(DBProyectoContext context)
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

            // insertamos enlaces
            inserts_enlaces(context);
            Console.WriteLine("Enlaces insertados en la BD");
        }



    }
}
