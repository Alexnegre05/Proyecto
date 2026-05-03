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
                    InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;

                    if (seleccion != null)
                    {
                        parametros.LabelEstacion.Text = $"Estación: {parametros.Estacion} ({seleccion.Nombre})";

                        parametros.LabelEstacion.TextColor = (Color)seleccion.Color;

                        parametros.BordePrincipal.Background = (Color)seleccion.Color;

                        parametros.Titulo.TextColor = Colors.White;

                        parametros.LineasView.IsVisible = false;
                        parametros.BtnFlecha.Text = "Cambiar Lineas ▼";

                        parametros.LineasView.SelectedItem = null;

                        send_num(3, parametros.frontend_socket);
                        enviar_texto(parametros.LabelEstacion.Text, parametros.frontend_socket);
                        int num_incidencias = recibir_numero(parametros.frontend_socket);

                        if (num_incidencias > 0)
                        {
                            parametros.lista_incidencias.IsVisible = true;

                            List<Incidencia> lista_temporal_incidencias = new List<Incidencia>();

                            for (int i = 0; i < num_incidencias; i++)
                            {
                                int numero_notas = recibir_numero(parametros.frontend_socket);

                                for (int j = 0; j < numero_notas; j++)
                                {
                                    string titulo_actual = recibir_texto(parametros.frontend_socket);
                                    string descripcion_actual = recibir_texto(parametros.frontend_socket);

                                    Incidencia incidencia = new Incidencia
                                    {
                                        titulo = titulo_actual.ToUpper(),
                                        descripcion = descripcion_actual,
                                        ColorTexto = (Color)seleccion.Color
                                    };

                                    lista_temporal_incidencias.Add(incidencia);

                                    parametros.lista_incidencias.ItemsSource = lista_temporal_incidencias;
                                }
                            }
                        }
                        else
                        {
                            parametros.lista_incidencias.ItemsSource = null;

                            //  FIX MOBILE: DisplayAlert seguro en UI thread
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await Shell.Current.DisplayAlert(
                                    "No hay notas que mostrar",
                                    "",
                                    "Cerrar"
                                );
                            });
                        }
                    }
                };
            });
        }

    }
}