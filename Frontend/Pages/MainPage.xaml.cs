using Frontend.Models;
using Frontend.PageModels;
using static BibliotecaFrontend.BibliotecaFrontend; // usamos la biblioteca solo para las funciones de cambiar de pantalla
namespace Frontend.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        // aqui no es como css aqui el hover se tiene que programar para decirle que quieres que haga al entrar y despues al salir
        private void OnHoverEnter(object sender, PointerEventArgs e)
        {
            if (sender is Button button)
            {
                // cambiamos tanto el borde como el texto de color
                button.BorderColor = Color.FromArgb("#FF6600");
                button.BorderWidth = 2;
                button.TextColor = Color.FromArgb("#FF6600");
            }
        }

        private void OnHoverExit(object sender, PointerEventArgs e)
        {
            if (sender is Button button)
            {
                button.BorderColor = Colors.Transparent;
                button.BorderWidth = 0;
                button.TextColor = Colors.Black;
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

    }
}