using System.Configuration;

namespace ZentitleOnPremDemo.Settings;

public static class Auth0Options
{
    public static string Domain => ConfigurationManager.AppSettings["Auth0Domain"] ?? string.Empty;
    public static string ClientId => ConfigurationManager.AppSettings["Auth0ClientId"] ?? string.Empty;
}