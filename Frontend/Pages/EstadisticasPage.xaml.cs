namespace Frontend.Pages;
using static BibliotecaFrontend.BibliotecaFrontend;
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;
using System.Net.Sockets;
using System.Text;
public partial class EstadisticasPage : ContentPage
{

    
	public EstadisticasPage()
	{
		InitializeComponent();
    }

    public Socket frontend_socket;
    protected override void OnAppearing()
    {

        base.OnAppearing();

        frontend_socket = crear_frontend_socket(1000);

        send_num(5, frontend_socket);
        send_num(1, frontend_socket); // enviamos un 1 para decir que nos pase los datos 

        recibir_numero(frontend_socket); // numero para el maximo de inicdencias el dia de hoy 


        // recibimos el top 5 de estaciones con mas incidencias

        int num = recibir_numero(frontend_socket);
        for(int i = 0; i < num;i = i + 1)
        {
            recibir_texto(frontend_socket);
            recibir_numero(frontend_socket);
        }

        // mostramos la linea con mas incidencias
        recibir_texto(frontend_socket);
        recibir_numero(frontend_socket);

        // mostramos las inicdencias de todo el año

        recibir_numero(frontend_socket);
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
}