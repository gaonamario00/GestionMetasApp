using Blazored.Toast;
using GestionMetasApp.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("GestionMetasApp.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("GestionMetasApp.ServerAPI"));

builder.Services.AddBlazoredToast();

// Leer la URL de la API desde appsettings.json
var apiUrl = builder.Configuration["Api"];

// Registrar el HttpClient con la URL de la API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiUrl) // Aquí asignamos la URL de la API
});

// Set culture to day/month/year (e.g., "es-MX" or "es-ES")
var host = builder.Build();

var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");

CultureInfo culture;
if (result != null)
{
    culture = new CultureInfo(result);
}
else
{
    culture = new CultureInfo("es-MX"); // Set to Mexico's culture for day/month/year format
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await builder.Build().RunAsync();
