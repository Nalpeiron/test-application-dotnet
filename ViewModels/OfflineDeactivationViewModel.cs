using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class OfflineDeactivationViewModel : ViewModelBase
    {
        public OfflineDeactivationViewModel()
        {
            Deactivate = ReactiveCommand.Create(() => { });
            SaveToFile = ReactiveCommand.Create(() => _deactivationConfirmation);
            Cancel = ReactiveCommand.Create(() => null as BoolResult);
            Success = ReactiveCommand.Create(() => new BoolResult(true));
            CopyToClipboard = ReactiveCommand.Create(() => _deactivationConfirmation);
        }

        public string? DeactivationConfirmation
        {
            get => _deactivationConfirmation;
            set => this.RaiseAndSetIfChanged(ref _deactivationConfirmation, value);
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
        public ReactiveCommand<Unit, BoolResult?> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Success { get; }
        public ReactiveCommand<Unit, Unit> Deactivate { get; }

        private string? _errorMessage;
        private string? _successMessage;
        private bool _loading;
        private string? _deactivationConfirmation;
        private string? _infoMessage;
    }
}