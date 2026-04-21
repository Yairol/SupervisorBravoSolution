using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using SupervisorBravo.Domain.Entities.Analysis;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Repository;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Registrar el DbContextFactory con PostgreSQL
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("connectionString"))
);

// Configuración de Kestrel
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(5000));

// Registrar repositorios como Scoped
builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<IAlarmRepository, AplicationRepository>();
builder.Services.AddScoped<IScheduledTaskExecutionLogRepository, AplicationRepository>();
builder.Services.AddScoped<IScheduledTaskRepository, AplicationRepository>();
builder.Services.AddScoped<IPLCDeviceRepository, AplicationRepository>();
builder.Services.AddScoped<IDataAnalysesRepository, AplicationRepository>();

// Ejemplo de registro adicional (si necesitas aliasar repositorios)
builder.Services.AddScoped<IScheduledTaskRepository>(provider =>
    (IScheduledTaskRepository)provider.GetRequiredService<IDixellRepository>());

// Configuración de licencia para EPPlus
ExcelPackage.License.SetNonCommercialPersonal("Bravo");

// Configuración de autenticación y autorización
builder.Services.AddAuthentication("MiCookieAuth")
    .AddCookie("MiCookieAuth", opts =>
    {
        opts.LoginPath = "/Login";
        opts.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    opts.AddPolicy("TecnicoOnly", policy => policy.RequireRole("Tecnico"));
});

// Compresión de respuestas
builder.Services.AddResponseCompression();

var app = builder.Build();

// Pipeline básico
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Migrar automáticamente si hay migraciones pendientes
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var db = factory.CreateDbContext();

    var applied = db.Database.GetAppliedMigrations().ToList();
    var all = db.Database.GetMigrations().ToList();

    var pending = all.Except(applied).ToList();

    if (pending.Any())
    {
        logger.LogInformation("Hay {Count} migración(es) pendiente(s). Ejecutando Migrate()...", pending.Count);
        db.Database.Migrate();
        logger.LogInformation("Migraciones pendientes aplicadas correctamente.");
    }
    else
    {
        logger.LogInformation("No hay migraciones pendientes. No se ejecuta Migrate().");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}"
);

app.Run();
