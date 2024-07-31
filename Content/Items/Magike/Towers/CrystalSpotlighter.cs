using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Towers
{
    public class CrystalSpotlighter : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public CrystalSpotlighter() : base(TileType<CrystalSpotlighterTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeFactories)
        { }

        public override int MagikeMax => 50;
        public string WorkTimeMax => "2";
        public string WorkCost => "25";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(2)
                .AddIngredient<Basalt>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrystalSpotlighterTile : OldBaseColumnTile
    {
        public override string Texture => AssetDirectory.MagikeLensTiles + "CrystalLensTile";
        public override string TopTextureName => AssetDirectory.MagikeFactoryTiles + Name + "_Top";

        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalSpotlighterEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.MagicCrystalPink);
            DustType = DustID.CrystalSerpent_Pink;
        }

        public override void HitWire(int i, int j)
        {
            if (MagikeHelper.TryGetEntity(i, j, out CrystalSpotlighterEntity entity))
                entity.StartWork();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, new Vector2(i, j) * 16);
            int x = i - frameX / 18;
            int y = j - frameY / 18;
            if (MagikeHelper.TryGetEntityWithTopLeft(x, y, out CrystalSpotlighterEntity sender))
                sender.Kill(x, y);
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
            float rotation = 0f;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out CrystalSpotlighterEntity entity))
            {
                if (entity.Active)   //如果处于活动状态那么就会上来，否则就落在底座上
                {
                    rotation = entity.rotation + 1.57f;
                }
                else
                    drawPos += new Vector2(0, halfHeight - 14);
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, color, rotation, origin, 1f, effects, 0f);
        }
    }

    public class CrystalSpotlighterEntity : MagikeFactory
    {
        public const int magikeCost = 25;
        public float rotation;
        public int targetNPC;
        public CrystalSpotlighterEntity() : base(50, 2 * 60) { }

        public override ushort TileType => (ushort)TileType<CrystalSpotlighterTile>();

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Coralite.Instance.MagicCrystalPink);
        }

        public override bool CanWork()
        {
            if (Lighting.Brightness(Position.X, Position.Y) > 0.5f)
                return base.CanWork();

            return false;
        }

        public override bool StartWork()
        {
            NPC npc = Helper.FindClosestEnemy(Position.ToWorldCoordinates(16, 16), 2000, npc => npc.CanBeChasedBy());
            if (magike >= magikeCost && workTimer == -1 && npc != null)
            {
                workTimer = 0;
                targetNPC = npc.whoAmI;
                active = true;
                return true;
            }

            return false;
        }

        public override void DuringWork()
        {
            if (targetNPC != -1)
            {
                NPC currentTarget = Main.npc[targetNPC];
                if (currentTarget.active && !currentTarget.friendly && currentTarget.CanBeChasedBy())
                {
                    rotation = (Main.npc[targetNPC].Center - Position.ToWorldCoordinates(16, 16)).ToRotation();
                }
                else
                {
                    NPC find = Helper.FindClosestEnemy(Position.ToWorldCoordinates(16, 16), 2000, npc => npc.CanBeChasedBy());
                    if (find != null)
                        targetNPC = find.whoAmI;
                    else
                        targetNPC = -1;
                    rotation = 0;
                }
            }
            else
            {
                rotation = 0;
                NPC find = Helper.FindClosestEnemy(Position.ToWorldCoordinates(16, 16), 2000, npc => npc.CanBeChasedBy());
                if (find != null)
                    targetNPC = find.whoAmI;
            }

            if (workTimer % 5 == 0)
            {
                int count = workTimer / 20;
                Vector2 center = Position.ToWorldCoordinates(16, 16);
                for (int i = 0; i < count; i++)
                {
                    Dust dust = Dust.NewDustPerfect(center + new Vector2(0, 12) + Main.rand.NextVector2Circular(count * 2, count * 2), DustID.Teleporter, Vector2.Zero, 0, Coralite.Instance.MagicCrystalPink, 1f + count * 0.05f);
                    dust.noGravity = true;
                }
            }
        }

        public override void WorkFinish()
        {
            if (magike < magikeCost)
                return;

            Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), Position.ToWorldCoordinates(16, 16),
                rotation.ToRotationVector2() * 16, ProjectileType<MagicBeam>(), 50, 2f, Main.myPlayer);

            Charge(-magikeCost);
            active = false;
        }

        public override void CheckActive() { }
    }

}
