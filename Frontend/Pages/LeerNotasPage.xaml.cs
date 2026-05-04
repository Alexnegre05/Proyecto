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

    List<string> lista_estaciones = new List<string>();

    Socket frontend_socket;


    string estacion_actual = "";
    protected override void OnAppearing()
    {
        base.OnAppearing();

        frontend_socket = crear_frontend_socket(1000);

        send_num(2, frontend_socket); // entrar en leer_notas

        // 🔹 LISTA DE ESTACIONES
        send_num(1, frontend_socket);

        int num = recibir_numero(frontend_socket);

        lista_estaciones.Clear();

        for (int i = 0; i < num; i++)
        {
            lista_estaciones.Add(recibir_texto(frontend_socket));
        }

        PickerEstaciones.ItemsSource = lista_estaciones;

        // 🔹 SOLO AQUÍ usamos EstacionCercana
        EstacionCercana(
            2,
            frontend_socket,
            LabelEstacion,
            LineasView,
            BordePrincipal,
            null,
            Titulo,
            BtnFlecha,
            BordePrincipal,
            lista_incidencias
        );

        // 🔹 guardamos estación inicial
        if (LabelEstacion.Text != null && LabelEstacion.Text.Contains(": "))
        {
            estacion_actual = LabelEstacion.Text.Split(": ")[1];
        }
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

    // funcion que se llama por cada ve< que alguien clica en una nueva estacion
    private void OnEstacionChanged(object sender, EventArgs e)
    {
        if (PickerEstaciones.SelectedItem == null)
            return;

        string nueva_estacion = PickerEstaciones.SelectedItem.ToString();

        estacion_actual = nueva_estacion;

        LabelEstacion.Text = "Estación: " + nueva_estacion;

        // 🔥 USAR SOLO OPCIÓN 4
        send_num(4, frontend_socket);
        enviar_texto(nueva_estacion, frontend_socket);

        int num_lineas = recibir_numero(frontend_socket);

        List<InfoLinea> lista = new List<InfoLinea>();

        for (int i = 0; i < num_lineas; i++)
        {
            string linea = recibir_texto(frontend_socket);

            lista.Add(new InfoLinea
            {
                Nombre = linea,
                Color = colores.GetValueOrDefault(linea, Colors.Gray)
            });
        }

        LineasView.ItemsSource = lista;

        // 🔥 RESET VISUAL (IMPORTANTE)
        lista_incidencias.IsVisible = false;
        lista_incidencias.ItemsSource = null;

        PickerEstaciones.IsVisible = false;
    }


    // funcion para mostrar o ocultar la lista dependiendo de si se toca o no 
    private void OnLabelEstacionTapped(object sender, EventArgs e)
    {
        PickerEstaciones.IsVisible = !PickerEstaciones.IsVisible;
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