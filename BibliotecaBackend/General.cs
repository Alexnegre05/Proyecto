using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.Sockets;

using Microsoft.EntityFrameworkCore;


namespace BibliotecaBackend
{

    // funcion general de calcular estacion cercana
    public class General
    {




        // funciones para las estaciones
        public static string calcular_estacion_cercana(double x, double y, double z, DBProyectoContext context)
        {
            if(context != null)
            {
                lock (context)
                {
                    List<Estacion> lista_estaciones = context.Estaciones.ToList(); // cogemos todas las estaciones

                    Estacion estacion_cercana = null; // vamos a crear una estacion como variable,
                                                      // vamos recorriendo todas las estaciones y si la nueva estacion es mas cercana a la que hemos encontrado,
                                                      // sera la nueva estacion_cercana

                    Estacion nueva_estacion = null;

                    double distancia = 0; // variable donde guardamos la distancia entre el usuario y las estaciones

                    double nueva_distancia = 0; // otra variable que servira para el calculo con la nueva estacion


                    for (int i = 0; i < lista_estaciones.Count; i = i + 1)
                    {
                        nueva_estacion = lista_estaciones[i];

                        if (estacion_cercana == null)
                        {
                            estacion_cercana = lista_estaciones[i]; // si es null, la estacion cercana es la primera estacion
                        }
                        if (distancia == 0) // si es 0 entonces cogemos de la estacion mas cercana la distancia
                        {
                            distancia = Math.Pow(((x - estacion_cercana.x) * (x - estacion_cercana.x) + (y - estacion_cercana.y) * (y - estacion_cercana.y) + (z - estacion_cercana.z) * (z - estacion_cercana.z)), 0.5);
                            // pitagoras en 3D
                        }

                        nueva_distancia = Math.Pow(((x - nueva_estacion.x) * (x - nueva_estacion.x) + (y - nueva_estacion.y) * (y - nueva_estacion.y) + (z - nueva_estacion.z) * (z - nueva_estacion.z)), 0.5);

                        if (nueva_distancia < distancia) // si la nueva distancia es menor, la estacione estara mas cerca
                        {
                            distancia = nueva_distancia;
                            estacion_cercana = nueva_estacion;
                        }

                    }


                    return estacion_cercana.nombre;
                }
            }
            return "";
            
            
        }
    }
}
