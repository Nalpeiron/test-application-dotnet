using ReactiveUI;
using System.Reactive;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels
{
    public class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel(string message, bool okButton = true, bool cancelButton = false)
        {
            Message = message;
            OkButton = okButton;
            CancelButton = cancelButton;

            Cancel = ReactiveCommand.Create(() =>
            {
                return new BoolResult(false);
            });

            Ok = ReactiveCommand.Create(() =>
            {
                return new BoolResult(true);
            });
        }

        public string Message { get; private set; } = string.Empty;
        public bool OkButton { get; private set; }
        public bool CancelButton { get; private set; }

        public ReactiveCommand<Unit, BoolResult> Cancel { get; }
        public ReactiveCommand<Unit, BoolResult> Ok { get; }
    }
}
