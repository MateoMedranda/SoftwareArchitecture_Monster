using System.Collections.ObjectModel;
using System.Globalization;
using ClienteEscritorioConversion.Modelos;
using ClienteEscritorioConversion.Servicios;

namespace ClienteEscritorioConversion.Controladores
{
    /// <summary>
    /// Controlador de la conversión: mantiene el estado de los selectores y del valor,
    /// aplica la regla de misma unidad y llama al servicio SOAP.
    /// Los mensajes del servidor se propagan sin modificar.
    /// </summary>
    public class ControladorConversion
    {
        public const string MensajeMismaUnidad = "No es posible convertir a la misma unidad.";
        public const string MensajeValorInvalido = "Ingresa un valor numérico válido.";

        private readonly ClienteSoapConversion _servicio;
        private readonly SesionActual _sesion;

        public ControladorConversion(ClienteSoapConversion servicio, SesionActual sesion)
        {
            _servicio = servicio;
            _sesion = sesion;
        }

        public IReadOnlyList<CategoriaUnidad> Categorias => CatalogoUnidades.Categorias;

        public ObservableCollection<string> Unidades { get; } = new ObservableCollection<string>();

        public CategoriaUnidad Categoria { get; private set; } = CategoriaUnidad.Temperatura;

        public string? UnidadOrigen { get; private set; }

        public string? UnidadDestino { get; private set; }

        public string ValorTexto { get; private set; } = string.Empty;

        public bool EsMismaUnidad =>
            !string.IsNullOrEmpty(UnidadOrigen) &&
            string.Equals(UnidadOrigen, UnidadDestino, StringComparison.Ordinal);

        /// <summary>Habilita el botón Convertir solo con datos completos y unidades distintas.</summary>
        public bool PuedeConvertir =>
            !EsMismaUnidad &&
            !string.IsNullOrEmpty(UnidadOrigen) &&
            !string.IsNullOrEmpty(UnidadDestino) &&
            EsValorValido(ValorTexto);

        public bool Exito { get; private set; }

        /// <summary>Resultado formateado a 4 decimales, listo para mostrar.</summary>
        public string Resultado { get; private set; } = string.Empty;

        /// <summary>Mensaje del servidor tal cual llegó (solo cuando Exito = false).</summary>
        public string? MensajeServidor { get; private set; }

        /// <summary>Error de validación local que debe mostrarse como advertencia (no llama al servidor).</summary>
        public string? MensajeLocal { get; private set; }

        public event Action? SesionCerrada;

        public void CambiarCategoria(int indiceCategoria)
        {
            if (indiceCategoria < 0 || indiceCategoria >= CatalogoUnidades.Categorias.Count)
            {
                return;
            }

            Categoria = CatalogoUnidades.Categorias[indiceCategoria];

            Unidades.Clear();
            foreach (string unidad in CatalogoUnidades.Obtener(Categoria))
            {
                Unidades.Add(unidad);
            }

            UnidadOrigen = null;
            UnidadDestino = null;
            LimpiarRespuesta();
        }

        public void CambiarUnidades(string? origen, string? destino)
        {
            UnidadOrigen = origen;
            UnidadDestino = destino;
            LimpiarRespuesta();
        }

        public void CambiarValor(string valor)
        {
            ValorTexto = valor ?? string.Empty;
        }

        /// <summary>
        /// Ejecuta la conversión. Devuelve un mensaje local cuando la petición se aborta
        /// antes de llamar al servidor; en caso contrario devuelve null y deja el
        /// resultado o el mensaje del servidor en las propiedades correspondientes.
        /// </summary>
        public async Task<string?> ConvertirAsync()
        {
            MensajeLocal = null;
            LimpiarRespuesta();

            if (EsMismaUnidad)
            {
                MensajeLocal = MensajeMismaUnidad;
                return MensajeLocal;
            }

            if (!TryParsearValor(ValorTexto, out double valor))
            {
                MensajeLocal = MensajeValorInvalido;
                return MensajeLocal;
            }

            if (string.IsNullOrEmpty(_sesion.Token))
            {
                MensajeLocal = "La sesión no está activa. Inicia sesión nuevamente.";
                return MensajeLocal;
            }

            RespuestaConversion respuesta = await _servicio.ConvertirAsync(
                _sesion.Token,
                Categoria,
                UnidadOrigen!,
                UnidadDestino!,
                valor);

            Exito = respuesta.Exito;

            if (respuesta.Exito)
            {
                Resultado = respuesta.ValorConvertido.ToString("0.0000", CultureInfo.InvariantCulture);
                return null;
            }

            MensajeServidor = respuesta.Mensaje;
            return null;
        }

        public void CerrarSesion()
        {
            _sesion.Cerrar();
            SesionCerrada?.Invoke();
        }

        public static bool EsValorValido(string texto) => TryParsearValor(texto, out _);

        /// <summary>Acepta negativos y separador decimal '.' o ',' con cultura invariante.</summary>
        private static bool TryParsearValor(string texto, out double valor)
        {
            valor = 0;

            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            string normalizado = texto.Trim().Replace(',', '.');

            return double.TryParse(
                       normalizado,
                       NumberStyles.Float,
                       CultureInfo.InvariantCulture,
                       out valor) &&
                   !double.IsNaN(valor) &&
                   !double.IsInfinity(valor);
        }

        private void LimpiarRespuesta()
        {
            Exito = false;
            Resultado = string.Empty;
            MensajeServidor = null;
        }
    }
}
