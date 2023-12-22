using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views
{
    public partial class ActivationWindow : ReactiveWindow<ActivationViewModel>
    {
        public ActivationWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Activate.Subscribe(async a => await Activate(a))));
        }

        private async Task Activate(string? result)
        {
            try
            {
                ViewModel!.ErrorMessage = String.Empty;
                ViewModel.Loading = true;

                await Zentitle.Instance.Activation.Activate(result!);
                ViewModel!.SuccessMessage = "Activation Successful";
            }
            catch
            {
                ViewModel!.ErrorMessage = "Activation failed. Check your activation code and try again";
            }
            finally
            {
                ViewModel!.Loading = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}