using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class OfflineActivationViewModel : ViewModelBase
    {
        public OfflineActivationViewModel()
        {
            LoadFromFile = ReactiveCommand.Create(() => { });
            Activate = ReactiveCommand.Create(() => _activationConfirmation);
            SaveToFile = ReactiveCommand.Create(() => _activationCode);
            Cancel = ReactiveCommand.Create(() => null as BoolResult);
            Success = ReactiveCommand.Create(() => new BoolResult(true));
            CopyToClipboard = ReactiveCommand.Create(() => _activationCode);
        }

        public string? ActivationCode
        {
            get => _activationCode;
            set => this.RaiseAndSetIfChanged(ref _activationCode, value);
        }

        public string? ActivationRequest
        {
            get => _activationRequest;
            set => this.RaiseAndSetIfChanged(ref _activationRequest, value);
        }

        public string? ActivationConfirmation
        {
            get => _activationConfirmation;
            set => this.RaiseAndSetIfChanged(ref _activationConfirmation, value);
        }

        public string? InfoMessage
        {
            get => _infoMessage;
            set => this.RaiseAndSetIfChanged(ref _infoMessage, value);
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

        public ReactiveCommand<Unit, string?> CopyToClipboard { get; }
        public ReactiveCommand<Unit, string?> SaveToFile { get; }
        public ReactiveCommand<Unit, Unit> LoadFromFile { get; }
        public ReactiveCommand<Unit, BoolResult?> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Success { get; }
        public ReactiveCommand<Unit, string?> Activate { get; }

        private string? _errorMessage;
        private string? _successMessage;
        private bool _loading;
        private string? _activationCode;
        private string? _activationRequest;
        private string? _activationConfirmation;
        private string? _infoMessage;
    }
}