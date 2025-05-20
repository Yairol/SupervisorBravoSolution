using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.WorkerService;
using SupervisorBravo.WorkerService.Utilities;

var builder = Host.CreateApplicationBuilder(args);

// Primero registrar todas las dependencias
builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"))
);

builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<ITemperatureRepository, AplicationRepository>();
builder.Services.AddScoped<IAlarmRepository, AplicationRepository>();
builder.Services.AddScoped<IModbusService, ModbusService>();


// Luego registrar el Worker que depende de los anteriores
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
