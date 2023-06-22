using System;
using Terraria;

namespace Coralite.Core
{
    public interface ISingleItemContainer
    {
        bool InsertItem(Item item);
        bool CanInsertItem(Item item);
        Item GetItem();
        bool CanGetItem();
        bool TryOutputItem(Func<bool> rule, out Item item);
    }
}
