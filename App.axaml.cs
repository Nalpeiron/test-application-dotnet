using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ZentitleOnPremDemo.ViewModels;
using ZentitleOnPremDemo.Views;
using System.Threading.Tasks;
using ZentitleOnPremDemo.Zentitle;

namespace ZentitleOnPremDemo;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashScreenViewModel = new SplashScreenViewModel();
            var splashScreen = new SplashScreen
            {
                DataContext = splashScreenViewModel
            };

            desktop.MainWindow = splashScreen;

            splashScreen.Show();
            try
            {
                await Instance.Activation.Initialize();
            }
            catch (TaskCanceledException)
            {
                splashScreen.Close();
                return;
            }

            var mainWin = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            desktop.MainWindow = mainWin;
            mainWin.Show();

            splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}