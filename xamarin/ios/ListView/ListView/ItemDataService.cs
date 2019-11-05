using System;
using System.Collections.Generic;
using DynamicData;

namespace ListView
{
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

        public void Add(Item item) => _source.Edit(innerList => innerList.AddOrUpdate(item));

        public void Add(IEnumerable<Item> item) => _source.Edit(innerList => innerList.AddOrUpdate(item));
    }
}