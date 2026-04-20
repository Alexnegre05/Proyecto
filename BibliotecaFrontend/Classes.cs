using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaFrontend
{
    public class Classes
    {
        public class InfoLinea
        {
            public string Nombre { get; set; }
            public Color Color { get; set; }
        }


        public class Incidencia
        {
            public string titulo { get; set; }
            public string descripcion { get; set; }

            public Color ColorTexto { get; set; } // añadimos el color que tendra el texto que depende de la linea
        }

        public struct PonerNotasParams
        {
            public Socket frontend_socket;
            public string Estacion;
            public List<InfoLinea> Paradas;
            public Label LabelEstacion;
            public CollectionView LineasView;
            public Border BordePrincipal;
            public Button Guardar;
            public Label Titulo;
            public Button BtnFlecha;
            public Border ContenedorIncidencias;
        }

        public struct LeerNotasParams
        {
            public Socket frontend_socket;
            public string Estacion;
            public List<InfoLinea> Paradas;
            public Label LabelEstacion;
            public CollectionView LineasView;
            public Border BordePrincipal;
            public Label Titulo;
            public Button BtnFlecha;
            public CollectionView lista_incidencias;
            public List<string> estaciones; // lista de todas las estaciones
            public Picker PickerEstaciones; // picker para seleccionar estacion
        }
    }
}
