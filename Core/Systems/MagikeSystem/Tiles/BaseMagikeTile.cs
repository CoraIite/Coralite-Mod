﻿using Coralite.Content.Items.Magike;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Components;
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

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseMagikeTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false) : ModTile
    {
        public Dictionary<MagikeApparatusLevel, Asset<Texture2D>> ExtraAssets { get; private set; }

        public abstract int DropItemType { get; }

        public override void Load()
        {
            ExtraAssets = new Dictionary<MagikeApparatusLevel, Asset<Texture2D>>();

            MagikeApparatusLevel[] levels = GetAllLevels();
            if (levels == null || levels.Length == 0)
                return;

            //加载等级贴图
            for (int i = 0; i < levels.Length; i++)
                QuickLoadAsset(levels[i]);
        }

        public void QuickLoadAsset(MagikeApparatusLevel level)
        {
            if (level == MagikeApparatusLevel.None)
                return;

            string name = Enum.GetName(level);
            if (string.IsNullOrEmpty(name))
                return;

            ExtraAssets.Add(level, ModContent.Request<Texture2D>(Texture + "_" + name));
        }

        /// <summary>
        /// 返回所有可以有的魔能等级
        /// </summary>
        /// <returns></returns>
        public virtual MagikeApparatusLevel[] GetAllLevels() { return null; }

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

            //默认防岩浆
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetEntityInstance().Hook_AfterPlacement, -1, 0, true);

            //顶部是平台那么不需要下沉，并且最下面额外扩展一格
            if (topSoild)
            {
                TileObjectData.newTile.CoordinateHeights[^1] = 18;
                Main.tileSolidTop[Type] = true;
                //Main.tileSolid[Type] = true;
                Main.tileNoAttach[Type] = true;
                Main.tileTable[Type] = true;
            }
            else
            {
                TileObjectData.newTile.DrawYOffset = 2;

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
                TileObjectData.newAlternate.Width = height;
                TileObjectData.newAlternate.Height = width;
                TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Height, 0);
                TileObjectData.newAlternate.Origin = new Point16(0, width / 2);

                TileObjectData.newAlternate.CoordinateHeights = new int[width];
                Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                TileObjectData.addAlternate(height * 2 / width);

                //放置在右边
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
                TileObjectData.newAlternate.DrawYOffset = 0;
                TileObjectData.newAlternate.Width = height;
                TileObjectData.newAlternate.Height = width;
                TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Height, 0);
                TileObjectData.newAlternate.Origin = new Point16(height - 1, width / 2);
                TileObjectData.newAlternate.CoordinateHeights = new int[width];
                Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                TileObjectData.addAlternate(height * 2 / width + 1);
            }

            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
            DustType = dustType;

            MinPick = minPick;

            MagikeApparatusLevel[] levels = GetAllLevels();
            if (levels == null || levels.Length == 0)
                return;

            //加载等级字典
            MagikeSystem.RegisterApparatusLevel(Type, levels);
        }

        public abstract MagikeTileEntity GetEntityInstance();

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Tile t = Framing.GetTileSafely(i, j);
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
            if (MagikeHelper.TryGetEntity(new Point16(i, j), out MagikeTileEntity entity))
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

        public override bool RightClick(int i, int j)
        {
            if (!MagikeHelper.TryGetEntity(i, j, out MagikeTileEntity entity))
                return false;

            Main.playerInventory = true;

            UILoader.GetUIState<MagikeApparatusPanel>().visible=true;
            MagikeApparatusPanel.CurrentEntity = entity;
            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            if (!Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
                return;
            if (!cp.HasEffect(nameof(MagikeAnalyser)))
                return;

            if (!MagikeHelper.TryGetEntity(i, j, out MagikeTileEntity entity))
                return;

            //鼠标移上去时显示魔能仪器的各种信息
            IEntity entityBase = entity;
            string text = "";

            //  魔能量 / 魔能上限
            //  对于上限，如果大于基础魔能上限显示为绿色，否则显示为红色
            if (entityBase.HasComponent(MagikeComponentID.MagikeContainer))
            {
                string amountText = MagikeAmountText((MagikeContainer)entityBase.GetSingleComponent(MagikeComponentID.MagikeContainer));
                if (string.IsNullOrEmpty(text))
                    text = amountText;
                else
                    text = string.Concat(text, amountText);
            }

            //连接显示
            if (entityBase.HasComponent(MagikeComponentID.MagikeSender)
                && entityBase.GetSingleComponent(MagikeComponentID.MagikeSender) is MagikeLinerSender linerSender)
            {
                string amountText = LinerConnectText(linerSender);
                if (string.IsNullOrEmpty(text))
                    text = amountText;
                else
                    text = string.Concat(text, amountText,"\n");
            }

            //  所有的滤镜对应的的物品图标
            string filterText = FilterText(entity);
            if (string.IsNullOrEmpty(text))
                text = filterText;
            else
                text = string.Concat(text, filterText);

            //  额外自定义显示内容
            Main.instance.MouseText(text);
        }

        /// <summary>
        /// 获取魔能容量的文本
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual string MagikeAmountText(MagikeContainer container)
        {
            string colorCode;
            if (container.MagikeMax > container.MagikeMaxBase)
                colorCode = "80d3ff";//蓝色
            else if (container.MagikeMax < container.MagikeMaxBase)
                colorCode = "ff1919";//红色
            else
                colorCode = "ffffff";

            return string.Concat(MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.MagikeAmount)
                , container.Magike, " / ", $"[c/{colorCode}:{container.MagikeMax}]\n");
        }

        public virtual string LinerConnectText(MagikeLinerSender sender)
        {
            string have = "◈";
            string nothave = "◇";

            string text = MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.ConnectAmount);

            for (int i = 0; i < sender.MaxConnect; i++)
            {
                string temp;
                if (sender.Receivers.IndexInRange(i))
                    temp = have;
                else
                    temp = nothave;

                if (i >= sender.MaxConnectBase)
                    temp = $"[c/80d3ff:{temp}]";

                text = string.Concat(text, temp);
            }

            return text;
        }

        public virtual string FilterText(MagikeTileEntity entity)
        {
            string empty = $"[i:{ModContent.ItemType<BasicFilter>()}]";
            string text = "";

            List<Component> list;
            entity.Components.TryGetValue(MagikeComponentID.MagikeFilter, out list);

            for (int i = 0; i < entity.ExtendFilterCapacity; i++)
            {
                string filterIcon;
                if (list == null || !list.IndexInRange(i))
                    filterIcon = empty;
                else
                    filterIcon = $"[i:{(list[i] as MagikeFilter).ItemType}]";

                text = string.Concat(text, filterIcon);
            }

            return text;
        }

        #region 特殊绘制

        public sealed override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Point16? topLeft = MagikeHelper.ToTopLeft(i, j);

            if (!topLeft.HasValue)
                return;

            if (i == topLeft.Value.X && j == topLeft.Value.Y)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public sealed override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块
            Point16 p = new Point16(i, j);//这个就是左上角
            Terraria.Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //在这里获取帧图
            //得判断自身现在处于一个什么样的放置情况，然后获取对应的data
            MagikeHelper.GetMagikeAlternateData(i, j, out TileObjectData data, out int alternate);

            float rotation = alternate switch
            {
                1 => MathHelper.PiOver2,//在顶部，正方向朝下
                2 => 0,//朝向右
                3 => MathHelper.Pi,//朝向左
                _ => -MathHelper.PiOver2
            };

            var level = MagikeSystem.FrameToLevel(tile.TileType, tile.TileFrameX / data.CoordinateFullWidth);
            if (!level.HasValue)
                return;

            //获取初始绘制参数
            if (!ExtraAssets.TryGetValue(level.Value, out Asset<Texture2D> asset))
                return;

            if (!MagikeHelper.TryGetEntity(p, out MagikeTileEntity entity))
                return;

            Texture2D texture = asset.Value;

            Rectangle tileRect = new Rectangle(i * 16, j * 16, data == null ? 16 : data.Width * 16, data == null ? 16 : data.Height * 16);
            Vector2 offset = offScreen - Main.screenPosition;
            Color lightColor = Lighting.GetColor(p.X, p.Y);

            DrawExtraTex(spriteBatch, texture, tileRect, offset, lightColor, rotation, entity);
        }

        /// <summary>
        /// 绘制额外一层
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="tileRect"></param>
        /// <param name="offset"></param>
        /// <param name="entity"></param>
        public virtual void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity)
        {

        }

        #endregion
    }
}
