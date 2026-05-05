

// cosas referentes a las librerias

using static BibliotecaBackend.Clases;
using static BibliotecaBackend.SQL;

using static BibliotecaBackend.IP;

using static BibliotecaBackend.Hilos;




namespace clases
{


    internal class Program
    {
        





        // funcion principal
        static void Main(string[] args)
        {

            DBProyectoContext context = new DBProyectoContext();


            insert_ensurecreated(context); // funcion de inserts... todo lo de sql 

            // cerrar conexion
            closeconnection(context);

            int try_except = 1; // variable que sirve para que si entras en el catch el while se repita todo el rato, dentro de los try excepts hay un try_except = 0

            while (try_except == 1)
            {

                // ip

                // leemos el fichero_configuracion 

                string ip = calculo_ip();

                // sockets
                // aqui se enviara el socket a el backend


                if (ip == null)
                {

                    Console.WriteLine(ip);

                }

                crear_hilo_principal(ip);

                try_except = 0; // salimos de el bucle de try_except
            }


            


        }
    }

}
    

