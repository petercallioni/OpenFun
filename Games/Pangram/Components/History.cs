using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pangram.Models;
using Pangram.PageModels;
using System.Collections.ObjectModel;

namespace Pangram.Components
{
    public partial class History : ObservableObject
    {
        ObservableCollection<PangramDataVM> pangramHistory = new ObservableCollection<PangramDataVM>();

        private bool isVisible = false;
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

        public ObservableCollection<PangramDataVM> PangramHistory
        {
            get => pangramHistory;
            set
            {
                if (pangramHistory != value)
                {
                    pangramHistory = value;
                    OnPropertyChanged();
                }
            }
        }

        public History()
        {
        }

        /// <summary>
        /// Updates the history collection asynchronously without blocking the UI.
        /// Sorting and processing is done on a background thread, then results are marshaled
        /// back to the UI thread for collection updates.
        /// </summary>
        public async Task UpdateHistoryAsync(IEnumerable<PangramData> newHistory)
        {
            // Sort on background thread to avoid blocking UI with large collections
            List<PangramData> sortedHistory = await Task.Run(() =>
                newHistory.OrderByDescending(x => x.Date).ToList()
            );

            // Clear and update collection on UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PangramHistory.Clear();
                foreach (PangramData? item in sortedHistory)
                {
                    PangramDataVM pangramDataVM = new PangramDataVM(item);
                    PangramHistory.Add(pangramDataVM);
                }
            });
        }

        [RelayCommand]
        private void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }

        public void ClosePangramDetail(PangramData data)
        {
            foreach (var item in pangramHistory.Where(x => x.PangramData.Id == data.Id))
            {
                item.IsVisible = false;
            }
        }
    }
}
