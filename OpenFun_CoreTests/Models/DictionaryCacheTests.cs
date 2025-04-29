using OpenFun_Core.Abstractions;

namespace OpenFun_Core.Models.Tests
{
    [TestClass()]
    public class DictionaryCacheTests
    {
        [TestMethod()]
        public void ReadDictionaryTest()
        {
            DictionaryCache dictionaryReader = new DictionaryCache(new TestFileProvider());
            Assert.IsTrue(dictionaryReader.ReadDictionary(Dictionaries.en_AU).Result);
        }
    }
}