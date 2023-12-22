using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using ZentitleOnPremDemo.Settings;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views
{
    public partial class ConfigurationWindow : ReactiveWindow<ConfigurationViewModel>
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Save.Subscribe(OnNext)));
        }

        private async void OnNext(string url)
        {
            await Save(url);
        }

        private async Task Save(string url)
        {
            try
            {
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel!.UrlError = string.Empty;
                ViewModel.Loading = true;

                if(!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    ViewModel!.UrlError = "Provided value is not valid Url";
                    return;
                }

                if (!ZentitleOptions.UpdateZentitleUrl(url))
                {
                    ViewModel!.ErrorMessage = "Cannot update configuration file";
                    return;
                }

                await Zentitle.Instance.ReloadActivation();
                ViewModel!.SuccessMessage = "Configuration saved";
            }
            catch
            {
                ViewModel!.ErrorMessage = "Configuration update failed";
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