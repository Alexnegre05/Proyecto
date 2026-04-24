using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaFrontend.Classes;
using static BibliotecaFrontend.Sockets;
namespace BibliotecaFrontend
{
    public class LeerNota
    {



        public static void mainthreadLeerNotas(LeerNotasParams parametros)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                parametros.LabelEstacion.Text = "Estación: " + parametros.Estacion;
                parametros.LineasView.ItemsSource = parametros.Paradas;
                // lineas view es como el id de collectionview y sirve para poder tenerlo en el backend y
                // modificar sus atributos igual a label estación que es el nombre de la estacion 


                // el selectionchanges es que cuando se cambie la estacion(el selector) que se cambie el nombre
                // el += no es un a= a + 1 sino aqui
                // es un añade también esta función a la lista de cosas por hacer cuando ocurra el evento SelectionChanged

                // sender el objeto que disparo el evento y la e es el objeto que tiene los datos sobre el evento

                parametros.LineasView.SelectionChanged += (s, e) =>
                {
                    //  Obtenemos el elemento seleccionado actual, el que es el primero y lo ponemos como dinamico
                    // lo tenemos que pasar con el as a InfoLinea

                    InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;
                    // dynamic salta la comprovacion de a la hora de crear un objeto 

                    if (seleccion != null) // miramos que no sea nulo
                    {
                        // Cambiamos el texto
                        parametros.LabelEstacion.Text = $"Estación: {parametros.Estacion} ({seleccion.Nombre})";


                        // Usamos el color que viene guardado en el objeto seleccionado
                        parametros.LabelEstacion.TextColor = (Color)seleccion.Color;

                        parametros.BordePrincipal.Background = (Color)seleccion.Color;


                        parametros.Titulo.TextColor = Colors.White;

                        parametros.LineasView.IsVisible = false;
                        parametros.BtnFlecha.Text = "Cambiar Lineas ▼";

                        parametros.LineasView.SelectedItem = null;


                        // cada vez que cambie de linea aqui le pedimos la opcion 3 al backend en leer notas para que nos pase las notas
                        // ahora ponemos la opcion 3 a el backend 
                        send_num(3, parametros.frontend_socket);
                        enviar_texto(parametros.LabelEstacion.Text, parametros.frontend_socket);
                        int num_incidencias = recibir_numero(parametros.frontend_socket);



                        if (num_incidencias > 0)
                        {
                            // hacemos que el contenedor sea visible
                            parametros.lista_incidencias.IsVisible = true;

                            List<Incidencia> lista_temporal_incidencias = new List<Incidencia>();



                            for (int i = 0; i < num_incidencias; i = i + 1) // vamos incidencia por incidencia
                            {

                                int numero_notas = recibir_numero(parametros.frontend_socket);

                                for(int j = 0; j < numero_notas; j = j + 1)
                                {
                                    // como funciona el binding exactamente 
                                    string titulo_actual = recibir_texto(parametros.frontend_socket);
                                    string descripcion_actual = recibir_texto(parametros.frontend_socket);


                                    Incidencia incidencia = new Incidencia
                                    {
                                        titulo = titulo_actual.ToUpper(),
                                        descripcion = descripcion_actual,
                                        ColorTexto = (Color)seleccion.Color
                                    };
                                    lista_temporal_incidencias.Add(incidencia); // añadimos la incidencia a la lista


                                    // el binding funciona como que le dices que tienes un objeto y que quieres mostrar las propiedades de ese objeto

                                    parametros.lista_incidencias.ItemsSource = lista_temporal_incidencias;
                                    // le decimos que la lista de objetos de incidencia es lo que tiene que mostrar,
                                    // el binding mira si existe la propiedad titulo o descripcion en lo que esta recibiendo de el collectionview
                                    // solo puede leer si tiene {get;set}
                                }



                            }
                        }
                        else
                        {

                            parametros.lista_incidencias.ItemsSource = null; // borramos por si las moscas el item source poniendolo a null
                            Shell.Current.DisplayAlert("No hay notas que mostrar", "", "cerrar");
                        }

                    }
                };
            });
        }

    }
}
