using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ApplicationDbContext>(option => 
option.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"))
);
builder.Services.AddScoped<IDixellRepository, AplicationRepository>();
builder.Services.AddScoped<ITemperatureRepository, AplicationRepository>();

var host = builder.Build();
host.Run();
