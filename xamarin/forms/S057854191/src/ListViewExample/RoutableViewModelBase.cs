using ReactiveUI;
using Splat;

namespace ListViewExample
{
    public abstract class RoutableViewModelBase : ViewModelBase, IRoutableViewModel
    {
        public virtual string UrlPathSegment { get; }
        public virtual IScreen HostScreen => Locator.Current.GetService<IScreen>();
    }

    public abstract class ViewModelBase : ReactiveObject { }
}