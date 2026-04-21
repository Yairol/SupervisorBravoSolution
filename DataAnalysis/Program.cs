using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.DataAnalysisProgram.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SupervisorBravo.DataAnalysisProgram
{
    class Program
    {
        private readonly IDixellRepository _dixellRepository;
        private readonly IDataAnalysesRepository _dataAnalyses;

        public Program(IDixellRepository dixellRepository, IDataAnalysesRepository dataAnalyses)
        {
            _dixellRepository = dixellRepository;
            _dataAnalyses = dataAnalyses;
        }

        static async Task Main(string[] args)
        {
            // ✅ Cargar configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            // ✅ Usar la cadena de conexión desde appsettings.json
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Registrar repositorios
            services.AddScoped<AplicationRepository>();
            services.AddScoped<IDixellRepository>(provider => provider.GetRequiredService<AplicationRepository>());
            services.AddScoped<IDataAnalysesRepository>(provider => provider.GetRequiredService<AplicationRepository>());

            var provider = services.BuildServiceProvider();
            var program = ActivatorUtilities.CreateInstance<Program>(provider);

            try
            {
                await ProcessingHelper.RunAnalysis(program._dixellRepository, program._dataAnalyses);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Se produjo un error durante la ejecución:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Presiona ENTER para salir...");
            Console.ReadLine();
        }
    }
}
