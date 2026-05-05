using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.Clases;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
namespace BibliotecaBackend
{
    public class Estadisticas
    {
        public static void estadisticas(Socket backend_service_socket, DBProyectoContext context)
        {
            Console.WriteLine("Menu");
            Console.WriteLine("0: salir");
            Console.WriteLine("1: mostrar incidencias");

            int opcion = recibir_numero(backend_service_socket);

            int totalIncidenciasAño;
            int totalIncidenciasHoy;
            int puesto = 1;

            while (opcion != 0)
            {

                if (opcion == 1)
                {
                    // incidencias totales de el dia de hoy 
                    DateTime hoy = DateTime.UtcNow.Date;

                    // Realizamos el conteo filtrando por la fecha
                    totalIncidenciasHoy = context.Incidencias
                        .Where(i => i.fecha.Date == hoy && i.fecha < hoy.AddDays(1)) // comprovamos que el dia es el actual
                        .Count();

                    // Ejemplo de cómo podrías usar el resultado
                    Console.WriteLine($"Total de incidencias hoy ({hoy:dd/MM/yyyy}): {totalIncidenciasHoy}");

                    enviar_numero(totalIncidenciasHoy,backend_service_socket); // enviamos este numero 

                    // la mayor estacion con incidencias TOP 5
                    var topEstaciones = context.Incidencias
                .GroupBy(i => i.Paradas.EstacionId)
                .Select(grupo => new
                {
                    EstacionId = grupo.Key,
                    TotalIncidencias = grupo.Count()
                })
                .OrderByDescending(x => x.TotalIncidencias)
                .Take(5) // Tomamos solo las 5 primeras
                .Join(context.Estaciones,
                      res => res.EstacionId,
                      est => est.Id,
                      (res, est) => new { est.nombre, res.TotalIncidencias })
                .ToList();

                    // 2. Mostrar resultados
                    Console.WriteLine("--- TOP 5 ESTACIONES CON MÁS INCIDENCIAS ---");
                    

                    enviar_numero(topEstaciones.Count, backend_service_socket);

                    foreach (var e in topEstaciones)
                    {
                        Console.WriteLine($"{puesto}. {e.nombre}: {e.TotalIncidencias} incidencias");

                        enviar_texto(e.nombre, backend_service_socket); // enviamos el texto y el numero 
                        enviar_numero(e.TotalIncidencias, backend_service_socket);
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
