using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.Base;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;
using Coralite.Content.Items.RedJades;

namespace Coralite.Content.Items.Magike.Towers
{
    public class RedJadeCluster : BaseMagikePlaceableItem
    {
        public RedJadeCluster() : base(TileType<RedJadeClusterTile>(), Item.sellPrice(0, 0, 10, 0), ItemRarityID.Blue, 0)
        { }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(5)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeClusterTile : BaseColumnTile
    {
        public override string Texture => AssetDirectory.MagikeTiles + "RedJadeLensTile";

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] {
                16,
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<RedJadeClusterEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.RedJadeRed);
            DustType = DustID.GemRuby;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, new Vector2(i, j) * 16);
            int x = i - frameX / 18;
            int y = j - frameY / 18;
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out RedJadeClusterEntity sender))
                sender.Kill(x, y);
        }

        public override void HitWire(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out RedJadeClusterEntity entity))
                entity.StartWork();
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //这是特定于照明模式的，如果您手动绘制瓷砖，请始终包含此内容
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块它是否真的存在
            Point p = new Point(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //获取初始绘制参数
            Texture2D texture = TopTexture.Value;

            // 根据项目的地点样式拾取图纸上的框架
            int frameY = tile.TileFrameX / FrameWidth;
            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

            Color color = Lighting.GetColor(p.X, p.Y);

            //这与我们之前注册的备用磁贴数据有关
            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out RedJadeClusterEntity entity))
            {
                if (entity.Active)   //如果处于活动状态那么就会上来，否则就落在底座上
                {
                    float factor = 3f*(entity.workTimer / (float)entity.workTimeMax);
                    drawPos += Main.rand.NextVector2Circular(factor, factor);
                }
                else
                    drawPos += new Vector2(0, halfHeight - 14);
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
        }
    }

    public class RedJadeClusterEntity : MagikeFactory
    {
        public RedJadeClusterEntity() : base(100, 3 * 60) { }

        public override ushort TileType => (ushort)TileType<RedJadeClusterTile>();

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.RedJadeRed);
        }

        public override bool StartWork()
        {
            if (magike >= 50 && workTimer == -1)
            {
                workTimer = 0;
                active = true;
                return true;
            }

            return false;
        }

        public override void DuringWork()
        {
            if (workTimer % 5 == 0)
            {
                int count = workTimer / 20;
                Vector2 center = Position.ToWorldCoordinates(16, 16);
                for (int i = 0; i < count; i++)
                {
                    Dust dust = Dust.NewDustPerfect(center + new Vector2(0, 12) + Main.rand.NextVector2Circular(count * 2, count * 2), DustID.GemRuby, Vector2.Zero, 0, default, 1f + count * 0.1f);
                    dust.noGravity = true;
                }
            }
        }

        public override void WorkFinish()
        {
            if (magike < 50)
                return;
            Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Position.ToWorldCoordinates(16, 16),
                Vector2.Zero, ProjectileType<RedJadeBigBoom>(), 100, 6f, Main.myPlayer);

            Charge(-50);
            active = false;
        }

        public override void CheckActive() { }
    }
}