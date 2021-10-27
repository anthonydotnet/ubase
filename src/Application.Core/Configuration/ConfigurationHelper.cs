using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Application.Core.Configuration
{
    public interface IConfigurationHelper
    {
        T GetValue<T>(string key);

    }


    public class ConfigurationHelper: IConfigurationHelper
    {
        private readonly IConfiguration _config;

        public ConfigurationHelper(IConfiguration config)
        {
            _config = config;
        }

        public T GetValue<T>(string key)
        {
            return _config.GetValue<T>(key);
        }

        //public bool IsServiceCacheEnabled()
        //{
        //    return ConfigurationManager.AppSettings["ServiceCache:Enabled"] == bool.TrueString.ToLower();
        //}
    }
}
