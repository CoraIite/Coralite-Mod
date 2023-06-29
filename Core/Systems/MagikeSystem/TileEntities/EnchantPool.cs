using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeFactory_EnchantPool : MagikeFactory, ISingleItemContainer
    {
        public Item containsItem = new Item();

        public MagikeFactory_EnchantPool(int magikeMax, int workTimeMax) : base(magikeMax, workTimeMax) { }

        public abstract Color MainColor { get; }

        public override void WorkFinish()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                (containsItem.damage > 0 || containsItem.accessory || containsItem.defense > 0))
            {

                Vector2 position = Position.ToWorldCoordinates(24, -8);

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(3, 2, Position + new Point16(0, -2), MainColor);
            }
        }


        public virtual bool CanInsertItem(Item item)
        {
            return item.damage > 0 || item.accessory || item.defense > 0;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public bool CanGetItem()
        {
            return workTimer == -1;
        }

        public bool TryOutputItem(Func<bool> rule, out Item item)
        {
            item = containsItem;
            return true;
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
