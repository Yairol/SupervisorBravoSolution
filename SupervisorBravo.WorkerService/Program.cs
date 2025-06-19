using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.WorkerService;
using SupervisorBravo.WorkerService.Utilities;
using NpgsqlTypes;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

// Primero registrar todas las dependencias
builder.Services.AddDbContextFactory<ApplicationDbContext>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("connectionString"))
);

builder.Services.AddScoped<IDixellRepository, AplicationRepository>();

// Luego registrar el Worker que depende de los anteriores
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
