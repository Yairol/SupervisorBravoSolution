using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.FluentConfigurations;
using Npgsql.EntityFrameworkCore.PostgreSQL;


namespace SupervisorBravo.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DixellBase> DixellBases { get; set; }
        public DbSet<DixellXR60CX> DixellXR60CXs { get; set; }
        public DbSet<DixellXT111C> DixellXT111Cs { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<DeviceAlarm> DeviceAlarms { get; set; }

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=DBTest;User Id=postgres;Password=yairol123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DixellBaseFluentConfiguration());
            modelBuilder.ApplyConfiguration(new DixellXR60CFluentConfiguration());
            modelBuilder.ApplyConfiguration(new TemperatureFluentConfiguration());
            modelBuilder.ApplyConfiguration(new DixellXT111CFluentConfiguration());
            modelBuilder.ApplyConfiguration(new AlarmFluentConfiguration());
            modelBuilder.ApplyConfiguration(new DeviceAlarmFluentConfiguration());
        }
    }
}
