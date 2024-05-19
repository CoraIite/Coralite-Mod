using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.Magike;
using Coralite.Core;
using Terraria;

namespace Coralite.Content.Items.Magike.OtherPlaceables
{
    public class OpalTower:ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OpalTowerTile>());
            Item.value = Item.sellPrice(0, 0, 50);
            Item.rare=ModContent.RarityType<MagicCrystalRarity>();
        }
    }

    public class OpalBuff:ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
           int a= player.buffTime[buffIndex];
            if (player.TryGetModPlayer(out MagikePlayer cp))
                cp.equippedMagikeMonoclastic = true;
        }
    }
}
