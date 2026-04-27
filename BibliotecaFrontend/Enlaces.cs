using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaFrontend
{
    public class Enlaces
    {
        // 1. Declaras la colección (fuera de la función, en la clase)
        public static ObservableCollection<string> ListaParaPickers = new ObservableCollection<string>();

        // 2. Tu función actualizada
        public static void mainthreadEnlaces(List<string> lista_estaciones, CollectionView Coleccion)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {

                // Vamos elemento por elemento añadiendo a la colección vinculada a la UI
                foreach (string nombre in lista_estaciones)
                {
                    ListaParaPickers.Add(nombre);
                    
                }

                Coleccion.ItemsSource = ListaParaPickers; // añladimos el itemsource  las paradas
            });
        }

    }
}
