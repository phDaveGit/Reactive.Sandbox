using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace ListViewExample
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; protected set; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new Clients(), typeof(IViewFor<ClientsViewModel>));
            Locator.CurrentMutable.Register(() => new Clients(), typeof(IViewFor<ClientsViewModel>));
            Locator.CurrentMutable.RegisterLazySingleton<IApiClientService>(() => new ApiClientService());

            Router
                .NavigateAndReset
                .Execute(new ClientsViewModel())
                .Subscribe();
        }

        public Page CreateMainPage()
        {
            // NB: This returns the opening page that the platform-specific
            // boilerplate code will look for. It will know to find us because
            // we've registered our AppBootstrapper as an IScreen.
            return new RoutedViewHost();
        }
    }
}
