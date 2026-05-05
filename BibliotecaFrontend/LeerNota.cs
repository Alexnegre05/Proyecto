
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

                // Evitamos que el evento se acumule si se vuelve a entrar a la página
                parametros.LineasView.SelectionChanged -= LineasView_SelectionChanged;
                parametros.LineasView.SelectionChanged += LineasView_SelectionChanged;

                async void LineasView_SelectionChanged(object s, SelectionChangedEventArgs e)
                {
                    InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;

                    if (seleccion == null)
                        return;

                    parametros.LineasView.SelectedItem = null;

                    string estacion_actual = parametros.LabelEstacion.Text
                        .Replace("Estación: ", "")
                        .Split(" (")[0];

                    // aqui queremos solo el nombre de la estacion sin parentesis ni nada, se recibe como estacion(linea)

                    parametros.LabelEstacion.Text = $"Estación: {estacion_actual} ({seleccion.Nombre})";
                    parametros.LabelEstacion.TextColor = (Color)seleccion.Color;
                    parametros.BordePrincipal.Background = (Color)seleccion.Color;
                    parametros.Titulo.TextColor = Colors.White;

                    parametros.LineasView.IsVisible = false;
                    parametros.BtnFlecha.Text = "Cambiar Lineas ▼";

                    await Task.Run(() => // como vamos a hacer cosas de sockets hay que cambiar a el hilo secundario
                    {
                        send_num(3, parametros.frontend_socket); // la opcion 3 es la de mostrar notas

                        string texto_envio = $"Estación: {estacion_actual} ({seleccion.Nombre})";

                        enviar_texto(texto_envio, parametros.frontend_socket); // enviamos la estacion actual junto a que parada se ha seleccionado

                        int num_incidencias = recibir_numero(parametros.frontend_socket);
                        // recibimos el numero de incidencias,
                        // vamos una por una y recibimos por cada incidencia cuantas notas hay por incidencias

                        List<Incidencia> lista_temporal_incidencias = new List<Incidencia>();

                        if (num_incidencias > 0)
                        {
                            for (int i = 0; i < num_incidencias; i = i + 1)
                            {
                                int numero_notas = recibir_numero(parametros.frontend_socket);

                                for (int j = 0; j < numero_notas; j = j + 1)
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
                        // como volvemos a cambiar cosas de el hilo prioncipal xaml lo cambiamos 
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
                }
            });
        }

    }
}