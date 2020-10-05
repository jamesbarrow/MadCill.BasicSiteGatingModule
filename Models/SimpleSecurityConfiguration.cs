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
        private static string ConfigurationParameter_HttpHeaderParameter = "SimpleSecurity.HttpHeaderParameter";
        private static string ConfigurationParameter_HttpHeaderCode = "SimpleSecurity.HttpHeaderCode";
        private static string ConfigurationParameter_UrlWhitelist = "SimpleSecurity.UrlWhitelist";
		private static string ConfigurationParameter_DomainWhitelist = "SimpleSecurity.DomainWhitelist";

		private static string DefaultPassword = "!password";
        private static string DefaultCookieName = "SimpleSecurity";

        //whitelist configuration lists
        private string[] _ipWhitelist;
        private string[] _urlWhitelist;
        private string[] _domainWhitelist;

        public SimpleSecurityConfiguration(NameValueCollection appSettings)
        {
            ConfiguredPassword = GetAppSetting(appSettings, ConfigurationParameter_Password, DefaultPassword);
            CookieName = GetAppSetting(appSettings, ConfigurationParameter_CookieName, DefaultCookieName);
            EncryptionKey = GetAppSetting(appSettings, ConfigurationParameter_EncryptionKey);
            EncryptionIV = GetAppSetting(appSettings, ConfigurationParameter_EncryptionIV);
            HttpHeaderParameter = GetAppSetting(appSettings, ConfigurationParameter_HttpHeaderParameter);
            HttpHeaderCode = GetAppSetting(appSettings, ConfigurationParameter_HttpHeaderCode);
            _ipWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_IPWhitelist);
            _urlWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_UrlWhitelist);
            _domainWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_DomainWhitelist);
			SessionLifetime = int.Parse(GetAppSetting(appSettings, ConfigurationParameter_SessionLifetime, "0"));
            try
            {
                SecurityType = (SupportedSecurityType)Enum.Parse(typeof(SupportedSecurityType), GetAppSetting(appSettings, ConfigurationParameter_SecurityType, SupportedSecurityType.Hashed.ToString()));
            }
            catch(Exception ex)
            {
                SecurityType = SupportedSecurityType.Hashed;
            }
        }

        private string GetAppSetting(NameValueCollection appSettings, string key, string defaultValue = null)
        {
            return appSettings.AllKeys.Contains(key) ? appSettings[key] : defaultValue;
        }

        private string[] GetAppSettingList(NameValueCollection appSettings, string key, char separator = ';')
        {
            return GetAppSetting(appSettings, key, "").Split(separator).Select(x => x.Trim()).ToArray();
        }

        public SupportedSecurityType SecurityType { get; private set; }

        public string ConfiguredPassword { get; private set; }

        public string EncryptionKey { get; private set; }

        public string EncryptionIV { get; private set; }

        public string HttpHeaderParameter { get; private set; }

        public string HttpHeaderCode { get; private set; }

        public string CookieName { get; private set; }

        public bool IsIPWhitelisted(string ipAddress)
        {
            return IsWhitelisted(_ipWhitelist, ipAddress);
        }

        public bool IsUrlWhitelisted(Uri url)
        {
            return IsWhitelisted(_urlWhitelist, url?.LocalPath);
        }

		public bool IsDomainWhitelisted(Uri url)
		{
            //if the domain isn't set then allow all domains
			return _domainWhitelist == null || _domainWhitelist.Length == 0 || IsWhitelisted(_domainWhitelist, url?.Host);
		}

		private static bool IsWhitelisted(string[] whitelist, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (whitelist.Length > 0)
                {
                    if (whitelist.Any(x => x == value))
                    {
                        //bypass check for whitelist
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsUsingHttpHeaderBypass(NameValueCollection httpHeaders)
        {
            if(!string.IsNullOrEmpty(HttpHeaderParameter) 
                && !string.IsNullOrEmpty(HttpHeaderCode)
                && httpHeaders.AllKeys.Contains(HttpHeaderParameter)
                && httpHeaders[HttpHeaderParameter] == HttpHeaderCode)
            {
                return true;
            }

            return false;
        }

        // 0 = session, 1(+) = length in days
        public int SessionLifetime { get; private set; }
    }
}
