using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Repository;
using System.Threading.Tasks;

namespace SupervisorBravo.MSTest
{
    [TestClass]
    public class DixellTests
    {
        private readonly IDixellRepository _dixellRepository;
        private readonly ApplicationDbContext _context;

        public DixellTests()
        {
            _context = new ApplicationDbContext("Server=DESKTOP-J0L95CI\\SQLEXPRESS;Database=DBTest;Trusted_Connection=True;TrustServerCertificate=True;");
            _dixellRepository = new AplicationRepository(_context);
        }

        [TestMethod]
        [DataRow("Sala 1", 3)]
        public async Task Can_Create_Dixell(string roomName, int modbusId)
        {
            await _dixellRepository.BeginTransaction();

            var newDixell = await _dixellRepository.CreateDixellXR60CX(roomName, modbusId);
            await _dixellRepository.PartialCommit();

            var loadedDixell = await _dixellRepository.GetDixellById<DixellXR60CX>(newDixell.Id);
            await _dixellRepository.CommitTransaction();

            Assert.IsNotNull(loadedDixell);
            Assert.AreEqual(roomName, loadedDixell.RoomName);
            Assert.AreEqual(modbusId, loadedDixell.MoodbusId);
        }

        [TestMethod]
        [DataRow(2)]
        public async Task Can_Get_Dixell(int modbusId)
        {
            await _dixellRepository.BeginTransaction();
            var dixells = await _dixellRepository.GetAllDixells<DixellXR60CX>();

            var dixell = dixells.FirstOrDefault(d => d.MoodbusId ==  modbusId);
            await _dixellRepository.CommitTransaction();
            Assert.IsNotNull(dixell);
            
        }

        [TestMethod]
        [DataRow(0, 20,"Sala 10", false, 10 )]
        public async Task Can_Update_Dixell(int pos, double setPoint, string roomName, bool control,int modbusId)
        {
            await _dixellRepository.BeginTransaction();
            var dixells = await _dixellRepository.GetAllDixells<DixellXR60CX>();

            var dixell = dixells.ElementAtOrDefault(pos);

            Assert.IsNotNull(dixell);

            dixell.SetPoint = setPoint;
            dixell.RoomName = roomName;
            dixell.ControlON_OFF = control;
            dixell.MoodbusId = modbusId;

            await _dixellRepository.UpdateDixell<DixellXR60CX>(dixell);

            await _dixellRepository.PartialCommit();

            var dixellUpdates = await _dixellRepository.GetAllDixells<DixellXR60CX>();
            Assert.IsNotNull(dixellUpdates);

            await _dixellRepository.CommitTransaction();
            var newDixell = dixellUpdates.ElementAtOrDefault(pos);
            Assert.IsNotNull(newDixell);

            Assert.AreEqual(modbusId, newDixell.MoodbusId);
            Assert.AreEqual(setPoint, newDixell.SetPoint);
            Assert.AreEqual(control, newDixell.ControlON_OFF);
            Assert.AreEqual(roomName, newDixell.RoomName);

        }
    }
}
