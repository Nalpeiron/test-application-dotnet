using Microsoft.IdentityModel.Tokens;
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
    private const string UsageCountTokenKey = "FilesToConvert";

    private static readonly HttpClient HttpClient = new(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
    });

    public static string CompanyName => GetAttributeValue(CompanyAttributeKey);
    public static string PlanName => GetAttributeValue(PlanNameAttributeKey);
    public static DateTime? ExpiryDate => Activation.Info?.Entitlement?.EntitlementExpiry?.ToLocalTime().DateTime;

    private static string GetAttributeValue(string key)
    {
        return Activation.Info?.Attributes?.FirstOrDefault(x => x.Key == key)?.Value ?? string.Empty;
    }

    public static Exception? LastEncounteredException { get; private set; }

    public static IActivation Activation { get; private set; } = CreateActivation();

    public static Task ReloadActivation()
    {
        Activation = CreateActivation();
        return Activation.Initialize();
    }

    public static ActivationFeature GetElementPoolFeature()
    {
        return Activation.Features.Get(ElementPoolKey) ??
               new ActivationFeature(ElementPoolKey, FeatureType.ElementPool);
    }

    public static ActivationFeature GetUsageCountFeature()
    {
        return Activation.Features.Get(UsageCountTokenKey) ??
               new ActivationFeature(UsageCountTokenKey, FeatureType.UsageCount);
    }

    public static Task<bool> CheckoutUsageCountFeature(long amount = 1)
    {
        return CheckoutFeature(UsageCountTokenKey, amount);
    }

    public static Task<bool> ReturnElementPoolFeature(long amount)
    {
        return ReturnFeature(ElementPoolKey, amount);
    }

    public static Task<bool> CheckoutElementPoolFeature(long amount)
    {
        return CheckoutFeature(ElementPoolKey, amount);
    }

    public static async Task<bool> Refresh()
    {
        try
        {
            var result = false;
            if (Activation.State == ActivationState.NotActivated || Activation.Info.Mode != ActivationMode.Online)
                return result;

            result = await Activation.RefreshLease();
            if (result)
            {
                //Force to reload the state from the server.
                //This is done for demonstration purposes only.
                //In a real-world scenario, you would not need to do this so many times.
                await Activation.PullRemoteState();
            }

            return result;
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

    public static IActivation CreateActivation()
    {
        return new Activation(opt =>
        {
            opt.WithTenant(ZentitleOptions.TenantId)
                .WithProduct(ZentitleOptions.ProductId)
                .WithOfflineActivationSupport(
                    offline => offline.UseTenantPublicKey(new JsonWebKey
                    {
                        Alg = "RSA",
                        N = ZentitleOptions.TenantPublicKey,
                        E = "AQAB"
                    })
                )
                .WithSeatId(() => ZentitleOptions.SeatId);
            opt.WithOnlineActivationSupport(online => online
                    .UseLicensingApi(new Uri(ZentitleOptions.ZentitleUrl))
                    .UseHttpClientFactory(() => HttpClient))
                .UseStorage(new FileActivationStorage("persistentData.json"));
        });
    }
}