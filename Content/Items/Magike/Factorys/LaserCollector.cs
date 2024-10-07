﻿using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    public class LaserCollector() : MagikeApparatusItem(TileType<LaserCollectorTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeFactories)
    {
        public static LocalizedText NoTile { get;private set; }
        public static LocalizedText NoCrystalCluster { get;private set; }
        public static LocalizedText LevelIncorrect { get;private set; }
        public static LocalizedText MagikeNotEnough { get;private set; }

        public override void Load()
        {
            NoTile = this.GetLocalization(nameof(NoTile));
            NoCrystalCluster = this.GetLocalization(nameof(NoCrystalCluster));
            LevelIncorrect = this.GetLocalization(nameof(LevelIncorrect));
            MagikeNotEnough = this.GetLocalization(nameof(MagikeNotEnough));
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(20)
                .AddIngredient<MagicCrystalBlock>(10)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LaserCollectorTile() : BaseMagikeTile
        (1, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<LaserCollector>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<LaserCollectorTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
            ];
        }

        public override void QuickLoadAsset(MALevel level) { }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) { }
    }

    public class LaserCollectorTileEntity() : MagikeTileEntity()
    {
        public sealed override ushort TileType => (ushort)TileType<LaserCollectorTile>();

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartFactory());
            AddComponent(GetStartGetOnlyItemContainer());
        }

        public MagikeContainer GetStartContainer()
            => new LaserCollectorContainer();

        public MagikeFactory GetStartFactory()
            => new LaserCollectorFactory();

        public GetOnlyItemContainer GetStartGetOnlyItemContainer()
            => new GetOnlyItemContainer()
            {
                CapacityBase = 4,
            };
    }

    public class LaserCollectorContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 200,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class LaserCollectorFactory : MagikeFactory, IUIShowable, IUpgradeable
    {
        public bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";
            Point16 point = (Entity as MagikeTileEntity).Position;

            if (!TryGetTile(point, out Tile tile))
            {
                text = LaserCollector.NoTile.Value;
                return false;
            }

            if (!CheckTile(tile, out ICrystalCluster crystalCluster))
            {
                text = LaserCollector.NoCrystalCluster.Value;
                return false;
            }

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            var level = MagikeSystem.FrameToLevel(Framing.GetTileSafely(point).TileType, tile.TileFrameX / data.CoordinateFullWidth);

            if (!level.HasValue || level.Value != crystalCluster.Level)
            {
                text = LaserCollector.LevelIncorrect.Value;
                return false;
            }

            if (Entity.GetMagikeContainer().Magike < crystalCluster.MagikeCost)
            {
                text = LaserCollector.MagikeNotEnough.Value;
                return false;
            }

            return true;
        }

        private static bool TryGetTile(Point16 point,out Tile tile)
        {
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            tile = default ;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    tile = Framing.GetTileSafely(point.X, point.Y - 1);
                    break;
                case MagikeAlternateStyle.Top:
                    tile = Framing.GetTileSafely(point.X, point.Y + data.Height);
                    break;
                case MagikeAlternateStyle.Left:
                    tile = Framing.GetTileSafely(point.X + data.Width, point.Y);
                    break;
                case MagikeAlternateStyle.Right:
                    tile = Framing.GetTileSafely(point.X - 1, point.Y);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public bool CheckTile(Tile tile,out ICrystalCluster crystalCluster)
        {
            crystalCluster = null;
            if (!tile.HasTile)
                return false;

            if (TileLoader.GetTile(tile.TileType) is ICrystalCluster crystalCluster1)
            {
                crystalCluster = crystalCluster1;
                return true;
            }

            return false;
        }

        public override void Work()
        {
            Point16 point = (Entity as MagikeTileEntity).Position;

            string text = "";

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

            Tile? tile = null;
            Point16 topPos = point;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    topPos = new Point16(point.X, point.Y - 1);
                    tile = Framing.GetTileSafely(topPos);
                    break;
                case MagikeAlternateStyle.Top:
                    topPos = new Point16(point.X, point.Y + data.Height);
                    tile = Framing.GetTileSafely(topPos);
                    break;
                case MagikeAlternateStyle.Left:
                    topPos = new Point16(point.X + data.Width, point.Y);
                    tile = Framing.GetTileSafely(topPos);
                    break;
                case MagikeAlternateStyle.Right:
                    topPos = new Point16(point.X - 1, point.Y);
                    tile = Framing.GetTileSafely(topPos);
                    break;
                default:
                    break;
            }

            if (!tile.HasValue)
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = LaserCollector.NoTile.Value,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Helper.GetMagikeTileCenter((Entity as MagikeTileEntity).Position) - (Vector2.UnitY * 32));
                return;
            }

            if (!CheckTile(tile.Value, out ICrystalCluster crystalCluster))
                text = LaserCollector.NoCrystalCluster.Value;

            if (Entity.GetMagikeContainer().Magike < crystalCluster.MagikeCost)
                text = LaserCollector.MagikeNotEnough.Value;

            if (!string.IsNullOrEmpty(text))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.MagicCrystalPink,
                    Text = text,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Helper.GetMagikeTileCenter((Entity as MagikeTileEntity).Position) - (Vector2.UnitY * 32));
                return;
            }

            Entity.GetMagikeContainer().ReduceMagike(crystalCluster.MagikeCost);

            if (Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                container.AddItem(crystalCluster.ItemType, crystalCluster.ItemStack);

            Vector2 center = topPos.ToWorldCoordinates();
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(center, DustID.FireworksRGB, (i * MathHelper.TwoPi / 12).ToRotationVector2() * 2, newColor: Coralite.MagicCrystalPink);
                d.noGravity = true;
            }
        }

        public void ShowInUI(UIElement parent)
        {
        }

        public void Upgrade(MALevel incomeLevel)
        {
            WorkTimeBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 8,
                _ => 10_0000_0000/60,
            };

            WorkTimeBase *= 60;
        }
    }
}