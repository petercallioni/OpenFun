using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using OpenFun.PageModels;
using OpenFun_Core.Services;

namespace OpenFun
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("IBMPlexMono-Regular.ttf", "IBMPlexMonoRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<DialogService>();
            builder.Services.AddSingleton<HomePageModel>();
            builder.Services.AddSingleton<Pangram.PageModels.GamePageModel>();

            return builder.Build();
        }
    }
}
