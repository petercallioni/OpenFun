using OpenFun_Core.Services;

namespace Pangram.Models.Tests
{
    [TestClass]
    public class PangramDataDatabaseTests
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private DatabaseService _databaseService;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private const string TestDatabaseName = "PangramTest.db";

        [TestInitialize]
        public void Setup()
        {
            _databaseService = new DatabaseService(TestDatabaseName);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await _databaseService.DeleteAllAsync<PangramData>();
        }

        [TestMethod]
        public async Task SaveAndRetrieve_PangramData_Works()
        {
            // Arrange
            DateTime now = System.DateTime.UtcNow;
            PangramData data = new PangramData(now, "abcdefg", "example") { MaxScore = 10, GotPangram = true };

            // Act
            await _databaseService.SaveAsync(data);
            List<PangramData> all = (await _databaseService.RetrieveAllAsync<PangramData>()).ToList();

            // Assert
            Assert.AreEqual(1, all.Count);
            PangramData retrieved = all.First();
            Assert.AreEqual("example", retrieved.Word);
            Assert.AreEqual(10, retrieved.MaxScore);
            Assert.IsTrue(retrieved.GotPangram);
        }

        [TestMethod]
        public async Task DeleteAll_ClearsTable()
        {
            // Arrange
            await _databaseService.SaveAsync(new PangramData(System.DateTime.UtcNow, "abcdefg", "one"));
            await _databaseService.SaveAsync(new PangramData(System.DateTime.UtcNow, "hijklmn", "two"));

            // Act
            await _databaseService.DeleteAllAsync<PangramData>();
            List<PangramData> all = (await _databaseService.RetrieveAllAsync<PangramData>()).ToList();

            // Assert
            Assert.AreEqual(0, all.Count);
        }
    }
}
