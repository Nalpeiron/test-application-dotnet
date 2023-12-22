using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;
using ZentitleOnPremDemo.Settings;

namespace ZentitleOnPremDemo.ViewModels
{
    public class ConfigurationViewModel : ViewModelBase
    {
        public ConfigurationViewModel()
        {
            _licensingApiUrl = ZentitleOptions.ZentitleUrl;
            Cancel = ReactiveCommand.Create(() => null as BoolResult);

            Success = ReactiveCommand.Create(() => new BoolResult(true));

            Save = ReactiveCommand.Create(() => _licensingApiUrl);
        }

        public string LicensingApiUrl
        {
            get => _licensingApiUrl;
            set => this.RaiseAndSetIfChanged(ref _licensingApiUrl, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public string? SuccessMessage
        {
            get => _successMessage;
            set => this.RaiseAndSetIfChanged(ref _successMessage, value);
        }

        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        public string? UrlError
        {
            get => _urlError;
            set => this.RaiseAndSetIfChanged(ref _urlError, value);
        }

        public ReactiveCommand<Unit, string> Save { get; }
        public ReactiveCommand<Unit, BoolResult?> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Success { get; }

        private string? _errorMessage;
        private string? _successMessage;
        private bool _loading;
        private string _licensingApiUrl;
        private string? _urlError;
    }
}
