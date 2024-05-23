using System;
using System.Linq;
using System.Text;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Newtonsoft.Json;
using ReactiveUI;
using Zentitle.Licensing.Client;
using Zentitle.Licensing.Client.Api;
using ZentitleOnPremDemo.Models;
using ZentitleOnPremDemo.ViewModels;
using ZentitleOnPremDemo.Zentitle;

namespace ZentitleOnPremDemo.Views
{
    public partial class UserLoginWindow : ReactiveWindow<UserLoginViewModel>
    {
        public UserLoginWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Activate.Subscribe(Activate)));
        }

        private async void Activate(UserCredentials result)
        {
            try
            {
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel.Loading = true;

                await Instance.Activation.ActivateWithPassword(result.Username, result.Password);
                ViewModel!.SuccessMessage = "Activation Successful";
            }
            catch (ArgumentException ex)
            {
                ViewModel!.ErrorMessage = ex.Message;
            }
            catch (LicensingApiException ex)
            {
                var apiError = JsonConvert.DeserializeObject<ApiError>(ex.Response);
                if (apiError?.ValidationErrors != null)
                {
                    var validationMessage = new StringBuilder();
                    apiError.ValidationErrors?.ToList()
                        .ForEach(apiErrorValidationError => validationMessage.AppendLine(apiErrorValidationError.Message));
                    ViewModel!.ErrorMessage = validationMessage.ToString().TrimEnd();
                }
                else
                {
                    ViewModel!.ErrorMessage = apiError!.Error;
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Activation failed.";
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