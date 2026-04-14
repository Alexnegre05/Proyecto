using System.Net;
using System.Net.Sockets;
using System.Text;
using BibliotecaFrontend;
using static BibliotecaFrontend.BibliotecaFrontend;

namespace Frontend.Pages;

public partial class LeerNotasPage : ContentPage
{

    public class InfoLinea
    {
        public string Nombre { get; set; }
        public Color Color { get; set; }
    }


    public LeerNotasPage()
	{
		InitializeComponent();
	}


    // funcion que pasa de grados a radianes
    public static double grados_a_radianes(double grados)
    {
        double radianes = (grados * Math.PI) / 180;

        return radianes;
    }




    Socket frontend_socket;

    public Dictionary<string, Color> colores = new Dictionary<string, Color>
    {
                { "R1", Color.FromArgb("#4499D4") },
                { "R2", Color.FromArgb("#009900") },
                { "R2N", Color.FromArgb("#99C83E") },
                { "R2S", Color.FromArgb("#00642E") },
                { "R3", Color.FromArgb("#FF131A") },
                { "R4", Color.FromArgb("#FF9221") },
                { "R7", Color.FromArgb("#BD7DB5") },
                { "R8", Color.FromArgb("#9B1987") },
                { "RG1", Color.FromArgb("#007DC3") },
                { "R10", Color.FromArgb("#930030") },
                { "R11", Color.FromArgb("#0064A5") },
                { "R12", Color.FromArgb("#FFDC00") },
                { "R13", Color.FromArgb("#E52E87") },
                { "R14", Color.FromArgb("#675199") },
                { "R15", Color.FromArgb("#9A8A76") },
                { "R16", Color.FromArgb("#AF0036") },
                { "R17", Color.FromArgb("#E97300") }
    };


    protected override void OnAppearing()
    {
        base.OnAppearing();
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    protected override void OnDisappearing()
    {

        base.OnDisappearing();
        // cuando hace esto le enviamos un 0 en el frontend a el backend

        send_num(0, frontend_socket);



        // y cerramos los sockets
        frontend_socket.Dispose();
        frontend_socket.Close();
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
        BtnFlecha.Text = LineasView.IsVisible ? "▲" : "▼";


    }



    private void mainthread(string estacion, List<InfoLinea> paradas)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LabelEstacion.Text = "Estacion: " + estacion;
            LineasView.ItemsSource = paradas;
            // lineas view es como el id de collectionview y sirve para poder tenerlo en el backend y
            // modificar sus atributos igual a label estación que es el nombre de la estacion 


            // el selectionchanges es que cuando se cambie la estacion(el selector) que se cambie el nombre
            // el += no es un a= a + 1 sino aqui
            // es un añade también esta función a la lista de cosas por hacer cuando ocurra el evento SelectionChanged

            // sender el objeto que disparo el evento y la e es el objeto que tiene los datos sobre el evento

            LineasView.SelectionChanged += (s, e) =>
            {
                //  Obtenemos el elemento seleccionado actual, el que es el primero y lo ponemos como dinamico
                // lo tenemos que pasar con el as a InfoLinea

                InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;
                // dynamic salta la comprovacion de a la hora de crear un objeto 

                if (seleccion != null) // miramos que no sea nulo
                {
                    // Cambiamos el texto
                    LabelEstacion.Text = $"Estación: {estacion} ({seleccion.Nombre})";


                    // Usamos el color que viene guardado en el objeto seleccionado
                    LabelEstacion.TextColor = (Color)seleccion.Color;

                    BordePrincipal.Background = (Color)seleccion.Color;

                   
                    Titulo.TextColor = Colors.White;

                    LineasView.IsVisible = false;
                    BtnFlecha.Text = "▼";

                    LineasView.SelectedItem = null;
                   

                }
            };
        });
    }

    // repetimos la funcion de estacion cercana para que te salga por defecto la estacion mas cercana 

    private async void EstacionCercana()
    {

        try
        {

            frontend_socket = crear_frontend_socket(1000);

            // enviamos un 2 para decir que va a recibir algo de enviar notas 

            send_num(2, frontend_socket);

            // enviamos otro 1 para decirle que queremos que nos de la opcion de la estacion mas cercana en el backend
            send_num(1, frontend_socket);


            // enviamos el xyz a el servidor
            await send_xyz(frontend_socket);

            string estacion = recibir_texto(frontend_socket);




            int num = recibir_numero(frontend_socket); // numero que nos dice cuantas paradas hay 

            List<InfoLinea> paradas = new List<InfoLinea>();
            // Infolinea es una clase donde se guardan los colores y los nombres de las lineas

            // recorremos toda la lista
            for (int i = 0; i < num; i = i + 1)
            {
                string linea = recibir_texto(frontend_socket);
                // obtenemos una de las lineas de la estacion y lo añadimos a paradas

                //esto es un objeto de la clase InfoLinea que se añade a las paradas
                InfoLinea linea_actual = new InfoLinea
                {
                    Nombre = linea,
                    Color = colores.GetValueOrDefault(linea, Colors.Gray)
                };


                paradas.Add(linea_actual);

                //new Nombre = linea, Color = colores.GetValueOrDefault(linea, Colors.Gray)

            }


            // aqui es donde se cambia el nombre, y todo el tema de el color.

            // El MainThread es el que se encarga de dibujar por pantalla
            // le decimos a ese hilo que se invoque y que cambie el texto y que las lineas son las paradas que hemos cogido

            mainthread(estacion, paradas);




        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

    }
}