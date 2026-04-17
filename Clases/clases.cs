
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Npgsql.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Text;
using System.Globalization;
using System.Net.Sockets;
using System.Net;

// cosas referentes a las librerias
using BibliotecaBackend;
using static BibliotecaBackend.Clases;
using static BibliotecaBackend.SQL;
using static BibliotecaBackend.Sockets;
using static BibliotecaBackend.IP;
using static BibliotecaBackend.General;
using Microsoft.Win32;



namespace clases
{





    internal class Program
    {
        


        static public void hilo_cliente(object list)
        {
            object[] parametros = (object[])list;
            
            DBProyectoContext context = new DBProyectoContext(); // los hilos no pueden compartir contextos, hay que crear uno por cada usuario 

            Socket backend_service_socket = (Socket)parametros[0]; // pasamos esta variable a ip

            int codigo = recibir_numero(backend_service_socket);

            if (codigo == 1)
            {

                poner_notas(backend_service_socket, context);

            }
            else if (codigo == 2)
            {
                leer_notas(backend_service_socket, context);
            }

            backend_service_socket.Close(); // cerramos el socket

            closeconnection(context); // cerramos la conexion 
        }


        static public void bucle_principal(object list)
        {

            object[] parametros = (object[])list;
            
            
            string ip = (string)parametros[0]; // pasamos esta variable a ip


            Socket backend_socket = crear_backend_socket(ip);



            while (backend_socket.IsBound == true)
            {
                // ponemos un hilo aqui 
                Socket backend_service_socket = backend_socket.Accept();

                Console.WriteLine("Conectado");


                object[] parametros_cliente = new object[] {  backend_service_socket };
                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(hilo_cliente);
                Thread hilo_server_cliente = new Thread(threadStart);
                hilo_server_cliente.Start(parametros_cliente);


            }


            backend_socket.Close();
        }




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

                object[] parametros = new object[] { ip };

                ParameterizedThreadStart threadStart = new ParameterizedThreadStart(bucle_principal);
                Thread hilo_server_bucle = new Thread(threadStart);
                hilo_server_bucle.Start(parametros);

                try_except = 0; // salimos de el bucle de try_except
            }


            


        }
    }

}
    

