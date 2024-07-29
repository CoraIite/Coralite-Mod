using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.LightLens
{
    public class DarknessLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
    {
        public DarknessLens() : base(TileType<DarknessLensTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeLens)
        { }

        public override int MagikeMax => 300;
        public string SendDelay => "10";
        public int HowManyPerSend => 16;
        public int ConnectLengthMax => 5;
        public int HowManyToGenerate => 16;
        public string GenerateDelay => "10";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineMagike>(2)
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class DarknessLensTile : BaseLensTile
    {
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DarknessLensEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Black);
            DustType = DustID.Granite;
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
            Rectangle frame = texture.Frame(2, 12, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

            //这与我们之前注册的备用磁贴数据有关
            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // 一些数学魔法，使其随着时间的推移平稳地上下移动
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out IMagikeContainer container))
            {
                if (container.Active)   //如果处于活动状态那么就会上下移动，否则就落在底座上
                {
                    const float TwoPi = (float)Math.PI * 2f;
                    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
                    drawPos += new Vector2(0f, offset * 4f);
                    int yframe = (int)(5 * Main.GlobalTimeWrappedHourly % 12);
                    frame = texture.Frame(2, 12, 0, yframe);
                }
                else
                    drawPos += new Vector2(0, halfHeight - 16);
            }

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, frame, Color.White * 0.8f, 0f, origin, 1f, effects, 0f);
        }
    }

    public class DarknessLensEntity : MagikeGenerator_Normal
    {
        public const int sendDelay = 10 * 60;
        public int sendTimer;
        public DarknessLensEntity() : base(300, 5 * 16, 10 * 60) { }

        public override ushort TileType => (ushort)TileType<DarknessLensTile>();

        public override int HowManyPerSend => 16;
        public override int HowManyToGenerate => 16;

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

        public override bool CanGenerate()
        {
            Color lightColor = Lighting.GetColor(Position.ToPoint());
            float light = 0.3f * lightColor.R + 0.6f * lightColor.G + 0.1f * lightColor.B;
            return light < 55;
        }

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Black, DustID.Granite);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Black, DustID.Granite);
        }
    }

}
