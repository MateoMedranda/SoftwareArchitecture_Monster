using System.Text;
using System.Xml.Linq;
using MonsterConversorMovil.Models;

namespace MonsterConversorMovil.Services;

/// <summary>
/// Cliente SOAP 1.1 para el basicHttpBinding expuesto por el servicio WCF.
/// Se usa HTTP/XML estándar para que funcione en Android sin dependencias WCF de escritorio.
/// </summary>
public sealed class SoapConversionService : ISoapConversionService
{
    private const string ContractNamespace = "http://monster/servicioconversionunidades";
    private readonly HttpClient httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

#if ANDROID
    public string Endpoint { get; set; } = "http://10.0.2.2:49159/Servicios/ServicioConversionUnidades.svc";
#else
    public string Endpoint { get; set; } = "http://localhost:49159/Servicios/ServicioConversionUnidades.svc";
#endif

    public async Task<LoginResponse> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var body = new XElement(XName.Get("IniciarSesion", ContractNamespace),
            new XElement(XName.Get("solicitud", ContractNamespace),
                new XElement(XName.Get("NombreUsuario", ContractNamespace), request.UserName),
                new XElement(XName.Get("Contrasena", ContractNamespace), request.Password)));

        try
        {
            var document = await SendAsync("IniciarSesion", body, null, cancellationToken);
            var result = Find(document, "IniciarSesionResult");
            return new LoginResponse
            {
                IsSuccessful = ReadBool(result, "Exito"),
                Token = ReadValue(result, "Token"),
                Message = ReadValue(result, "Mensaje") ?? "No se recibió un mensaje del servicio."
            };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or InvalidOperationException)
        {
            return new LoginResponse { Message = Explain(exception) };
        }
    }

    public async Task<ConversionResponse> ConvertAsync(ConversionRequest request, CancellationToken cancellationToken = default)
    {
        var body = new XElement(XName.Get("solicitud", ContractNamespace),
            new XElement(XName.Get("Conversion", ContractNamespace),
                new XElement(XName.Get("Categoria", ContractNamespace), request.Category),
                new XElement(XName.Get("UnidadOrigen", ContractNamespace), request.SourceUnit),
                new XElement(XName.Get("UnidadDestino", ContractNamespace), request.TargetUnit),
                new XElement(XName.Get("Valor", ContractNamespace), request.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))));

        var header = new XElement(XName.Get("Token", ContractNamespace), request.Token);

        try
        {
            var document = await SendAsync("Convertir", body, header, cancellationToken);
            var result = Find(document, "ConvertirResult");
            var value = double.TryParse(ReadValue(result, "ValorConvertido"), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var converted) ? converted : 0;

            return new ConversionResponse
            {
                IsSuccessful = ReadBool(result, "Exito"),
                ConvertedValue = value,
                Message = ReadValue(result, "Mensaje") ?? "No se recibió un mensaje del servicio."
            };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or InvalidOperationException)
        {
            return new ConversionResponse { Message = Explain(exception) };
        }
    }

    private async Task<XDocument> SendAsync(string operation, XElement operationBody, XElement? soapHeader, CancellationToken cancellationToken)
    {
        var endpoint = NormalizeEndpoint(Endpoint);
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var serviceUri))
        {
            throw new InvalidOperationException("La dirección del servicio no es válida.");
        }

        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
        var envelope = new XElement(soap + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soap", soap));

        if (soapHeader != null)
        {
            envelope.Add(new XElement(soap + "Header", soapHeader));
        }

        envelope.Add(new XElement(soap + "Body", operationBody));

        var document = new XDocument(envelope);

        using var content = new StringContent(envelope.ToString(SaveOptions.DisableFormatting), Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", $"\"{ContractNamespace}/IServicioConversionUnidades/{operation}\"");
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, serviceUri) { Content = content };

        // IIS Express suele estar registrado para localhost. 10.0.2.2 mantiene la ruta
        // desde Android al anfitrión y este encabezado conserva el host permitido por IIS.
        if (string.Equals(serviceUri.Host, "10.0.2.2", StringComparison.OrdinalIgnoreCase))
        {
            httpRequest.Headers.Host = $"localhost:{serviceUri.Port}";
        }

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        var xml = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var fault = TryReadFault(xml);
            throw new InvalidOperationException(fault ?? $"El servicio respondió con HTTP {(int)response.StatusCode}.");
        }

        var document = XDocument.Parse(xml);
        var soapFault = TryReadFault(document);
        if (!string.IsNullOrWhiteSpace(soapFault))
        {
            throw new InvalidOperationException(soapFault);
        }

        return document;
    }

    private static string NormalizeEndpoint(string endpoint) => endpoint.Trim().Replace("?wsdl", string.Empty, StringComparison.OrdinalIgnoreCase);

    private static XElement Find(XContainer document, string localName) =>
        document.Descendants().FirstOrDefault(element => element.Name.LocalName == localName)
        ?? throw new InvalidOperationException("La respuesta SOAP no tiene el formato esperado.");

    private static string? ReadValue(XContainer parent, string localName) =>
        parent.Descendants().FirstOrDefault(element => element.Name.LocalName == localName)?.Value;

    private static bool ReadBool(XContainer parent, string localName) =>
        bool.TryParse(ReadValue(parent, localName), out var value) && value;

    private static string? TryReadFault(string xml)
    {
        try { return TryReadFault(XDocument.Parse(xml)); }
        catch { return null; }
    }

    private static string? TryReadFault(XContainer document) =>
        document.Descendants().FirstOrDefault(element => element.Name.LocalName == "faultstring")?.Value;

    private static string Explain(Exception exception) => exception switch
    {
        TaskCanceledException => "El servicio tardó demasiado en responder. Verifica que esté iniciado e inténtalo nuevamente.",
        HttpRequestException => "No fue posible conectar con el servicio de conversión. Verifica tu conexión e inténtalo de nuevo.",
        _ => exception.Message
    };
}
