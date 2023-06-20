using System;
using Terraria;

namespace Coralite.Core
{
    public interface IContainer
    {
        bool InsertItem(Item item);
        bool TryOutputItem(Func<bool> rule,out Item item);
    }
}
