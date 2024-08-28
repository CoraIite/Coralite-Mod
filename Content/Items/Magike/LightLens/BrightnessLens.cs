﻿//using Coralite.Content.Items.MagikeSeries2;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.TileEntities;
//using Coralite.Core.Systems.MagikeSystem.Tiles;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Magike.LightLens
//{
//    public class BrightnessLens : BaseMagikePlaceableItem, IMagikeGeneratorItem, IMagikeSenderItem
//    {
//        public BrightnessLens() : base(TileType<BrightnessLensTile>(), Item.sellPrice(0, 0, 10, 0)
//            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeLens)
//        { }

//        public override int MagikeMax => 300;
//        public string SendDelay => "10";
//        public int HowManyPerSend => 16;
//        public int ConnectLengthMax => 5;
//        public int HowManyToGenerate => 16;
//        public string GenerateDelay => "10";

//        public override void AddRecipes()
//        {
//            CreateRecipe()
//                .AddIngredient<CrystallineMagike>(2)
//                .AddIngredient(ItemID.SoulofLight, 10)
//                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
//                .AddTile(TileID.Anvils)
//                .Register();
//        }
//    }

//    public class BrightnessLensTile : OldBaseLensTile
//    {
//        public override void SetStaticDefaults()
//        {
//            Main.tileShine[Type] = 400;
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
//            TileID.Sets.IgnoredInHouseScore[Type] = true;

//            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
//            TileObjectData.newTile.Height = 3;
//            TileObjectData.newTile.CoordinateHeights = new int[3] {
//                16,
//                16,
//                16
//            };
//            TileObjectData.newTile.DrawYOffset = 2;
//            TileObjectData.newTile.LavaDeath = false;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrightnessLensEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Color.White);
//            DustType = DustID.WhiteTorch;
//        }

//        public override void NearbyEffects(int i, int j, bool closer)
//        {
//            Tile t = Framing.GetTileSafely(i, j);
//            if (t.TileFrameX == 0 && t.TileFrameY == 0)
//                if (Helper.OnScreen(new Vector2(i * 16, j * 16) - Main.screenPosition) && MagikeHelper.TryGetEntity(i, j, out BrightnessLensEntity entity))
//                {
//                    Color lightColor = Lighting.GetColor(entity.Position.ToPoint());
//                    float light = 0.3f * lightColor.R + 0.6f * lightColor.G + 0.1f * lightColor.B;
//                    entity.BrightEnough = light > 200;
//                }
//        }

//        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
//        {
//            //这是特定于照明模式的，如果您手动绘制瓷砖，请始终包含此内容
//            Vector2 offScreen = new(Main.offScreenRange);
//            if (Main.drawToScreen)
//                offScreen = Vector2.Zero;

//            //检查物块它是否真的存在
//            Point p = new(i, j);
//            Tile tile = Main.tile[p.X, p.Y];
//            if (tile == null || !tile.HasTile)
//                return;

//            //获取初始绘制参数
//            Texture2D texture = TopTexture.Value;

//            // 根据项目的地点样式拾取图纸上的框架
//            Rectangle frame = texture.Frame(2, 1, 0, 0);

//            Vector2 origin = frame.Size() / 2f;
//            Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

//            Color color = Lighting.GetColor(p.X, p.Y);

//            //这与我们之前注册的备用磁贴数据有关
//            bool direction = tile.TileFrameY / FrameHeight != 0;
//            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

//            // 一些数学魔法，使其随着时间的推移平稳地上下移动
//            Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
//            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out IMagikeContainer container))
//            {
//                if (container.Active)   //如果处于活动状态那么就会上下移动，否则就落在底座上
//                {
//                    const float TwoPi = (float)Math.PI * 2f;
//                    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
//                    drawPos += new Vector2(0f, offset * 4f);
//                    //int yframe = (int)(5 * Main.GlobalTimeWrappedHourly % 10);
//                    frame = texture.Frame(2, 1, 1, 0);
//                }
//                else
//                    drawPos += new Vector2(0, halfHeight - 16);
//            }

//            // 绘制主帖图
//            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
//        }
//    }

//    public class BrightnessLensEntity : MagikeGenerator_Normal
//    {
//        public const int sendDelay = 10 * 60;
//        public int sendTimer;
//        public bool BrightEnough;
//        public BrightnessLensEntity() : base(300, 5 * 16, 10 * 60) { }

//        public override ushort TileType => (ushort)TileType<BrightnessLensTile>();

//        public override int HowManyPerSend => 16;
//        public override int HowManyToGenerate => 16;

//        public override bool CanSend()
//        {
//            sendTimer++;
//            if (sendTimer > sendDelay)
//            {
//                sendTimer = 0;
//                return true;
//            }

//            return false;
//        }

//        public override void OnGenerate(int howMany)
//        {
//            GenerateAndChargeSelf(howMany);
//        }

//        public override bool CanGenerate()
//        {
//            return BrightEnough;
//        }

//        public override void SendVisualEffect(IMagikeContainer container)
//        {
//            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.White);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.White);
//        }
//    }
//}
