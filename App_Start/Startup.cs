using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;
using Microsoft.Owin.Host.SystemWeb;
using System.Collections.Generic;
using System;
using System.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
[assembly: OwinStartup(typeof(WebsiteGiayKingShoes.Startup))]
namespace WebsiteGiayKingShoes
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login"),
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseFacebookAuthentication(new FacebookAuthenticationOptions
            {
                AppId = "Your-App-Id",
                AppSecret = "Your-App-Secret",
                SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie,
                CallbackPath = new PathString("/User/FacebookLoginCallback"),
            });

           
        }


    }
}
