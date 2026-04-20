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
        frontend_socket = crear_frontend_socket(1000);
        
        // como no hay boton de guardar es null
        EstacionCercana(2, frontend_socket, LabelEstacion, LineasView, BordePrincipal,null, Titulo, BtnFlecha, BordePrincipal, lista_incidencias, PickerEstaciones);
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


    private void OnEstacionLabelTapped(object sender, EventArgs e)
    {
        PickerEstaciones.Focus();
    }

    private void OnEstacionSelected(object sender, EventArgs e)
    {
        if (PickerEstaciones.SelectedIndex != -1)
        {

            // PickerEstaciones.ItemsSource es la lista de cosas que puedes seleccionar en el picker
            // en nuestro caso son las estaciones, como es una lista PickerEstaciones.SelectedIndex indica el indice si es 0,1...
            string seleccionada = (string)PickerEstaciones.ItemsSource[PickerEstaciones.SelectedIndex];
            LabelEstacion.Text = "Estacion: " + seleccionada;
            lista_incidencias.ItemsSource = null;
            LineasView.ItemsSource = null;
            // Aquí deberías llamar a la lógica para cargar las incidencias 
            // de esta nueva estación seleccionada
        }
    }




    
}