using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MadCill.BasicSiteGatingModule.Models
{
    public class SimpleSecurityConfiguration
    {
        private static string ConfigurationParameter_CustomLogin = "SimpleSecurity.CustomLoginHtml";
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

        public SimpleSecurityConfiguration(NameValueCollection appSettings)
        {
            CustomLoginPath = GetAppSetting(appSettings, ConfigurationParameter_CustomLogin, string.Empty);
            ConfiguredPassword = GetAppSetting(appSettings, ConfigurationParameter_Password, DefaultPassword);
            CookieName = GetAppSetting(appSettings, ConfigurationParameter_CookieName, DefaultCookieName);
            EncryptionKey = GetAppSetting(appSettings, ConfigurationParameter_EncryptionKey);
            EncryptionIV = GetAppSetting(appSettings, ConfigurationParameter_EncryptionIV);
            HttpHeaderParameter = GetAppSetting(appSettings, ConfigurationParameter_HttpHeaderParameter);
            HttpHeaderCode = GetAppSetting(appSettings, ConfigurationParameter_HttpHeaderCode);
            IpWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_IPWhitelist);
            UrlWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_UrlWhitelist);
            DomainWhitelist = GetAppSettingList(appSettings, ConfigurationParameter_DomainWhitelist);
            SessionLifetime = int.Parse(GetAppSetting(appSettings, ConfigurationParameter_SessionLifetime, "0"));
            try
            {
                SecurityType = (SupportedSecurityType)Enum.Parse(typeof(SupportedSecurityType), GetAppSetting(appSettings, ConfigurationParameter_SecurityType, SupportedSecurityType.Hashed.ToString()));
            }
            catch (Exception ex)
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

        private string CustomLoginPath { get; set; }

        public string MapCustomLoginPath(HttpRequest request)
        {
            if (!string.IsNullOrEmpty(CustomLoginPath))
            {
                return request.MapPath(CustomLoginPath);
            }

            return string.Empty;
        }


        public string ConfiguredPassword { get; private set; }

        public string EncryptionKey { get; private set; }

        public string EncryptionIV { get; private set; }

        public string HttpHeaderParameter { get; private set; }

        public string HttpHeaderCode { get; private set; }

        public string CookieName { get; private set; }

        private string[] IpWhitelist;

        public bool IsIPWhitelisted(string ipAddress)
        {
            return IsWhitelisted(IpWhitelist, ipAddress);
        }

        private string[] _urlWhitelist;
        private string[] _urlWildcardWhitelist;

        private string[] UrlWhitelist
        {
            set
            {
                if (value != null && value.Length > 0)
                {
                    //select wildcards
                    var wildcards = value.Where(x => x[x.Length - 1] == '~');
                    if (wildcards.Any())
                    {
                        _urlWildcardWhitelist = wildcards.Select(x => x.Substring(0, x.Length - 1)).ToArray();
                    }

                    _urlWhitelist = value.Where(x => x[x.Length - 1] != '~').ToArray();
                }
                else
                {
                    _urlWhitelist = new string[0];
                    _urlWildcardWhitelist = new string[0];
                }
            }
        }

        public bool IsUrlWhitelisted(Uri url)
        {
            var path = url?.LocalPath;

            if (!string.IsNullOrEmpty(path))
            {
                if (_urlWhitelist.Length > 0)
                {
                    if(IsWhitelisted(_urlWhitelist, path))
                    {
                        return true;
                    }

                    //check for wildcards
                    return _urlWildcardWhitelist.Any(x => path.StartsWith(x));
                }
            }

            return false;
        }

        private string[] DomainWhitelist;

        public bool IsDomainWhitelisted(Uri url)
        {
            return IsWhitelisted(DomainWhitelist, url?.Host);
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
            if (!string.IsNullOrEmpty(HttpHeaderParameter)
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
