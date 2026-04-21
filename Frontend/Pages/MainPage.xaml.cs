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

        private void OnHoverEnter(object sender, PointerEventArgs e)
        {
            if (sender is Button button)
            {
                button.BorderColor = Color.FromArgb("#FF6600");
                button.BorderWidth = 2;
            }
        }

        private void OnHoverExit(object sender, PointerEventArgs e)
        {
            if (sender is Button button)
            {
                button.BorderColor = null;
                button.BorderWidth = 0;
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