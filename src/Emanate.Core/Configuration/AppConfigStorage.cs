using System.Configuration;

namespace Emanate.Core.Configuration
{
    public class AppConfigStorage : IConfigurationStorage
    {
        public string GetString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public bool GetBool(string key)
        {
            var value = GetString(key);
            bool booleanValue;
            return bool.TryParse(value, out booleanValue) && booleanValue;
        }

        public int GetInt(string key)
        {
            var value = GetString(key);
            int integerValue;
            return int.TryParse(value, out integerValue) ? integerValue : 0;
        }
    }
}