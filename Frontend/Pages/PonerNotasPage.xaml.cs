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

    // funcion que pasa de grados a radianes
    public static double grados_a_radianes(double grados)
    {
        double radianes = (grados * Math.PI) / 180;

        return radianes;
    }


    // funcion para enviar el x,y,z de el movil 

    public static async void send_xyz(Socket frontend_socket)
    {
        // con geolocation sacamos el x,y,z de el movil, el await y el async es porque la funcion es asincrona 
        // esto le dice cuanta precision queremos que haya GeolocationAccuracy.Medium
        // usamos medium para no consumir muchos datos de el movil 

        // esto es latitud, longitud... hay que pasarlo a x,y,z
        Location location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

        double x = location.Longitude;
        double y = location.Latitude;
        double z = location.Altitude ?? 0;
        // la z puede ser 0 y ya esta en metros pero en 2D, se usa para sumarle a el radio ya que es la altura al nivel de el mar 


        // radio de la tierra
        float R = 6371.00877f * 1000;

        // lo pasamos a radianes
        x = grados_a_radianes(x);
        y = grados_a_radianes(y);


        // lo pasamos a metros, misma formula que el backend pero sumandole la altura de el mar 
        x = (R + z) * Math.Cos(y) * Math.Cos(x);
        y = (R + z) * Math.Cos(y) * Math.Sin(x);
        z = (R + z) * Math.Sin(y);


        // enviamos el x,y,z a el backend

        send_parameter_xyz(x, frontend_socket);
        send_parameter_xyz(y, frontend_socket);
        send_parameter_xyz(z, frontend_socket);
    }
    public static void send_parameter_xyz(double var, Socket frontend_socket)
    {
        byte[] bytes = new byte[sizeof(double)];
        byte[] length = BitConverter.GetBytes(bytes.Length);
        bytes = BitConverter.GetBytes(var);
        frontend_socket.Send(length);
        frontend_socket.Send(bytes);
    }


    // funcion para saber el nombre de la estacion mas cercana
    // usamos de nuevo el async y el await pero esta vez en la conexion de el socket 
    private async void EstacionCercana()
    {

        try
        {
            // ip que se usa para conectarse en el movil
            string ip = "172.27.240.1";

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