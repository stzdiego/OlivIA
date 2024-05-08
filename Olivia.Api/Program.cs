// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Olivia.AI.Agents;
using Olivia.AI.Plugins;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Olivia API", Version = "v0.1" });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<DbContext, OliviaDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Olivia.Api")));

////Services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IProgramationService, ProgramationService>();
builder.Services.AddScoped<IDatabase, DatabaseService>();

////Plugins
builder.Services.AddScoped<PatientsManagerPlugin>();
builder.Services.AddScoped<DoctorsManagerPlugin>();
builder.Services.AddScoped<ProgramationManagerPlugin>();

////OpenAI Agent
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection(nameof(OpenAISettings)));
builder.Services.AddScoped<IAgentSettings>(s => s.GetRequiredService<IOptions<OpenAISettings>>().Value);
builder.Services.AddScoped<IAgent, OpenAIAgent>();

////Google Calendar
builder.Services.Configure<GoogleCalendarSettings>(builder.Configuration.GetSection(nameof(GoogleCalendarSettings)));
builder.Services.AddSingleton<IGoogleCalendarSettings>(s => s.GetRequiredService<IOptions<GoogleCalendarSettings>>().Value);
builder.Services.AddScoped<ICalendarService, GoogleCalendarService>();

var app = builder.Build();

////Execute migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<OliviaDbContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Olivia Api");
});

app.MapControllers();

app.Run();