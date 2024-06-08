using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
using ScadaPlus.Data.Devices;

namespace ScadaPlus.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            services.AddTransient<Client>();
            services.AddTransient(s =>
                new MainWindow(s.GetRequiredService<Client>(),
                s.GetRequiredService<Client>(),
                s.GetRequiredService<Client>()));

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }

}
