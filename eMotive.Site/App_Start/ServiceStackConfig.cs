/*using Funq;
using Ninject;
using Ninject.Web.Common;
using ServiceStack;
using ServiceStack.Configuration;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(eMotive.MMI.App_Start.AppHost), "Start")]

namespace eMotive.MMI.App_Start
{
    public class AppHost : AppHostBase
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();
      //  public AppHost() : base("SCE Web Services", typeof(InterviewService).Assembly) { }

        public override void Configure(Container container)
        {//Create Ninject IoC container
          //  IKernel kernel = new StandardKernel(new eMotiveModule());
            //Now register all depedencies to your custom IoC container
            //...
      //      Plugins.Add(new AuthFeature(() => new AuthUserSession(),
  // new IAuthProvider[] {
    //    new BasicAuthProvider()
    //  }));
            //Register Ninject IoC container, so ServiceStack can use it
            container.Adapter = new NinjectIocAdapter(bootstrapper.Kernel);
        }

        public static void Start()
        {
            new AppHost().Init();
        }
    }

    public class NinjectIocAdapter : IContainerAdapter
    {
        private readonly IKernel kernel;

        public NinjectIocAdapter(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public T Resolve<T>()
        {
            return this.kernel.Get<T>();
        }

        public T TryResolve<T>()
        {
            return this.kernel.TryGet<T>();
        }
    }
}*/