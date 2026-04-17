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




        // funcion para enviar el x,y,z de el movil 
        public static async Task send_xyz(Socket frontend_socket)
        {
            // con geolocation sacamos el x,y,z de el movil, el await y el async es porque la funcion es asincrona 
            // esto le dice cuanta precision queremos que haya GeolocationAccuracy.Medium
            // usamos medium para no consumir muchos datos de el movil 

            // esto es latitud, longitud... hay que pasarlo a x,y,z
            Location location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            double x = location.Longitude;
            double y = location.Latitude;
            double z = location.Altitude ?? 0;
            // la z puede ser 0 y ya esta en metros pero en 2D, se usa para sumarle a el radio ya que es la altura al nivel de el mar 


            // radio de la tierra
            float R = 6371.00877f * 1000;

            // lo pasamos a radianes
            x = grados_a_radianes(x);
            y = grados_a_radianes(y);


            // lo pasamos a metros, misma formula que el backend pero sumandole la altura de el mar 
            double final_x = (R + z) * Math.Cos(y) * Math.Cos(x);
            double final_y = (R + z) * Math.Cos(y) * Math.Sin(x);
            double final_z = (R + z) * Math.Sin(y);

            // enviamos el x,y,z a el backend

            send_parameter_xyz(final_x, frontend_socket);
            send_parameter_xyz(final_y, frontend_socket);
            send_parameter_xyz(final_z, frontend_socket);
        }








    }
}
