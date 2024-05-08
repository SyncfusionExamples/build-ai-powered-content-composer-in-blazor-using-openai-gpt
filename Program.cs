using AIContentComposer;
using AIContentComposer.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Licensing;

/* Refer here https://blazor.syncfusion.com/documentation/getting-started/license-key/how-to-generate */
SyncfusionLicenseProvider.RegisterLicense("Your Syncfusion License Key");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSyncfusionBlazor();
//await builder.Build().RunAsync();

OpenAIService svc = new OpenAIService();
builder.Services.AddSingleton<OpenAIService>(svc);

var app = builder.Build();
var config = builder.Configuration;
var openAIKey = config["OpenAIKey"];
var openAIEndpoint = config["OpenAIEndpoint"];
svc.Initialize(openAIKey, openAIEndpoint);
await app.RunAsync();
