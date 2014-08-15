using System.Web.Mvc;
using eMotive.MMI.Controllers;
using eMotive.IoCBindings.Funq;
using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Common;
using ServiceStack.Configuration;
using ServiceStack.Mvc;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;


namespace eMotive.MMI
{
    public class AppHost : AppHostBase
    {
        public AppHost() : base("MMI Web Services", typeof(AppHost).Assembly) { }

        public override void Configure(Container container)
        {
            FunqBindings.Configure(container);

            container.Register<ICacheClient>(new MemoryCacheClient());
            container.Register<ISessionFactory>(c => new SessionFactory(c.Resolve<ICacheClient>()));

            ControllerBuilder.Current.SetControllerFactory(new FunqControllerFactory(container));
            ServiceStackController.CatchAllController = reqCtx => container.TryResolve<AccountController>();
            JsConfig.DateHandler = JsonDateHandler.ISO8601;

            SetConfig(new EndpointHostConfig
            {
                EnableFeatures = Feature.All.Remove(Feature.Metadata)
            });
            AuthService.Init(() => new AuthUserSession(), new IAuthProvider[] {new CredentialsAuthProvider()});


            /*Plugins.Add(new AuthFeature(() => new AuthUserSession(),
                    new IAuthProvider[] { 
                    new BasicAuthProvider(), //Sign-in with Basic Auth
                    new CredentialsAuthProvider(), //HTML Form post of UserName/Password credentials
                  }));*/
            /*    var appSettings = new AppSettings();
            Plugins.Add(new AuthFeature(this, new CustomUserSession(), 
                new IAuthProvider[]
                {
                    new CredentialsAuthProvider(appSettings), 
                }));*/
        }
    }

    public class CustomCredentialsAuthProvider : CredentialsAuthProvider
    {
        public override bool TryAuthenticate(IServiceBase authService,
            string userName, string password)
        {
            return userName == "john" && password == "test";
        }
    }
}