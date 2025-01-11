using Coralite.Content.Items.DigDigDig;
using Coralite.Content.WorldGeneration;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.GlobalItems.DigDigDig
{
    public partial class DigDigDigGItem
    {
        public static HashSet<int> ShouldTransform = new HashSet<int>();

        public void AddBanItems()
        {
            //Ban掉泰拉闪耀靴合成路线
            AddDigTransform(ItemID.TerrasparkBoots, ItemID.FrostsparkBoots, ItemID.LavaWaders
                , ItemID.ObsidianWaterWalkingBoots, ItemID.LavaCharm, ItemID.ObsidianRose, ItemID.MoltenCharm, ItemID.WaterWalkingBoots, ItemID.MoltenSkullRose, ItemID.ObsidianSkull
                , ItemID.LightningBoots, ItemID.IceSkates
                , ItemID.Aglet, ItemID.SpectreBoots, ItemID.AnkletoftheWind
                , ItemID.RocketBoots, ItemID.SailfishBoots, ItemID.HermesBoots, ItemID.SandBoots, ItemID.FlurryBoots);

        }

        private void AddDigTransform(params int[] types)
        {
            foreach (var type in types)
                ShouldTransform.Add(type);
        }

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (CoraliteWorld.DigDigDigWorld)
                UpdateDigTransForm(item);
        }

        public override void UpdateInventory(Item item, Player player)
        {
            if (CoraliteWorld.DigDigDigWorld)
                UpdateDigTransForm(item);
        }

        public void UpdateDigTransForm(Item item)
        {
            if (ShouldTransform.Contains(item.type))
            {
                Item i = item.Clone();
                item.SetDefaults(ModContent.ItemType<DigUnloadItem>());
                (item.ModItem as DigUnloadItem).containsItem = i;
            }
        }
    }
}
