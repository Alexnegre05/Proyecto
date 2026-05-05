
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.Clases;
using System.Net.Sockets;

namespace BibliotecaBackend
{
    public class Estadisticas
    {

        
        public static void estadisticas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("Menu");
            Console.WriteLine("0: salir");
            Console.WriteLine("1: mostrar incidencias");

            int opcion = recibir_numero(backend_service_socket); // obtenemos la opcion para el menu

            int totalIncidenciasAño;
            int totalIncidenciasHoy;
            int puesto = 1; // variable que nos sirve para saber la posicion en el ranking de cada estacion

            while (opcion != 0)
            {

                if (opcion == 1)
                {
                    // incidencias totales de el dia de hoy 
                    DateTime hoy = DateTime.UtcNow.Date;

                    // Realizamos el conteo filtrando por la fecha
                    totalIncidenciasHoy = context.Incidencias
                        .Where(i => i.fecha.Date == hoy && i.fecha < hoy.AddDays(1)) // comprovamos que el dia es el actual y miorando que no sea el dia superior y que sea == fecha.date
                        .Count(); // el count es para que me de el numero de incidencias que coinciden con el where

                    // Ejemplo de cómo podrías usar el resultado
                    Console.WriteLine($"Total de incidencias hoy ({hoy:dd/MM/yyyy}): {totalIncidenciasHoy}");

                    enviar_numero(totalIncidenciasHoy,backend_service_socket); // enviamos este numero 

                    // la mayor estacion con incidencias TOP 5
                    List<RankingEstacion> topEstaciones = context.Incidencias
                .GroupBy(i => i.Paradas.EstacionId) // esto agrupa todas las incidencias que hay en una parada y nos lo da en muchos grupos
                .Select(grupo => new // seleccionamos de cada grupo de incidencias por paradas su id de la estacion y el total de incidencias
                                     // (key es el valor que hemos agrupado el grupo(que es como un diccionario con clave y valor)
                {
                    EstacionId = grupo.Key,
                    TotalIncidencias = grupo.Count() // aqui queremos saber cuantas incidencias tiene ese grupo de paradas por estacion
                })
                .OrderByDescending(x => x.TotalIncidencias) // ordena por el numero de incidencias la primera la que mas hasta la que queda abajo de el todo

                .Take(5) // Tomamos solo las 5 primeras, es un LIMIT es sql

                .Join(context.Estaciones, // el join es para unir la tabla estaciones con el id que acabamos de encontrar el estacion Id para ver que estacion coincide ç
                                          // y saber el nombre de la estacion, no el id
                      resultado => resultado.EstacionId,
                      estacion => estacion.Id,
                      (resultado, estacion) => new RankingEstacion { Nombre = estacion.nombre, TotalIncidencias = resultado.TotalIncidencias }) // hacemos que solo queremos estos dos valores, para ello necesitamos una clase nueva que 
                .ToList(); // lo ponemos en una lista

                    // 2. Mostrar resultados
                    Console.WriteLine("--- TOP 5 ESTACIONES CON MÁS INCIDENCIAS ---");
                    

                    enviar_numero(topEstaciones.Count, backend_service_socket);

                    for(int i = 0; i < topEstaciones.Count; i = i+ 1)
                    {
                        Console.WriteLine($"{puesto}. {topEstaciones[i].Nombre}: {topEstaciones[i].TotalIncidencias} incidencias");

                        enviar_texto(topEstaciones[i].Nombre, backend_service_socket); // enviamos el texto y el numero 
                        enviar_numero(topEstaciones[i].TotalIncidencias, backend_service_socket);
                        puesto = puesto + 1;
                    }

                    // linea con mas incidencias
                    var lineaConMasIncidencias = context.Incidencias
                .GroupBy(i => i.Paradas.LineaId)
                .Select(grupo => new
                {
                    LineaId = grupo.Key,
                    Total = grupo.Count()
                })
                .OrderByDescending(g => g.Total)
                .Select(res => new
                {
                    // Unimos con la tabla Lineas para obtener el nombre
                    NombreLinea = context.Lineas
                        .Where(l => l.Id == res.LineaId)
                        .Select(l => l.nombre)
                        .FirstOrDefault(),
                    TotalIncidencias = res.Total
                })
                .FirstOrDefault(); // Tomamos solo la primera (la que más tiene)

                    // 2. Mostrar el resultado
                    if (lineaConMasIncidencias != null)
                    {
                        Console.WriteLine($"La línea con más incidencias es: {lineaConMasIncidencias.NombreLinea} " +
                                          $"con un total de {lineaConMasIncidencias.TotalIncidencias} incidencias.");

                        enviar_texto(lineaConMasIncidencias.NombreLinea, backend_service_socket);
                        enviar_numero(lineaConMasIncidencias.TotalIncidencias, backend_service_socket);
                    }
                    else
                    {
                        Console.WriteLine("No se registraron incidencias en ninguna línea.");
                        enviar_texto("", backend_service_socket);
                        enviar_numero(0, backend_service_socket);
                    }

                    // mostrar las incidencias de todo el año 
                    // todo esto es para que no haya problemas entre datetime distintos DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc
                    DateTime inicio = new DateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime fin = inicio.AddYears(1);

                    totalIncidenciasAño = context.Incidencias
                        .Where(i => i.fecha >= inicio && i.fecha < fin)
                        .Count();

                    // Ejemplo de cómo podrías usar el resultado
                    Console.WriteLine($"Total de incidencias hoy ({inicio:dd/MM/yyyy}): {totalIncidenciasAño}");

                    enviar_numero(totalIncidenciasAño, backend_service_socket); // enviamos este numero 

                    opcion = recibir_numero(backend_service_socket); // miramos si es un 0
                }

                

                
            }
                
        }

    }
}
