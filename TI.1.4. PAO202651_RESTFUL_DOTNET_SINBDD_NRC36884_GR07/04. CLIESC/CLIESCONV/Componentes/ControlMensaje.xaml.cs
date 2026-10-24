using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClienteEscritorioConversion.Componentes
{
    /// <summary>Tipo de mensaje que determina los colores del banner.</summary>
    public enum TipoMensaje
    {
        Error,
        Advertencia,
        Exito
    }

    /// <summary>
    /// Banner pequeño para mensajes amigables.
    /// Los mensajes provenientes del servidor se muestran tal cual, sin reescribirlos.
    /// </summary>
    public partial class ControlMensaje : UserControl
    {
        private DispatcherTimer? _temporizador;

        public ControlMensaje()
        {
            InitializeComponent();
        }

        /// <summary>Texto que se está mostrando (o null si el banner está oculto).</summary>
        public string? Texto { get; private set; }

        /// <summary>Último tipo aplicado al banner.</summary>
        public TipoMensaje Tipo { get; private set; } = TipoMensaje.Error;

        public void Mostrar(string texto, TipoMensaje tipo, TimeSpan? duracion = null)
        {
            Texto = texto;
            Tipo = tipo;

            AplicarColores(tipo);
            Etiqueta.Text = texto;
            Visibility = Visibility.Visible;

            DetenerTemporizador();

            if (duracion.HasValue)
            {
                _temporizador = new DispatcherTimer { Interval = duracion.Value };
                _temporizador.Tick += (_, _) => Ocultar();
                _temporizador.Start();
            }
        }

        public void Ocultar()
        {
            DetenerTemporizador();
            Texto = null;
            Etiqueta.Text = string.Empty;
            Visibility = Visibility.Collapsed;
        }

        private void AplicarColores(TipoMensaje tipo)
        {
            switch (tipo)
            {
                case TipoMensaje.Exito:
                    Marco.Background = new SolidColorBrush(Color.FromRgb(0xE8, 0xF5, 0xE9));
                    Marco.BorderBrush = new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32));
                    Etiqueta.Foreground = new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32));
                    break;

                case TipoMensaje.Advertencia:
                    Marco.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xF8, 0xE1));
                    Marco.BorderBrush = new SolidColorBrush(Color.FromRgb(0xEF, 0x6C, 0x00));
                    Etiqueta.Foreground = new SolidColorBrush(Color.FromRgb(0xEF, 0x6C, 0x00));
                    break;

                default:
                    Marco.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xEB, 0xEE));
                    Marco.BorderBrush = new SolidColorBrush(Color.FromRgb(0xC6, 0x28, 0x28));
                    Etiqueta.Foreground = new SolidColorBrush(Color.FromRgb(0xC6, 0x28, 0x28));
                    break;
            }
        }

        private void DetenerTemporizador()
        {
            if (_temporizador is null)
            {
                return;
            }

            _temporizador.Stop();
            _temporizador = null;
        }
    }
}
