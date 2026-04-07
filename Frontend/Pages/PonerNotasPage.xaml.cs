using System.Net.Sockets;
using System.Net;

namespace Frontend.Pages;

public partial class PonerNotasPage : ContentPage
{
	public PonerNotasPage()
	{
		InitializeComponent();
	}

    // la funcion onAppearing sirve para indicarle a la aplicacion que cuando se abra la pagina  se llame a la funcion que quieras
    protected override void OnAppearing()
    {
        base.OnAppearing(); 
        // esto es para decirle que como estamos sobreescribiendo una pagina que primero ejecute lo que hacia antes la funcion original(con el base)

        EstacionCercana();
    }

    

    // funcion para saber el nombre de la estacion mas cercana
    // usamos de nuevo el async y el await pero esta vez en la conexion de el socket 
    private async void EstacionCercana()
    {

        try
        {
            // ip que se usa para conectarse en el movil
            string ip = "192.168.1.50";

            // Creamos el socket 
            IPAddress address = IPAddress.Parse(ip);  // creamos la ip y el endpoint
            IPEndPoint endpoint = new IPEndPoint(address, 1000); // el puerto es el 1000
            Socket frontend_socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await frontend_socket.ConnectAsync(endpoint); // es lo mimso que connect pero preparada para el async 

            Console.WriteLine("Conectado");
            // cerramos el socket
            frontend_socket.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }
}