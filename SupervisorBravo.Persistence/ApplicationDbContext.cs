using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Analysis;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.FluentConfigurations;


namespace SupervisorBravo.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<DixellBase> DixellBases { get; set; }
        public DbSet<DixellXR> DixellXR60CXs { get; set; }
        public DbSet<DixellXT> DixellXT111Cs { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<DeviceAlarm> DeviceAlarms { get; set; }
        public DbSet<ScheduledTask> ScheduledTasks { get; set; }
        public DbSet<ScheduledTaskExecutionLog> ScheduledTaskExecutionLogs { get; set; }
        public DbSet<PLCDevice> PLCDevices { get; set; }
        public DbSet<PLCAnalogVariable> AnalogVariables { get; set; }
        public DbSet<PLCDigitalVariable> DigitalVariables { get; set; }
        public DbSet<AnalogMeasurement> AnalogMeasurements { get; set; }
        public DbSet<DigitalMeasurement> DigitalMeasurements { get; set; }
        public DbSet<DataAnalysis> DataAnalyses { get; set; }
        public DbSet<AnalyzedData> AnalyzedData { get; set; }

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
            modelBuilder.ApplyConfiguration(new ScheduledTaskFluentConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduledTaskExecutionLogFluentConfiguration());
            modelBuilder.ApplyConfiguration(new AnalogMeasurementFluentConfiguration());
            modelBuilder.ApplyConfiguration(new DigitalMeasurementFluentConfiguration());
            modelBuilder.ApplyConfiguration(new PLCAnalogVariableFluentConfiguration());
            modelBuilder.ApplyConfiguration(new PLCDigitalVariableFLuentConfiguration());
            modelBuilder.ApplyConfiguration(new PLCDeviceFluentConfiguration());
            modelBuilder.ApplyConfiguration(new PLCVariableFluentConfiguration());

        }
    }
}
