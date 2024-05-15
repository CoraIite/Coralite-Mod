﻿using Coralite.Content.Tiles.ShadowCastle;
using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration
{
    public class WorldGenTester : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            //Item.shoot = 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //if (Main.myPlayer == player.whoAmI)
            //{
            //    float rot = (Main.MouseWorld - player.Center).ToRotation();
            //    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CrystalLaser>(), 10, 0, player.whoAmI, rot);

            //}
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            Point pos = Main.MouseWorld.ToTileCoordinates();
            WorldGen.PlaceObject(pos.X, pos.Y, ModContent.TileType<MercuryPlatformTile>());

            //ModItem modItem = ItemLoader.GetItem(5614);
            //Main.NewText(modItem.Name);
            //Main.dayTime = true;
            //Main.time = 4000;
            //Main.windSpeedTarget = 0.8f;
            //TileEntity.Clear();

            //Main.tile.ClearEverything();

            //CoraliteWorld.DungeonRoom((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, TileID.CrystalBlock, WallID.CryocoreBrickWall);

            return base.CanUseItem(player);
        }

    }
}
