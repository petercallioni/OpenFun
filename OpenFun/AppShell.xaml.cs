using OpenFun.Models;

namespace OpenFun
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            foreach (NavigationSelection route in Routes.GetRoutes())
            {
                Routing.RegisterRoute(route.Route, route.GameClass);
            }
        }
    }
}
