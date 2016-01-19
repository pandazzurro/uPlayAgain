﻿using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Web.Http;
using uPlayAgain.Providers;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;
using System.Web.Http.Validation;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using uPlayAgain.Utilities;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using uPlayAgain.Data.EF.Context;
using uPlayAgain.Data.EF.Models;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(uPlayAgain.Startup))]
namespace uPlayAgain
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static GoogleOAuth2AuthenticationOptions googleAuthOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }
        public static IDataProtectionProvider DataProtectionProvider { get; set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            //config.MessageHandlers.Add(new CancelledTaskBugWorkaroundMessageHandler());
            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            // Pulire la validazione
            config.Services.Clear(typeof(System.Web.Http.Validation.ModelValidatorProvider));
            // Configura il model validator corretto
            GlobalConfiguration.Configuration.Services.Replace(typeof(IBodyModelValidator), new GeographyBodyModelValidator());
            //Webapi
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();

            //HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();
            //SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);            
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();
            app.CreatePerOwinContext(uPlayAgainContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            CookieAuthenticationOptions option = new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                    validateInterval: TimeSpan.FromMinutes(30),
                    regenerateIdentity: (manager, user) => manager.CreateIdentityAsync(user, "UPlayAgainAuth"))
                }
            };

            app.UseCookieAuthentication(option);
            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            //use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                Provider = new SimpleAuthorizationServerProvider(),
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configure Google External Login
            googleAuthOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "xxxxxx",
                ClientSecret = "xxxxxx",
                Provider = new GoogleAuthProvider()
            };
            app.UseGoogleAuthentication(googleAuthOptions);

            //Configure Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                AppId = "xxxxxx",
                AppSecret = "xxxxxx",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(facebookAuthOptions);

            // Register disposable OwinContext
            app.CreatePerOwinContext<OwinContextDisposal>((o, c) => new OwinContextDisposal(c));
        }
    }    
}