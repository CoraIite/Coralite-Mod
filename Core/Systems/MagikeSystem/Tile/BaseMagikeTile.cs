using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Tile
{
    public abstract class BaseMagikeTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false) : ModTile
    {
        public Dictionary<MagikeApparatusLevel, Asset<Texture2D>> ExtraAssets { get; private set; }

        public abstract int DropItemType { get; }

        public override void Load()
        {
            ExtraAssets = new Dictionary<MagikeApparatusLevel, Asset<Texture2D>>();
            RegisterApparatusLevel();
            LoadAssets();
        }

        /// <summary>
        /// 加载额外贴图
        /// </summary>
        public virtual void LoadAssets()
        {

        }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(width / 2, height - 1);
            //TileObjectData.newTile.StyleWrapLimit = 100;
            TileObjectData.newTile.CoordinateHeights = new int[height];
            Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);

            //顶部是平台那么不需要下沉，并且最下面额外扩展一格
            if (topSoild)
            {
                TileObjectData.newTile.CoordinateHeights[^1] = 18;
                Main.tileSolidTop[Type] = true;
                Main.tileSolid[Type] = true;
            }
            else
            {
                TileObjectData.newTile.DrawYOffset = 2;

                //默认防岩浆
                TileObjectData.newTile.LavaDeath = false;
                TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
                TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
                TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetEntityInstance().Hook_AfterPlacement, -1, 0, true);

                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.AnchorLeft = AnchorData.Empty;
                TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
                TileObjectData.newAlternate.AnchorTop = AnchorData.Empty;

                TileObjectData.addAlternate(0);

                //放置在天花板上
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                TileObjectData.newAlternate.DrawYOffset = -2;
                TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
                TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Width, 0);
                TileObjectData.newAlternate.Origin = new Point16(width / 2, 0);

                TileObjectData.addAlternate(1);

                //放置在左边
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
                TileObjectData.newAlternate.DrawYOffset = 0;
                TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Width, 0);
                TileObjectData.newAlternate.Width = height;
                TileObjectData.newAlternate.Height = width;
                TileObjectData.newAlternate.Origin = new Point16(0, width / 2);

                TileObjectData.newAlternate.CoordinateHeights = new int[width];
                Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                TileObjectData.addAlternate(height * 2);

                //放置在右边
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
                TileObjectData.newAlternate.DrawYOffset = 0;
                TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Width, 0);
                TileObjectData.newAlternate.Width = height;
                TileObjectData.newAlternate.Height = width;
                TileObjectData.newAlternate.Origin = new Point16(height - 1, width / 2);
                TileObjectData.newAlternate.CoordinateHeights = new int[width];
                Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                TileObjectData.addAlternate(height * 2 + width);
            }

            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
            DustType = dustType;

            MinPick = minPick;
        }

        public abstract BaseMagikeTileEntity GetEntityInstance();

        /// <summary>
        /// 在这里注册魔能仪器的等级，用于改变帧图和判断能否插入偏振滤镜<br></br>
        /// 请使用<see cref="MagikeSystem.RegisterApparatusLevel"/>
        /// </summary>
        public virtual void RegisterApparatusLevel() { }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Terraria.Tile t = Framing.GetTileSafely(i, j);
            if (Main.tileSolidTop[t.TileType])
                return;

            MagikeHelper.GetMagikeAlternateData(i, j, out _, out int alternate);

            switch (alternate)
            {
                default:
                case 0:
                    return;
                case 1:
                    offsetY = -2;
                    return;
                case 2:
                case 3:
                    offsetY = 0;
                    return;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            OnTileKilled(i, j);
            if (MagikeHelper.TryGetEntity(new Point16(i, j), out BaseMagikeTileEntity entity))
            {
                (entity as IEntity).RemoveAllComponent();
                entity.Kill(entity.Position.X, entity.Position.Y);
            }
        }

        /// <summary>
        /// 在物块破坏时触发，用于生成音效等特殊效果
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        public virtual void OnTileKilled(int i, int j)
        {
            SoundEngine.PlaySound(CoraliteSoundID.GlassBroken_Shatter, new Vector2(i, j) * 16);
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(DropItemType)];
        }

        public override void MouseOver(int i, int j)
        {
            //鼠标移上去时显示魔能仪器的各种信息，不需要任何东西
            //  魔能量 / 魔能上限
            //  对于上限，如果大于基础魔能上限显示为绿色，否则显示为红色
            //  当前魔能量的百分比
            //  所有的滤镜对应的的物品图标
            //  额外自定义显示内容


            //MagikeHelper.ShowMagikeNumber(i, j);
        }

        public sealed override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Point16 topLeft = MagikeHelper.ToTopLeft(i, j).Value;
            if (i == topLeft.X && j == topLeft.Y)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public sealed override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块
            Point16 p = new Point16(i, j);
            Terraria.Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //在这里获取帧图
            //得判断自身现在处于一个什么样的放置情况，然后获取对应的data






            var level = MagikeSystem.FrameToLevel(tile.TileType, 1);
            if (!level.HasValue)
                return;

            //获取初始绘制参数
            if (!ExtraAssets.TryGetValue(level.Value, out Asset<Texture2D> asset))
                return;

            if (!MagikeHelper.TryGetEntity(p, out BaseMagikeTileEntity entity))
                return;

            Texture2D texture = asset.Value;
            TileObjectData data = TileObjectData.GetTileData(tile);

            Rectangle tileRect = new Rectangle(p.X * 16, p.Y * 16, data == null ? 16 : data.Width * 16, data == null ? 16 : data.Height * 16);
            Vector2 offset = offScreen - Main.screenPosition;
            Color lightColor = Lighting.GetColor(p.X, p.Y);

            DrawExtraTex(spriteBatch, texture, tileRect, offset, lightColor, entity);
        }

        /// <summary>
        /// 绘制额外一层
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="tileRect"></param>
        /// <param name="offset"></param>
        /// <param name="entity"></param>
        public virtual void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, BaseMagikeTileEntity entity)
        {

        }

        public MagikeApparatusLevel FrameToLevel(int frameX)
        {
            return (MagikeApparatusLevel)frameX;
        }
    }
}
