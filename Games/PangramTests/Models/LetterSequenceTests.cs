using OpenFun_Core.Models;
using OpenFun_CoreTests.Mock;

namespace Pangram.Models.Tests
{
    [TestClass()]
    public class LetterSequenceTests
    {
        private DictionaryCache dictionaryCache;
        public LetterSequenceTests()
        {
            dictionaryCache = new DictionaryCache(new TestFileProvider());
        }

        [TestMethod()]
        public void GetSequenceTest_SeedProducesDifferentResults()
        {
            IDailySeed dailySeed1 = new TestDailySeed(1);
            IDailySeed dailySeed2 = new TestDailySeed(2);

            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1).Letters;
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence(dailySeed2).Letters;

            Assert.IsFalse(letterSequence1.SequenceEqual(letterSequence2));
        }

        [TestMethod()]
        public void GetSequenceTest_CorrectCharacters()
        {
            IDailySeed seed = new TestDailySeed(1);

            List<char> letterSequence = new LetterSequence(dictionaryCache).GetSequence(seed).Letters;
            Assert.AreEqual(7, letterSequence.Distinct().ToList().Count); // 7 Unique characters
        }

        [TestMethod()]
        public void GetSequenceTest_SameSeedProducesSameResults()
        {
            IDailySeed dailySeed1 = new TestDailySeed(1);

            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1).Letters;
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1).Letters;

            Assert.IsTrue(letterSequence1.SequenceEqual(letterSequence2));
        }

        [TestMethod()]
        public void GetSequenceTest_RandomProducesRandomResults()
        {
            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence().Letters;
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence().Letters;

            Assert.IsFalse(letterSequence1.SequenceEqual(letterSequence2));
        }
    }
}