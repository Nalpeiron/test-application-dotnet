using System.Threading;
using ReactiveUI;

namespace ZentitleOnPremDemo.ViewModels
{
    internal class SplashScreenViewModel : ViewModelBase
    {
        public string StartupMessage
        {
            get => _startupMessage;
            set { this.RaiseAndSetIfChanged(ref _startupMessage, value); }
        }
        private string _startupMessage = "License checking...";

        public void Cancel()
        {
            StartupMessage = "Cancelling...";
            _cts.Cancel();
        }

        private CancellationTokenSource _cts = new ();

        public CancellationToken CancellationToken => _cts.Token;
    }
}
