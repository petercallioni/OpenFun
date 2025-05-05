using OpenFun_Core.Extensions;
using OpenFun_Core.Models;

namespace Pangram.Models
{
    public class LetterSequence
    {
        private static readonly HashSet<char> Vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };

        DictionaryCache dictionaryCache;

        public LetterSequence(DictionaryCache dictionaryCache)
        {
            this.dictionaryCache = dictionaryCache;
        }

        public List<char> GetSequence(IDailySeed? dailySeed = null)
        {
            Random random = dailySeed != null
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
