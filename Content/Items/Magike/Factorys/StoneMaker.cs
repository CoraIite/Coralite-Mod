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
                .Register();
        }
    }

    public class StoneMakerTile() : BaseMagikeTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<StoneMaker>();

        public override MagikeTileEntity GetEntityInstance() => GetInstance<StoneMakerTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
            ];
        }

        public override void QuickLoadAsset(MALevel level) { }

        public override void DrawExtra(SpriteBatch spriteBatch, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTileEntity entity)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset;
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);

            if (entity.TryGetComponent(MagikeComponentID.MagikeFactory,out StoneMakerFactory stoneMaker))
            {
                if (!StoneMakerFactory.TryGetTile(entity.Position, out Tile tile))
                    return ;

                int? itemType = stoneMaker.GetStoneItemType(tile);
                if (!itemType.HasValue)
                    return;

                Main.instance.LoadItem(itemType.Value);
                Texture2D mainTex = TextureAssets.Item[itemType.Value].Value;

                spriteBatch.Draw(mainTex, drawPos + rotation.ToRotationVector2() * 24-Main.screenPosition, null, lightColor, 0, mainTex.Size() / 2, 1, 0, 0);
            }
        }
    }

    public class StoneMakerTileEntity() : MagikeTileEntity()
    {
        public sealed override ushort TileType => (ushort)TileType<StoneMakerTile>();

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

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class StoneMakerFactory : MagikeFactory, IUIShowable, IUpgradeable
    {
        public int ProduceItemType;

        public bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        /// <summary>
        /// 工作消耗
        /// </summary>
        public int WorkCost { get; set; }

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

            ProduceItemType=itemType.Value;
            return true;
        }

        public static bool TryGetTile(Point16 point, out Tile tile)
        {
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            tile = default;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    tile = Framing.GetTileSafely(point.X, point.Y +data.Height);
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
            Point16 point = (Entity as MagikeTileEntity).Position;

            string text = "";

            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out MagikeAlternateStyle alternate);

            Point16 topPos = point;

            switch (alternate)
            {
                case MagikeAlternateStyle.Bottom:
                    topPos = new Point16(point.X+data.Width/2, point.Y - 1);
                    break;
                case MagikeAlternateStyle.Top:
                    topPos = new Point16(point.X+data.Width/2, point.Y + data.Height);
                    break;
                case MagikeAlternateStyle.Left:
                    topPos = new Point16(point.X + data.Width, point.Y+data.Height/2);
                    break;
                case MagikeAlternateStyle.Right:
                    topPos = new Point16(point.X - 1, point.Y+data.Height/2);
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
                }, Helper.GetMagikeTileCenter((Entity as MagikeTileEntity).Position) - (Vector2.UnitY * 32));
                return;
            }

            Entity.GetMagikeContainer().ReduceMagike(WorkCost);

            if (Entity.TryGetComponent(MagikeComponentID.ItemGetOnlyContainer, out GetOnlyItemContainer container))
                container.AddItem(ProduceItemType, 1);

            Vector2 center = new Vector2(topPos.X,topPos.Y)*16;
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(center, DustID.FireworksRGB, (i * MathHelper.TwoPi / 12).ToRotationVector2() * 2, newColor: Coralite.MagicCrystalPink);
                d.noGravity = true;
            }
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
                _  => null
            };
        }

        public void ShowInUI(UIElement parent)
        {
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
    }
}
