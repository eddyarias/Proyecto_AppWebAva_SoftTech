using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GaleriaArteFrontend;
using GaleriaArteFrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurar HttpClient con la URL base del API Gateway
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000/") };
    // Configurar para incluir cookies en las requests
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    return httpClient;
});

// Registro de servicios personalizados
builder.Services.AddScoped<ObrasService>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();

