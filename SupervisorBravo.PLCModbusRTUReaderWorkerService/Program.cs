using System;
using System.IO;
using System.Linq;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.PLCModbusRTUReaderWorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);

// 1) Configuración
builder.Configuration
       .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();

// 2) Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

// 3) Validación de archivos y configuración
Console.WriteLine($"[CONFIG CHECK] WorkingDirectory: {Environment.CurrentDirectory}");
foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
    Console.WriteLine("  • " + Path.GetFileName(file));

Console.WriteLine("==== CONFIGURATION KEYS ====");
foreach (var kv in builder.Configuration.AsEnumerable().Where(kv => !string.IsNullOrEmpty(kv.Value)))
    Console.WriteLine($"{kv.Key} = {kv.Value}");
Console.WriteLine("============================");

// 4) Validar cadena de conexión
var connStr = builder.Configuration.GetConnectionString("connectionString");
if (string.IsNullOrWhiteSpace(connStr))
{
    Console.WriteLine("ERROR: La cadena de conexión 'connectionString' es nula o vacía.");
    Environment.Exit(-1);
}

// 5) Prueba directa a PostgreSQL
try
{
    using var testConn = new NpgsqlConnection(connStr);
    testConn.Open();
    Console.WriteLine("[DB TEST] Conexión a PostgreSQL OK");
}
catch (Exception ex)
{
    Console.WriteLine("[DB TEST] Error al conectar a la BD:");
    Console.WriteLine(ex.Message);
    Environment.Exit(-1);
}

// 6) Registrar DbContextFactory
builder.Services.AddDbContextFactory<ApplicationDbContext>(opts =>
    opts.UseNpgsql(connStr)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information));

// ✅ 7) Registrar repositorios requeridos para el Worker
builder.Services.AddScoped<IScheduledTaskRepository, AplicationRepository>();
builder.Services.AddScoped<IScheduledTaskExecutionLogRepository, AplicationRepository>();
builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<IPLCDeviceRepository, AplicationRepository>();
builder.Services.AddScoped<IAnalogMeasurementRepository, AplicationRepository>();
builder.Services.AddScoped<IAnalogVariableRepository, AplicationRepository>();
builder.Services.AddScoped<IDigitalMeasurementRepository, AplicationRepository>();
builder.Services.AddScoped<IDigitalVariableRepository, AplicationRepository>();

// Servicio Modbus como Singleton (no depende de DbContext)
builder.Services.AddSingleton<IModbusReaderService, ModbusReaderService>();

// 8) Registrar Worker
builder.Services.AddHostedService<Worker>();

// 9) Ejecutar Host
var host = builder.Build();
host.Run();
