using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using Pangram.Models;

namespace Pangram.Components
{
    public partial class Sidebar : ObservableObject
    {
        private string baseText = "Words: ";
        private string displayText;
        private bool isVisible;
        private string rank;
        private bool _rankVisible;
        public bool RankVisible
        {
            get => _rankVisible;
            set
            {
                if (_rankVisible != value)
                {
                    _rankVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public Sidebar()
        {
            displayText = baseText;
            isVisible = false;
            rank = "";
            _rankVisible = false;
        }

        public string Rank
        {
            get => rank;
            set
            {
                if (rank != value)
                {
                    rank = value;
                    RankVisible = !string.IsNullOrEmpty(rank);
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
            string rankText = "";
            if (score <= 0 || maxScore <= 0)
            {
                Rank = "";
                return;
            }

            double percentage = (double)score / (double)maxScore * 100;

            rankText = percentage switch
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

            if (gotPangram && !"".Equals(rankText))
            {
                rankText += "+";
            }

            Rank = rankText;
        }

        public void ClearRank()
        {
            Rank = "";
        }
    }
}
