using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pangram.Models;

namespace Pangram.Components
{
    public partial class Sidebar : ObservableObject
    {
        private string baseText = "Words: ";
        private string displayText;
        private bool isVisible;
        private string rank;

        public Sidebar()
        {
            displayText = baseText;
            isVisible = false;
            rank = "";
        }

        public string Rank
        {
            get => rank;
            set
            {
                if (rank != value)
                {
                    rank = value;
                    OnPropertyChanged();
                }
            }
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
            string text = $"{baseText}{gameModel.Score}";
            setRank(gameModel.Score, gameModel.MaxScore, !string.IsNullOrEmpty(gameModel.FoundPangramWord));
            DisplayText = text;
        }

        public void ToggleAndUpdate(GameModel gameModel)
        {
            IsVisible = !IsVisible;
            Update(gameModel);
        }
        
        private void setRank(int score, int maxScore, bool gotPangram)
        {
            if (score <= 0 || maxScore <= 0)
            {
                Rank = "";
                return;
            }

            double percentage = (double) score / (double) maxScore * 100;

            rank = percentage switch
            {
                >= 70 => "S",
                >= 60 => "A",
                >= 50 => "B",
                >= 40 => "C",
                >= 30 => "D",
                >= 20 => "E",
                >= 10 => "F",
                _ => ""
            };

            if (gotPangram)
            {
                Rank = rank + "+";
            }
        }
    }
}
