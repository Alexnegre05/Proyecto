
namespace Frontend.Pages
{
    public partial class MainPage : ContentPage
    {
        // viene por defecto no se toca
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }

        // aqui no es como css aqui el hover se tiene que programar para decirle que quieres que haga al entrar y despues al salir
        
        // object sender es quien envio la funcion, es una clase general de la que hereda button, label..
        // la e tiene informacion sobre como se presiono el boton(fue un click o como fue)...
        private void OnHoverEnter(object sender, PointerEventArgs e)
        {
            // con esto comprovbamos si lo que realmente a llamado a la funcion es un boton que llamaremos button
            if (sender is Button button == true)
            {
                // cambiamos tanto el borde como el texto de color, necesitas Color 
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


        // async es para indicar que queremos que se pueda pausar la parte de programacion(esta funcion en concreto) sin afectar a la que procesa toda la parte visual de la pagina(mas informacion en los main thread)

        // mientras que se esta ejecutando lo que pongamos en el await cuando acaba se despausa
        protected async void OnVolverAlMenuClicked(object sender, EventArgs e)
        {
            // Al navegar a otra página, el evento OnDisappearing se ejecutará cerrando todo lo de sockets... la pagina principal se denomina internamente main
            // como usamos await no usamos GoTO sino GoToAsync que es una funcion especializada paar estos casos

            // shell es el contenedor que contiene todas las paginas y sus rutas, current es para decirle que muestre la actual y con go to le decimos que queremos que pase
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
}