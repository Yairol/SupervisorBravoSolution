using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.MSTest
{
    [TestClass]
    public class TemperatureTests
    {
        private ITemperatureRepository _temperaturesRepository;
        private ApplicationDbContext _context;

        public TemperatureTests()
        {
            _context = new ApplicationDbContext("Server=DESKTOP-J0L95CI\\SQLEXPRESS;Database=DBTest;Trusted_Connection=True;TrustServerCertificate=True;");    
            _temperaturesRepository = new AplicationRepository(_context);
        }

        [TestMethod]
        [DataRow(0, 25 )]
        [DataRow(0, 23 )]
        [DataRow(0, 25 )]
        [DataRow(0, 25 )]
        [DataRow(0, 20 )]
        
        public async Task Can_Create_Temperature(int pos, double temperatureValue)
        {
            await _temperaturesRepository.BeginTransaction();

            var dixells = await ((IDixellRepository)_temperaturesRepository).GetAllDixells<DixellXR60CX>();
            Assert.IsNotNull(dixells);

            var dixell = dixells.ElementAtOrDefault(pos);
            Assert.IsNotNull(dixell);

            var temperature = await _temperaturesRepository.CreateTemperature(temperatureValue, dixell.Id);
            Assert.IsNotNull(temperature);

            await _temperaturesRepository.CommitTransaction();

        }

        [TestMethod]
        [DataRow(2025, 4, 26, 12, 20, 0, 2025, 4, 26, 13, 24, 0)]
        public async Task Can_Get_Temperature_ByRangeDate(int yearStart, int monthStrat, int dayStart,int hourStart, int minuteStart, int secondStart, int yearEnd, int monthEnd, int dayEnd, int hourEnd, int minuteEnd, int secondEnd)
        {
            await _temperaturesRepository.BeginTransaction();

            DateTime dateTimeStart = new DateTime(yearStart, monthStrat, dayStart, hourStart, minuteStart, secondStart);
            DateTime dateTimeEnd = new DateTime(yearEnd, monthEnd, dayEnd, hourEnd, minuteEnd, secondEnd);

            var temeperatures = await _temperaturesRepository.GetTemperaturesByDateRange(dateTimeStart, dateTimeEnd);

            Assert.IsNotNull(temeperatures);
            Assert.AreEqual(temeperatures.Count, 4);
            Assert.IsTrue(temeperatures.Any());
        }
    }
}
