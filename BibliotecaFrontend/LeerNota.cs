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

        // 🔹 guardamos la estación REAL seleccionada (IMPORTANTE)
        static string estacion_actual = "";
        static bool evento_conectado = false;

        public static void mainthreadLeerNotas(LeerNotasParams parametros)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                parametros.LabelEstacion.Text = "Estación: " + parametros.Estacion;
                parametros.LineasView.ItemsSource = parametros.Paradas;

                // ❌ ELIMINADO estado global
                // usamos SIEMPRE la estación que viene en parámetros

                if (!evento_conectado)
                {
                    evento_conectado = true;

                    parametros.LineasView.SelectionChanged += async (s, e) =>
                    {
                        InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;

                        if (seleccion == null)
                            return;

                        parametros.LineasView.SelectedItem = null;

                        // 🔥 SIEMPRE usamos la estación ACTUAL desde parámetros
                        string estacion_actual = parametros.LabelEstacion.Text.Replace("Estación: ", "").Split(" (")[0];

                        // UI inmediata
                        parametros.LabelEstacion.Text = $"Estación: {estacion_actual} ({seleccion.Nombre})";
                        parametros.LabelEstacion.TextColor = (Color)seleccion.Color;
                        parametros.BordePrincipal.Background = (Color)seleccion.Color;
                        parametros.Titulo.TextColor = Colors.White;

                        parametros.LineasView.IsVisible = false;
                        parametros.BtnFlecha.Text = "Cambiar Lineas ▼";

                        await Task.Run(() =>
                        {
                            send_num(3, parametros.frontend_socket);

                            string texto_envio = $"Estación: {estacion_actual} ({seleccion.Nombre})";
                            enviar_texto(texto_envio, parametros.frontend_socket);

                            int num_incidencias = recibir_numero(parametros.frontend_socket);

                            List<Incidencia> lista_temporal_incidencias = new List<Incidencia>();

                            if (num_incidencias > 0)
                            {
                                for (int i = 0; i < num_incidencias; i++)
                                {
                                    int numero_notas = recibir_numero(parametros.frontend_socket);

                                    for (int j = 0; j < numero_notas; j++)
                                    {
                                        string titulo_actual = recibir_texto(parametros.frontend_socket);
                                        string descripcion_actual = recibir_texto(parametros.frontend_socket);

                                        lista_temporal_incidencias.Add(new Incidencia
                                        {
                                            titulo = titulo_actual.ToUpper(),
                                            descripcion = descripcion_actual,
                                            ColorTexto = (Color)seleccion.Color
                                        });
                                    }
                                }
                            }

                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                if (num_incidencias > 0)
                                {
                                    parametros.lista_incidencias.IsVisible = true;
                                    parametros.lista_incidencias.ItemsSource = lista_temporal_incidencias;
                                }
                                else
                                {
                                    parametros.lista_incidencias.ItemsSource = null;

                                    await Shell.Current.DisplayAlert(
                                        "No hay notas que mostrar",
                                        "",
                                        "Cerrar"
                                    );
                                }
                            });
                        });
                    };
                }
            });
        }
    
    }
}