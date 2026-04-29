using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;

namespace Frontend.Pages;

public partial class EnlacesPage : ContentPage
{

    
	public EnlacesPage()
	{
		InitializeComponent();
	}

    Socket frontend_socket;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)
        frontend_socket = crear_frontend_socket(1000);
        enlaces(frontend_socket);
        
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);


        if (frontend_socket != null)
        {
            // y cerramos los sockets
            frontend_socket.Dispose();
            frontend_socket.Close();
        }
    }
    // funcion para poner todas las estaciones
    private void OnEnlaceSelected(object sender, SelectionChangedEventArgs e)
    {

    }

    // botones para moverse por las pantallas
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


    protected async void OnEnlaceClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("EnlacesPage");
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (Origen.SelectedItem == Destino.SelectedItem)
        {
            // si pone la misma estacion dos veces ponemos un mensaje 
            Shell.Current.DisplayAlert("Has seleccionado dos estaciones que son iguales","","Cerrar");
        }
        else if (Origen.SelectedItem == null || Destino.SelectedItem == null)
        {
            if (Origen.SelectedItem == null && Origen.SelectedItem == null)
            {
                Shell.Current.DisplayAlert("Introduce tanto el origen como el destino", "", "Cerrar");
            }
            else if (Origen.SelectedItem == null)
            {
                Shell.Current.DisplayAlert("Selecciona la estacion de origen", "", "Cerrar");
            }
            else if (Destino.SelectedItem == null)
            {
                Shell.Current.DisplayAlert("Selecciona la estacion de destino", "", "Cerrar");
            }
        }
        else // aqui hacemos la logica, enviamos un 2
        {
            send_num(2, frontend_socket);

            // enviamos la estacion de origen y de destino

            enviar_texto((string)Origen.SelectedItem, frontend_socket);
            enviar_texto((string)Destino.SelectedItem, frontend_socket);

            // recibimos el numero de cuantas estaciones vamos a recibir

            int num = recibir_numero(frontend_socket);

            for(int i = 0; i < num; i = i + 1)
            {
                string estacion = recibir_texto(frontend_socket);
                string linea = recibir_texto(frontend_socket);
            }
        }
    }
}