using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.CoreKeeper.Bases
{
    public abstract class PolishableAccessory : ModItem
    {
        public bool polished;

        public abstract void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack);
        public abstract void ClonePolish( Item item);

        public override void SaveData(TagCompound tag)
        {
            if (polished)
                tag.Add("Polished", polished);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("Polished", out bool p))
                polished = p;
        }

        public override ModItem Clone(Item newEntity)
        {
            if (newEntity.ModItem is PolishableAccessory pa)
            {
                pa.polished = polished;
                ClonePolish(newEntity);
            }
            return base.Clone(newEntity);
        }
    }
}
