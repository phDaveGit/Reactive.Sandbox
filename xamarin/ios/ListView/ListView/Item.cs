using System;

namespace ListView
{
    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ItemType Type { get; set; } = ItemType.Some;

        public bool IsToggled { get; set; }
    }
}