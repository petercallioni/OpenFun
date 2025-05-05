using OpenFun_Core.Extensions;
using OpenFun_Core.Models;

namespace Pangram.Models
{
    public class LetterSequence
    {
        private static readonly HashSet<char> Vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };

        IDailySeed dailySeed;
        DictionaryCache dictionaryCache;

        public LetterSequence(IDailySeed dailySeed, DictionaryCache dictionaryCache)
        {
            this.dailySeed = dailySeed;
            this.dictionaryCache = dictionaryCache;
        }

        public List<char> GetSequence(bool daily = false)
        {
            Random random = daily
                ? new Random(dailySeed.GetDailySeed())
                : new Random();

            List<string> candidates = dictionaryCache.RootWords().Result
                .Where(word => word.Length == 7 &&
                               word.All(char.IsLetter) &&
                               word.ToLower().Distinct().Count() == 7 &&
                               word.ToLower().Any(chars => Vowels.Contains(chars)))
                .ToList();

            string candidateWord = candidates[random.Next(candidates.Count)];

            List<char> letters = candidateWord.ToLower().ToList();

            letters.Shuffle(random);

            return letters;
        }
    }
}
