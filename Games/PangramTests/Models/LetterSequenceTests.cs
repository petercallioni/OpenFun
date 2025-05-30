﻿using OpenFun_Core.Models;
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

            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1);
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence(dailySeed2);

            Assert.IsFalse(letterSequence1.SequenceEqual(letterSequence2));
        }

        [TestMethod()]
        public void GetSequenceTest_SameSeedProducesSameResults()
        {
            IDailySeed dailySeed1 = new TestDailySeed(1);

            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1);
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence(dailySeed1);

            Assert.IsTrue(letterSequence1.SequenceEqual(letterSequence2));
        }

        [TestMethod()]
        public void GetSequenceTest_RandomProducesRandomResults()
        {
            List<char> letterSequence1 = new LetterSequence(dictionaryCache).GetSequence();
            List<char> letterSequence2 = new LetterSequence(dictionaryCache).GetSequence();

            Assert.IsFalse(letterSequence1.SequenceEqual(letterSequence2));
        }
    }
}