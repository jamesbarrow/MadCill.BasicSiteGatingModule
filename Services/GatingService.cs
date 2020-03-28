using MadCill.BasicSiteGatingModule.Models;
using MadCill.BasicSiteGatingModule.Secure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MadCill.BasicSiteGatingModule.Services
{
    public class GatingService
    {
        private static string ErrorMessage = "The password was incorrect password. Please try again.";
        private static string PostQueryString = "simplesecure";
        private static string UserPasswordParamName = "UserPass";
        private static string RedirectParamName = "redirectUrl";
        private static string RememberMeParamName = "chkboxPersist";

        private HttpRequest Request;
        private HttpResponse Response;

        private HtmlService HtmlService;
        private IEncryptionService EncryptionService;

        public GatingService(HttpRequest request, HttpResponse response)
        {
            Request = request;
            Response = response;
            HtmlService = new HtmlService();
            switch (Configuration.SecurityType)
            {
                case SupportedSecurityType.SimpleEncryption:
                    EncryptionService = new SimpleEncryptionService(Configuration.EncryptionKey, Configuration.EncryptionIV);
                    break;
                default:
                    EncryptionService = new HashService();
                    break;
            }   
        }

        public void HandleGating()
        {
            if (Request.HttpMethod == "POST")
            {
                if (Request.QueryString.AllKeys.Contains(PostQueryString) && Request.QueryString[PostQueryString] == "1")
                {
                    var redirectUrl = Request.Form[RedirectParamName];
                    var rememberMe = Request.Form[RememberMeParamName] == "1";
                    var password = Request.Form[UserPasswordParamName];
                    //clear password check
                    if (password == Configuration.ConfiguredPassword)
                    {
                        var encryptedPassword = EncryptionService.Encrypt(Configuration.ConfiguredPassword);
                        var cookie = new HttpCookie(Configuration.CookieName) { Value = encryptedPassword, HttpOnly = false, Secure = Request.IsSecureConnection };
                        if (Configuration.SessionLifetime > 0
                            && rememberMe
                            && Configuration.SessionLifetime < 365)
                        {
                            cookie.Expires = DateTime.Now.AddDays(Configuration.SessionLifetime);
                        }
                        Response.Cookies.Add(cookie);
                        Response.Redirect(redirectUrl ?? "/");
                    }
                    else
                    {
                        LoginResponse(ErrorMessage);
                    }
                }
            }
            else
            {
                //check for ip-whitelist
                if (Configuration.IsIPWhitelisted(GetIPAddress(Request)))
                {
                    //bypass, this ip is whitelisted.
                    return;
                }

                var simpleSecureCookie = Request.Cookies[Configuration.CookieName];

                var encryptedPassword = EncryptionService.Encrypt(Configuration.ConfiguredPassword);
                if (simpleSecureCookie == null || simpleSecureCookie.Value != encryptedPassword)
                {
                    LoginResponse(string.Empty);
                }
                //else --> the gate is open
            }
        }

        public void HandleError(string messages, bool endResponse = true)
        {
            try
            {
                Response.ContentType = "text/html";
                Response.Write(HtmlService.ErrorHtml(messages));               
            }
            catch(Exception ex)
            {
                Response.Write("<html><head><title>Gating Error - configuration</title></head><body><div>An unknown error has occurred. Plese check the configuration.</div></body></html>");
            }

            if (endResponse)
            {
                Response.Flush();
                Response.End();
            }
        }

        private void LoginResponse(string messages, bool endResponse = true)
        {
            var redirectUrl = Request.Form[RedirectParamName];
            Response.ContentType = "text/html";
            Response.Write(HtmlService.LoginHtml(messages, (Configuration.SessionLifetime > 0), redirectUrl));

            if (endResponse)
            {
                Response.Flush();
                Response.End();
            }
        }

        private SimpleSecurityConfiguration _Configuration;
        private SimpleSecurityConfiguration Configuration
        {
            get
            {
                if (_Configuration == null)
                {
                    _Configuration = new SimpleSecurityConfiguration(ConfigurationManager.AppSettings);
                }

                return _Configuration;
            }
        }

        private static string GetIPAddress(HttpRequest request)
        {
            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
