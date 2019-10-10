using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using CoreFoundation;
using UIKit;
using Foundation;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
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
        private TableView _tableView;
        private UISegmentedControl _itemType;

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
            base.ViewDidLoad();

            CreateUserInterface();

            RegisterObservers();
        }

        public override void ViewWillAppear(bool animated)
        {
            this.ViewModel.SelectedItem = null;
            base.ViewWillAppear(animated);
        }

        private void CreateUserInterface()
        {
            _itemType = new UISegmentedControl
            {
                BackgroundColor = UIColor.Clear,
                TintColor = UIColor.Clear
            };

            _itemType.InsertSegment(ItemType.Some.ToString(), (nint)0, true);
            _itemType.InsertSegment(ItemType.Other.ToString(), (nint)1, true);
            _itemType.InsertSegment(ItemType.All.ToString(), (nint)2, true);
            _itemType.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    Font = UIFont.FromName("Arial", 14),
                    TextColor = UIColor.FromRGBA(255, 255, 255, 180),
                }, UIControlState.Normal);

            _itemType.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    Font = UIFont.FromName("Arial", 14),
                    TextColor = UIColor.FromRGBA(255, 255, 255, 255),
                }, UIControlState.Selected);

            _tableView = new TableView()
            {
                DelaysContentTouches = false,
                SeparatorColor = UIColor.Black,
                TableFooterView = new UIView()
            };

            _tableView.RegisterClassForCellReuse(typeof(TableCell), TableCell.ReuseKey);

            View = new UniversalView();
            View.AddSubview(_itemType);
            View.AddSubview(_tableView);
        }

        private void RegisterObservers()
        {
            _itemType
                .Events()
                .ValueChanged
                .Select(x => Convert.ToInt32(_itemType.SelectedSegment))
                .InvokeCommand(this, x => x.ViewModel.ChangeSegment)
                .DisposeWith(ControlBindings);

            var tableViewSource = this.WhenAnyValue(x => x.ViewModel.Items)
                .Select(x => x == null ? null : new TableSource(_tableView, x, TableCell.ReuseKey))
                .Publish();

            tableViewSource
                .BindTo(_tableView, x => x.Source)
                .DisposeWith(ControlBindings);

            tableViewSource
                .Where(x => x != null)
                .Select(x => x.ElementSelected)
                .Switch()
                .Cast<TableCellViewModel>()
                .Subscribe(x => ViewModel.SelectedItem = x)
                .DisposeWith(ControlBindings);
        }

    }

    public class ListViewModel : ReactiveObject
    {
        private static readonly CompositeDisposable Registrations = new CompositeDisposable();
        private readonly ItemDataService _itemDataService;
        private ObservableCollection<TableCellViewModel> _items;
        private readonly ReadOnlyObservableCollection<TableCellViewModel> _someItems;
        private readonly ReadOnlyObservableCollection<TableCellViewModel> _otherItems;
        private readonly ReadOnlyObservableCollection<TableCellViewModel> _allItems;
        private TableCellViewModel _selectedItem;

        public ListViewModel(ItemDataService itemDataService)
        {
            _itemDataService = itemDataService;

            _itemDataService
                .ChangedItems
                .Transform(x => new TableCellViewModel(x))
                .Bind(out _allItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Registrations);

            _itemDataService
                .ChangedItems
                .Filter(x => x.Type == ItemType.Some)
                .Transform(x => new TableCellViewModel(x))
                .Bind(out _someItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Registrations);

            _itemDataService
                .ChangedItems
                .Filter(x => x.Type == ItemType.Other)
                .Transform(x => new TableCellViewModel(x))
                .Bind(out _otherItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(Registrations);

            ChangeSegment = ReactiveCommand.Create<int, Unit>(segment =>
            {
                switch (segment)
                {
                    case 0:
                        Items = new ObservableCollection<TableCellViewModel>(_someItems);
                        break;
                    case 1:
                        Items = new ObservableCollection<TableCellViewModel>(_otherItems);
                        break;
                    case 2:
                        Items = new ObservableCollection<TableCellViewModel>(_allItems);
                        break;
                }

                return Unit.Default;
            });
        }

        public ObservableCollection<TableCellViewModel> Items
        {
            get => _items;
            set => this.RaiseAndSetIfChanged(ref _items, value);
        }

        public TableCellViewModel SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ReactiveCommand<int, Unit> ChangeSegment { get; set; }
    }

    public class ItemDataService
    {
        private readonly SourceCache<Item, Guid> _source;

        public ItemDataService()
        {
            _source = new SourceCache<Item, Guid>(x => x.Id);

            _source.AddOrUpdate(new Item[30]);

            ChangedItems = _source.Connect().RefCount();
        }

        public IObservable<IChangeSet<Item, Guid>> ChangedItems { get; }

        public void Add(Item item) => _source.AddOrUpdate(item);

        public void Add(IEnumerable<Item> item) => _source.AddOrUpdate(item);
    }

    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ItemType Type { get; set; } = ItemType.Some;
    }

    public enum ItemType
    {
        Some,

        Other,

        All
    }
}