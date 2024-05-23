using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class UserLoginViewModel : ViewModelBase
    {
        public UserLoginViewModel()
        {
            Cancel = ReactiveCommand.Create(() => null as BoolResult);

            Success = ReactiveCommand.Create(() => new BoolResult(true));

            Activate = ReactiveCommand.Create(() => new UserCredentials
            {
                Username = Username,
                Password = Password
            });
        }

        public bool FieldsNotEmpty => !string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_password);

        public string Username
        {
            get => _username;
            set
            {
                this.RaiseAndSetIfChanged(ref _username, value);
                this.RaisePropertyChanged(nameof(FieldsNotEmpty));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                this.RaiseAndSetIfChanged(ref _password, value);
                this.RaisePropertyChanged(nameof(FieldsNotEmpty));
            }
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

        public ReactiveCommand<Unit, UserCredentials> Activate { get; }
        public ReactiveCommand<Unit, BoolResult?> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Success { get; }

        private string _errorMessage = null!;
        private string _successMessage = null!;
        private bool _loading;
        private string _username = null!;
        private string _password = null!;
    }
}
