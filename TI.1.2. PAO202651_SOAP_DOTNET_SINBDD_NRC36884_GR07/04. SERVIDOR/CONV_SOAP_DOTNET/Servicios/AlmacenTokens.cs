using System;
using System.Collections.Concurrent;

namespace CONV_SOAP_DOTNET.Servicios
{
    public class EntradaToken
    {
        public string NombreUsuario { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime? FechaExpiracion { get; set; }
    }

    public interface IAlmacenTokens
    {
        string GenerarToken(string nombreUsuario);

        bool ValidarToken(string token);

        void RevocarToken(string token);
    }

    public class AlmacenTokens : IAlmacenTokens
    {
        private static readonly AlmacenTokens InstanciaCompartida = new AlmacenTokens();

        private readonly ConcurrentDictionary<string, EntradaToken> tokens =
            new ConcurrentDictionary<string, EntradaToken>(StringComparer.Ordinal);

        public static AlmacenTokens Instancia
        {
            get { return InstanciaCompartida; }
        }

        public string GenerarToken(string nombreUsuario)
        {
            string token = Guid.NewGuid().ToString("N");
            tokens[token] = new EntradaToken
            {
                NombreUsuario = nombreUsuario,
                FechaEmision = DateTime.UtcNow,
                FechaExpiracion = null
            };

            return token;
        }

        public bool ValidarToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return tokens.ContainsKey(token.Trim());
        }

        public void RevocarToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            EntradaToken entrada;
            tokens.TryRemove(token.Trim(), out entrada);
        }
    }
}
