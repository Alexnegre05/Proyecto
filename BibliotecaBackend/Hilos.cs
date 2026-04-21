using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.SQL;
using static BibliotecaBackend.Notas;

namespace BibliotecaBackend
{
    public class Hilos
    {


        public static void crear_hilo_principal(string ip)
        {
            object parametros = (object)ip; // como tenemos que pasar la variable ip a la funcion hay que usar paramethized en la funcion
                                            // los objetos se pasan como (object)
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(bucle_principal);
            Thread hilo_server_bucle = new Thread(threadStart);
            hilo_server_bucle.Start(parametros); // iniciamos el hilo
        }

        public static void crear_hilo_secundario(Socket backend_service_socket)
        {

            // lo mismo pero la idea es que cada cliente tenga su propio backend_service_socket
            object parametros_cliente = (object)backend_service_socket;
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(hilo_cliente);
            Thread hilo_server_cliente = new Thread(threadStart);
            hilo_server_cliente.Start(parametros_cliente);

        }

        static public void hilo_cliente(object list)
        {
            object parametros = (object)list;

            DBProyectoContext context = new DBProyectoContext(); // los hilos no pueden compartir contextos, hay que crear uno por cada usuario 

            Socket backend_service_socket = (Socket)parametros; // pasamos esta variable a ip

            int codigo = recibir_numero(backend_service_socket);

            if (codigo == 1)
            {

                poner_notas(backend_service_socket, context);

            }
            else if (codigo == 2)
            {
                leer_notas(backend_service_socket, context);
            }
            else if (codigo == 3)
            {
                modificar_notas(backend_service_socket, context);
            }
                backend_service_socket.Close(); // cerramos el socket

            closeconnection(context); // cerramos la conexion 
        }


        static public void bucle_principal(object list)
        {

            object parametros = (object)list;


            string ip = (string)parametros; // pasamos esta variable a ip


            Socket backend_socket = crear_backend_socket(ip);



            while (backend_socket.IsBound == true)
            {
                // ponemos un hilo aqui 
                Socket backend_service_socket = backend_socket.Accept();

                Console.WriteLine("Conectado");

                crear_hilo_secundario(backend_service_socket); // funcion que crea el hilo secundario(que atiende a cada cliente individualmente

            }


            backend_socket.Close();
        }


    }
}
