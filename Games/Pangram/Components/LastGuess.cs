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
                GuessWordResults.INVALID => "Word Not Valid",
                GuessWordResults.ALREADY_GUESSED => "Already Guessed",
                GuessWordResults.FORBIDDEN_CHARACTERS => "Contains Invalid Characters",
                GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => "Does Not Contain Main Letter",
                GuessWordResults.VALID => string.Empty,
                GuessWordResults.NONE => string.Empty,
                _ => string.Empty,
            };

        private Color GetColorForGuessResult(GuessWordResults result) =>
            result switch
            {
                GuessWordResults.VALID => Colors.Lime,
                GuessWordResults.INVALID => Colors.Tomato,
                GuessWordResults.ALREADY_GUESSED => Colors.LightGray,
                GuessWordResults.FORBIDDEN_CHARACTERS => Colors.Purple,
                GuessWordResults.DOES_NOT_CONTAIN_MAIN_LETTER => Colors.Red,
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
