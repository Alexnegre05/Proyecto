using static BibliotecaFrontend.BibliotecaFrontend;
using static BibliotecaFrontend.Sockets;
using static BibliotecaFrontend.Classes;
using static BibliotecaFrontend.LeerNota;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Frontend.Pages
{
    public partial class ModificarNotasPage : ContentPage
    {
        public ModificarNotasPage()
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

            // ejecutamos primero una funcion que coja de el backend las estaciones disponibles, solo los nombres
            todas_estaciones();

            frontend_socket = crear_frontend_socket(1000);
            // como no hay boton de guardar es null
            EstacionCercana(3, frontend_socket, LabelEstacion, LineasView, BordePrincipal, null, Titulo, BtnFlecha, BordePrincipal, lista_incidencias);
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

            // Cambia la flecha según el estado, usamos un if else ternario
            BtnFlecha.Text = LineasView.IsVisible ? "Cambiar Lineas ▲" : "Cambiar Lineas ▼";


        }




        // repetimos la funcion de estacion cercana para que te salga por defecto la estacion mas cercana 

        private void todas_estaciones()
        {

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