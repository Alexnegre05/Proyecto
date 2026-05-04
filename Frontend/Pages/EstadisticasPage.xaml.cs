namespace Frontend.Pages;

public partial class EstadisticasPage : ContentPage
{
	public EstadisticasPage()
	{
		InitializeComponent();
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