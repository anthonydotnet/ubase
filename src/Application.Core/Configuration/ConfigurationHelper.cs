using System.Configuration;

namespace Application.Core.Configuration
{
    public static class ConfigurationHelper
    {
        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static bool IsServiceCacheEnabled()
        {
            return ConfigurationManager.AppSettings["ServiceCache:Enabled"] == "true";
        }
    }
}
