namespace OpenFun.Models
{
    public class NavigationSelection
    {
        private readonly string route;
        private readonly string humanName;
        private readonly Type gameClass;
        private readonly bool isGame;
        private readonly string iconName;

        public NavigationSelection(string route,
            string humanName,
            Type gameClass)
        {
            this.route = route;
            this.humanName = humanName;
            this.gameClass = gameClass;
            iconName = route + Routes.ICON_SUFFIX;
            isGame = route.StartsWith(Routes.GAME_PREFIX);
        }

        public bool IsGame => isGame;

        public string Route => route;

        public string HumanName => humanName;

        public Type GameClass => gameClass;

        public string IconName => iconName;
    }
}