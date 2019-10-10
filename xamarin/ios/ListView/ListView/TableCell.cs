using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using ReactiveUI;
using UIKit;

namespace ListView
{
    public class TableCell : ReactiveTableViewCell<TableCellViewModel>
    {
        public static NSString ReuseKey = new NSString(nameof(TableCell));
    }

    public class TableCellViewModel : ReactiveObject
    {
        private ItemType _type;

        public TableCellViewModel(Item item) { Type = item.Type; }

        public ItemType Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }
    }
}