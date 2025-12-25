using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pangram.Models;

namespace Pangram.Components
{
    public partial class Sidebar : ObservableObject
    {
        private string displayText;
        private bool isVisible;

        public Sidebar()
        {
            displayText = "Score: ";
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

        public void Update(GameModel gameModel)
        {
            string text = IsVisible ? "Hide:" : "Score:";
            text += $" {gameModel.Score}";

            if (gameModel.MaxScore > 0)
            {
                text += $" / {gameModel.MaxScore}";
            }

            DisplayText = text;
        }

        public void ToggleAndUpdate(GameModel gameModel)
        {
            IsVisible = !IsVisible;
            Update(gameModel);
        }
    }
}
