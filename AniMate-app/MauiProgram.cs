using AniMate_app.Services.AnilibriaService;
using AniMate_app.ViewModels;
using AniMate_app.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AniMate_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement();

            ConfigureServices(builder.Services);
          
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Service configuration
            services.AddSingleton<IPlatformHttpMessageHandler>(sp =>
            {
#if ANDROID
                return new AndroidHttpMessageHandler();
#else
                return null!;
#endif
            });

            services.AddHttpClient("custom-httpclient", httpClient =>
            {
                var baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7231" : "https://localhost:7231";
                httpClient.BaseAddress = new Uri(baseAddress);
            }).ConfigureHttpMessageHandlerBuilder(configBuilder =>
            {
                var platformMessageHandler = configBuilder.Services.GetRequiredService<IPlatformHttpMessageHandler>();
                configBuilder.PrimaryHandler = platformMessageHandler.GetHttpMessageHandler();
            });

            services.AddHttpClient<AnilibriaService>();

            // Pages configuration
            services.AddTransient<TitlePage>();
            services.AddTransient<TitleViewModel>();

            services.AddTransient<MainPage>();
            services.AddTransient<MainViewModel>();

            services.AddTransient<UpdatesPage>();
            services.AddTransient<UpdatesViewModel>();

            services.AddTransient<SearchPage>();
            services.AddTransient<SearchViewModel>();

            services.AddTransient<GenreViewModel>();
            services.AddTransient<GenrePage>();

            services.AddTransient<PlayerViewModel>();
            services.AddTransient<PlayerPage>();
        }
    }
}