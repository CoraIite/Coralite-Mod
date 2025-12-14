using Coralite.Content.Items.Magike;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.ModPlayers;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Loaders;
using Coralite.Core.Network;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Filters;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Tiles
{
    public abstract class BaseMagikeTile(int width, int height, Color mapColor, int dustType, int minPick = 0, bool topSoild = false) : ModTile
    {
        public Dictionary<ushort, ATex> ExtraAssets { get; private set; }

        public abstract int DropItemType { get; }
        public abstract CoraliteSetsSystem.MagikeTileType PlaceType { get; }

        public void LoadAssets()
        {
            ExtraAssets = [];

            List<ushort> levels = GetAllLevels();
            if (levels == null || levels.Count == 0)
                return;

            //加载等级贴图
            for (int i = 0; i < levels.Count; i++)
                QuickLoadAsset(levels[i]);
        }

        public virtual void QuickLoadAsset(ushort level)
        {
            string name = CoraliteContent.GetMagikeLevel(level).LevelName;
            name = Texture + "_" + name;

            if (ModContent.HasAsset(name))
                ExtraAssets.Add(level, ModContent.Request<Texture2D>(name));
        }

        /// <summary>
        /// 返回所有可以有的魔能等级
        /// </summary>
        /// <returns></returns>
        public virtual List<ushort> GetAllLevels() => null;

        /// <summary>
        /// 获取可放置的物块类型
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetAnchorValidTiles() => null;

        public override void SetStaticDefaults()
        {
            //if (!VaultUtils.isServer)
            //    LoadAssets();

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
            TileObjectData.newTile.CoordinateHeights = new int[height];

            CoraliteSetsSystem.MagikeTileTypes.Add(Type, PlaceType);

            int[] tiles = GetAnchorValidTiles();
            if (tiles != null)
                TileObjectData.newTile.AnchorValidTiles = tiles;

            Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);

            //默认防岩浆
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            //TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetEntityInstance().Hook_AfterPlacement, -1, 0, true);

            //顶部是平台那么不需要下沉，并且最下面额外扩展一格
            if (PlaceType == CoraliteSetsSystem.MagikeTileType.None)
            {
                TileObjectData.newTile.CoordinateHeights[^1] = 18;
                if (topSoild)
                {
                    Main.tileSolidTop[Type] = true;
                    Main.tileTable[Type] = true;
                }

                Main.tileNoAttach[Type] = true;
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
                TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Height, 0);

                if (PlaceType == CoraliteSetsSystem.MagikeTileType.FourWayNormal)
                {
                    TileObjectData.newAlternate.Width = height;
                    TileObjectData.newAlternate.Height = width;
                    TileObjectData.newAlternate.CoordinateHeights = new int[width];
                    Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                    TileObjectData.newAlternate.Origin = new Point16(0, width / 2);

                    TileObjectData.addAlternate(height * 2 / width);
                }
                else
                {
                    TileObjectData.newAlternate.Origin = new Point16(0, height / 2);
                    TileObjectData.addAlternate(2);
                }

                //放置在右边
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
                TileObjectData.newAlternate.DrawYOffset = 0;
                TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom | AnchorType.SolidSide, TileObjectData.newAlternate.Height, 0);
                if (PlaceType == CoraliteSetsSystem.MagikeTileType.FourWayNormal)
                {
                    TileObjectData.newAlternate.Width = height;
                    TileObjectData.newAlternate.Height = width;
                    TileObjectData.newAlternate.CoordinateHeights = new int[width];
                    Array.Fill(TileObjectData.newAlternate.CoordinateHeights, 16);
                    TileObjectData.newAlternate.Origin = new Point16(height - 1, width / 2);

                    TileObjectData.addAlternate((height * 2 / width) + 1);
                }
                else
                {
                    TileObjectData.newAlternate.Origin = new Point16(width - 1, height / 2);
                    TileObjectData.addAlternate(3);
                }
            }

            TileObjectData.addTile(Type);

            AddMapEntry(mapColor);
            DustType = dustType;

            MinPick = minPick;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            if (PlaceType == CoraliteSetsSystem.MagikeTileType.None)
                return;

            GetMagikeAlternateData(i, j, out _, out MagikeAlternateStyle alternate);

            switch (alternate)
            {
                default:
                    return;
                case MagikeAlternateStyle.Top:
                    offsetY = -2;
                    return;
                case MagikeAlternateStyle.Left:
                case MagikeAlternateStyle.Right:
                    offsetY = 0;
                    return;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            OnTileKilled(i, j);
            if (TryGetEntityWithTopLeft(new Point16(i, j), out MagikeTP entity))
            {
                entity.Kill();
                MagikeApparatusPanel.CurrentEntity = null;
                UILoader.GetUIState<MagikeApparatusPanel>().visible = false;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
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

        /// <summary>
        /// 获取左上角位置，默认返回null即启用默认的逻辑
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public virtual Point16? ToTopLeft(int i, int j)
        {
            return null;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(DropItemType)];
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            if (TryGetEntity(i, j, out MagikeTP tp))
            {
                if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(Main.tile[i, j].TileType
                    , out var list))//获取帧图-》等级的键值对
                    return;

                if (list.Count < 2)//对于只有一个等级的就不动它
                    return;

                if (list[0] != NoneLevel.ID)//初始等级不是无的不动它                                         
                    return;

                ushort level = list[1];
                PolarizedFilter.ChangeTileFrame(level, tp);
                foreach (var component in tp.ComponentsCache)
                {
                    if (component is IUpgradeable upgrade)
                        upgrade.Upgrade(level);
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            //if (Main.LocalPlayer.HeldItem.ModItem != null &&
            //    Main.LocalPlayer.HeldItem.ModItem.AltFunctionUse(Main.LocalPlayer))
            //    return false;

            if (!TryGetEntity(i, j, out MagikeTP entity))
                return false;

            bool itemTimeIsZero = Main.LocalPlayer.ItemTimeIsZero;
            if (itemTimeIsZero && entity.HasComponent(MagikeComponentID.ItemGetOnlyContainer))
            {
                GetOnlyItemContainer container = entity.GetSingleComponent<GetOnlyItemContainer>(MagikeComponentID.ItemGetOnlyContainer);

                if (Main.keyState.PressingShift())//按shift加右键取出物品
                    if (container.DropItem())
                    {
                        Helper.PlayPitched(CoraliteSoundID.Dig, new Vector2(i, j) * 16);
                        return true;
                    }
            }

            if (itemTimeIsZero && entity.HasComponent(MagikeComponentID.ItemContainer))
            {
                ItemContainer container = entity.GetSingleComponent<ItemContainer>(MagikeComponentID.ItemContainer);

                if (Main.keyState.PressingShift())//按shift加右键取出物品
                    if (container.DropItem())
                    {
                        Helper.PlayPitched(CoraliteSoundID.Dig, new Vector2(i, j) * 16);
                        return true;
                    }

                //放入物品
                //有物品容器，同时可以放入物品，就直接塞进去
                Player localPlayer = Main.LocalPlayer;
                Item item = localPlayer.HeldItem;//localPlayer.inventory[localPlayer.selectedItem];
                if (localPlayer.selectedItem == 58 && !item.IsAir && !item.favorited
                    && container.CanAddItem(item.type, item.stack))
                {
                    localPlayer.GamepadEnableGrappleCooldown();
                    PlaceItemInFrame(localPlayer, entity.Position.X, entity.Position.Y, container);
                    Recipe.FindRecipes();
                    Helper.PlayPitched(CoraliteSoundID.Grab, new Vector2(i, j) * 16);

                    return true;
                }
            }

            OpenMagikeUI(entity);

            return true;
        }

        private static void OpenMagikeUI(MagikeTP entity)
        {
            Main.playerInventory = true;
            Helper.PlayPitched("Fairy/CursorExpand", 0.5f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().visible = true;
            MagikeApparatusPanel.CurrentEntity = entity;
            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
        }

        public static void PlaceItemInFrame(Player player, int i, int j, ItemContainer container)
        {
            if (!player.ItemTimeIsZero)
                return;

            if (VaultUtils.isClient)
            {
                NetMessage.SendData(MessageID.ItemFrameTryPlacing, -1, -1, null, i, j, player.selectedItem, player.whoAmI, 1);

                Item item = player.inventory[player.selectedItem].Clone();
                player.inventory[player.selectedItem].TurnToAir();
                container.AddItem(item);

                ModPacket modPacket = Coralite.Instance.GetPacket();
                modPacket.Write((byte)CoraliteNetWorkEnum.ItemContainer_SpecificIndex);
                modPacket.Write(Main.myPlayer);
                modPacket.WritePoint16(container.Entity.Position);
                ItemIO.Send(item, modPacket, true);

                modPacket.Send();
            }
            else if (VaultUtils.isSinglePlayer)
            {
                Item item = player.inventory[player.selectedItem].Clone();
                player.inventory[player.selectedItem].TurnToAir();
                container.AddItem(item);
            }

            if (player.selectedItem == 58)
                Main.mouseItem = player.inventory[player.selectedItem].Clone();

            player.releaseUseItem = false;
            player.mouseInterface = true;
            player.PlayDroppedItemAnimation(20);
        }

        public override void MouseOver(int i, int j)
        {
            if (!Main.LocalPlayer.TryGetModPlayer(out CoralitePlayer cp))
                return;
            if (!cp.HasEffect(nameof(MagikeAnalyser)))
                return;

            if (!TryGetEntity(i, j, out MagikeTP entity))
                return;

            //鼠标移上去时显示魔能仪器的各种信息
            Main.LocalPlayer.noThrow = 2;
            string text = "";

            //可升级字样
            text = UpgradeableText(entity);

            //  魔能量 / 魔能上限
            //  对于上限，如果大于基础魔能上限显示为绿色，否则显示为红色
            if (string.IsNullOrEmpty(text))
                text = MagikeAmountText(entity);
            else
                text = string.Concat(text, Environment.NewLine, MagikeAmountText(entity));

            //连接显示
            if (entity.HasComponent(MagikeComponentID.MagikeSender)
                && entity.GetSingleComponent(MagikeComponentID.MagikeSender) is MagikeLinerSender linerSender)
            {
                string amountText = LinerConnectText(linerSender);
                if (string.IsNullOrEmpty(text))
                    text = amountText;
                else
                    text = string.Concat(text, "\n", amountText);
            }

            //  所有的滤镜对应的的物品图标
            string filterText = FilterText(entity);
            if (string.IsNullOrEmpty(text))
                text = filterText;
            else
                text = string.Concat(text, "\n", filterText);

            if (entity.HasComponent(MagikeComponentID.MagikeFactory))
            {
                var factory = entity.GetSingleComponent<MagikeFactory>(MagikeComponentID.MagikeFactory);

                if (factory.IsWorking)
                {
                    string workText = MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.IsWorking);
                    if (string.IsNullOrEmpty(text))
                        text = workText;
                    else
                        text = string.Concat(text, "\n", workText);
                }
            }

            //  额外自定义显示内容
            Main.instance.MouseText(text);

            MagikeSystem.DrawSpecialTileText = true;
            MagikeSystem.SpecialTileText = text;
        }

        public string UpgradeableText(MagikeTP entity)
        {
            if (entity.TryGetComponent(MagikeComponentID.ApparatusInformation, out ApparatusInformation info))
            {
                if (!info.TryGetUpgradeableLevel(out MagikeLevel level)|| !info.ShowPolarizedTip)
                    return "";

                return $"{MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.Upgradeable)} [i:{level.PolarizedFilterItemType}] {MagikeSystem.GetMALevelText(level.Type)}";
            }

            return "";
        }

        /// <summary>
        /// 获取魔能容量的文本
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public virtual string MagikeAmountText(MagikeTP entity)
        {
            if (entity.HasComponent(MagikeComponentID.MagikeContainer))
            {
                var container = entity.GetMagikeContainer();
                string colorCode = GetBonusColorCode(container.MagikeMaxBonus);

                return string.Concat(MagikeSystem.GetApparatusDescriptionText(MagikeSystem.ApparatusDescriptionID.MagikeAmount)
                , $"{container.MagikeText} / [c/{colorCode}:{container.MagikeMaxText}]");
            }

            return "";
        }

        public virtual string LinerConnectText(MagikeLinerSender sender)
        {
            string have = "◆";
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

        public virtual string FilterText(MagikeTP entity)
        {
            string empty = $"[i:{ModContent.ItemType<BasicFilter>()}]";
            string text = "";

            List<MagikeComponent> list = null;
            if (entity.HasComponent(MagikeComponentID.MagikeFilter))
                list = entity.Components[MagikeComponentID.MagikeFilter] as List<MagikeComponent>;

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

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            Point16? topLeft = MagikeHelper.ToTopLeft(i, j);

            if (!topLeft.HasValue)
                return;

            if (i == topLeft.Value.X && j == topLeft.Value.Y)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public sealed override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            //检查物块
            Point16 p = new(i, j);//这个就是左上角
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            //在这里获取帧图
            //得判断自身现在处于一个什么样的放置情况，然后获取对应的data
            GetMagikeAlternateData(i, j, out TileObjectData data, out MagikeAlternateStyle alternate);

            float rotation = alternate.GetAlternateRotation();

            Rectangle tileRect = new(i * 16, j * 16, data == null ? 16 : data.Width * 16, data == null ? 16 : data.Height * 16);
            Vector2 offset = offScreen - Main.screenPosition;
            Color lightColor = Lighting.GetColor(p.X, p.Y);

            if (!TryGetEntityWithTopLeft(p, out MagikeTP entity))
                return;

            PreDrawExtra(spriteBatch, tileRect, offScreen, lightColor, rotation, entity);

            var level = NoneLevel.ID;

            if (entity.TryGetComponent(MagikeComponentID.ApparatusInformation, out ApparatusInformation info))
                level = info.CurrentLevel;

            if (level == NoneLevel.ID)
                return;

            //获取初始绘制参数
            if (!ExtraAssets.TryGetValue(level, out ATex asset))
                return;

            Texture2D texture = asset.Value;

            DrawExtraTex(spriteBatch, texture, tileRect, offset, lightColor, rotation, entity, level);
        }

        /// <summary>
        /// 额外绘制一层，这一层在绘制特定的贴图之前，没有额外贴图的话也会调用到
        /// </summary>
        public virtual void PreDrawExtra(SpriteBatch spriteBatch, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity)
        {

        }

        /// <summary>
        /// 绘制额外一层
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="tileRect"></param>
        /// <param name="offset"></param>
        /// <param name="entity"></param>
        public virtual void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, ushort level)
        {

        }

        #endregion

        public override void HitWire(int i, int j)
        {
            if (TryGetEntityWithComponent(i, j, MagikeComponentID.MagikeFactory, out MagikeTP entity))
                entity.GetSingleComponent<MagikeFactory>(MagikeComponentID.MagikeFactory).Activation(out _);
        }
    }
}
