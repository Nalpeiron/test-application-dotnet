using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Zentitle.Licensing.Client;
using Zentitle.Licensing.Client.Api;
using Zentitle.Licensing.Client.Persistence.Storage;
using ZentitleOnPremDemo.Settings;

namespace ZentitleOnPremDemo.Zentitle;

public static class Instance
{
    private const string CompanyAttributeKey = "Company";
    private const string PlanNameAttributeKey = "Plan";
    private const string ElementPoolKey = "Threads";
    private const string ConsumptionTokenKey = "FilesToConvert";

    private static readonly HttpClient HttpClient = new HttpClient(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
    });

    private static IActivation activation = CreateActivation();

    public static string CompanyName => GetAttributeValue(CompanyAttributeKey);
    public static string PlanName => GetAttributeValue(PlanNameAttributeKey);
    public static DateTime? ExpiryDate => Activation.Info?.EntitlementExpiryDate?.ToLocalTime().DateTime;

    private static string GetAttributeValue(string key)
    {
        return Activation.Info?.Attributes?.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
    }

    public static Exception? LastEncounteredException { get; private set; }

    public static IActivation Activation => activation;

    public static Task ReloadActivation()
    {
        activation = CreateActivation();
        return activation.Initialize();
    }

    public static ActivationFeature GetElementPoolFeature()
    {
        return Activation.Features.Get(ElementPoolKey) ??
               new ActivationFeature(ElementPoolKey, FeatureType.ElementPool);
    }

    public static ActivationFeature GetConsumptionToken()
    {
        return Activation.Features.Get(ConsumptionTokenKey) ??
               new ActivationFeature(ConsumptionTokenKey, FeatureType.Consumption);
    }

    public static Task<bool> CheckoutConsumptionToken(long amount = 1)
    {
        return CheckoutFeature(ConsumptionTokenKey, amount);
    }

    public static Task<bool> ReturnElementPoolFeature(long amount)
    {
        return ReturnFeature(ElementPoolKey, amount);
    }

    public static Task<bool> CheckoutElementPoolFeature(long amount)
    {
        return CheckoutFeature(ElementPoolKey, amount);
    }

    public static async Task<bool> TryPullRemoteState()
    {
        try
        {
            await Activation.PullRemoteState();
            return true;
        }
        catch (Exception ex)
        {
            LastEncounteredException = ex;
            return false;
        }
    }


    private static async Task<bool> CheckoutFeature(string key, long amount)
    {
        try
        {
            await Activation.Features.Checkout(key, amount);
            return true;
        }
        catch (Exception ex)
        {
            LastEncounteredException = ex;
            return false;
        }
    }

    private static async Task<bool> ReturnFeature(string key, long amount)
    {
        try
        {
            await Activation.Features.Return(key, amount);
            return true;
        }
        catch (Exception ex)
        {
            LastEncounteredException = ex;
            return false;
        }
    }

    private static IActivation CreateActivation()
    {
        return new Activation(opt =>
        {
            opt.WithTenant(ZentitleOptions.TenantId)
                .WithProduct(ZentitleOptions.ProductId)
                .WithSeatId(() => ZentitleOptions.SeatId);

            opt.WithOnlineActivationSupport(onl => onl
                    .UseLicensingApi(new Uri(ZentitleOptions.ZentitleUrl))
                    .UseHttpClientFactory(() => HttpClient))
                .UseStorage(new FileActivationStorage("persistentData.json"));
        });
    }
}