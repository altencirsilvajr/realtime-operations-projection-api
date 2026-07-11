using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RealtimeOperationsProjection.Dashboard;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("http://localhost:5308/") });
builder.Services.AddScoped<OperationsApiClient>();
builder.Services.AddScoped<RealtimeOperationsClient>();

await builder.Build().RunAsync();
