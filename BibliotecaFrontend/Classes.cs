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
            public int? Id { get; set; } // este id es null y solo sirve para modificar notas para despues saber que incidencia se ha clicado y guardado

            public string titulo { get; set; }
            public string descripcion { get; set; }

            public Color ColorTexto { get; set; } // añadimos el color que tendra el texto que depende de la linea
        }


        // aqui creamos unos structs para que la funcion no reciba muchos parametros sueltos 
        // cada opcion requiere unos parametros u otros, lo ponemos asi porque cuando llamemos a la funcion es mas facil de modificar 
        // en caso de que haya que añadir alguna cosa
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
        }

        public struct ModificarNotasParams
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
            public CollectionView lista_incidencias;
        }
    }
}
