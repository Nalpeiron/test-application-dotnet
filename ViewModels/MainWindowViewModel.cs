using System;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using Zentitle.Licensing.Client.States;
using ZentitleOnPremDemo.Models;

namespace ZentitleOnPremDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ICommand ShowConverter { get; }
    public ICommand ShowLicense { get; }
    public ICommand Deactivate { get; }
    public ICommand SetActivationCode { get; }
    public ICommand EditConfiguration { get; }
    public Interaction<ActivationViewModel, BoolResult> ShowDialog { get; }
    public Interaction<ConfigurationViewModel, BoolResult> ShowConfigurationForm { get; }
    private bool _activated = false;
    private LicenseWindowViewModel _licenseWindowViewModel;
    private ConverterViewModel _converterViewModel;
    private string _companyName;
    private string _planName;
    private DateTime? _expiryDate;

    public MainWindowViewModel()
    {
        ShowDialog = new Interaction<ActivationViewModel, BoolResult>();
        ShowConfigurationForm = new Interaction<ConfigurationViewModel, BoolResult>();

        ConverterViewModel = new ConverterViewModel();

        SetActivationCode = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new ActivationViewModel();

            var result = await ShowDialog.Handle(store);

            if (result is { Success: true })
            {
                LoadLicenseData();
            }
        });

        EditConfiguration = ReactiveCommand.CreateFromTask(async () =>
        {
            var store = new ConfigurationViewModel();

            var result = await ShowConfigurationForm.Handle(store);
        });

        ShowLicense = ReactiveCommand.CreateFromTask(async () =>
        {
            LicenseWindowViewModel = new LicenseWindowViewModel();
            LoadLicenseData();
        });

        Deactivate = ReactiveCommand.CreateFromTask(async () =>
        {
            await Zentitle.Instance.Activation.Deactivate();
            LicenseWindowViewModel = null;
            LoadLicenseData();
        });

        ShowConverter = ReactiveCommand.CreateFromTask(async () =>
        {
            LicenseWindowViewModel = null;
            LoadLicenseData();
        });

        LoadLicenseData();
    }

    private async void LoadLicenseData()
    {
        await Zentitle.Instance.TryPullRemoteState();
        Activated = Zentitle.Instance.Activation.State == ActivationState.Active;
        ConverterViewModel.ReloadLicenseData();
        CompanyName = Zentitle.Instance.CompanyName;
        PlanName = Zentitle.Instance.PlanName;
        ExpiryDate = Zentitle.Instance.ExpiryDate;
    }

    public bool Activated
    {
        get => _activated;
        set => this.RaiseAndSetIfChanged(ref _activated, value);
    }

    public string CompanyName
    {
        get => _companyName;
        set => this.RaiseAndSetIfChanged(ref _companyName, value);
    }

    public string PlanName
    {
        get => _planName;
        set => this.RaiseAndSetIfChanged(ref _planName, value);
    }

    public DateTime? ExpiryDate
    {
        get => _expiryDate;
        set => this.RaiseAndSetIfChanged(ref _expiryDate, value);
    }

    public LicenseWindowViewModel LicenseWindowViewModel
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