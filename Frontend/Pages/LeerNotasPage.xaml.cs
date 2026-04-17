using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;
using static BibliotecaFrontend.LeerNota;
namespace Frontend.Pages;

public partial class LeerNotasPage : ContentPage
{


    public LeerNotasPage()
	{
		InitializeComponent();
	}


    public class Incidencia
    {
        public string titulo { get; set; }
        public string descripcion { get; set; }

        public Color ColorTexto { get; set; } // añadimos el color que tendra el texto que depende de la linea
    }



    Socket frontend_socket;



    protected override void OnAppearing()
    {
        base.OnAppearing();
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)
        
        // ejecutamos primero una funcion que coja de el backend las estaciones disponibles, solo los nombres
        todas_estaciones();

        EstacionCercana(2);
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
        BtnFlecha.Text = LineasView.IsVisible ? "Cambiar Lineas ▲" : "Cambiar Lineas ▼";


    }




    // repetimos la funcion de estacion cercana para que te salga por defecto la estacion mas cercana 

    private void todas_estaciones()
    {

    }
    private async void EstacionCercana(int num_opcion)
    {

        try
        {

            frontend_socket = crear_frontend_socket(1000);

            // enviamos un 2 para decir que va a recibir algo de enviar notas, un 1 si es de 
           
            send_num(num_opcion, frontend_socket);
            
            

            // enviamos otro 2 para decirle que queremos que nos de la opcion de la estacion mas cercana en el backend
            send_num(2, frontend_socket);


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
            if (num == 1)
            {
                mainthreadPonerNotas(estacion, paradas, LabelEstacion, LineasView, BordePrincipal, guardar, Titulo, BtnFlecha, ContenedorIncidencias);
            }
            else if (num == 2)
            {
                mainthreadLeerNotas(estacion, paradas, LabelEstacion, LineasView, BordePrincipal, Titulo, BtnFlecha, frontend_socket, lista_incidencias);
            }
            

            


        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }
}