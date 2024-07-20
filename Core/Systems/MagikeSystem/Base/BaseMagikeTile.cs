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
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.Base
{
    public class BaseMagikeTile<TEntity>(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false) : ModTile
        where TEntity : BaseMagikeTileEntity
    {
        public Dictionary<MagikeApparatusLevel, Asset<Texture2D>> ExtraAssets { get; private set; }

        public override void Load()
        {
            ExtraAssets = new Dictionary<MagikeApparatusLevel, Asset<Texture2D>>();
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

            TileObjectData.newTile.CoordinateHeights = new int[height];
            Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);

            //顶部是平台那么不需要下沉，并且最下面额外扩展一格
            if (topSoild)
            {
                TileObjectData.newTile.CoordinateHeights[^1] = 18;
                Main.tileSolidTop[Type] = true;
            }
            else
                TileObjectData.newTile.DrawYOffset = 2;

            //默认防岩浆
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<TEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
            DustType = dustType;

            MinPick = minPick;

        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            OnTileKilled(i, j);
            if (MagikeHelper.TryGetEntity(i, j, out BaseMagikeTileEntity entity))
                entity.Kill(entity.Position.X, entity.Position.Y);
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

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块
            Point16 p = new Point16(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //获取初始绘制参数
            if (!ExtraAssets.TryGetValue(FrameToLevel(MagikeHelper.GetFrameX(p).Value), out Asset<Texture2D> asset))
                return;
            
            Texture2D texture = asset.Value;

            //// 根据项目的地点样式拾取图纸上的框架
            //int frameY = tile.TileFrameX / FrameWidth;
            //Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

            //Vector2 origin = frame.Size() / 2f;
            //Vector2 worldPos = p.ToWorldCoordinates(halfWidth, halfHeight);

            //Color color = Lighting.GetColor(p.X, p.Y);

            ////这与我们之前注册的备用磁贴数据有关
            //bool direction = tile.TileFrameY / FrameHeight != 0;
            //SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            //Vector2 drawPos = worldPos + offScreen - Main.screenPosition;
            //float rotation = 0;
            //if (MagikeHelper.TryGetEntityWithTopLeft(i, j, out MagikeSender_Line sender))
            //{
            //    if (sender.receiverPoints[0] == Point16.NegativeOne)
            //        drawPos += new Vector2(0, halfHeight - 8);
            //    else
            //        rotation = (sender.receiverPoints[0].ToWorldCoordinates() - p.ToWorldCoordinates()).ToRotation() + 1.57f;
            //}

            //// 绘制主帖图
            //spriteBatch.Draw(texture, drawPos, frame, color, rotation, origin, 1f, effects, 0f);
        }

        public MagikeApparatusLevel FrameToLevel(int feameX)
        {
            return MagikeApparatusLevel.None;
        }
    }
}
