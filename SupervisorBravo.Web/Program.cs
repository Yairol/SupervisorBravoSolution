using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Repository;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("connectionString"))
);
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(5000));
builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<IAlarmRepository, AplicationRepository>();
ExcelPackage.License.SetNonCommercialPersonal("Bravo");

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

builder.Services.AddResponseCompression();

var app = builder.Build();

// Pipeline básico
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Sólo migrar si NO hay ninguna migración aplicada
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var db = factory.CreateDbContext();

    var applied = db.Database.GetAppliedMigrations();
    if (!applied.Any())
    {
        logger.LogInformation("No existen migraciones aplicadas. Ejecutando Migrate()...");
        db.Database.Migrate();
        logger.LogInformation("Migraciones aplicadas correctamente.");
    }
    else
    {
        logger.LogInformation("Ya hay {Count} migración(es) aplicada(s). No se ejecuta Migrate().", applied.Count());
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
