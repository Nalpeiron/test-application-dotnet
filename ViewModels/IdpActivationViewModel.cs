using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class IdpActivationViewModel : ViewModelBase
    {
        public string? Token { get; set; }

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

        public ReactiveCommand<Unit, BoolResult?> Cancel { get; } = ReactiveCommand.Create(() => null as BoolResult);
        public ReactiveCommand<Unit, BoolResult> Success { get; } = ReactiveCommand.Create(() => new BoolResult(true));

        private string? _errorMessage;
        private string? _successMessage;
        private bool _loading;
    }
}
