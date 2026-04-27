using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Maui.Devices.Sensors; // para los sensores
using Microsoft.Maui.ApplicationModel; // para qeu sepa que es de maui 
using Microsoft.Maui.Graphics; // para Color
using Microsoft.Maui.Controls; // para label y colectionview
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;
using static BibliotecaFrontend.PonerNota;
using static BibliotecaFrontend.LeerNota;
using static BibliotecaFrontend.ModificarNota;
using static BibliotecaFrontend.Enlaces;
namespace BibliotecaFrontend
{
    


    public class BibliotecaFrontend
    {


        



        // Diccionario de colores
        // aqui tenemos un diccionario que relaciona una linea con su color correspondiente
        public static Dictionary<string, Color> colores = new Dictionary<string, Color>
        {
                { "R1", Color.FromArgb("#4499D4") },
                { "R2", Color.FromArgb("#009900") },
                { "R2N", Color.FromArgb("#99C83E") },
                { "R2S", Color.FromArgb("#00642E") },
                { "R3", Color.FromArgb("#FF131A") },
                { "R4", Color.FromArgb("#FF9221") },
                { "R7", Color.FromArgb("#BD7DB5") },
                { "R8", Color.FromArgb("#9B1987") },
                { "RG1", Color.FromArgb("#007DC3") },
                { "R10", Color.FromArgb("#930030") },
                { "R11", Color.FromArgb("#0064A5") },
                { "R12", Color.FromArgb("#FFDC00") },
                { "R13", Color.FromArgb("#E52E87") },
                { "R14", Color.FromArgb("#675199") },
                { "R15", Color.FromArgb("#9A8A76") },
                { "R16", Color.FromArgb("#AF0036") },
                { "R17", Color.FromArgb("#E97300") }
        };



        // funciones de grados

        // funcion que pasa de grados a radianes
        public static double grados_a_radianes(double grados)
        {
            double radianes = (grados * Math.PI) / 180;

            return radianes;
        }


        // funciones de sockets


        public static async Task send_xyz(Socket frontend_socket)
        {
            Location location = null;

            // Primero intenta la última ubicación conocida (instantáneo)
            location = await Geolocation.Default.GetLastKnownLocationAsync();

            // Si no hay ninguna guardada, pide una nueva
            if (location == null)
            {
                location = await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(15)));
            }

            // Ahora sí comprobamos null ANTES de usar location
            if (location == null)
            {
                Console.WriteLine("ERROR: No se pudo obtener ubicación, usando Barcelona por defecto");
                send_parameter_xyz(4595227.0, frontend_socket);
                send_parameter_xyz(171864.0, frontend_socket);
                send_parameter_xyz(4078884.0, frontend_socket);
                return;
            }

            double x = location.Longitude;
            double y = location.Latitude;
            double z = location.Altitude ?? 0;

            float R = 6371.00877f * 1000;

            x = grados_a_radianes(x);
            y = grados_a_radianes(y);

            double final_x = (R + z) * Math.Cos(y) * Math.Cos(x);
            double final_y = (R + z) * Math.Cos(y) * Math.Sin(x);
            double final_z = (R + z) * Math.Sin(y);

            send_parameter_xyz(final_x, frontend_socket);
            send_parameter_xyz(final_y, frontend_socket);
            send_parameter_xyz(final_z, frontend_socket);
        }


    public async static void EstacionCercana(int num_opcion,
    Socket frontend_socket,
    Label LabelEstacion,
    CollectionView LineasView,
    Border BordePrincipal,
    Button guardar,
    Label Titulo,
    Button BtnFlecha,
    Border ContenedorIncidencias,
    CollectionView lista_incidencias)
        {
            try
            {
                
                // 1. Obtenemos la ubicación ANTES del Task.Run (en el hilo de UI)
                Location location = await Geolocation.Default.GetLastKnownLocationAsync();

                if (location == null)
                {
                    location = await Geolocation.Default.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(15)));
                }

                // 2. Calculamos x,y,z aquí todavía en hilo UI
                double final_x, final_y, final_z;

                if (location == null)
                {
                    Console.WriteLine("GPS no disponible, usando Barcelona por defecto");
                    final_x = 4595227.0;
                    final_y = 171864.0;
                    final_z = 4078884.0;
                }
                else
                {
                    float R = 6371.00877f * 1000;
                    double x = grados_a_radianes(location.Longitude);
                    double y = grados_a_radianes(location.Latitude);
                    double z = location.Altitude ?? 0;

                    final_x = (R + z) * Math.Cos(y) * Math.Cos(x);
                    final_y = (R + z) * Math.Cos(y) * Math.Sin(x);
                    final_z = (R + z) * Math.Sin(y);
                }

                // 3. Todo el trabajo de red en hilo de fondo, con las coordenadas ya calculadas
                await Task.Run(() =>
                {
                    send_num(num_opcion, frontend_socket);

                    if (num_opcion == 1) send_num(1, frontend_socket);
                    else if (num_opcion == 2) send_num(2, frontend_socket);
                    else if (num_opcion == 3) send_num(1, frontend_socket);

                    // Enviamos las coordenadas ya calculadas
                    send_parameter_xyz(final_x, frontend_socket);
                    send_parameter_xyz(final_y, frontend_socket);
                    send_parameter_xyz(final_z, frontend_socket);

                    string estacion = recibir_texto(frontend_socket);
                    int num = recibir_numero(frontend_socket);

                    List<InfoLinea> paradas = new List<InfoLinea>();
                    for (int i = 0; i < num; i++)
                    {
                        string linea = recibir_texto(frontend_socket);
                        InfoLinea linea_actual = new InfoLinea
                        {
                            Nombre = linea,
                            Color = colores.GetValueOrDefault(linea, Colors.Gray)
                        };
                        paradas.Add(linea_actual);
                    }

                    if (num_opcion == 1)
                    {
                        PonerNotasParams parametros = new PonerNotasParams
                        {
                            frontend_socket = frontend_socket,
                            Estacion = estacion,
                            Paradas = paradas,
                            LabelEstacion = LabelEstacion,
                            LineasView = LineasView,
                            BordePrincipal = BordePrincipal,
                            Guardar = guardar,
                            Titulo = Titulo,
                            BtnFlecha = BtnFlecha,
                            ContenedorIncidencias = ContenedorIncidencias
                        };
                        mainthreadPonerNotas(parametros);
                    }
                    else if (num_opcion == 2)
                    {
                        LeerNotasParams parametros = new LeerNotasParams
                        {
                            frontend_socket = frontend_socket,
                            Estacion = estacion,
                            Paradas = paradas,
                            LabelEstacion = LabelEstacion,
                            LineasView = LineasView,
                            BordePrincipal = BordePrincipal,
                            Titulo = Titulo,
                            BtnFlecha = BtnFlecha,
                            lista_incidencias = lista_incidencias,
                        };
                        mainthreadLeerNotas(parametros);
                    }
                    else if (num_opcion == 3)
                    {
                        ModificarNotasParams parametros = new ModificarNotasParams
                        {
                            frontend_socket = frontend_socket,
                            Estacion = estacion,
                            Paradas = paradas,
                            LabelEstacion = LabelEstacion,
                            LineasView = LineasView,
                            BordePrincipal = BordePrincipal,
                            Titulo = Titulo,
                            BtnFlecha = BtnFlecha,
                            ContenedorIncidencias = ContenedorIncidencias,
                            lista_incidencias = lista_incidencias,
                        };
                        mainthreaModificarNotas(parametros);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.ToString());
            }
        }

        public async static void enlaces(Socket frontend_socket)
        {
            send_num(4, frontend_socket); // enviamos un 4 para decir que estamos en enlaces

            // enviamos un 1 diciendo que queremos las listas de estaciones

            send_num(1, frontend_socket);

            // recibimos un numero

            int numero_bucle = recibir_numero(frontend_socket);

            List<string> lista_estaciones = new List<string>(); // lista de strings donde guardaremos todas las estaciones


            for(int i = 0; i <  numero_bucle; i = i + 1)
            {
                string estacion = recibir_texto(frontend_socket);
                lista_estaciones.Add(estacion);
            }

            mainthreadEnlaces(lista_estaciones);
        }







    }
}
