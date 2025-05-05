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

            Shuffle(letters, random);

            return letters;
        }

        /// <summary>
        /// Shuffle the list using the Fisher–Yates algorithm.
        /// </summary>
        private void Shuffle<T>(IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
    }
}
