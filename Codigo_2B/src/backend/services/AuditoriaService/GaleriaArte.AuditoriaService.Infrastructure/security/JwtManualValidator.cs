using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

public class JwtManualValidator
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtManualValidator(string secret, string issuer, string audience)
    {
        _secret = secret;
        _issuer = issuer;
        _audience = audience;
    }

    public bool ValidarToken(string token, out Dictionary<string, object> claims, out string error)
    {
        claims = new Dictionary<string, object>();
        error = string.Empty;

        var partes = token.Split('.');
        if (partes.Length != 3)
        {
            error = "El token no tiene las 3 partes estándar.";
            return false;
        }

        var header = partes[0];
        var payload = partes[1];
        var signature = partes[2];

        var payloadJson = Encoding.UTF8.GetString(ParseBase64(payload));
        
        using var doc = JsonDocument.Parse(payloadJson);
        var payloadDes = doc.RootElement;

        claims = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson)!;

        // Validar firma
        var expectedSignature = CrearFirma($"{header}.{payload}");
        if (!SeguroComparar(signature, expectedSignature))
        {
            error = "Firma inválida.";
            return false;
        }

        // Validar exp
if (!payloadDes.TryGetProperty("exp", out var expElement) || !expElement.TryGetInt64(out var exp))
{
    error = "Claim 'exp' inválido o faltante.";
    return false;
}
var expTime = DateTimeOffset.FromUnixTimeSeconds(exp);
if (expTime < DateTimeOffset.UtcNow)
{
    error = "Token expirado.";
    return false;
}

// Validar aud
if (!payloadDes.TryGetProperty("aud", out var audElement) || audElement.GetString() != _audience)
{
    error = "Audiencia inválida.";
    return false;
}

// Validar iss
if (!payloadDes.TryGetProperty("iss", out var issElement) || issElement.GetString() != _issuer)
{
    error = "Emisor inválido.";
    return false;
}

        return true;
    }

    private static byte[] ParseBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
            case 1: base64 += "==="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private string CrearFirma(string texto)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_secret);
        var textoBytes = Encoding.UTF8.GetBytes(texto);

        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(textoBytes);
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private bool SeguroComparar(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];

        return result == 0;
    }
}
