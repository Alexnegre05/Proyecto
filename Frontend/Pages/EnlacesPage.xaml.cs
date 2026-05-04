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
        enlaces(frontend_socket); // aqui es dfiferente, en vez de llamar a estacion cercana llamamos a enlaces que es uan funcion que se encarga exclusivamente de todo lo relacioonado con enlaces
        
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

    // funcion que sale cuando clicas en calcular ruta, primero hace unas comprovaciones, envia las estaciones y recibe la ruta
    private async void OnButtonClicked(object sender, EventArgs e)
    {

        // comprovaciones de que no haya algo vacio o estaciones repetidas
        if (Origen.SelectedItem == Destino.SelectedItem)
        {
            // si pone la misma estacion dos veces ponemos un mensaje 
            await Shell.Current.DisplayAlert("Has seleccionado dos estaciones que son iguales", "", "Cerrar");
        }
        else if (Origen.SelectedItem == null || Destino.SelectedItem == null)// comprovacio de estacions que son null
        {
            if (Origen.SelectedItem == null && Origen.SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("Introduce tanto el origen como el destino", "", "Cerrar");
            }
            else if (Origen.SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("Selecciona la estacion de origen", "", "Cerrar");
            }
            else if (Destino.SelectedItem == null)
            {
                await Shell.Current.DisplayAlert("Selecciona la estacion de destino", "", "Cerrar");
            }
        }
        else // aqui hacemos la logica, enviamos un 2 para decirle que va a recibir el texto
        {
            send_num(2, frontend_socket);

            // enviamos la estacion de origen y de destino, como es un selected item el objeto hay que pasarlo a texti

            enviar_texto((string)Origen.SelectedItem, frontend_socket);
            enviar_texto((string)Destino.SelectedItem, frontend_socket);

            // recibimos el numero de cuantas estaciones vamos a recibir

            int num = recibir_numero(frontend_socket);

            StringBuilder rutaTexto = new StringBuilder(); // striung buildr es como un string pero aqui
            // sirve para añadir texto detras con append...
            string lineaActual = "";
            string primeraEstacion = "";
            string ultimaEstacion = "";

            for (int i = 0; i < num; i = i + 1)
            {
                string estacion = recibir_texto(frontend_socket); // recibimos la estacion y la linea
                string linea = recibir_texto(frontend_socket);

                if (i == 0) // guardamos el nombre y la linea inicial 
                {
                    lineaActual = linea;
                    primeraEstacion = estacion;
                    ultimaEstacion = estacion;
                    continue; // le pide que salte a la siguiente vuelta de el bucle sin avanzar mas por el codigo
                }

                if (linea == lineaActual)
                {
                    // seguimos en la misma línea por tanto la ultima estacion de momento es la estacion que acabamso de recibir
                    ultimaEstacion = estacion;
                }
                else
                {
                    // como la linea actual es diferente a la linea de ahora, a el texto que vamos a mostrar le añadimos una flecha
                    // para indicar el cambio de linea
                    if (rutaTexto.Length > 0)
                        rutaTexto.Append(" -> ");

                    if (primeraEstacion == ultimaEstacion) // esto es por si solo hay una estacion
                        rutaTexto.Append($"{primeraEstacion}({lineaActual})");
                    else
                        rutaTexto.Append($"{primeraEstacion}({lineaActual})...{ultimaEstacion}({lineaActual})"); // o si recorriste varias en esa linea

                    // iniciamos de nuevo las variables ya que estamos ahora en otra linea y sirve para empezar otra vez
                    lineaActual = linea;
                    primeraEstacion = estacion;
                    ultimaEstacion = estacion;
                }
            }

            // escribir el último tramo que no se guarda en la ultima vuelta de el for
            if (rutaTexto.Length > 0)
                rutaTexto.Append(" -> ");

            if (primeraEstacion == ultimaEstacion)
                rutaTexto.Append($"{primeraEstacion}({lineaActual})");
            else
                rutaTexto.Append($"{primeraEstacion}({lineaActual})...{ultimaEstacion}({lineaActual})");

            RutaLabel.Text = rutaTexto.ToString(); // mostramos toda la ruta aqui como es un stringbuilder lo pasamos a string
        }
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


    protected async void OnEstadisticasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("EstadisticasPage");
    }


    

    
}