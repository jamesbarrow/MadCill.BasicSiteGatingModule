using MadCill.BasicSiteGatingModule.Models;
using MadCill.BasicSiteGatingModule.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        //public void OnLogRequest(object sender, EventArgs e)
        //{
        //    HttpContext context = ((HttpApplication)sender).Context;
        //    var path = context.Server.MapPath(@"~\App_Data\SimpleSecurity.txt");

        //    try
        //    {
        //        using (StreamWriter streamWriter = new StreamWriter(path))
        //        {
        //            streamWriter.WriteLine(context.Request.Path);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //
        //    }
        //}
        //public void OnEndRequest(object sender, EventArgs e)
        //{
        //    HttpContext context = ((HttpApplication)sender).Context;
        //    //Write your custom code here

        //}

        #endregion

    }
}
