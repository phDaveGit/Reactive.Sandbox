using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Foundation;
using ReactiveUI;
using UIKit;

namespace ListView
{
    class ExampleTableViewSource : ReactiveTableViewSource<ExampleTableCellViewModel>
    {
        public ExampleTableViewSource(UITableView tableView,
            INotifyCollectionChanged collection,
            NSString cellKey,
            float sizeHint,
            Action<UITableViewCell> initializeCellAction = null) : base(tableView,
            collection,
            cellKey,
            sizeHint,
            initializeCellAction)
        {
        }

        public ExampleTableViewSource(UITableView tableView,
            IReadOnlyList<TableSectionInformation<ExampleTableCellViewModel>> sectionInformation)
            : base(tableView, sectionInformation)
        {
        }

        public ExampleTableViewSource(UITableView tableView) : base(tableView)
        {
        }
    }
}