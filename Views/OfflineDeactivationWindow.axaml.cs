using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Zentitle.Licensing.Client.Api;
using ZentitleOnPremDemo.ViewModels;

namespace ZentitleOnPremDemo.Views
{
    public partial class OfflineDeactivationWindow : ReactiveWindow<OfflineDeactivationViewModel>
    {
        public OfflineDeactivationWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CopyToClipboard.Subscribe(CopyToClipboardAction)));
            this.WhenActivated(d => d(ViewModel!.SaveToFile.Subscribe(SaveToFileAction)));
            this.WhenActivated(d => d(ViewModel!.Deactivate.Subscribe(DeactivateAction)));
        }

        private async void DeactivateAction(Unit _)
        {
            await Deactivate();
        }

        private async void SaveToFileAction(string? activationCode)
        {
            await SaveToFile(activationCode);
        }

        private async void CopyToClipboardAction(string? activationCode)
        {
            await CopyToClipboard(activationCode);
        }

        private async Task Deactivate()
        {
            try
            {
                ClearMessages();
                ViewModel!.Loading = true;

                ViewModel.DeactivationConfirmation = await Zentitle.Instance.Activation.DeactivateOffline();
                ViewModel!.InfoMessage = "Deactivated!";
            }
            catch (LicensingApiException<ApiError> ex)
            {
                ViewModel!.ErrorMessage = ex.Result.Details;
            }
            catch
            {
                ViewModel!.ErrorMessage = "Deactivation failed.";
            }
            finally
            {
                ViewModel!.Loading = false;
            }
        }


        private async Task CopyToClipboard(string? result)
        {
            try
            {
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel.Loading = true;

                var clipboard = GetTopLevel(this)?.Clipboard;
                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(result);
                    ViewModel!.InfoMessage =
                        "Confirmation of deactivation has been successfully copied to the clipboard";
                }
                else
                {
                    ViewModel!.ErrorMessage = "Clipboard not available";
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Failed to copy deactivation confirmation to clipboard";
            }
            finally
            {
                ViewModel!.Loading = false;
            }
        }

        private async Task SaveToFile(string? result)
        {
            try
            {
                ClearMessages();
                ViewModel!.Loading = true;
                var topLevel = GetTopLevel(this);
                var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Save deactivation confirmation",
                    SuggestedFileName = "deactivationConfirmation"
                });
                if (file is not null)
                {
                    await File.WriteAllTextAsync(file.Path.LocalPath, result);
                    ViewModel!.InfoMessage = "Confirmation of deactivation has been successfully stored in the file";
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Failed to store deactivation confirmation in the file";
            }
            finally

            {
                ViewModel!.Loading = false;
            }
        }

        private void ClearMessages()
        {
            ViewModel!.ErrorMessage = string.Empty;
            ViewModel!.InfoMessage = string.Empty;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}