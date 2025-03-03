using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineLemna : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 8;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.autoReuse = true;
        }

        public override bool CanUseItem(Player player)
        {
            Tile t = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

            if (t.TileType == ModContent.TileType<SkarnTile>() || t.TileType == ModContent.TileType<SmoothSkarnTile>())
                return true;

            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Tile t = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

            ushort tileType;

            if (t.TileType == ModContent.TileType<SkarnTile>())
                tileType = (ushort)ModContent.TileType<ChalcedonySkarn>();
            else if (t.TileType == ModContent.TileType<SmoothSkarnTile>())
                tileType = (ushort)ModContent.TileType<ChalcedonySmoothSkarn>();
            else
                return false;

            WorldGen.KillTile(Player.tileTargetX, Player.tileTargetY, noItem: true);
            WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, tileType, false);

            NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, Player.tileTargetX, Player.tileTargetY, 1f);

            return false;
        }
    }
}
