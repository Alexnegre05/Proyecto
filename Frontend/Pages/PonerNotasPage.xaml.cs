namespace Frontend.Pages;

public partial class PonerNotasPage : ContentPage
{
	public PonerNotasPage()
	{
		InitializeComponent();
	}

    // la funcion onAppearing sirve para indicarle a la aplicacion que cuando se abra la pagina  se llame a la funcionq ue quieras
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    // funcion para saber el nombre de la estacion mas cercana
    private void EstacionCercana()
    {

    }
}