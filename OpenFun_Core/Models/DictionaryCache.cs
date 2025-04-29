using OpenFun_Core.Abstractions;
using WeCantSpell.Hunspell;

namespace OpenFun_Core.Models
{
    public class DictionaryCache(IAppFileProvider appFileProvider)
    {
        private readonly IAppFileProvider appFileProvider = appFileProvider;

        public async Task<bool> ReadDictionary(Dictionaries dictionaries)
        {
            string dic = Enum.GetName(dictionaries)!.Replace('_', '-');

            // Paths to the dictionary files in the app package
            string dicFilePath = $"Dictionaries/{dic}/index.dic";
            string affFilePath = $"Dictionaries/{dic}/index.aff";

            // Load the .dic file content
            using (Stream dicStream = await appFileProvider.OpenAppPackageFileAsync(dicFilePath))
            using (Stream affStream = await appFileProvider.OpenAppPackageFileAsync(affFilePath))
            {
                WordList wordList = WordList.CreateFromStreamsAsync(dicStream, affStream).Result;

                // Check if a word is valid
                string word = "kangaroo";
                bool isValid = wordList.Check(word);
                return isValid;
            }
        }
    }
}