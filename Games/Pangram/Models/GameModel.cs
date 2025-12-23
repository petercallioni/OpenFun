using OpenFun_Core.Abstractions;
using OpenFun_Core.Models;

namespace Pangram.Models
{
    public class GameModel
    {
        private WordLetterSequence? wordLetterSequence;
        private List<string>? words;
        private int score;
        private int maxScore;
        private bool isDaily;
        private DateTime createdDate;

        private string? foundPangramWord;

        private IAppFileProvider appFileProvider;
        private IDailySeed dailySeed;
        private DictionaryCache dictionaryCache;
        public int Score { get => score; set => score = value; }
        public WordLetterSequence? WordLetterSequence { get => wordLetterSequence; set => wordLetterSequence = value; }
        public List<string>? GuessedWords { get => words; }
        public int MaxScore { get => maxScore; }
        public bool IsDaily { get => isDaily; }
        public string? FoundPangramWord { get => foundPangramWord; }
        public DateTime CreatedDate { get => createdDate; }

        public GameModel(IAppFileProvider appFileProvider, IDailySeed dailySeed)
        {
            this.appFileProvider = appFileProvider;
            this.dailySeed = dailySeed;
            WordLetterSequence = null;
            dictionaryCache = null!;
            words = null;
            maxScore = 0;
            score = 0;
        }

        public void ForfeitGame()
        {
            foundPangramWord = wordLetterSequence!.Word;
        }

        public async Task LoadSavedGame(PangramData data)
        {
            dictionaryCache ??= new DictionaryCache(appFileProvider);
            maxScore = data.MaxScore;
            words = data.GetGuessedWordsList();
            score = words.Count;
            createdDate = data.Date;

            isDaily = data.IsDaily;
            await dictionaryCache.LoadDictionaryAsync();

            WordLetterSequence = new WordLetterSequence(
                data.LetterSequence.ToLower().ToList(),
                data.Word.ToLower());

            if (data.GotPangram)
            {
                foundPangramWord = data.Word;
            }
            else
            {
                foundPangramWord = null;
            }
        }

        public async Task InitialiseGame(bool daily)
        {
            dictionaryCache ??= new DictionaryCache(appFileProvider);
            maxScore = 0;
            score = 0;
            foundPangramWord = null;
            isDaily = daily;
            createdDate = DateTime.UtcNow;

            await dictionaryCache.LoadDictionaryAsync();

            WordLetterSequence = new LetterSequence(
                dictionaryCache)
                .GetSequence(daily ? dailySeed : null);
            words = new List<string>();
        }

        public async Task<int> FindMaxWords()
        {
            if (maxScore != 0)
            {
                return maxScore;
            }

            maxScore = await new LetterSequence(
                 dictionaryCache)
                 .FindMaxWords(WordLetterSequence!);

            return maxScore;
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

                // If the word contains all letters, it's a pangram
                if (WordLetterSequence.Letters.All(c => word.Contains(c)))
                {
                    foundPangramWord = word;
                    return GuessWordResults.VALID_PANGRAM;
                }

                return GuessWordResults.VALID;
            }
            else
            {
                return GuessWordResults.INVALID;
            }
        }
    }
}
