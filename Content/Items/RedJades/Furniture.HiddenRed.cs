using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class HiddenRed : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HiddenRedTile>());
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 10, 0);
        }

        public override void UpdateInventory(Player player)
        {
            KnowledgeSystem.CheckForUnlock<RedJadeKnowledge>(player.Center, Coralite.RedJadeRed);
        }
    }

    public class HiddenRedTile : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            this.PaintingPrefab(2, 3, Coralite.RedJadeRed, DustID.GemRuby);
        }

    }
}
