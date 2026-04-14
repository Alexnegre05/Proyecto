using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;

namespace Frontend.Pages;

public partial class LeerNotasPage : ContentPage
{


    public LeerNotasPage()
	{
		InitializeComponent();
	}


    // funcion que pasa de grados a radianes
    public static double grados_a_radianes(double grados)
    {
        double radianes = (grados * Math.PI) / 180;

        return radianes;
    }




    Socket frontend_socket;



    protected override void OnAppearing()
    {
        base.OnAppearing();
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    protected override void OnDisappearing()
    {

        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);



        // y cerramos los sockets
        frontend_socket.Dispose();
        frontend_socket.Close();
    }



    private void OnFlechaClicked(object sender, EventArgs e)
    {
        // Invierte la visibilidad: si está abierta se cierra, si está cerrada se abre,
        // esta propiedad es la que indica si la flecha muestra todas las estaciones o no lo muestra
        // se indica con un booleano 

        if (LineasView.IsVisible == true)
        {
            LineasView.IsVisible = false;
        }
        else
        {
            LineasView.IsVisible = true;
        }

        // Cambia la flecha según el estado 
        BtnFlecha.Text = LineasView.IsVisible ? "▲" : "▼";


    }




    // repetimos la funcion de estacion cercana para que te salga por defecto la estacion mas cercana 

    private async void EstacionCercana()
    {

        try
        {

            frontend_socket = crear_frontend_socket(1000);

            // enviamos un 2 para decir que va a recibir algo de enviar notas 

            send_num(2, frontend_socket);

            // enviamos otro 1 para decirle que queremos que nos de la opcion de la estacion mas cercana en el backend
            send_num(1, frontend_socket);


            // enviamos el xyz a el servidor
            await send_xyz(frontend_socket);

            string estacion = recibir_texto(frontend_socket);




            int num = recibir_numero(frontend_socket); // numero que nos dice cuantas paradas hay 

            List<InfoLinea> paradas = new List<InfoLinea>();
            // Infolinea es una clase donde se guardan los colores y los nombres de las lineas

            // recorremos toda la lista
            for (int i = 0; i < num; i = i + 1)
            {
                string linea = recibir_texto(frontend_socket);
                // obtenemos una de las lineas de la estacion y lo añadimos a paradas

                //esto es un objeto de la clase InfoLinea que se añade a las paradas
                InfoLinea linea_actual = new InfoLinea
                {
                    Nombre = linea,
                    Color = colores.GetValueOrDefault(linea, Colors.Gray)
                };


                paradas.Add(linea_actual);

                //new Nombre = linea, Color = colores.GetValueOrDefault(linea, Colors.Gray)

            }


            // aqui es donde se cambia el nombre, y todo el tema de el color.

            // El MainThread es el que se encarga de dibujar por pantalla
            // le decimos a ese hilo que se invoque y que cambie el texto y que las lineas son las paradas que hemos cogido

            mainthreadLeerNotas(estacion, paradas, LabelEstacion, LineasView, BordePrincipal, Titulo, BtnFlecha);




        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }
}