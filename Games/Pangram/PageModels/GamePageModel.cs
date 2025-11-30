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
        private readonly DatabaseService databaseService;

        // Components
        private readonly Sidebar sidebar = new Sidebar();
        private readonly LastGuess lastGuess = new LastGuess();
        private readonly Loading loading = new Loading();
        private readonly History history = new History();

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
        public History History => history;
        private Task loadHistory;

        public GamePageModel(ModalErrorHandler errorHandler, DatabaseService databaseService)
        {
            this.errorHandler = errorHandler;
            this.databaseService = databaseService;
            LastGuessResult = GuessWordResults.NONE;
            currentWord = string.Empty;
            otherCharacters = new List<char>();
            primeCharacter = '\0';
            gameModel = new GameModel(new MauiFileProvider(), new DailySeed());
            loadHistory = LoadHistory();
        }

        [RelayCommand]
        private void DeleteGame(PangramDataVM pangramDataVM)
        {
            History.PangramHistory.Remove(pangramDataVM);
            _ = databaseService.DeleteAsync<PangramData>(pangramDataVM.PangramData.Id);
        }

        [RelayCommand]
        private async Task LoadGame(PangramData data)
        {
            try
            {
                // Save current challenge if one is loaded
                if (Loading.HasLoaded)
                {
                    await SaveOrUpdateCurrentChallenge();
                }

                Loading.IsLoading = true;
                Loading.HasLoaded = false;

                await gameModel.LoadSavedGame(data);

                Loading.IsLoading = false;
                Loading.HasLoaded = true;
                OtherCharacters = gameModel.WordLetterSequence!.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);

                Sidebar.UpdateScore(gameModel.Score);
                Sidebar.UpdateMaxScore(gameModel.MaxScore);
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
            }
            finally
            {
                loading.IsLoading = false;
            }

            GuessedWords.Clear();
            Sidebar.UpdateScore(0);
            Sidebar.UpdateMaxScore(await gameModel.FindMaxWords());
        }


        [RelayCommand]
        private async Task NewGame(string daily)
        {
            bool newGameIsDaily = bool.Parse(daily);
            try
            {
                await loadHistory; // Ensure history is loaded
                if (newGameIsDaily)
                {
                    PangramData? existingDaily = History.PangramHistory
                        .Where(item => item.PangramData.IsDaily && item.PangramData.Date.Date == DateTime.Now.Date)
                        .ToList()
                        .FirstOrDefault()
                        ?.PangramData;

                    if (existingDaily != null)
                    {
                        await LoadGame(existingDaily);
                        return; // Daily game already loaded
                    }
                }

                loading.IsLoading = true;
                loading.HasLoaded = false;
                await gameModel.InitialiseGame(newGameIsDaily);
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

            GuessedWords.Clear();
            Sidebar.UpdateScore(0);
            Sidebar.UpdateMaxScore(await gameModel.FindMaxWords());

            await SaveOrUpdateCurrentChallenge(); // Adds the game to the history
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

            if (LastGuessResult == GuessWordResults.VALID || LastGuessResult == GuessWordResults.VALID_PANGRAM)
            {
                gameModel?.GuessedWords?.Where(word => !GuessedWords.Contains(word.ToUpper()))
                                      .ToList()
                                      .ForEach(word => GuessedWords.Add(word.ToUpper()));

                CurrentWord = string.Empty;

                sidebar.UpdateScore(gameModel!.Score);
            }

            lastGuess.SetLastGuess(LastGuessResult);
        }

        [RelayCommand]
        private async Task SaveOrUpdateCurrentChallenge()
        {
            PangramData data = new PangramData(gameModel);

            await databaseService.AddOrUpdateAsync<PangramData>(data, word => word.LetterSequence == data.LetterSequence);

            _ = LoadHistory();
        }

        private async Task LoadHistory()
        {
            try
            {
                IEnumerable<PangramData> history = await databaseService.RetrieveAllAsync<PangramData>();
                await History.UpdateHistoryAsync(history);
            }
            catch (Exception ex)
            {
                errorHandler.HandleError(ex);
                return;
            }
        }
    }
}
