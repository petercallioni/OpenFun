using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pangram.Models;

namespace Pangram.PageModels
{
    public partial class PangramDataVM : ObservableObject
    {
        private bool isVisible;
        private string rank;

        private PangramData pangramData;

        public PangramDataVM(PangramData pangramData)
        {
            this.pangramData = pangramData;
            isVisible = false;
            rank = Utilities.Rank.GetRank(pangramData.GetGuessedWordsList().Count, pangramData.MaxScore, pangramData.GotPangram);
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

        public PangramData PangramData { get => pangramData; }

        [RelayCommand]
        private void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }
    }
}
