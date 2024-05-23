using System;
using System.Reactive.Concurrency;
using ReactiveUI;
using System.Reactive.Linq;

using System.Windows.Input;
using ZentitleOnPremDemo.Models;
using Auth0.OidcClient;
using Zentitle.Licensing.Client;
using Zentitle.Licensing.Client.Api;
using ZentitleOnPremDemo.Settings;

namespace ZentitleOnPremDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand ShowConverter { get; }
    public ICommand ShowLicense { get; }
    public ICommand Deactivate { get; }
    public ICommand SetActivationCode { get; }
    public ICommand IdpLogin { get; }
    public ICommand UserLogin { get; }
    public ICommand EditConfiguration { get; }
    public ICommand DeactivateOffline { get; }
    public ICommand ActivateOffline { get; }
    public Interaction<ActivationViewModel, BoolResult> ShowActivationCodeDialog { get; }
    public Interaction<IdpActivationViewModel, BoolResult> ShowIdpLoginDialog { get; }
    public Interaction<UserLoginViewModel, BoolResult> ShowUserLoginDialog { get; }
    public Interaction<ConfigurationViewModel, BoolResult> ShowConfigurationForm { get; }
    public Interaction<OfflineActivationViewModel, BoolResult> ShowOfflineActivationDialog { get; }
    public Interaction<OfflineDeactivationViewModel, BoolResult> ShowOfflineDeactivationDialog { get; }
    private bool _activated;
    private bool _offline;
    private bool _online;
    private LicenseWindowViewModel? _licenseWindowViewModel;
    private ConverterViewModel _converterViewModel;
    private string? _companyName;
    private string? _planName;
    private DateTime? _expiryDate;
    private string? _activationModeText;

    public MainWindowViewModel()
    {
        ShowActivationCodeDialog = new Interaction<ActivationViewModel, BoolResult>();
        ShowUserLoginDialog = new Interaction<UserLoginViewModel, BoolResult>();
        ShowIdpLoginDialog = new Interaction<IdpActivationViewModel, BoolResult>();
        ShowConfigurationForm = new Interaction<ConfigurationViewModel, BoolResult>();
        ShowOfflineDeactivationDialog = new Interaction<OfflineDeactivationViewModel, BoolResult>();
        ShowOfflineActivationDialog = new Interaction<OfflineActivationViewModel, BoolResult>();

        _converterViewModel = new ConverterViewModel();

        DeactivateOffline = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new OfflineDeactivationViewModel();

            var result = await ShowOfflineDeactivationDialog.Handle(store);

            if (result is { Success: true })
            {
                await LoadLicenseData();
            }
        });

        ActivateOffline = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new OfflineActivationViewModel();

            var result = await ShowOfflineActivationDialog.Handle(store);

            if (result is { Success: true })
            {
                await LoadLicenseData();
            }
        });

        SetActivationCode = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new ActivationViewModel();

            var result = await ShowActivationCodeDialog.Handle(store);

            if (result is { Success: true })
            {
                await LoadLicenseData();
            }
        });

        UserLogin = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new UserLoginViewModel();

            var result = await ShowUserLoginDialog.Handle(store);

            if (result is { Success: true })
            {
                await LoadLicenseData();
            }
        });

        IdpLogin = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new IdpActivationViewModel();
            var auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = Auth0Options.Domain,
                ClientId = Auth0Options.ClientId
            });

            try
            {
                var auth0Result = await auth0Client.LoginAsync();
                if (auth0Result.Error == null)
                {
                    store.Token = auth0Result.IdentityToken;
                    await auth0Client.LogoutAsync();
                }
                else
                {
                    store.ErrorMessage = $"Login failed '{auth0Result.Error}'";
                }
            }
            catch(Exception ex)
            {
                store.ErrorMessage = ex.Message;
            }

            var result = await ShowIdpLoginDialog.Handle(store);
            if (result is { Success: true })
            {
                await LoadLicenseData();
            }

        });

        EditConfiguration = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new ConfigurationViewModel();

            await ShowConfigurationForm.Handle(store);
        });

        ShowLicense = ReactiveCommand.CreateFromTask(async () =>
        {
            LicenseWindowViewModel = new LicenseWindowViewModel();
            await LoadLicenseData();
        });

        Deactivate = ReactiveCommand.CreateFromTask(async () =>
        {
            await Zentitle.Instance.Activation.Deactivate();
            LicenseWindowViewModel = null;
            await LoadLicenseData();
        });

        ShowConverter = ReactiveCommand.CreateFromTask(async () =>
        {
            LicenseWindowViewModel = null;
            await LoadLicenseData();
        });

        RxApp.MainThreadScheduler.Schedule(ScheduleLicenseDataLoad);
    }

    private async void ScheduleLicenseDataLoad()
    {
        await LoadLicenseData();
    }

    private async System.Threading.Tasks.Task LoadLicenseData()
    {

        await Zentitle.Instance.Refresh();
        Offline = Zentitle.Instance.Activation.Info.Mode == ActivationMode.Offline;
        Online = Zentitle.Instance.Activation.Info.Mode == ActivationMode.Online;
        Activated = Zentitle.Instance.Activation.State == ActivationState.Active;
        ActivationModeText = Zentitle.Instance.Activation.Info.Mode.ToString();
        ConverterViewModel.ReloadLicenseData();
        CompanyName = Zentitle.Instance.CompanyName;
        PlanName = Zentitle.Instance.PlanName;
        ExpiryDate = Zentitle.Instance.ExpiryDate;
    }

    public string? ActivationModeText
    {
        get => _activationModeText;
        set => this.RaiseAndSetIfChanged(ref _activationModeText, value);
    }

    public bool Activated
    {
        get => _activated;
        set => this.RaiseAndSetIfChanged(ref _activated, value);
    }

    public string? CompanyName
    {
        get => _companyName;
        set => this.RaiseAndSetIfChanged(ref _companyName, value);
    }

    public string? PlanName
    {
        get => _planName;
        set => this.RaiseAndSetIfChanged(ref _planName, value);
    }

    public DateTime? ExpiryDate
    {
        get => _expiryDate;
        set => this.RaiseAndSetIfChanged(ref _expiryDate, value);
    }

    public bool Online
    {
        get => _online;
        set => this.RaiseAndSetIfChanged(ref _online, value);
    }

    public bool Offline
    {
        get => _offline;
        set => this.RaiseAndSetIfChanged(ref _offline, value);
    }

    public LicenseWindowViewModel? LicenseWindowViewModel
    {
        get => _licenseWindowViewModel;
        set => this.RaiseAndSetIfChanged(ref _licenseWindowViewModel, value);
    }

    public ConverterViewModel ConverterViewModel
    {
        get => _converterViewModel;
        set => this.RaiseAndSetIfChanged(ref _converterViewModel, value);
    }
}