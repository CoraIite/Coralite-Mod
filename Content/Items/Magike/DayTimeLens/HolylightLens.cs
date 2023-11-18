using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.DayTimeLens
{
    public class HolylightLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public HolylightLens() : base(TileType<HolylightLensTile>(), Item.sellPrice(0, 2, 0, 0), RarityType<CrystallineMagikeRarity>(), 300)
        { }

        public override int MagikeMax => 1200;
        public string SendDelay => "6";
        public int HowManyPerSend => 30;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => 60;
        public string GenerateDelay => "12";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineMagike>(2)
                .AddIngredient<FragmentsOfLight>(2)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<CrystallineMagike>()
                .AddIngredient<FragmentsOfLight>(2)
                .AddIngredient<SunlightLens>()
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HolylightLensTile : BaseCostItemLensTile
    {
        public Asset<Texture2D> bottomGemTex;

        public override void Load()
        {
            base.Load();
            bottomGemTex = Request<Texture2D>(AssetDirectory.MagikeTiles + Name + "_Bottom");
        }

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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<HolylightLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Orange);
            DustType = DustID.OrangeTorch;
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
            Rectangle frame = texture.Frame(2, 18, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

            Color color = Lighting.GetColor(p.X, p.Y);

            //这与我们之前注册的备用磁贴数据有关
            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // 一些数学魔法，使其随着时间的推移平稳地上下移动
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out IMagikeContainer container))
            {
                if (container.Active)   //如果处于活动状态那么就会上下移动，否则就落在底座上
                {
                    int yframe = (int)(6 * Main.GlobalTimeWrappedHourly % 18);

                    Texture2D bottomGem = bottomGemTex.Value;
                    Rectangle bottomFrame = bottomGem.Frame(1, 18, 0, yframe);
                    spriteBatch.Draw(bottomGem, drawPos + new Vector2(0, 18), bottomFrame, color, 0f, bottomFrame.Size() / 2, 1f, effects, 0f);

                    const float TwoPi = (float)Math.PI * 2f;
                    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
                    drawPos += new Vector2(0f, offset * -4f);
                    frame = texture.Frame(2, 18, 0, yframe);

                    // 绘制周期性发光效果
                    float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
                    Color effectColor = color;
                    effectColor.A = 0;
                    effectColor = effectColor * 0.2f * scale;
                    for (float m = 0f; m < 1f; m += 355f / (678f * (float)Math.PI))
                        spriteBatch.Draw(texture, drawPos + (TwoPi * m).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
                }
                else
                    drawPos += new Vector2(0, halfHeight - 16);
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
        }
    }

    public class HolylightLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 6 * 60;
        public int sendTimer;
        public HolylightLensEntity() : base(1200, 5 * 16, 12 * 60) { }

        public override ushort TileType => (ushort)TileType<HolylightLensTile>();

        public override int HowManyPerSend => 30;
        public override int HowManyToGenerate => 60;

        public override bool CanSend()
        {
            sendTimer++;
            if (sendTimer > sendDelay)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }

        public override void OnGenerate(int howMany)
        {
            GenerateAndChargeSelf(howMany);
        }

        public override bool CanGenerate() => Main.dayTime;

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Orange);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Orange, DustID.FireworksRGB);
        }
    }

}
