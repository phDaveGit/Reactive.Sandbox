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
                    .Subscribe(e =>
                    {
                        var item = (ClientItemViewModel) e.Item;

                        if (item.Id == ViewModel.Items.Select(i => i).Last().Id)
                        {
                            Observable.Return(Unit.Default).InvokeCommand(this, x => x.ViewModel.RefreshCommand);
                        }
                    })
                    .DisposeWith(disposables);

                ClientsList
                    .Events()
                    .ItemTapped
                    .Subscribe(e => { Observable.Return(e.Item).InvokeCommand(this,x => x.ViewModel.OpenCommand); })
                    .DisposeWith(disposables);


                this.WhenAnyValue(x => x.ViewModel.RefreshCommand)
                    .InvokeCommand(this, x => x.ViewModel.RefreshCommand)
                    .DisposeWith(disposables);
            });
        }
    }
}