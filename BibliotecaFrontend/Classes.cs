using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
