using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Disposables;
using CoreFoundation;
using UIKit;
using Foundation;
using ReactiveUI;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;

namespace ListView
{
    [Register("UniversalView")]
    public class UniversalView : UIView
    {
        public UniversalView()
        {
            Initialize();
        }

        public UniversalView(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.Red;
        }
    }

    [Register("ListController")]
    public class ListController : ReactiveViewController<ListViewModel>
    {
        private static readonly CompositeDisposable ControlBindings = new CompositeDisposable();

        public ListController()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            var tableView = new TableView()
            {
                DelaysContentTouches = false,
                SeparatorColor = UIColor.Black,
                TableFooterView = new UIView()
            };
            tableView.RegisterClassForCellReuse(typeof(TableCell), TableCell.ReuseKey);
            View = tableView;

            base.ViewDidLoad();

            var tableViewSource = this.WhenAnyValue(x => x.ViewModel.Items)
                .Select(x => x == null ? null : new TableSource(tableView, x, TableCell.ReuseKey))
                .Publish();

            tableViewSource
                .BindTo(tableView, x => x.Source)
                .DisposeWith(ControlBindings);

            tableViewSource
                .Where(x => x != null)
                .Select(x => x.ElementSelected)
                .Switch()
                .Cast<TableCellViewModel>()
                .Subscribe(x => ViewModel.SelectedItem = x)
                .DisposeWith(ControlBindings);
        }

        public override void ViewWillAppear(bool animated)
        {
            this.ViewModel.SelectedItem = null;
            base.ViewWillAppear(animated);
        }
    }

    public class ListViewModel : ReactiveObject
    {
        private static readonly CompositeDisposable Registrations = new CompositeDisposable();
        private readonly ItemDataService _itemDataService;
        private readonly ReadOnlyObservableCollection<TableCellViewModel> _items;
        private TableCellViewModel _selectedItem;

        public ListViewModel(ItemDataService itemDataService)
        {
            _itemDataService = itemDataService;

            _itemDataService
                .ChangedItems
                .Transform(x => new TableCellViewModel())
                .Bind(out _items)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Registrations);
        }

        public ReadOnlyObservableCollection<TableCellViewModel> Items => _items;

        public TableCellViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }
    }

    public class ItemDataService
    {
        private SourceList<Item> _source;

        public ItemDataService()
        {
            _source = new SourceList<Item>();

            ChangedItems = _source.Connect().RefCount();
        }

        public IObservable<IChangeSet<Item>> ChangedItems { get; }

        public void Add(Item item) { _source.Add(item); }
    }

    public class Item
    {

    }
}