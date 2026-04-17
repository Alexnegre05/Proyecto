using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BibliotecaFrontend.Classes;

namespace BibliotecaFrontend
{
    public class PonerNota
    {

        public static void mainthreadPonerNotas(string estacion, List<InfoLinea> paradas, Label LabelEstacion, CollectionView LineasView, Border BordePrincipal, Button guardar, Label Titulo, Button BtnFlecha, Border ContenedorIncidencias)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                LabelEstacion.Text = "Estacion: " + estacion;
                LineasView.ItemsSource = paradas;
                // lineas view es como el id de collectionview y sirve para poder tenerlo en el backend y
                // modificar sus atributos igual a label estación que es el nombre de la estacion 


                // el selectionchanges es que cuando se cambie la estacion(el selector) que se cambie el nombre
                // el += no es un a= a + 1 sino aqui
                // es un añade también esta función a la lista de cosas por hacer cuando ocurra el evento SelectionChanged

                // sender el objeto que disparo el evento y la e es el objeto que tiene los datos sobre el evento

                LineasView.SelectionChanged += (s, e) =>
                {
                    //  Obtenemos el elemento seleccionado actual, el que es el primero y lo ponemos como dinamico
                    // lo tenemos que pasar con el as a InfoLinea

                    InfoLinea seleccion = e.CurrentSelection.FirstOrDefault() as InfoLinea;
                    // dynamic salta la comprovacion de a la hora de crear un objeto 

                    if (seleccion != null) // miramos que no sea nulo
                    {
                        // Cambiamos el texto
                        LabelEstacion.Text = $"Estación: {estacion} ({seleccion.Nombre})";


                        // Usamos el color que viene guardado en el objeto seleccionado
                        LabelEstacion.TextColor = (Color)seleccion.Color;

                        BordePrincipal.Background = (Color)seleccion.Color;

                        guardar.Background = (Color)seleccion.Color;
                        Titulo.TextColor = Colors.White;

                        LineasView.IsVisible = false;
                        BtnFlecha.Text = "▼";

                        LineasView.SelectedItem = null;
                        // esto es para que el selector se pueda volver a clicar una segunda vez
                        // si esta en la version resumida ya que si es la misma letra no detectara el evento 


                        // si no hay nada seleccionado, que no salga la posibilidad de poner notas
                        if (seleccion == null)
                        {
                            ContenedorIncidencias.IsVisible = false;// para que se oculte 
                        }
                        else // caso contrario
                        {
                            ContenedorIncidencias.IsVisible = true;
                        }


                    }
                };
            });

        }
    }
}
