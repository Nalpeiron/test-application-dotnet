using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
using ZentitleOnPremDemo.Models;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowConfigurationForm.RegisterHandler(DoShowEditConfigurationDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ConverterViewModel.ShowDialog.RegisterHandler(DoShowMessageBoxAsync)));
    }

    private async Task DoShowMessageBoxAsync(InteractionContext<MessageBoxViewModel, BoolResult> interaction)
    {
        var dialog = new MessageBoxWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<BoolResult>(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowDialogAsync(InteractionContext<ActivationViewModel, BoolResult> interaction)
    {
        var dialog = new ActivationWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<BoolResult>(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowEditConfigurationDialogAsync(InteractionContext<ConfigurationViewModel, BoolResult> interaction)
    {
        var dialog = new ConfigurationWindow
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<BoolResult>(this);
        interaction.SetOutput(result);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}