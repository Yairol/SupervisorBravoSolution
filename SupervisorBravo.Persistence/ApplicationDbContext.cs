using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.FluentConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<DixellBase> DixellBases { get; set; }
        public DbSet<DixellXR60CX> DixellXR60CXs {  get; set; }
        public DbSet<Temperature> Temperatures { get; set; }

        public ApplicationDbContext() { }

        public ApplicationDbContext(string connectionString)
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-J0L95CI\\SQLEXPRESS;Database=DBTest;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DixellBaseFluentConfiguration());
            modelBuilder.ApplyConfiguration(new DixellXR60CFluentConfiguration());
            modelBuilder.ApplyConfiguration(new TemperatureFluentConfiguration());
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder (), connectionString).Options;
        }

    }
}
