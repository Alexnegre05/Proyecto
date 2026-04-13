using Frontend.Models;
using Frontend.PageModels;

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
        // async y await son para que estas funciones se hagan en segundo plano
        // el async es como hilos
        private async void OnPonerIncidenciaClicked(object sender, EventArgs e)
        {
            // Navegación usando Shell
            await Shell.Current.GoToAsync("PonerNotasPage");
        }

        private async void OnLeerIncidenciaClicked(object sender, EventArgs e)
        {
            // Navegación usando Shell
            await Shell.Current.GoToAsync("LeerNotasPage");
        }
    }
}