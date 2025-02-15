using Coralite.Content.Items.MagikeSeries2;
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
            //Point pos = Main.MouseWorld.ToTileCoordinates();
            //WorldGen.PlaceObject(pos.X, pos.Y, ModContent.TileType<MercuryPlatformTile>());

            //ModItem modItem = ItemLoader.GetItem(5614);
            //Main.NewText(modItem.Name);
            //Main.dayTime = true;
            //Main.time = 4000;
            //Main.windSpeedTarget = 0.8f;
            //TileEntity.Clear();

            //Main.tile.ClearEverything();

            int x = (int)(Main.rand.NextFloat() * 100);
            int y = (int)(Main.rand.NextFloat() * 100);

            for (int i = 0; i < 100; i++)
                for (int j = 0; j < 100; j++)
                {
                    float mainNoise = ModContent.GetInstance<CoraliteWorld>().MainNoise(new Vector2(x + i, y+j), new Vector2(100, 100)*5);
                    if (mainNoise > 0.8f)
                    {
                        Dust d = Dust.NewDustPerfect(Main.MouseWorld + new Vector2(i, j) * 8, DustID.GemDiamond, Vector2.Zero, Scale: 2);
                        d.noGravity = true;
                    }
                }


            //ModContent.GetInstance<CoraliteWorld>().GenMainSkyIsland(player.Center.ToTileCoordinates());
            //ClearWorldTile()

            // Main.NewText(NPC.downedBoss3);

            return base.CanUseItem(player);
        }

        /// <summary>
        /// 清空整个世界的物块
        /// </summary>
        public void ClearWorldTile()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
                for (int j = 0; j < Main.maxTilesY; j++)
                    Main.tile[i, j].Clear(TileDataType.All);
        }
    }
}
