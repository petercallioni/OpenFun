using OpenFun_Core.Extensions;
using OpenFun_Core.Models;

namespace Pangram.Models
{
    public partial class LetterSequence
    {
        DictionaryCache dictionaryCache;

        public LetterSequence(DictionaryCache dictionaryCache)
        {
            this.dictionaryCache = dictionaryCache;
        }


        public async Task<int> FindMaxWords(WordLetterSequence wordLetterSequence)
        {
            if (dictionaryCache == null)
            {
                throw new InvalidOperationException("Dictionary cache is not initialized.");
            }

            List<string> words = (await dictionaryCache.RootWords())
                .Select(w => w.ToLowerInvariant())
                .ToList();

            Dictionary<string, bool> filteredWords = new Dictionary<string, bool>();

            words
                .Where(word => IsValidWord(word, wordLetterSequence))
                .Where(word => dictionaryCache.CheckWord(word).Result)
                .ToList()
                .ForEach(word => filteredWords.TryAdd(word, true));

            int count = filteredWords.Count;

            return count;
        }

        private bool IsValidWord(string word, WordLetterSequence wordLetterSequence)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            // Ensure the word contains the first character of the sequence
            char firstChar = wordLetterSequence.Letters[0];
            if (!word.Contains(firstChar))
                return false;

            // Ensure the word contains at least one other character from the sequence
            bool containsOtherCharacter = wordLetterSequence.Letters
                .Skip(1) // Skip the first character
                .Any(letter => word.Contains(letter));

            if (!containsOtherCharacter)
                return false;

            // Ensure the word only contains characters from the sequence
            HashSet<char> sequenceSet = new HashSet<char>(wordLetterSequence.Letters);
            if (word.Any(c => !sequenceSet.Contains(c)))
                return false;

            return true;
        }

        public WordLetterSequence GetSequence(IDailySeed? dailySeed = null)
        {
            Random random = dailySeed != null
                ? new Random(dailySeed.GetDailySeed())
                : new Random();

            List<string> words = dictionaryCache.RootWords().Result
                .Select(w => w.ToLowerInvariant())
                .Where(w => w.Length >= 7)
                .ToList();

            words.Shuffle(random);

            while (true)
            {
                // Generate a random sequence of 7 alphabetical characters.
                string validSequence = GenerateRandomSequence(7, random);

                // Find the first word from the dictionary that can be composed
                // from the letters in validSequence.
                string? validWord = words
                    .AsParallel()
                    .FirstOrDefault(word => CanConstructWordFromLetters(word, validSequence)
                                                  && dictionaryCache.CheckWord(word).Result);

                if (validWord != null)
                {
                    Console.WriteLine("Valid sequence found: " + validSequence + " with word: " + validWord);
                    // Return the record with the found word and the character list.
                    return new WordLetterSequence(validSequence.ToList(), validWord);
                }
                // If no valid word is found, loop again with a new sequence.
            }
        }

        // Generates a random sequence of 'length' characters from a-z.
        private static string GenerateRandomSequence(int length, Random random)
        {
            List<char> avaliableLetters = new List<char>("abcdefghijklmnopqrstuvwxyz");
            List<char> letters = new List<char>();

            while (letters.Count < length)
            {
                // Generate a random index to select a letter from the available letters.
                int index = random.Next(avaliableLetters.Count);
                char letter = avaliableLetters[index];

                letters.Add(letter);
                avaliableLetters.RemoveAt(index); // Remove the letter to avoid duplicates.
            }

            return new string(letters.ToArray());
        }

        // Checks whether 'word' contains all letters (with required counts) from 'sequence'.
        private static bool CanConstructWordFromLetters(string word, string sequence)
        {
            // Build a frequency dictionary for the letters in the sequence.
            Dictionary<char, int> sequenceLetterCounts = sequence.GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

            // Contains a letter not in the sequence
            if (word.Any(c => !sequenceLetterCounts.ContainsKey(c)))
            {
                return false;
            }

            foreach (KeyValuePair<char, int> kvp in sequenceLetterCounts)
            {
                char letter = kvp.Key;
                int requiredCount = kvp.Value;

                // Count how many times 'letter' appears in 'word'.
                int wordCount = word.Count(c => c == letter);

                // If the word doesn't have enough occurrences, it doesn't qualify.
                if (wordCount < requiredCount)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

