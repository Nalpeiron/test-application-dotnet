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
        this.WhenActivated(d => d(ViewModel!.ShowActivationCodeDialog.RegisterHandler(DoShowDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowUserLoginDialog.RegisterHandler(DoShowUserLoginDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowIdpLoginDialog.RegisterHandler(DoShowLoginDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowOfflineDeactivationDialog.RegisterHandler(ShowOfflineDeactivationDialogAsync)));
        this.WhenActivated(d => d(ViewModel!.ShowOfflineActivationDialog.RegisterHandler(ShowOfflineActivationDialogAsync)));
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

    private async Task ShowOfflineDeactivationDialogAsync(InteractionContext<OfflineDeactivationViewModel, BoolResult> interaction)
    {
        var dialog = new OfflineDeactivationWindow()
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<BoolResult>(this);
        interaction.SetOutput(result);
    }

    private async Task ShowOfflineActivationDialogAsync(InteractionContext<OfflineActivationViewModel, BoolResult> interaction)
    {
        var dialog = new OfflineActivationWindow()
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

    private async Task DoShowUserLoginDialogAsync(InteractionContext<UserLoginViewModel, BoolResult> interaction)
    {
        var dialog = new UserLoginWindow()
        {
            DataContext = interaction.Input
        };

        var result = await dialog.ShowDialog<BoolResult>(this);
        interaction.SetOutput(result);
    }

    private async Task DoShowLoginDialogAsync(InteractionContext<IdpActivationViewModel, BoolResult> interaction)
    {
        var dialog = new IdpActivationWindow()
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