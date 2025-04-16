using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class ANewBirthOfIce : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ANewBirthOfIceTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            KnowledgeSystem.CheckForUnlock<IceDragon1Knowledge>(Item.Center, Coralite.IcicleCyan);
        }

        public override void UpdateInventory(Player player)
        {
            KnowledgeSystem.CheckForUnlock<IceDragon1Knowledge>(player.Center, Coralite.IcicleCyan);
        }
    }

    public class ANewBirthOfIceTile : ModTile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetStaticDefaults()
        {
            this.PaintingPrefab(3, 2, Coralite.IcicleCyan, DustID.Frost);
        }
    }
}
