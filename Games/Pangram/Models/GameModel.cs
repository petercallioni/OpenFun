using OpenFun_Core.Abstractions;
using OpenFun_Core.Models;

namespace Pangram.Models
{
    public class GameModel
    {
        private WordLetterSequence? wordLetterSequence;
        private List<string>? words;
        private int score;
        private IAppFileProvider appFileProvider;
        private IDailySeed dailySeed;
        private DictionaryCache dictionaryCache;

        public int Score { get => score; set => score = value; }
        public WordLetterSequence? WordLetterSequence { get => wordLetterSequence; set => wordLetterSequence = value; }
        public List<string>? GuessedWords { get => words; }

        public GameModel(IAppFileProvider appFileProvider, IDailySeed dailySeed)
        {
            this.appFileProvider = appFileProvider;
            this.dailySeed = dailySeed;
            WordLetterSequence = null;
            dictionaryCache = null!;
            words = null;
            score = 0;
        }

        public async Task InitialiseGame(bool daily)
        {
            dictionaryCache = new DictionaryCache(appFileProvider);

            await dictionaryCache.LoadDictionaryAsync();

            WordLetterSequence = new LetterSequence(
                dictionaryCache)
                .GetSequence(daily ? dailySeed : null);
            words = new List<string>();
        }

        public async Task<GuessWordResults> GuessWord(string word)
        {
            word = word.ToLower(); // Convert to lowercase for consistency

            if (words == null || WordLetterSequence == null || dictionaryCache == null)
            {
                throw new InvalidOperationException("Game not initialised.");
            }

            // This should never happen, but just in case.
            if (word.ToCharArray().Any(c => !WordLetterSequence.Letters.Contains(c)))
            {
                return GuessWordResults.FORBIDDEN_CHARACTERS;
            }

            if (!word.Contains(WordLetterSequence.Letters[0]))
            {
                return GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER;
            }

            if (words!.Contains(word))
            {
                return GuessWordResults.ALREADY_GUESSED;
            }

            if (await dictionaryCache.CheckWord(word))
            {
                score++;
                words.Add(word);
                return GuessWordResults.VALID;
            }
            else
            {
                return GuessWordResults.INVALID;
            }
        }
    }
}
