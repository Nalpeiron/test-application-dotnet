using System.Configuration;

namespace ZentitleOnPremDemo.Settings;

public static class ZentitleOptions
{
    private const string ZentitleUrlKey = "ZentitleUrl";

    public static string SeatId => ConfigurationManager.AppSettings["SeatId"] ?? string.Empty;
    public static string ProductId => ConfigurationManager.AppSettings["ProductId"] ?? string.Empty;
    public static string TenantId => ConfigurationManager.AppSettings["TenantId"] ?? string.Empty;
    public static string ZentitleUrl => ConfigurationManager.AppSettings[ZentitleUrlKey] ?? string.Empty;
    public static string TenantPublicKey => ConfigurationManager.AppSettings["TenantPublicKey"] ?? string.Empty;

    public static bool UpdateZentitleUrl(string url)
    {
       return AddOrUpdateAppSettings(ZentitleUrlKey, url);
    }

    private static bool AddOrUpdateAppSettings(string key, string value)
    {
        try
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        catch
        {
            return false;
        }

        return true;
    }
}