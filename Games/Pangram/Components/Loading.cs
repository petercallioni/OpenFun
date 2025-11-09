using CommunityToolkit.Mvvm.ComponentModel;

namespace Pangram.Components
{
    public partial class Loading : ObservableObject
    {
        private bool isLoading;
        private bool hasLoaded;

        public Loading()
        {
            isLoading = false;
            hasLoaded = false;
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool HasLoaded
        {
            get => hasLoaded;
            set
            {
                if (hasLoaded != value)
                {
                    hasLoaded = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
