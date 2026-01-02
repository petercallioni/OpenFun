using CommunityToolkit.Mvvm.ComponentModel;
using Pangram.Models;

namespace Pangram.Components
{
    public partial class LastGuess : ObservableObject
    {
        private string message;
        private Color colour;
        private bool isVisible;

        public LastGuess()
        {
            message = string.Empty;
            colour = Colors.Orange;
            isVisible = false;
        }

        public void SetLastGuess(GuessWordResults guessWordResults)
        {
            Message = GetMessageForGuessResult(guessWordResults);
            Colour = GetColorForGuessResult(guessWordResults);
            IsVisible = !Message.Equals(string.Empty);
        }

        private string GetMessageForGuessResult(GuessWordResults result) =>
            result switch
            {
                GuessWordResults.ALREADY_GUESSED => "Already Scored",
                GuessWordResults.FORBIDDEN_CHARACTERS => "Contains Invalid Characters",
                GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => "Must Contain Main Letter",
                GuessWordResults.INVALID => "Word Not In Dictionary",
                GuessWordResults.NOT_LONG_ENOUGH => "Word Must Be >= 3 Characters",
                GuessWordResults.VALID => string.Empty,
                GuessWordResults.VALID_PANGRAM => string.Empty,
                GuessWordResults.NONE => string.Empty,
                _ => string.Empty,
            };

        private Color GetColorForGuessResult(GuessWordResults result) =>
            result switch
            {
                GuessWordResults.VALID => Colors.Lime,
                GuessWordResults.VALID_PANGRAM => Colors.Gold,
                GuessWordResults.FORBIDDEN_CHARACTERS => Colors.Purple,
                GuessWordResults.ALREADY_GUESSED => Colors.LightGray,
                GuessWordResults.NOT_LONG_ENOUGH => Colors.Purple,
                GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => Colors.Red,
                GuessWordResults.INVALID => Colors.Tomato,
                GuessWordResults.NONE => Colors.Orange,
                _ => Colors.Transparent,
            };

        public string Message
        {
            get => message;
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color Colour
        {
            get => colour;
            set
            {
                if (colour != value)
                {
                    colour = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
