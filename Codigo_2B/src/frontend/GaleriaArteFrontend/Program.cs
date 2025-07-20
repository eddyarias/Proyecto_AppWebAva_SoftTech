using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GaleriaArteFrontend;
using GaleriaArteFrontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("http://localhost:5003/") });

// Registro de servicios personalizados
builder.Services.AddScoped<ObrasService>();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();

