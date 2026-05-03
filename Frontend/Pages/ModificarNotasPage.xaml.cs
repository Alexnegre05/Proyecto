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
            public int? id { get; set; }
            public string titulo { get; set; }
            public string descripcion { get; set; }

            public Color ColorTexto { get; set; } // añadimos el color que tendra el texto que depende de la linea
        }

        Socket frontend_socket;
        BibliotecaFrontend.Classes.Incidencia incidencia_actual; // variable que sirve para saber la incidencia actual y saber el id mas tarde a la hora de guardar 
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            frontend_socket = crear_frontend_socket(1000);

            await EstacionCercana(
                3,
                frontend_socket,
                LabelEstacion,
                LineasView,
                BordePrincipal,
                guardar,
                Titulo,
                BtnFlecha,
                ContenedorIncidencias,
                lista_incidencias
            );
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





        // funciones de guardar y eliminar incidencias
        private async void OnGuardarClicked(object sender, EventArgs e)
        {


            // usamos el puerto 1000

            // comprovamos si el texto no es null 

            int? id = incidencia_actual.Id;


            string titulo = TituloIncidencia.Text;
            string descripcion = DescripcionIncidencia.Text;

            if (string.IsNullOrWhiteSpace(titulo) || string.IsNullOrWhiteSpace(descripcion)) // si el titulo o la descripcion es nula entonces no enviar nada
            {
                await Shell.Current.DisplayAlert("Introduce texto", "", "Cerrar");
            }
            else
            {
                // le decimos que la opcion es la 3
                send_num(3, frontend_socket);


                // Lógica para enviar los datos al socket o guardarlos localmente



                if (id != null)
                {

                    enviar_texto(LabelEstacion.Text, frontend_socket);

                    send_num((int)id, frontend_socket); // si o si sabemos que no puede ser null
                    enviar_texto(titulo, frontend_socket);
                    enviar_texto(descripcion, frontend_socket);

                    await Shell.Current.DisplayAlert("Incidencia guardada", "", "Cerrar");
                    // cuando enviamos el numero llamamos a OnEliminarCliked para reiniciar el texto 
                    OnEliminarClicked(sender, e);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ha habido un error", "", "Cerrar");

                    OnEliminarClicked(sender, e);
                }

            }

        }

        private void OnIncidenciaSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            {
                
                return;
            }


            BibliotecaFrontend.Classes.Incidencia seleccionada = e.CurrentSelection.FirstOrDefault() as BibliotecaFrontend.Classes.Incidencia; // la seleccion no es tipo incidenica es tipo BibliotecaFrontend.Classes.Incidencia

            if (seleccionada == null)
            {
                
                return;
            }
                

            TituloIncidencia.Text = "";
            DescripcionIncidencia.Text = "";

            ContenedorIncidencias.IsVisible = true;

            Shell.Current.DisplayAlert(seleccionada.Id?.ToString() ?? "ID null", "", "Cerrar");

            incidencia_actual = seleccionada;

            

            lista_incidencias.IsVisible = false;
            ContenedorIncidencias.IsVisible = true;

            ((CollectionView)sender).SelectedItem = null;
        }

        private void OnEliminarClicked(object sender, EventArgs e)
        {

            // Lógica para borrar la nota que estabas escribiendo dejamos el placeholder

            TituloIncidencia.Text = "";
            DescripcionIncidencia.Text = "";
        }

        // funciones para moverse por el menu
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
}