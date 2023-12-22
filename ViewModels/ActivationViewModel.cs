using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class ActivationViewModel : ViewModelBase
    {
        public ActivationViewModel()
        {
            Cancel = ReactiveCommand.Create(() =>
            {
                return null as BoolResult;
            });

            Success = ReactiveCommand.Create(() =>
            {
                return new BoolResult(true);
            });

            Activate = ReactiveCommand.Create(() =>
            {
                return _activationCode;
            });
        }

        public string ActivationCode
        {
            get => _activationCode;
            set => this.RaiseAndSetIfChanged(ref _activationCode, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => this.RaiseAndSetIfChanged(ref _successMessage, value);
        }

        public bool Loading
        {
            get => _loading;
            set => this.RaiseAndSetIfChanged(ref _loading, value);
        }

        public ReactiveCommand<Unit, string?> Activate { get; }
        public ReactiveCommand<Unit, BoolResult?> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Success { get; }

        private string _errorMessage;
        private string _successMessage;
        private bool _loading; 
        private string _activationCode;
    }
}
