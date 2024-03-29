﻿using MadCill.BasicSiteGatingModule.Models;
using MadCill.BasicSiteGatingModule.Secure;
using System;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MadCill.BasicSiteGatingModule.Services
{
    public class GatingService
    {
        private static string ErrorMessage = "The password was incorrect password. Please try again.";
        //private static string PostQueryString = "simplesecure";
        private static string UserPasswordParamName = "UserPass";
        private static string SiteGatingPageCheck = "SiteGating";
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

        /// <summary>
        /// Accessible method to set the site gating cookie should you want to set this based on custom code requirements. 
        /// </summary>
        /// <param name="rememberMe"></param>
        /// <param name="redirect"></param>
        public void SetAccessCookie(bool rememberMe, bool redirect = false)
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

            if (redirect)
            {
                Response.Redirect(Request.RawUrl);
            }
        }

        public void HandleGating()
        {
            if (Request.HttpMethod == "POST")
            {
                if (Request.Form[SiteGatingPageCheck] == "true")
                {
                    var rememberMe = Request.Form[RememberMeParamName] == "1";
                    var password = Request.Form[UserPasswordParamName];
                    //clear password check
                    if (password == Configuration.ConfiguredPassword)
                    {
                        SetAccessCookie(rememberMe, true);
                    }
                    else
                    {
                        LoginResponse(ErrorMessage);
                    }
                }
                // should someone just post to a page to get access through the gating then ideally the server will reject it and the next page will be gated.
            }
            else
            {
				//check for domain-whitelist
				if (Configuration.IsDomainWhitelisted(Request.Url))
				{
					// bypass, this domain is whitelisted
					return;
				}
                //check for ip-whitelist
                if (Configuration.IsIPWhitelisted(GetIPAddress(Request)))
                {
                    //bypass, this ip is whitelisted.
                    return;
                }
                //check for ip-whitelist
                if (Configuration.IsUrlWhitelisted(Request.Url))
                {
                    //bypass, this url is whitelisted.
                    return;
                }
                if (Configuration.IsUsingHttpHeaderBypass(Request.Headers))
                {
                    //bypass, this is using our configured HTTP header bypass
                    return;
                }

                //TODO: If the site is redirected to from another domain then the cookie will be missing! This means that we will show the login page even if they have access.
                //Solution needs some thought. i.e. check referrer - if not this domain or null then we need to maybe write the encrypted password to the login page (somehow). 
                //If this field is written to the page then, can we check it against the cookie (might not be accessible client side) or we can just post back... need to know how to check this.

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
                Response.StatusCode = 500;
                Response.StatusDescription = "The resource gating has encountered an error. The resource will remain gated to protect assets until the issue is resolved.";
                Response.Status = "Internal Server Error (gating)";
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
            Response.ContentType = "text/html";
            
            Response.Write(HtmlService.LoginHtml(messages, (Configuration.SessionLifetime > 0), Configuration.MapCustomLoginPath(Request)));

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
