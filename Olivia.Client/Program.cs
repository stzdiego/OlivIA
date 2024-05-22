using Microsoft.Extensions.Options;
using Olivia.Client.Components;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();

////OliviaApi
builder.Services.Configure<OliviaApiSettings>(builder.Configuration.GetSection(nameof(OliviaApiSettings)));
builder.Services.AddSingleton<IOliviaApiSettings>(s => s.GetRequiredService<IOptions<OliviaApiSettings>>().Value);

builder.Services.AddHttpClient("OliviaApi", (s, c) =>
{
    var settings = s.GetRequiredService<IOliviaApiSettings>();
    c.BaseAddress = new Uri(settings.Url);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
