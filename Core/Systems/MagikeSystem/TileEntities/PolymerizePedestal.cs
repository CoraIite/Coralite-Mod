using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class PolymerizePedestal : MagikeContainer, IMagikeSingleItemContainer
    {
        public Item containsItem = new Item();

        public PolymerizePedestal() : base(1) { }

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public virtual bool CanInsertItem(Item item)
        {
            return true;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public bool CanGetItem()
        {
            return true;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotImplementedException();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
            {
                tag.Add("containsItem", containsItem);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
            {
                containsItem = item;
            }
        }

    }
}
