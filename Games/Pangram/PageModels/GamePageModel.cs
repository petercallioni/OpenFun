using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFun_Core.Abstractions;
using OpenFun_Core.Models;
using OpenFun_Core.Services;
using Pangram.Models;
using System.Collections.ObjectModel;

namespace Pangram.PageModels
{
    public partial class GamePageModel : ObservableObject
    {
        private readonly ModalErrorHandler errorHandler;
        private readonly GameModel gameModel;
        private readonly ObservableCollection<string> guessedWords = new ObservableCollection<string>();

        private GuessWordResults lastGuessResult;
        private List<char> otherCharacters;
        private char primeCharacter;
        private String currentWord;

        public List<char> OtherCharacters
        {
            get => otherCharacters; set
            {
                if (otherCharacters != value)
                {
                    otherCharacters = value;
                    OnPropertyChanged();
                }
            }
        }

        public char PrimeCharacter
        {
            get => primeCharacter; set
            {
                if (primeCharacter != value)
                {
                    primeCharacter = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentWord
        {
            get => currentWord;
            set
            {
                if (currentWord != value)
                {
                    currentWord = value;
                    OnPropertyChanged();
                }
            }
        }

        public GuessWordResults LastGuessResult
        {
            get => lastGuessResult;
            set
            {
                if (lastGuessResult != value)
                {
                    lastGuessResult = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> GuessedWords => guessedWords;

        public GamePageModel(ModalErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
            LastGuessResult = GuessWordResults.NONE;
            currentWord = string.Empty;
            otherCharacters = new List<char>();
            primeCharacter = '\0';
            gameModel = new GameModel(new MauiFileProvider(), new DailySeed());
        }

        [RelayCommand]
        private async Task NewGame(string daily)
        {
            try
            {
                await gameModel.InitialiseGame(bool.Parse(daily));
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);
                OtherCharacters = gameModel.WordLetterSequence.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
            }
        }

        [RelayCommand]
        private void JumbleLetters()
        {
            Random random = new Random();
            OtherCharacters = OtherCharacters.OrderBy(_ => random.Next()).ToList();
        }

        [RelayCommand]
        private void AddLetter(char letter)
        {
            CurrentWord = CurrentWord + letter;
        }

        [RelayCommand]
        private void RemoveLetter()
        {
            if (CurrentWord.Length > 0)
            {
                CurrentWord = CurrentWord.Remove(CurrentWord.Length - 1, 1);
            }
        }

        [RelayCommand]
        private async Task SubmitWord()
        {
            LastGuessResult = await gameModel.GuessWord(currentWord);

            if (LastGuessResult == GuessWordResults.VALID)
            {
                gameModel?.GuessedWords?.Where(word => !GuessedWords.Contains(word))
                                      .ToList()
                                      .ForEach(word => GuessedWords.Add(word.ToUpper()));

                CurrentWord = string.Empty;
            }
        }
    }
}
