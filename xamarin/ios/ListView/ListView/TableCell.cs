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

    public class TableCellViewModel
    {
    }
}