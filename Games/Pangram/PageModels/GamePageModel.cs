using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFun_Core.Abstractions;
using OpenFun_Core.Models;
using OpenFun_Core.Services;
using Pangram.Components;
using Pangram.Models;
using System.Collections.ObjectModel;

namespace Pangram.PageModels
{
    public partial class GamePageModel : ObservableObject
    {
        private readonly ModalErrorHandler errorHandler;
        private readonly GameModel gameModel;
        private readonly ObservableCollection<string> guessedWords = new ObservableCollection<string>();

        // Components
        private readonly Sidebar sidebar = new Sidebar();
        private readonly LastGuess lastGuess = new LastGuess();
        private readonly Loading loading = new Loading();

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


        // Components
        public Sidebar Sidebar => sidebar;
        public LastGuess LastGuess => lastGuess;
        public Loading Loading => loading;

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
                loading.IsLoading = true;
                loading.HasLoaded = false;
                await gameModel.InitialiseGame(bool.Parse(daily));
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);
                OtherCharacters = gameModel.WordLetterSequence.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();
                loading.HasLoaded = true;
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
            }
            finally
            {
                loading.IsLoading = false;
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
            CurrentWord += letter;
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
            if (gameModel == null)
            {
                return; // Do nothing
            }

            LastGuessResult = await gameModel.GuessWord(currentWord);

            if (LastGuessResult == GuessWordResults.VALID)
            {
                gameModel?.GuessedWords?.Where(word => !GuessedWords.Contains(word.ToUpper()))
                                      .ToList()
                                      .ForEach(word => GuessedWords.Add(word.ToUpper()));

                CurrentWord = string.Empty;

                sidebar.UpdateScore(gameModel!.Score);
            }

            lastGuess.SetLastGuess(LastGuessResult);
        }
    }
}
