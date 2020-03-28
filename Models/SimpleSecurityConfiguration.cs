using MadCill.BasicSiteGatingModule.Secure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadCill.BasicSiteGatingModule.Models
{
    public class SimpleSecurityConfiguration
    {
        private static string ConfigurationParameter_Password = "SimpleSecurity.Password";
        private static string ConfigurationParameter_CookieName = "SimpleSecurity.CookieName";
        private static string ConfigurationParameter_SessionLifetime = "SimpleSecurity.SessionLifetime";
        private static string ConfigurationParameter_IPWhitelist = "SimpleSecurity.IPWhitelist";
        private static string ConfigurationParameter_SecurityType = "SimpleSecurity.SecurityType";
        private static string ConfigurationParameter_EncryptionKey = "SimpleSecurity.EncryptionKey";
        private static string ConfigurationParameter_EncryptionIV = "SimpleSecurity.EncryptionIV";

        private static string DefaultPassword = "!password";
        private static string DefaultCookieName = "SimpleSecurity";

        public SimpleSecurityConfiguration(NameValueCollection appSettings)
        {
            ConfiguredPassword = GetAppSetting(appSettings, ConfigurationParameter_Password, DefaultPassword);
            CookieName = GetAppSetting(appSettings, ConfigurationParameter_CookieName, DefaultCookieName);
            EncryptionKey = GetAppSetting(appSettings, ConfigurationParameter_EncryptionKey);
            EncryptionIV = GetAppSetting(appSettings, ConfigurationParameter_EncryptionIV);
            _ipWhitelist = GetAppSetting(appSettings, ConfigurationParameter_IPWhitelist, "").Split(';').Select(x => x.Trim()).ToArray();
            SessionLifetime = int.Parse(GetAppSetting(appSettings, ConfigurationParameter_SessionLifetime, "0"));
            SecurityType = (SupportedSecurityType)Enum.Parse(typeof(SupportedSecurityType), GetAppSetting(appSettings, ConfigurationParameter_SecurityType, SupportedSecurityType.Hashed.ToString()));
        }

        private string GetAppSetting(NameValueCollection appSettings, string key, string defaultValue = null)
        {
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : defaultValue;
        }

        public SupportedSecurityType SecurityType { get; private set; }

        public string ConfiguredPassword { get; private set; }

        public string EncryptionKey { get; private set; }

        public string EncryptionIV { get; private set; }

        public string CookieName { get; private set; }

        private string[] _ipWhitelist;

        public bool IsIPWhitelisted(string ipAddress)
        {
            if (!string.IsNullOrEmpty(ipAddress))
            {
                if (_ipWhitelist.Length > 0)
                {
                    if (_ipWhitelist.Any(x => x == ipAddress))
                    {
                        //bypass check for IP addresses
                        return true;
                    }
                }
            }

            return false;
        }

        // 0 = session, 1(+) = length in days
        public int SessionLifetime { get; private set; }
    }
}
