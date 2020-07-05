using Humidity.Core.Services;
using Humidity.UI.State.Navigators;
using Humidity.UI.ViewModels;
using Humidity.UI.ViewModels.Factories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace Humidity.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IServiceProvider serviceProvider = CreateServiceProvider();

            Window window = serviceProvider.GetRequiredService<MainWindow>();
            window.Show();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                var differentViewModel = scope.ServiceProvider.GetRequiredService<MainViewModel>();
                var equal = differentViewModel == window.DataContext;
            }

            base.OnStartup(e);
        }

        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ISensorManager, SensorManager>();
            services.AddSingleton<ICalculator, Calculator>();

            services.AddSingleton<IHumidityViewModelFactory, HumidityViewModelFactory>();
            services.AddSingleton<HomeViewModel>(s => new HomeViewModel(
                s.GetRequiredService<ISensorManager>(),
                s.GetRequiredService<ICalculator>()
                ));
            services.AddSingleton<LogViewModel>();

            services.AddSingleton<CreateViewModel<HomeViewModel>>(s =>
            {
                return () => s.GetRequiredService<HomeViewModel>();
            });

            services.AddSingleton<CreateViewModel<LogViewModel>>(s =>
            {
                return () => s.GetRequiredService<LogViewModel>();
            });

            services.AddScoped<INavigator, Navigator>();
            services.AddScoped<MainViewModel>();
            services.AddScoped<HomeViewModel>();
            services.AddScoped<LogViewModel>();

            services.AddScoped<MainWindow>(s => new MainWindow(s.GetRequiredService<MainViewModel>()));

            return services.BuildServiceProvider();
        }
    }
}
