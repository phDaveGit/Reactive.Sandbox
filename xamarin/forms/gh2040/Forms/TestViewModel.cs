using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace Forms
{
    public class TestAVM : ReactiveObject, IEnableLogger, IRoutableViewModel
    {
        public TestAVM(IScreen hostScreen)
        {
            _HostScreen = hostScreen;
            //
            TestBVM testBVM = new TestBVM(_HostScreen);
            //
            _ClickCommand = ReactiveCommand.CreateFromObservable(() => _HostScreen.Router.Navigate.Execute(testBVM).Select(_ => Unit.Default));
            //
            testBVM.WhenNavigatingFromObservable().Subscribe((_) =>
            {
                ShowActivityIndicator = true;
            });
        }
        //
        public bool ShowActivityIndicator { get; set; }
        //
        private readonly ReactiveCommand<Unit, Unit> _ClickCommand; public ReactiveCommand<Unit, Unit> ClickCommand => _ClickCommand;
        //
        private readonly IScreen _HostScreen; public IScreen HostScreen => _HostScreen;
        //
        public string UrlPathSegment => "TestA";
    }

    public class TestBVM : ReactiveObject, IEnableLogger, IRoutableViewModel
    {
        public TestBVM(IScreen hostScreen)
        {
            _HostScreen = hostScreen;
        }
        //
        public ReactiveCommand<Unit, Unit> ClickCommand => _HostScreen.Router.NavigateBack;
        //
        private readonly IScreen _HostScreen; public IScreen HostScreen => _HostScreen;
        //
        public string UrlPathSegment => "TestB";
    }
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState router = null)
        {
            Router = router ?? new RoutingState();
            //
            RegisterParts(dependencyResolver ?? Locator.CurrentMutable);
            //
            Router.Navigate.Execute(new TestAVM(this));
        }

        public RoutingState Router { get; private set; }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));
            //
            dependencyResolver.Register(() => new TestA(), typeof(IViewFor<TestAVM>));
            dependencyResolver.Register(() => new TestB(), typeof(IViewFor<TestBVM>));
        }

        public Page CreateMainPage()
        {
            return new RoutedViewHost();
        }
    }
}
