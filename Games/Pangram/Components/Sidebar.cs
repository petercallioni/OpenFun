using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Pangram.Components
{
    public partial class Sidebar : ObservableObject
    {
        private string displayText;
        private bool isVisible;
        private int score;

        public Sidebar()
        {
            displayText = "Score: ";
            score = -1;
            isVisible = false;
        }

        public string DisplayText
        {
            get => displayText;
            set
            {
                if (displayText != value)
                {
                    displayText = value;
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

        public void SetDisplayText()
        {
            string text = IsVisible ? "Hide" : "Score";
            text += score > 0 ? $" ({score})" : "";

            DisplayText = text;
        }

        public void UpdateScore(int newScore)
        {
            score = newScore;
            SetDisplayText();
        }

        [RelayCommand]
        private void ToggleVisibility()
        {
            IsVisible = !IsVisible;
            SetDisplayText();
        }
    }
}
