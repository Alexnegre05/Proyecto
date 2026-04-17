
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



namespace clases
{

   


    

    internal class Program
    {

        




        // funcion principal
        static void Main(string[] args)
        { 

            DBProyectoContext context = new DBProyectoContext();


            sql(context); // funcion de inserts... todo lo de sql 

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

                    Console.WriteLine("IP: " + ip);

                    Socket backend_socket = crear_backend_socket(ip);


                    while (backend_socket.IsBound == true)
                    {
                       
                        Socket backend_service_socket = backend_socket.Accept();

                        Console.WriteLine("Conectado");

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




                    }
                    

                    backend_socket.Close();
                    try_except = 0; // salimos de el bucle de try_except
                }
                

                // cerrar conexion
                closeconnection(context);
            }
            

        }
    }

