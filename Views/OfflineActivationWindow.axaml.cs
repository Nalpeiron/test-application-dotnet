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
    public partial class OfflineActivationWindow : ReactiveWindow<OfflineActivationViewModel>
    {
        public OfflineActivationWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Success.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.CopyToClipboard.Subscribe(CopyToClipboardAction)));
            this.WhenActivated(d => d(ViewModel!.SaveToFile.Subscribe(SaveToFileAction)));
            this.WhenActivated(d => d(ViewModel!.LoadFromFile.Subscribe(LoadFromFileAction)));
            this.WhenActivated(d => d(ViewModel!.Activate.Subscribe(ActivateAction)));
        }

        private async void ActivateAction(string? activationToken)
        {
            await Activate(activationToken);
        }

        private async void LoadFromFileAction(Unit _)
        {
            await LoadFromFile();
        }

        private async void SaveToFileAction(string? activationCode)
        {
            await SaveToFile(activationCode);
        }

        private async void CopyToClipboardAction(string? activationCode)
        {
            await CopyToClipboard(activationCode);
        }

        private async Task Activate(string? activationToken)
        {
            try
            {
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel!.InfoMessage = string.Empty;
                ViewModel.Loading = true;

                await Zentitle.Instance.Activation.ActivateOffline(activationToken);

                ViewModel!.SuccessMessage = "Activated!";
            }
            catch (LicensingApiException<ApiError> ex)
            {
                ViewModel!.ErrorMessage = ex.Result.Details;
            }
            catch
            {
                ViewModel!.ErrorMessage = "Activation failed. Check your activation response and try again";
            }
            finally
            {
                ViewModel!.Loading = false;
            }
        }

        private async Task LoadFromFile()
        {
            try
            {
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel.Loading = true;

                var topLevel = GetTopLevel(this);
                var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                {
                    Title = "Load Activation confirmation"
                });
                if (files.Count == 1)
                {
                    var filePath = files[0].Path.LocalPath;
                    ViewModel.ActivationConfirmation = await File.ReadAllTextAsync(filePath);
                    ViewModel!.InfoMessage = "Activation confirmation loaded from file";
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Failed to load activation confirmation from file";
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

                ViewModel.ActivationRequest =
                    await Zentitle.Instance.Activation.GenerateOfflineActivationRequestToken(result);
                var clipboard = GetTopLevel(this)?.Clipboard;

                if (clipboard != null)
                {
                    await clipboard.SetTextAsync(ViewModel.ActivationRequest);
                    ViewModel!.InfoMessage = "Activation request has been successfully copied to the clipboard";
                }
                else
                {
                    ViewModel!.ErrorMessage = "Clipboard not available";
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Failed to copy activation request to clipboard";
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
                ViewModel!.ErrorMessage = string.Empty;
                ViewModel.Loading = true;

                ViewModel.ActivationRequest =
                    await Zentitle.Instance.Activation.GenerateOfflineActivationRequestToken(result);

                var topLevel = GetTopLevel(this);
                var file = await topLevel!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                {
                    Title = "Save activation request",
                    SuggestedFileName = "activationRequest",
                });
                if (file is not null)
                {
                    await File.WriteAllTextAsync(file.Path.LocalPath, ViewModel.ActivationRequest);
                    ViewModel!.InfoMessage = "Activation request has been successfully stored in the file";
                }
            }
            catch
            {
                ViewModel!.ErrorMessage = "Failed to store activation confirmation in the file";
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