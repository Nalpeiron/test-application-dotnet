using ReactiveUI;
using ZentitleOnPremDemo.Zentitle;

namespace ZentitleOnPremDemo.ViewModels
{
    public class LicenseWindowViewModel : ViewModelBase
    {
        private string _licenseJson = string.Empty;

        public LicenseWindowViewModel()
        {
            LicenseJson = Instance.Activation.Info?.ToString() ?? string.Empty;
        }

        public string LicenseJson
        {
            get => _licenseJson;
            set => this.RaiseAndSetIfChanged(ref _licenseJson, value);
        }
    }
}
