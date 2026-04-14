using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;


namespace Frontend.Pages;



public partial class PonerNotasPage : ContentPage
{

    
    // variable global frontend sockets
    public Socket frontend_socket;


    public PonerNotasPage()
	{
		InitializeComponent();
	}

    // la funcion onAppearing sirve para indicarle a la aplicacion que cuando se abra la pagina  se llame a la funcion que quieras
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    // es lo mismo que on apearing pero para cuando un usuario cierra la pantalla o cambia de pestaña
    protected override void OnDisappearing()
    {

        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);

        

        // y cerramos los sockets
        frontend_socket.Dispose();
        frontend_socket.Close();
    }



    
    // COMO MOSTRAR TEXTO AQUI CONSOLE WRITELINE

    // await Shell.Current.DisplayAlert("Estación Encontrada", estacion, "Cerrar"); poner funcion como async 



    // Función para el botón de la flecha desplegable
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


    private void OnGuardarClicked(object sender, EventArgs e)
    {


        // usamos el puerto 1000

        // comprovamos si el texto no es null 

        string titulo = TituloIncidencia.Text;
        string descripcion = DescripcionIncidencia.Text;

        if(titulo == null || descripcion == null) // si el titulo o la descripcion es nula entonces no enviar nada
        {
            Shell.Current.DisplayAlert("Introduce texto", "", "Cerrar");
        }
        else
        {
            // le decimos que la opcion es la 2
            send_num(2, frontend_socket);


            // Lógica para enviar los datos al socket o guardarlos localmente

            enviar_texto(LabelEstacion.Text, frontend_socket);
            enviar_texto(titulo, frontend_socket);
            enviar_texto(descripcion, frontend_socket);

            Shell.Current.DisplayAlert("Incidencia guardada", "", "Cerrar");
            // cuando enviamos el numero llamamos a OnEliminarCliked para reiniciar el texto 
            OnEliminarClicked(sender, e);
        }
        
    }

    private void OnEliminarClicked(object sender, EventArgs e)
    {

        // Lógica para borrar la nota que estabas escribiendo dejamos el placeholder

        TituloIncidencia.Text = TituloIncidencia.Placeholder;
        DescripcionIncidencia.Text = DescripcionIncidencia.Placeholder;
    }






    // funcion para saber el nombre de la estacion mas cercana
    // usamos de nuevo el async y el await pero esta vez en la conexion de el socket 
    private async void EstacionCercana()
    {

        try
        {
          
            frontend_socket = crear_frontend_socket(1000);
            
            // enviamos un 1 para decir que va a recibir algo de poner notas 

            send_num(1, frontend_socket);

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

            mainthreadPonerNotas(estacion, paradas, LabelEstacion, LineasView, BordePrincipal, guardar, Titulo, BtnFlecha, ContenedorIncidencias);




        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }
}