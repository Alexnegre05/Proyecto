using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Graphics;
using static BibliotecaFrontend.Classes;

namespace BibliotecaFrontend
{
    public class PonerNota
    {
        
        public static void mainthreadPonerNotas(PonerNotasParams parametros)
        {
            // aqui invopcamos el main therad que es el unico que es capaz de cambiar variables de maui 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Usamos parametros. para acceder a los datos del struct
                parametros.LabelEstacion.Text = "Estacion: " + parametros.Estacion;
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
                        // Cambiamos el texto usando los datos del struct y la selección
                        parametros.LabelEstacion.Text = $"Estación: {parametros.Estacion} ({seleccion.Nombre})";


                        // Usamos el color que viene guardado en el objeto seleccionado
                        parametros.LabelEstacion.TextColor = (Color)seleccion.Color;

                        parametros.BordePrincipal.Background = (Color)seleccion.Color;

                        parametros.Guardar.Background = (Color)seleccion.Color;
                        parametros.Titulo.TextColor = Colors.White;

                        parametros.LineasView.IsVisible = false;
                        parametros.BtnFlecha.Text = "▼";

                        parametros.LineasView.SelectedItem = null;
                        // esto es para que el selector se pueda volver a clicar una segunda vez
                        // si esta en la version resumida ya que si es la misma letra no detectara el evento 


                        // si no hay nada seleccionado, que no salga la posibilidad de poner notas
                        // (Nota: aquí 'seleccion' ya sabemos que no es null por el if de arriba, 
                        // pero mantenemos tu lógica de visibilidad)
                        if (seleccion == null)
                        {
                            parametros.ContenedorIncidencias.IsVisible = false;// para que se oculte 
                        }
                        else // caso contrario
                        {
                            parametros.ContenedorIncidencias.IsVisible = true;
                        }
                    }
                };
            });
        }
    }
}
