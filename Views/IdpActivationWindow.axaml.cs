using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using Zentitle.Licensing.Client;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views
{
    public partial class IdpActivationWindow : ReactiveWindow<IdpActivationViewModel>
    {
        public IdpActivationWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
        }

        protected override void OnOpened(EventArgs e)
        {
            Login();
            base.OnOpened(e);
        }

        private async void Login()
        {
            if (string.IsNullOrEmpty(ViewModel!.Token)) return;
            ViewModel!.Loading = true;
            try
            {
                await Zentitle.Instance.Activation.ActivateWithOpenIdToken(ViewModel.Token);
                ViewModel!.SuccessMessage = "Activation Successful";
            }
            catch
            {
                ViewModel.ErrorMessage = "Activation failed.";
            }
            finally
            {
                ViewModel.Loading = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}