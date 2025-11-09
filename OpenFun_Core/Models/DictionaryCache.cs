using OpenFun_Core.Abstractions;
using System.Text.RegularExpressions;
using WeCantSpell.Hunspell;

namespace OpenFun_Core.Models
{
    public class DictionaryCache(IAppFileProvider appFileProvider)
    {
        private readonly IAppFileProvider appFileProvider = appFileProvider;

        private WordList? wordList;
        private Dictionaries? loadedDictionary;

        public async Task<bool> CheckWord(string word)
        {
            if (wordList == null)
            {
                await LoadDictionaryAsync();
            }

            bool isValid = true;

            Regex[] regexes =
            {
                new Regex(@"^[A-Z]"),       // Proper noun
                new Regex(@"\d"),           // Contains digit
                new Regex(@"[A-Z]{2,}"),    // Consecutive capital letters
                new Regex(@"[']")           // Forbidden characters
            };

            if (word.Length < 2)
            {
                isValid = false;
            }
            else if (regexes.Any(x => x.IsMatch(word)))
            {
                isValid = false;
            }
            else
            {
                isValid = wordList!.Check(word); // Null overriden as dictionary is loaded from LoadDictionaryAsync()
            }

            return isValid;
        }

        public async Task<IEnumerable<string>> RootWords()
        {
            if (wordList == null)
            {
                await LoadDictionaryAsync();
            }

            return wordList!.RootWords; // Null overriden as dictionary is loaded from LoadDictionaryAsync()
        }

        public async Task LoadDictionaryAsync(Dictionaries dictionary = Dictionaries.en_AU) // Australian dictionary is default
        {
            if (loadedDictionary == dictionary)
                return;

            loadedDictionary = null;

            string dic = Enum.GetName(dictionary)!.Replace('_', '-');

            // Paths to the dictionary files in the app package
            string dicFilePath = $"Dictionaries/{dic}/index.dic";
            string affFilePath = $"Dictionaries/{dic}/index.aff";

            // Load the .dic file content
            using (Stream dicStream = await appFileProvider.OpenAppPackageFileAsync(dicFilePath))
            using (Stream affStream = await appFileProvider.OpenAppPackageFileAsync(affFilePath))
            using (StreamReader dicStreamReader = new StreamReader(dicStream))
            {

                wordList = await WordList.CreateFromStreamsAsync(dicStream, affStream);
            }

            loadedDictionary = dictionary;
        }

        public bool IsDictionaryLoaded(Dictionaries dictionary)
        {
            return loadedDictionary == dictionary;
        }

        public Dictionaries? GetCurrentlyLoadedDictionary()
        {
            return loadedDictionary;
        }
    }
}