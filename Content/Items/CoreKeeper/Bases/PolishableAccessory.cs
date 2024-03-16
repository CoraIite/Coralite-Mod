using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.CoreKeeper.Bases
{
    public abstract class PolishableAccessory : ModItem
    {
        public bool polished;

        public abstract void Polish(Recipe recipe, Item item, List<Item> list, Item destinationStack);

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
    }
}
