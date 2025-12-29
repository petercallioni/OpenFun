using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFun_Core.Abstractions;
using OpenFun_Core.Models;
using OpenFun_Core.Services;
using Pangram.Components;
using Pangram.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Collections.Specialized;

namespace Pangram.PageModels
{
    public partial class GamePageModel : ObservableObject
    {
        private readonly ModalErrorHandler errorHandler;
        private readonly GameModel gameModel;
        private readonly DatabaseService databaseService;
        private readonly DialogService dialogService;
        private ObservableCollection<string> guessedWords = new ObservableCollection<string>();

        // Reusable HttpClient for lookups
        private static readonly HttpClient httpClient = new HttpClient();

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
        private bool showAutoAddSuffixes;
        public bool ShowAutoAddSuffixes
        {
            get => showAutoAddSuffixes;
            set
            {
                if (showAutoAddSuffixes != value)
                {
                    showAutoAddSuffixes = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool enableAutoAddSuffixes;
        public bool EnableAutoAddSuffixes
        {
            get => enableAutoAddSuffixes;
            set
            {
                if (enableAutoAddSuffixes != value)
                {
                    enableAutoAddSuffixes = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> autoAddSuffixes = new List<string>
        {
            "es",
            "ed",
            "er",
            "ee",
            "s"
        };

        private List<string> availableAutoAddSuffixes;
        public List<string> AvailableAutoAddSuffixes
        {
            get => availableAutoAddSuffixes;
            set
            {
                if (availableAutoAddSuffixes != value)
                {
                    availableAutoAddSuffixes = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ObservableCollection<string> GuessedWords
        {
            get => guessedWords; set
            {
                if (guessedWords != value)
                {
                    guessedWords = value;
                    OnPropertyChanged();
                }
            }
        }


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
            availableAutoAddSuffixes = new List<string>();
            ShowAutoAddSuffixes = false;
            EnableAutoAddSuffixes = false;
            canRevealWord = false;
        }

        private void SetGuessedWordList(IEnumerable<string>? strings)
        {
            if (strings != null)
            {
                GuessedWords = new ObservableCollection<string>(strings
                    .OrderBy(w => w)
                    .Select(w => w.ToUpper())
                );
            }
            else
            {
                GuessedWords.Clear();
            }
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
        private async Task DeleteCompletedGames()
        {
            bool confirmed = await dialogService.DisplayConfirmationAsync(
                "Delete Games",
                "Are you sure you want to all completed games (games that have a crown)?",
                "Delete",
                "Cancel"
            );

            if (confirmed)
            {
                var toDelete = History.PangramHistory.Where(x => x.PangramData.GotPangram).ToList();

                foreach (var game in toDelete)
                {
                    History.PangramHistory.Remove(game);
                    _ = databaseService.DeleteAsync<PangramData>(game.PangramData.Id);
                }
            }
        }

        [RelayCommand]
        private async Task DeleteCurrentGame()
        {
            bool confirmed = await dialogService.DisplayConfirmationAsync(
                "Delete Game",
                "Are you sure you want to delete this game?",
                "Delete",
                "Cancel"
            );

            if (confirmed)
            {
                var game = History.PangramHistory
                    .Where(item => item.PangramData.Date == gameModel.CreatedDate)
                    .ToList()
                    .FirstOrDefault();

                if (game != null)
                {
                    History.PangramHistory.Remove(game);
                    _ = databaseService.DeleteAsync<PangramData>(game.PangramData.Id);

                    LastGuessResult = GuessWordResults.NONE;
                    CurrentWord = string.Empty;
                    OtherCharacters = new List<char>();
                    PrimeCharacter = '\0';
                    AvailableAutoAddSuffixes = new List<string>();
                    ShowAutoAddSuffixes = false;
                    ShowAutoAddSuffixes = false;
                    EnableAutoAddSuffixes = false;
                    CanRevealWord = false;
                    Loading.HasLoaded = false;
                    FoundPangramWord = "";
                    GuessedWords.Clear();
                    Sidebar.Update(gameModel);
                }
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
                Sidebar.ClearRank();
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

                    // Only allow revealing the word for daily challenges after the day has passed
                    CanRevealWord = DateTime.Now.AddDays(-1).Date >= data.Date.ToLocalTime().Date;
                }

                OtherCharacters = gameModel.WordLetterSequence!.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);

                LastGuessResult = GuessWordResults.NONE;

                SetGuessedWordList(gameModel.GuessedWords);

                Sidebar.Update(gameModel);

                AvailableAutoAddSuffixes = suffixesAvailable();
                ShowAutoAddSuffixes = AvailableAutoAddSuffixes.Count > 0;

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
                // Save current challenge if one is loaded
                if (Loading.HasLoaded)
                {
                    await SaveOrUpdateCurrentChallenge();
                }

                await loadHistory; // Ensure history is loaded
                if (newGameIsDaily)
                {
                    PangramData? existingDaily = History.PangramHistory
                        .Where(item => item.PangramData.IsDaily && item.PangramData.Date.AddHours(10).Date == DateTime.UtcNow.AddHours(10).Date) // Needs to be inline with DailySeed
                        .ToList()
                        .FirstOrDefault()
                        ?.PangramData;

                    if (existingDaily != null)
                    {
                        await LoadGame(existingDaily);
                        return; // Daily game already loaded
                    }
                }

                if (Loading.HasLoaded || !newGameIsDaily)
                {
                    bool confirmed = await dialogService.DisplayConfirmationAsync(
                        "New Game",
                        $"Start a {(newGameIsDaily ? "daily" : "random")} game?",
                        "Start",
                        "Cancel"
                    );

                    if (!confirmed)
                    {
                        return;
                    }
                }

                loading.IsLoading = true;
                loading.HasLoaded = false;
                Sidebar.ClearRank();
                await gameModel.InitialiseGame(newGameIsDaily);
                FoundPangramWord = "";
                PrimeCharacter = char.ToUpper(gameModel.WordLetterSequence!.Letters[0]);
                OtherCharacters = gameModel.WordLetterSequence.Letters
                    .Skip(1)
                    .Select(x => char.ToUpper(x))
                    .ToList();

                AvailableAutoAddSuffixes = suffixesAvailable();
                ShowAutoAddSuffixes = AvailableAutoAddSuffixes.Count > 0;

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
            Sidebar.Update(gameModel);

            await SaveOrUpdateCurrentChallenge(); // Adds the game to the history

            Task<int> max = gameModel.FindMaxWords();

            _ = max.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    Sidebar.Update(gameModel);
                    _ = SaveOrUpdateCurrentChallenge();
                }
            });
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

            List<string> wordsToCheck = new List<string>();
            wordsToCheck.Add(currentWord);

            if (EnableAutoAddSuffixes)
            {
                foreach (string suffix in AvailableAutoAddSuffixes)
                {
                    wordsToCheck.Add(currentWord + suffix);
                }
            }

            for (int i = 0; i < wordsToCheck.Count; i++)
            {
                var result = await gameModel.GuessWord(wordsToCheck[i]);

                if (i == 0)
                {
                    // Only display the result from the word the user entered
                    LastGuessResult = result;
                }

                if (result == GuessWordResults.VALID || result == GuessWordResults.VALID_PANGRAM)
                {
                    SetGuessedWordList(gameModel.GuessedWords);

                    CurrentWord = string.Empty;

                    if (gameModel?.FoundPangramWord != null)
                    {
                        FoundPangramWord = gameModel.FoundPangramWord.ToUpper();
                    }

                    Sidebar.Update(gameModel!);
                }
                else if (i == 0)
                {
                    // If the first word was not valid, do not attempt the variations
                    return;
                }
            }

            _ = SaveOrUpdateCurrentChallenge();
        }

        [RelayCommand]
        private async Task SaveOrUpdateCurrentChallenge()
        {
            PangramData data = new PangramData(gameModel);

            await databaseService.AddOrUpdateAsync<PangramData>(data, word => word.LetterSequence == data.LetterSequence);

            _ = LoadHistory();
        }

        [RelayCommand]
        private async Task ToggleSidebarVisibility()
        {
            Sidebar.ToggleAndUpdate(gameModel);
        }

        [RelayCommand]
        private async Task DisplayAvailableSuffixes()
        {
            StringBuilder message = new StringBuilder("If enabled using the checkbox, and the submitted word is scored, additionally scores words using your word with the below suffixes:\n");

            foreach (string suffix in AvailableAutoAddSuffixes)
            {
                message.AppendLine($"\"{suffix}\"");
            }

            await dialogService.DisplayAlertAsync(
                "Suffixes",
                message.ToString()
            );
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

        private List<string> suffixesAvailable()
        {
            List<string> availableSuffixes = new List<string>();
            // Check if any of the auto add suffixes can be made with the current letters
            foreach (string suffix in autoAddSuffixes)
            {
                Dictionary<char, int> letterCounts = new Dictionary<char, int>();

                if (gameModel!.WordLetterSequence == null)
                {
                    return availableSuffixes;
                }

                // Count letters in the current word
                foreach (char c in gameModel.WordLetterSequence.Letters)
                {
                    if (letterCounts.ContainsKey(c))
                    {
                        letterCounts[c]++;
                    }
                    else
                    {
                        letterCounts[c] = 1;
                    }
                }
                // Check if we can make the suffix
                bool canMake = true;
                foreach (char c in suffix)
                {
                    if (!letterCounts.ContainsKey(c) || letterCounts[c] == 0)
                    {
                        canMake = false;
                        break;
                    }
                    letterCounts[c]--;
                }
                if (canMake)
                {
                    availableSuffixes.Add(suffix);
                }
            }

            return availableSuffixes;
        }

        [RelayCommand]
        private async Task LookupWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                await dialogService.DisplayAlertAsync("Lookup", "No word provided to lookup.");
                return;
            }

            try
            {
                string query = Uri.EscapeDataString(word.Trim());
                string url = $"https://api.dictionaryapi.dev/api/v2/entries/en/{query}";

                using var resp = await httpClient.GetAsync(url);

                if (!resp.IsSuccessStatusCode)
                {
                    await dialogService.DisplayAlertAsync("Lookup", $"No definition found for '{word}'.");
                    return;
                }

                await using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                {
                    await dialogService.DisplayAlertAsync("Lookup", $"No definition found for '{word}'.");
                    return;
                }

                var first = root[0];
                var sb = new StringBuilder();

                if (first.TryGetProperty("word", out var w) && w.ValueKind == JsonValueKind.String)
                {
                    sb.AppendLine(w.GetString());
                }

                if (first.TryGetProperty("phonetic", out var phonetic) && phonetic.ValueKind == JsonValueKind.String)
                {
                    sb.AppendLine(phonetic.GetString());
                }

                if (first.TryGetProperty("meanings", out var meanings) && meanings.ValueKind == JsonValueKind.Array)
                {
                    int meaningIndex = 1;
                    foreach (var m in meanings.EnumerateArray())
                    {
                        if (m.TryGetProperty("partOfSpeech", out var pos) && pos.ValueKind == JsonValueKind.String)
                        {
                            sb.AppendLine($"{meaningIndex}. {pos.GetString()}");
                        }

                        if (m.TryGetProperty("definitions", out var defs) && defs.ValueKind == JsonValueKind.Array)
                        {
                            int defIndex = 1;
                            foreach (var d in defs.EnumerateArray())
                            {
                                if (d.TryGetProperty("definition", out var defVal) && defVal.ValueKind == JsonValueKind.String)
                                {
                                    sb.AppendLine($"   {defIndex}. {defVal.GetString()}");
                                    defIndex++;
                                }

                                if (d.TryGetProperty("example", out var ex) && ex.ValueKind == JsonValueKind.String)
                                {
                                    sb.AppendLine($"       Example: {ex.GetString()}");
                                }
                            }
                        }

                        sb.AppendLine();
                        meaningIndex++;
                    }
                }

                var message = sb.ToString().Trim();
                if (string.IsNullOrWhiteSpace(message))
                {
                    message = "No details available.";
                }

                // Limit the size of the message to avoid extremely long alerts
                const int maxLength = 4000;
                if (message.Length > maxLength)
                {
                    message = message.Substring(0, maxLength) + "\n... (truncated)";
                }

                await dialogService.DisplayAlertAsync($"Lookup: {word}", message);
            }
            catch (Exception ex)
            {
                // Report error via the app's error handler and also show a simple message
                errorHandler.HandleError(ex);
                await dialogService.DisplayAlertAsync("Lookup Error", "An error occurred while looking up the word.");
            }
        }
    }
}
