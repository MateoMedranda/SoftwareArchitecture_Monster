using System.Windows;
using ClienteEscritorioConversion.Controladores;

namespace ClienteEscritorioConversion
{
    /// <summary>
    /// Contenedor de la aplicación. Solo intercambia la vista activa; no contiene lógica.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Reemplaza el contenido por la vista indicada por el controlador.</summary>
        public void MostrarVista(UIElement vista)
        {
            ContenedorVistas.Children.Clear();
            ContenedorVistas.Children.Add(vista);
        }

        /// <summary>Conecta el controlador de aplicación con este contenedor.</summary>
        public void Conectar(ControladorAplicacion controlador)
        {
            controlador.VistaCambiada += MostrarVista;
            controlador.Iniciar();
        }
    }
}
