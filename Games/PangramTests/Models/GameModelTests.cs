using OpenFun_CoreTests.Mock;

namespace Pangram.Models.Tests
{
    [TestClass()]
    public class GameModelTests
    {
        private GameModel gameModel;
        public GameModelTests()
        {
            // Result of seed 1 'stnidug'  / studding
            gameModel = new GameModel(new TestFileProvider(), new TestDailySeed(1));
            gameModel.InitialiseGame(true).Wait(); // Simulate daily to keep results the same.
        }

        [TestMethod()]
        [DataRow("a", GuessWordResults.FORBIDDEN_CHARACTERS)]
        [DataRow("dug", GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER)]
        [DataRow("ssssss", GuessWordResults.INVALID)]
        [DataRow("stun", GuessWordResults.VALID)]
        [DataRow("stun", GuessWordResults.ALREADY_GUESSED)]
        public void GuessWordTest(string wordGuess, GuessWordResults expectedResult)
        {
            if (expectedResult == GuessWordResults.ALREADY_GUESSED)
            {
                gameModel.GuessWord(wordGuess).Wait(); // Add the word to the list first
            }

            GuessWordResults result = gameModel.GuessWord(wordGuess).Result;
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod()]
        public void IncrementScoreTest()
        {
            gameModel.GuessWord("stun").Wait();
            gameModel.GuessWord("stud").Wait();
            gameModel.GuessWord("test").Wait();
            Assert.AreEqual(2, gameModel.Score);
        }

        [TestMethod()]
        // Enusres that the actual pangram is correct
        public void EnsurePangramIsCorrect()
        {
            string pangram = gameModel.WordLetterSequence!.Word;
            Assert.AreEqual(GuessWordResults.VALID, gameModel.GuessWord(pangram).Result);
        }
    }
}