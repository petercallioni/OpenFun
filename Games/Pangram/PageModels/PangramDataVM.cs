using CommunityToolkit.Mvvm.ComponentModel;
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
    }
}
