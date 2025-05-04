using OpenFun_CoreTests.Mock;

namespace OpenFun_Core.Models.Tests
{
    [TestClass()]
    public class DictionaryCacheTests
    {
        private DictionaryCache? dictionaryCache;

        public DictionaryCacheTests()
        {
            dictionaryCache = new DictionaryCache(new TestFileProvider());
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            await dictionaryCache!.LoadDictionaryAsync(Dictionaries.en_AU);
        }

        [TestMethod()]
        [DataRow(Dictionaries.en_AU)]
        public void LoadDictionaryTest(Dictionaries dictionary)
        {
            Assert.IsTrue(dictionaryCache!.IsDictionaryLoaded(dictionary));
        }

        [TestMethod()]
        [DataRow("test", true)]
        [DataRow("tests", true)]
        [DataRow("tester", true)]
        [DataRow("1", false)]
        [DataRow("z", false)]
        [DataRow(".", false)]
        [DataRow("1st", false)]
        [DataRow("nonexistant_word", false)]
        [DataRow("Australian", false)]
        [DataRow("ICC", false)]
        [DataRow("I'll", false)]
        public void CheckWordTest(string word, bool shouldExist)
        {
            Assert.AreEqual(shouldExist, dictionaryCache!.CheckWord(word).Result);
        }
    }
}