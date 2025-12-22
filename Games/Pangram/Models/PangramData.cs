using OpenFun_Core.Models;
using SQLite;

namespace Pangram.Models
{
    /// <summary>
    /// Database table for storing Pangram game data.
    /// Extends DatabaseTable to define the schema for Pangram-specific information.
    /// Inherits the auto-incrementing Id primary key from DatabaseTable.
    /// </summary>
    [Table("PangramData")]
    public class PangramData : DatabaseTable
    {
        /// <summary>
        /// The date this game was played or created.
        /// </summary>
        [Indexed]
        public DateTime Date { get; set; }

        /// <summary>
        /// The sequence of letters available in the game.
        /// </summary>
        public string LetterSequence { get; set; } = string.Empty;

        /// <summary>
        /// The primary word for this sequence.
        /// </summary>
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether a pangram was found in this game.
        /// </summary>
        public bool GotPangram { get; set; } = false;

        /// <summary>
        /// Indicates whether this pangram data is for the daily challenge.
        /// </summary>
        public bool IsDaily { get; set; } = false;

        /// <summary>
        /// Gets or sets the guessed words as a comma-separated string.
        /// </summary>
        public String GuessedWords { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum score value for the given challenge.
        /// </summary>
        public int MaxScore { get; set; } = 0;

        public PangramData()
        {
        }

        public PangramData(GameModel gameModel)
        {
            string letterSequence = new string(gameModel!.WordLetterSequence!.Letters.ToArray()).ToUpper();

            Date = gameModel.CreatedDate;
            LetterSequence = letterSequence;
            Word = gameModel.FoundPangramWord ?? gameModel.WordLetterSequence?.Word ?? string.Empty; // Should never be null here
            GotPangram = !string.IsNullOrEmpty(gameModel.FoundPangramWord);
            IsDaily = gameModel.IsDaily;
            MaxScore = gameModel.MaxScore;
            SetGuessedWordsList(gameModel.GuessedWords ?? new List<string>());
        }

        public PangramData(DateTime date, string letterSequence, string word)
        {
            Date = date;
            LetterSequence = letterSequence;
            Word = word;
            GotPangram = false;
        }

        public List<String> GetGuessedWordsList()
        {
            if (string.IsNullOrWhiteSpace(GuessedWords))
            {
                return new List<string>();
            }
            return GuessedWords.Split(',').ToList();
        }

        public void SetGuessedWordsList(List<String> words)
        {
            GuessedWords = string.Join(',', words);
        }
    }
}
