using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ListViewExample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Clients : ReactiveContentPage<ClientsViewModel>
    {
        public Clients()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Items, view => view.ClientsList.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.RefreshCommand, view => view.ClientsList.RefreshCommand)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.IsBusy, view => view.ClientsList.IsRefreshing)
                    .DisposeWith(disposables);

                ClientsList
                    .Events()
                    .ItemAppearing
                    .Where(x => x != null && ((ClientItemViewModel)x.Item).Id == ViewModel.Items.Last().Id)
                    .Select(x => Unit.Default)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .InvokeCommand(this, x => x.ViewModel.RefreshCommand)
                    .DisposeWith(disposables);

                ClientsList
                    .Events()
                    .ItemTapped
                    .Select(x => (ClientItemViewModel)x.Item)
                    .InvokeCommand(this,x => x.ViewModel.OpenCommand)
                    .DisposeWith(disposables);

//                this.WhenAnyValue(x => x.ViewModel.RefreshCommand)
//                    .Select(x => Unit.Default)
//                    .ObserveOn(RxApp.MainThreadScheduler)
//                    .InvokeCommand(this, x => x.ViewModel.RefreshCommand)
//                    .DisposeWith(disposables);
            });
        }
    }
}