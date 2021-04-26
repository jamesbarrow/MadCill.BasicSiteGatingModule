using System;
using System.IO;
using System.Reflection;

namespace MadCill.BasicSiteGatingModule.Services
{
    public class HtmlService
    {
        private static string HtmlLoginResourceName = "MadCill.BasicSiteGatingModule.Html.login.html";
        private static string HtmlErrorResourceName = "MadCill.BasicSiteGatingModule.Html.error.html";
        private static string MessagesToken = "{{messages}}";
        private static string FormCSSClassToken = "{{form-css-token}}";
        private static string RememberMeToken = "{{isRemembermeDisabled}}";

        public string LoginHtml(string messages, bool allowLifetime, string customHtmlPath, string returnUrl = null)
        {
            string html = string.Empty;
            if (!string.IsNullOrEmpty(customHtmlPath) && File.Exists(customHtmlPath))
            {
                html = File.ReadAllText(customHtmlPath);
            }
            else
            {//fallback
                return LoginHtml(messages, allowLifetime, returnUrl);
            }

            return BuildLoginHtml(html, messages, allowLifetime, returnUrl);
        }

        public string LoginHtml(string messages, bool allowLifetime, string returnUrl = null)
        {
            var html = GetResource(HtmlLoginResourceName);
            return BuildLoginHtml(html, messages, allowLifetime, returnUrl);
        }

        public string BuildLoginHtml(string html, string messages, bool allowLifetime, string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(MessagesToken, messages)
                    .Replace(FormCSSClassToken, string.IsNullOrEmpty(messages) ? string.Empty : "gating-has-messages")
                    .Replace(RememberMeToken, (allowLifetime ? "" : "disabled"))
                    .Replace("{{return-url}}", (string.IsNullOrEmpty(returnUrl) ? "window.location.href" : $"'{returnUrl}'"));

                return html;
            }

            throw new Exception("Unable to load the login page");
        }

        public string ErrorHtml(string error)
        {
            var html = GetResource(HtmlErrorResourceName);

            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(MessagesToken, error);

                return html;
            }

            throw new Exception("Unable to load the error page");
        }

        private string GetResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string html = null;
            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}
