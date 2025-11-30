using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pangram.Models;

namespace Pangram.PageModels
{
    public partial class PangramDataVM : ObservableObject
    {
        private bool isVisible;
        private PangramData pangramData;

        public PangramDataVM(PangramData pangramData)
        {
            this.pangramData = pangramData;
            isVisible = false;
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
