using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MadCill.BasicSiteGatingModule.Services
{
    public class HtmlService
    {
        private static string HtmlLoginResourceName = "MadCill.BasicSiteGatingModule.Html.login.html";
        private static string HtmlErrorResourceName = "MadCill.BasicSiteGatingModule.Html.error.html";
        private static string MessagesToken = "{{messages}}";
        private static string RememberMeToken = "{{isRemembermeDisabled}}";

        public string LoginHtml(string messages, bool allowLifetime, string returnUrl = null)
        {
            var html = GetResource(HtmlLoginResourceName);

            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace(MessagesToken, messages)
                    .Replace(RememberMeToken, (allowLifetime ? "" : "disabled"))
                    .Replace("{{return-url}}", (string.IsNullOrEmpty(returnUrl) ? "window.location.href" : $"'{returnUrl}'"));

                return html;
            }

            throw new Exception("Unable to load the login page");
        }

        public string ErrorHtml(string error)
        {
            var html = GetResource(HtmlLoginResourceName);

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
