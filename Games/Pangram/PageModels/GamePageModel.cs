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
        private readonly DialogService dialogService;

        // Components
        private readonly Sidebar sidebar = new Sidebar();
        private readonly LastGuess lastGuess = new LastGuess();
        private readonly Loading loading = new Loading();
        private readonly History history = new History();

        private bool canRevealWord;

        private string foundPangramWord = string.Empty;
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
                    lastGuess.SetLastGuess(value);
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

        public string FoundPangramWord
        {
            get => foundPangramWord;
            set
            {
                if (foundPangramWord != value)
                {
                    foundPangramWord = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanRevealWord
        {
            get => canRevealWord;
            set
            {
                if (canRevealWord != value)
                {
                    canRevealWord = value;
                    OnPropertyChanged();
                }
            }
        }

        private Task loadHistory;

        public GamePageModel(
            ModalErrorHandler errorHandler,
            DatabaseService databaseService,
            DialogService dialogService
            )
        {
            this.errorHandler = errorHandler;
            this.databaseService = databaseService;
            this.dialogService = dialogService;
            LastGuessResult = GuessWordResults.NONE;
            currentWord = string.Empty;
            otherCharacters = new List<char>();
            primeCharacter = '\0';
            gameModel = new GameModel(new MauiFileProvider(), new DailySeed());
            loadHistory = LoadHistory();
            canRevealWord = false;
        }

        [RelayCommand]
        private async Task RevealPangram()
        {
            bool confirmed = await dialogService.DisplayConfirmationAsync(
                "Reveal Pangram",
                "Are you sure you want to reveal the pangram? This will end your current game.",
                "Reveal",
                "Cancel"
            );
            if (confirmed)
            {
                gameModel.ForfeitGame();
                CanRevealWord = false;
                FoundPangramWord = gameModel.FoundPangramWord?.ToUpper() ?? string.Empty;
                _ = SaveOrUpdateCurrentChallenge();
            }
        }

        [RelayCommand]
        private async Task DeleteGame(PangramDataVM pangramDataVM)
        {
            bool confirmed = await dialogService.DisplayConfirmationAsync(
                "Delete Game",
                "Are you sure you want to delete this game?",
                "Delete",
                "Cancel"
            );

            if (confirmed)
            {
                History.PangramHistory.Remove(pangramDataVM);
                _ = databaseService.DeleteAsync<PangramData>(pangramDataVM.PangramData.Id);
            }
        }

        [RelayCommand]
        private async Task LoadGame(PangramData data)
        {
            bool confirmed = await dialogService.DisplayConfirmationAsync(
                "Load Game",
                "Load this saved game? Your current progress will be saved.",
                "Load",
                "Cancel"
            );

            if (!confirmed)
            {
                return;
            }

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

                if (data.GotPangram)
                {
                    FoundPangramWord = data.Word.ToUpper();
                    CanRevealWord = false;
                }
                else
                {
                    FoundPangramWord = "";
                    CanRevealWord = DateTime.UtcNow.AddDays(-1).Date >= data.Date.Date;
                }

                OtherCharacters = gameModel.WordLetterSequence!.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);

                LastGuessResult = GuessWordResults.NONE;

                GuessedWords.Clear();
                gameModel.GuessedWords?.ForEach(word => GuessedWords.Add(word.ToUpper()));

                Sidebar.UpdateScore(gameModel.Score);
                Sidebar.UpdateMaxScore(gameModel.MaxScore);

                History.ClosePangramDetail(data);
                History.IsVisible = false;
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

                bool confirmed = await dialogService.DisplayConfirmationAsync(
                    "New Game",
                    $"Start a new {(newGameIsDaily ? "daily" : "random")} game?",
                    "Start",
                    "Cancel"
                );

                if (!confirmed)
                {
                    return;
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

            LastGuessResult = GuessWordResults.NONE;

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

            if (GuessedWords.Contains(currentWord))
            {
                LastGuessResult = GuessWordResults.ALREADY_GUESSED;
            }
            else
            {
                LastGuessResult = GuessWordResults.NONE;
            }
        }

        [RelayCommand]
        private void RemoveLetter()
        {
            if (CurrentWord.Length > 0)
            {
                CurrentWord = CurrentWord.Remove(CurrentWord.Length - 1, 1);
            }
            
            LastGuessResult = GuessWordResults.NONE;
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

                if (gameModel?.FoundPangramWord != null)
                {
                    FoundPangramWord = gameModel.FoundPangramWord.ToUpper();
                }

                sidebar.UpdateScore(gameModel!.Score);
                _ = SaveOrUpdateCurrentChallenge();
            }
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
