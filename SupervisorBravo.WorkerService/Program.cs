using Microsoft.EntityFrameworkCore;
using Npgsql;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.WorkerService;

var builder = Host.CreateApplicationBuilder(args);

// --------------------------------------------------
// 1) Forzar carga de appsettings.json y variables de entorno
// --------------------------------------------------
builder.Configuration
       .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();

// --------------------------------------------------
// 2) Opciones del Host
// --------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

// --------------------------------------------------
// 3) Debug: Directorio de trabajo y listado de archivos
// --------------------------------------------------
Console.WriteLine($"[CONFIG CHECK] WorkingDirectory: {Environment.CurrentDirectory}");
Console.WriteLine("[CONFIG CHECK] Files in working directory:");
foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
    Console.WriteLine("  • " + Path.GetFileName(file));
Console.WriteLine("--------------------------------------------------");

// --------------------------------------------------
// 4) Debug: Dump de configuración
// --------------------------------------------------
Console.WriteLine("==== CONFIGURATION KEYS ====");
foreach (var kv in builder.Configuration.AsEnumerable().Where(kv => !string.IsNullOrEmpty(kv.Value)))
    Console.WriteLine($"{kv.Key} = {kv.Value}");
Console.WriteLine("============================");

// --------------------------------------------------
// 5) Extraer y validar la cadena de conexión
// --------------------------------------------------
var connStr = builder.Configuration.GetConnectionString("connectionString");
if (string.IsNullOrWhiteSpace(connStr))
{
    Console.WriteLine("ERROR: La cadena de conexión 'connectionString' es nula o vacía.");
    Environment.Exit(-1);
}

// --------------------------------------------------
// 6) Prueba de conexión directa con Npgsql
// --------------------------------------------------
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

// --------------------------------------------------
// 7) Registrar DbContextFactory y repositorios
// --------------------------------------------------
builder.Services.AddDbContextFactory<ApplicationDbContext>(opts =>
    opts.UseNpgsql(connStr)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information)
);

builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<IAlarmRepository, AplicationRepository>();
builder.Services.AddScoped<IScheduledTaskRepository, AplicationRepository>();

// (Añade aquí otros repos, e.g. ITemperatureRepository, si los necesitas)

builder.Services.AddHostedService<ModbusWorker>();

// --------------------------------------------------
// 8) Construir y ejecutar el Host
// --------------------------------------------------
var host = builder.Build();
host.Run();
