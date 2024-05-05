// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#pragma warning disable SA1200 // UsingDirectivesMustBePlacedWithinNamespace
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Olivia.AI.Agents;
using Olivia.AI.Plugins;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Interfaces;

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
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<DoctorService>();
builder.Services.AddScoped<ProgramationService>();
builder.Services.AddScoped<IDatabase, DatabaseService>();

////Plugins
builder.Services.AddScoped<PatientsManagerPlugin>();
builder.Services.AddScoped<DoctorsManagerPlugin>();
builder.Services.AddScoped<ProgramationManagerPlugin>();

////Agents
builder.Services.AddScoped<OpenAIAgent>();

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