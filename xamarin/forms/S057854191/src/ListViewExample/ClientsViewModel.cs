using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListViewExample
{
    public class ClientsViewModel : RoutableViewModelBase
    {
        private readonly IApiClientService _apiClientService;
        private readonly ObservableAsPropertyHelper<bool> _isBusy;
        private readonly ReactiveCommand<Unit, IEnumerable<ClientItemViewModel>> _refreshCommand;
        private readonly ReactiveCommand<ClientItemViewModel, Unit> _openCommand;
        private readonly ObservableAsPropertyHelper<IEnumerable<ClientItemViewModel>> _items;

        public override string UrlPathSegment => nameof(ClientsViewModel);

        const string localizePrefix = "Client";

        public ClientsViewModel(IScreen hostScreen = null, IApiClientService apiClientService = null) 
        {
            _apiClientService = Locator.Current.GetService<IApiClientService>();

            _refreshCommand = ReactiveCommand.CreateFromTask(async () =>
                {
                    var list = await _apiClientService.GetList();
                    return list
                        .Select(a => new ClientItemViewModel(this)
                        {
                            Id = a.Id,
                            Name = a.Name
                        });
                });

            _refreshCommand.ThrownExceptions.Subscribe(ex => { });

            _isBusy = _refreshCommand
                .IsExecuting
                .DistinctUntilChanged()
                .ToProperty(this, x => x.IsBusy);

            _openCommand = ReactiveCommand.CreateFromObservable<ClientItemViewModel, Unit>(a =>
                HostScreen
                    .Router
                    .Navigate
                    .Execute(Locator.Current.GetService<ClientViewModel>().Init(a.Id))
                    .Select(_ => Unit.Default));

            _items = _refreshCommand
                .ToProperty(this, x => x.Items, scheduler: RxApp.MainThreadScheduler);

            //this.WhenActivated(disposable =>
            //{
            //    Observable.Return(Unit.Default).InvokeCommand(_refreshCommand);
            //});
        }



        public IEnumerable<ClientItemViewModel> Items => _items.Value;
        public bool IsBusy => _isBusy.Value;
        public ReactiveCommand<Unit, IEnumerable<ClientItemViewModel>> RefreshCommand => _refreshCommand;
        public ReactiveCommand<ClientItemViewModel, Unit> OpenCommand => _openCommand;

        internal async Task CreateUser(ClientItemViewModel clientItemViewModel) => await Task.CompletedTask;
    }
}
