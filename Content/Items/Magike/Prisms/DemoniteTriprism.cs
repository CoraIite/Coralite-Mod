﻿//using Coralite.Content.Items.Glistent;
//using Coralite.Content.Raritys;
//using Coralite.Core;
//using Coralite.Core.Systems.MagikeSystem;
//using Coralite.Core.Systems.MagikeSystem.BaseItems;
//using Coralite.Core.Systems.MagikeSystem.Tiles;
//using Coralite.Core.Systems.MagikeSystem.TileEntities;
//using Coralite.Helpers;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ObjectData;
//using static Terraria.ModLoader.ModContent;

//namespace Coralite.Content.Items.Magike.MultPrisms
//{
//    public class DemoniteTriprism : BaseMagikePlaceableItem, IMagikePolymerizable, IMagikeSenderItem
//    {
//        public DemoniteTriprism() : base(TileType<DemoniteTriprismTile>(), Item.sellPrice(0, 0, 50, 0)
//            , RarityType<MagicCrystalRarity>(), 25, AssetDirectory.MagikeMultPrisms)
//        { }

//        public override int MagikeMax => 50;
//        public string SendDelay => "4.5";
//        public int HowManyPerSend => 15;
//        public int ConnectLengthMax => 10;
//        public int HowManyCanConnect => 3;

//        public void AddMagikePolymerizeRecipe()
//        {
//            PolymerizeRecipe.CreateRecipe<DemoniteTriprism>(50)
//                .SetMainItem<Diprism>()
//                .AddIngredient<GlistentBar>()
//                .AddIngredient(ItemID.ShadowScale, 2)
//                .Register();
//        }
//    }

//    public class DemoniteTriprismTile : OldBaseRefractorTile
//    {
//        public override string Texture => AssetDirectory.MagikeRefractorTiles + "DemoniteRefractorTile";

//        public override void SetStaticDefaults()
//        {
//            //Main.tileShine[Type] = 400;
//            Main.tileFrameImportant[Type] = true;
//            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
//            TileID.Sets.IgnoredInHouseScore[Type] = true;

//            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
//            TileObjectData.newTile.Height = 2;
//            TileObjectData.newTile.CoordinateHeights = new int[2] {
//                16,
//                16
//            };
//            TileObjectData.newTile.DrawYOffset = 2;
//            TileObjectData.newTile.LavaDeath = false;
//            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DemoniteTriprismEntity>().Hook_AfterPlacement, -1, 0, true);

//            TileObjectData.addTile(Type);

//            AddMapEntry(Color.MediumPurple);
//            DustType = DustID.Demonite;
//        }

//        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
//        {
//            //这是特定于照明模式的，如果您手动绘制瓷砖，请始终包含此内容
//            Vector2 offScreen = new Vector2(Main.offScreenRange);
//            if (Main.drawToScreen)
//                offScreen = Vector2.Zero;

//            //检查物块它是否真的存在
//            Point p = new Point(i, j);
//            Tile tile = Main.tile[p.X, p.Y];
//            if (tile == null || !tile.HasTile)
//                return;

//            //获取初始绘制参数
//            Texture2D texture = TopTexture.Value;

//            // 根据项目的地点样式拾取图纸上的框架
//            int frameY = tile.TileFrameX / FrameWidth;
//            Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

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
//                    const float TwoPi = (float)Math.PI * 4f;
//                    float offset = MathF.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
//                    drawPos += new Vector2(0f, offset * 2f);
//                }
//                else
//                    drawPos += new Vector2(0, halfHeight - 8);
//            }

//            // 绘制主帖图
//            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
//        }
//    }

//    public class DemoniteTriprismEntity : MagikeSender_Line
//    {
//        public const int sendDelay = 4 * 60 + 30;
//        public int sendTimer;
//        public DemoniteTriprismEntity() : base(50, 10 * 16, 3) { }

//        public override int HowManyPerSend => 15;

//        public override ushort TileType => (ushort)TileType<DemoniteTriprismTile>();

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

//        public override void SendVisualEffect(IMagikeContainer container)
//        {
//            MagikeHelper.SpawnDustOnSend(1, 2, Position, container, Color.MediumPurple);
//        }

//        public override void OnReceiveVisualEffect()
//        {
//            MagikeHelper.SpawnDustOnGenerate(1, 2, Position, Color.MediumPurple);
//        }
//    }

//}
