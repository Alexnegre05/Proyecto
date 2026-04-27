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
        public static ObservableCollection<string> ListaParaPickers { get; set; } = new ObservableCollection<string>();

        // 2. Tu función simplificada (sin el parámetro CollectionView)
        public static void mainthreadEnlaces(List<string> lista_estaciones)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Limpiamos para evitar que se dupliquen si entras/sales de la página
                ListaParaPickers.Clear();

                // Llenamos la lista
                foreach (string nombre in lista_estaciones)
                {
                    ListaParaPickers.Add(nombre);
                }

                // Los Pickers del XAML se actualizan solos gracias al x:Static
            });
        }

    }
}
