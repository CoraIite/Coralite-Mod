using Coralite.Content.Items.DigDigDig.Stonelimes;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries1;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    public class StoneMaker() : MagikeApparatusItem(TileType<StoneMakerTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeFactories)
    {
        public static LocalizedText CantMake { get; private set; }
        public static LocalizedText MagikeNotEnough { get; private set; }

        public override void Load()
        {
            CantMake = this.GetLocalization(nameof(CantMake));
            MagikeNotEnough = this.GetLocalization(nameof(MagikeNotEnough));
        }

        public override void Unload()
        {
            CantMake = null;
            MagikeNotEnough = null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddIngredient<MagicCrystalBrick>(4)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .AddCondition(CoraliteConditions.NotInDigDigDig)
                .Register();

            CreateRecipe()
                .AddIngredient<StoneMakerCore>()
                .AddIngredient<Basalt>(2)
                .AddIngredient<HardBasalt>(3)
                .AddIngredient<MagicCrystalBrick>(2)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddCondition(CoraliteConditions.InDigDigDig)
                .AddCondition(CoraliteConditions.UseMultiBlockStructure)
                .Register();
        }
    }

    public class StoneMakerTile() : BaseMagikeTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<StoneMaker>();

        public override CoraliteSets.MagikeTileType PlaceType => CoraliteSets.MagikeTileType.FourWayNormal;

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
            ];
        }

        public override void QuickLoadAsset(MALevel level) { }

        public override void DrawExtra(SpriteBatch spriteBatch, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);

            if (entity.TryGetComponent(MagikeComponentID.MagikeFactory, out StoneMakerFactory stoneMaker))
            {
                if (!StoneMakerFactory.TryGetTile(entity.Position, out Tile tile))
                    return;

                int? itemType = stoneMaker.GetStoneItemType(tile);
                if (!itemType.HasValue)
                    return;

                Main.instance.LoadItem(itemType.Value);
                Texture2D mainTex = TextureAssets.Item[itemType.Value].Value;

                spriteBatch.Draw(mainTex, drawPos + rotation.ToRotationVector2() * 24 - Main.screenPosition, null, lightColor, 0, mainTex.Size() / 2, 1, 0, 0);
            }
        }
    }

    public class StoneMakerTileEntity() : MagikeTP()
    {
        public sealed override int TargetTileID => TileType<StoneMakerTile>();

        public override int MainComponentID => MagikeComponentID.MagikeFactory;

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartFactory());
            AddComponent(GetStartGetOnlyItemContainer());
        }

        public MagikeContainer GetStartContainer()
            => new StoneMakerContainer();

        public MagikeFactory GetStartFactory()
            => new StoneMakerFactory();

        public GetOnlyItemContainer GetStartGetOnlyItemContainer()
            => new GetOnlyItemContainer()
            {
                CapacityBase = 4,
            };
    }

    public class StoneMakerContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 200,
                _ => 0,
            };
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase * 2;
            //LimitAntiMagikeAmount();
        }
    }

    public class StoneMakerFactory : MagikeFactory, IUIShowable, IUpgradeable
    {
        public int ProduceItemType;

        /// <summary>
        /// 工作消耗
        /// </summary>
        public int WorkCost { get; set; }

        #region 升级部分

        public bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public void Upgrade(MALevel incomeLevel)
        {
            WorkTimeBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 10,
                _ => 10_0000_0000 / 60,
            };

            WorkCost = incomeLevel switch
            {
                MALevel.MagicCrystal => 20,
                _ => 0,
            };

            WorkTimeBase *= 60;
        }

        #endregion

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";
            Point16 point = Entity.Position;

            if (!TryGetTile(point, out Tile tile))
            {
                text = MagikeSystem.Error.Value;
                return false;
            }

            int? itemType = GetStoneItemType(tile);

            if (!itemType.HasValue)
            {
                text = StoneMaker.CantMake.Value;
                return false;
            }

            if (Entity.GetMagikeContainer().Magike < WorkCost)
            {
                text = StoneMaker.MagikeNotEnough.Value;
                return false;
            }

            ProduceItemType = itemType.Value;
            return true;
        }

        public static bool TryGetTile(Point16 point, out Tile tile)
        {
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            tile = default;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    tile = Framing.GetTileSafely(point.X, point.Y + data.Height);
                    break;
                case MagikeAlternateStyle.Top:
                    tile = Framing.GetTileSafely(point.X, point.Y - 1);
                    break;
                case MagikeAlternateStyle.Left:
                    tile = Framing.GetTileSafely(point.X - 1, point.Y);
                    break;
                case MagikeAlternateStyle.Right:
                    tile = Framing.GetTileSafely(point.X + data.Width, point.Y);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Work()
        {
            string text = "";

            Point16 point = Entity.Position;
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            Vector2 topPos = point.ToVector2() * 16;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    topPos += new Vector2(data.Width / 2, -0.5f) * 16;
                    break;
                case MagikeAlternateStyle.Top:
                    topPos += new Vector2(data.Width / 2, data.Height + 0.5f) * 16;
                    break;
                case MagikeAlternateStyle.Left:
                    topPos += new Vector2(data.Width + 0.5f, data.Height / 2) * 16;
                    break;
                case MagikeAlternateStyle.Right:
                    topPos += new Vector2(-0.5f, data.Height / 2) * 16;
                    break;
                default:
                    break;
            }

            if (Entity.GetMagikeContainer().Magike < WorkCost)
                text = StoneMaker.MagikeNotEnough.Value;

            if (!string.IsNullOrEmpty(text))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = text,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Helper.GetMagikeTileCenter(Entity.Position) - (Vector2.UnitY * 32));
                return;
            }

            Entity.GetMagikeContainer().ReduceMagike(WorkCost);

            if (Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                container.AddItem(ProduceItemType, 1);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(topPos, DustID.FireworksRGB, (i * MathHelper.TwoPi / 12).ToRotationVector2() * 2, newColor: Coralite.MagicCrystalPink);
                d.noGravity = true;
            }
        }

        public override void OnWorking()
        {
            float factor = 1 - Timer / (float)WorkTime;

            Point16 point = Entity.Position;
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            Vector2 topPos = point.ToVector2() * 16;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    topPos += new Vector2(data.Width / 2, -0.5f) * 16;
                    break;
                case MagikeAlternateStyle.Top:
                    topPos += new Vector2(data.Width / 2, data.Height + 0.5f) * 16;
                    break;
                case MagikeAlternateStyle.Left:
                    topPos += new Vector2(data.Width + 0.5f, data.Height / 2) * 16;
                    break;
                case MagikeAlternateStyle.Right:
                    topPos += new Vector2(-0.5f, data.Height / 2) * 16;
                    break;
                default:
                    break;
            }

            float width = 24 - factor * 22;
            Dust dust = Dust.NewDustPerfect(topPos + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: Coralite.MagicCrystalPink);
            dust.noGravity = true;
        }

        public int? GetStoneItemType(Tile tile)
        {
            if (tile.TileType == TileType<BasaltTile>())
                return ItemType<Basalt>();

            if (tile.TileType == TileType<SkarnTile>())
                return ItemType<Skarn>();

            return tile.TileType switch
            {
                TileID.Dirt => ItemID.DirtBlock,
                TileID.Stone => ItemID.StoneBlock,
                TileID.Marble => ItemID.Marble,
                TileID.Granite => ItemID.Granite,
                TileID.Ebonstone => ItemID.EbonstoneBlock,
                TileID.Crimstone => ItemID.CrimstoneBlock,
                TileID.Ash => ItemID.AshBlock,
                TileID.Pearlstone => ItemID.PearlstoneBlock,
                _ => null
            };
        }

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.StoneMakerName, parent);

            UIList list =
            [
                //工作时间
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.FactoryWorkTime) , parent),
                this.NewTextBar(SendDelayText , parent),

                //生产物品
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.StoneMakerOutPut), parent),
                this.NewTextBar(c =>
                {
                    if (!TryGetTile(c.Entity.Position, out Tile tile))
                        return "";

                    int? itemType = c.GetStoneItemType(tile);
                    if (!itemType.HasValue)
                        return "";

                    Main.instance.LoadItem(itemType.Value);
                    return $"[i:{itemType.Value}] {ContentSamples.ItemsByType[itemType.Value].Name}";
                } , parent),
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion

        #region 存取部分

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
            tag.Add(preName + nameof(WorkCost), WorkCost);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
            WorkCost = tag.GetInt(preName + nameof(WorkCost));
        }

        #endregion
    }
}
