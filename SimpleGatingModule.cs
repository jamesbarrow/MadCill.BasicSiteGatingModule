using MadCill.BasicSiteGatingModule.Services;
using System;
using System.Web;

namespace MadCill.BasicSiteGatingModule
{
    public class SimpleGatingModule : IHttpModule
    {
        public void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = ((HttpApplication)sender).Context;
            try
            {
                GatingService service = new GatingService(context.Request, context.Response);

                service.HandleGating();
            }
            catch(Exception ex)
            {
                //in order to remain less invasive, nothing will be rendered here!
            }
        }

              
        #region Setup / Teardown

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(OnBeginRequest);
            //context.EndRequest += new EventHandler(OnEndRequest);
            //context.LogRequest += new EventHandler(OnLogRequest);
        }

        public void Dispose()
        {
            //Write your custom code here to dispose any objects if needed
        }

        #endregion

    }
}
