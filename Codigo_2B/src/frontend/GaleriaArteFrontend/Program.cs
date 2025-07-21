using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GaleriaArteFrontend;
using GaleriaArteFrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar el handler personalizado para incluir cookies
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

// Configurar HttpClient con la URL base del API Gateway y con cookies
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<CustomAuthorizationMessageHandler>();
    var httpClient = new HttpClient(handler)
    {
        BaseAddress = new Uri("http://localhost:5000/")
    };
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    return httpClient;
});

// Registro de servicios personalizados
builder.Services.AddScoped<ObrasService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JwtService>();


await builder.Build().RunAsync();


// Handler personalizado para habilitar cookies en las peticiones
public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    public CustomAuthorizationMessageHandler()
    {
        InnerHandler = new HttpClientHandler();
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestOption("credentials", "include");
        return base.SendAsync(request, cancellationToken);
    }
}

// Extensión para configurar opciones del navegador
public static class HttpRequestMessageExtensions
{
    public static void SetBrowserRequestOption(this HttpRequestMessage request, string optionName, object value)
    {
        request.Properties[$"WebAssemblyFetchOptions:{optionName}"] = value;
    }
}
