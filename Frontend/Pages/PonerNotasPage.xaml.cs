using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;
using static BibliotecaFrontend.PonerNota;

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
        frontend_socket = crear_frontend_socket(1000);
        EstacionCercana(1,frontend_socket,LabelEstacion,LineasView,BordePrincipal,guardar,Titulo,BtnFlecha,ContenedorIncidencias,null);
    }

    // es lo mismo que on apearing pero para cuando un usuario cierra la pantalla o cambia de pestaña
    protected override void OnDisappearing()
    {

        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);

        if(frontend_socket != null) // comprovamos que el socket no sea null 
        {
            // y cerramos los sockets
            frontend_socket.Dispose();
            frontend_socket.Close();
        }

        
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

    protected async void OnVolverAlMenuClicked(object sender, EventArgs e)
    {
        // Al navegar a otra página, el evento OnDisappearing se ejecutará cerrando todo lo de sockets... la pagina principal se denomina internamente main
        await Shell.Current.GoToAsync("///main");
    }

    protected async void OnPonerIncidenciaClicked(object sender, EventArgs e)
    {
        // Navegación usando Shell
        await Shell.Current.GoToAsync("PonerNotasPage");
    }

    protected async void OnModificarIncidenciaClicked(object sender, EventArgs e)
    {
        // Navegación a la página de modificar
        await Shell.Current.GoToAsync("ModificarNotasPage");
    }


    protected async void OnLeerIncidenciaClicked(object sender, EventArgs e)
    {
        // Navegación usando Shell
        await Shell.Current.GoToAsync("LeerNotasPage");
    }
    

}