using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenFun.Models;
using OpenFun.Services;

namespace OpenFun.PageModels
{
    public partial class HomePageModel : ObservableObject
    {
        private readonly ModalErrorHandler errorHandler;

        private List<NavigationSelection> games;
        public List<NavigationSelection> Games
        {
            get => games; set
            {
                games = value;
                OnPropertyChanged();
            }
        }

        public HomePageModel(ModalErrorHandler errorHandler)
        {
            this.errorHandler = errorHandler;
            games = Routes.GetRoutes()
                .Where(x => x.IsGame)
                .ToList();
        }

        [RelayCommand]
        private Task Select(string route)
            => Shell.Current.GoToAsync(route);
    }
}