using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class RedJade : BaseMaterial
    {
        public RedJade() : base(9999, Item.sellPrice(0, 0, 2, 50), ItemRarityID.Blue, AssetDirectory.RedJadeItems) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.consumable = true;
            Item.createTile = ModContent.TileType<RedJadeTile>();
        }

        public override void UpdateInventory(Player player)
        {
            KnowledgeSystem.CheckForUnlock<RedJadeKnowledge>(player.Center, Coralite.RedJadeRed);
        }
    }
}
