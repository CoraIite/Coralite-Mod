﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.RemodelPools
{
    public class CrystalRemodelPool : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public CrystalRemodelPool() : base(TileType<CrystalRemodelPoolTile>(), Item.sellPrice(0, 0, 10, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeRemodelPools)
        { }

        public override int MagikeMax => 150;
        public string WorkTimeMax => "5";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(10)
                .AddIngredient<Basalt>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrystalRemodelPoolTile : BaseRemodelPool
    {
        public override string ExtraTextureName => AssetDirectory.MagikeRemodelPoolTiles + "BrilliantRemodelPoolTile_Top";

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalRemodelPoolEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.MagicCrystalPink);
            DustType = DustID.CrystalSerpent_Pink;
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

            Texture2D texture = ExtraTexture.Value;

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
            if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out MagikeFactory_RemodelPool pool))
            {
                if (pool.magike > 0) //大于0时才会绘制
                {
                    spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
                }

                DrawItem(pool, drawPos, spriteBatch);
            }
        }
    }

    public class CrystalRemodelPoolEntity : MagikeFactory_RemodelPool
    {
        public CrystalRemodelPoolEntity() : base(150, 5 * 60) { }

        public override ushort TileType => (ushort)TileType<CrystalRemodelPoolTile>();

        public override Color MainColor => Coralite.Instance.MagicCrystalPink;
    }
}