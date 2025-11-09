using OpenFun.Models;
using OpenFun.Pages;

namespace OpenFun
{
    public static class Routes
    {
        private static List<NavigationSelection> routes;
        public static List<NavigationSelection> GetRoutes()
        {
            return routes;
        }

        static Routes()
        {
            routes = new List<NavigationSelection>();

            routes.Add(new NavigationSelection(HOME_PAGE, "Home", typeof(HomePage)));
            routes.Add(new NavigationSelection(GAMES_PANGRAM, "Pangram", typeof(Pangram.Pages.Pangram)));
            //routes.Add(new NavigationSelection(GAMES_PANGRAM, "Pangram2", typeof(Pangram.Pages.Pangram)));
        }

        public static string GAME_PREFIX = "game_";
        public static string HOME_PAGE = "HomePage";
        public static string GAMES_PANGRAM = GAME_PREFIX + "pangram";
        public static string ICON_SUFFIX = "_icon.png";
    }
}