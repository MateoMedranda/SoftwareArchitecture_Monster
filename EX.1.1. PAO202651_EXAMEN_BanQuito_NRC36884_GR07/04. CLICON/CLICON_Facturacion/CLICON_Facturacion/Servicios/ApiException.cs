using System;

namespace CLICON_Facturacion.Servicios
{
    public sealed class ApiException : Exception
    {
        public int StatusCode { get; private set; }

        public ApiException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
