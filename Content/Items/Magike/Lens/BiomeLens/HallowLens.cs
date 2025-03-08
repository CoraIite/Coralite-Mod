using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Lens.BiomeLens
{
    public class HallowLens() : MagikeApparatusItem(TileType<HallowLensTile>(), Item.sellPrice(silver: 20)
        , RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeLens)
    {
        public override bool CanUseItem(Player player)
        {
            return player.ZoneHallow;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Skarn>(10)
                .AddIngredient(ItemID.UnicornHorn)
                .AddCondition(CoraliteConditions.LearnedMagikeAdvance)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class HallowLensTile() : BaseLensTile
        (Coralite.HallowYellow, DustID.HallowedTorch)
    {
        public override int DropItemType => ItemType<HallowLens>();

        public override int[] GetAnchorValidTiles()
        {
            return
            [
                TileID.HallowedGrass,TileID.HallowedIce,TileID.HallowHardenedSand
                ,TileID.HallowSandstone,TileID.GolfGrassHallowed,TileID.Pearlstone
                ,TileID.Pearlstone,TileID.PearlstoneBrick,TileID.Pearlwood
            ];
        }

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.Hallow,
                MALevel.HolyLight,
            ];
        }

        public override Vector2 GetTexFrameSize(Texture2D tex, MALevel level)
        {
            return level switch
            {
                MALevel.Hallow => tex.Frame(2, 10).Size(),
                MALevel.HolyLight => tex.Frame(2, 18).Size(),
                _ => base.GetTexFrameSize(tex, level),
            };
        }

        public override void DrawTopTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 drawPos, Color lightColor, MALevel level, bool canProduce)
        {
            switch (level)
            {
                default:
                    base.DrawTopTex(spriteBatch, tex, drawPos, lightColor, level, canProduce);
                    return;
                case MALevel.Hallow:
                    {
                        Rectangle frame;
                        if (canProduce)
                        {
                            int yframe = (int)(6 * Main.GlobalTimeWrappedHourly % 10);
                            frame = tex.Frame(2, 10, 0, yframe);
                        }
                        else
                            frame = tex.Frame(2, 10, 1, 0);

                        spriteBatch.Draw(tex, drawPos, frame, lightColor, 0, frame.Size() / 2, 1f, 0, 0f);
                    }
                    return;
                case MALevel.HolyLight:
                    {
                        Rectangle frame;
                        if (canProduce)
                        {
                            int yframe = (int)(6 * Main.GlobalTimeWrappedHourly % 18);
                            frame = tex.Frame(2, 18, 0, yframe);
                        }
                        else
                            frame = tex.Frame(2, 18, 1, 0);

                        spriteBatch.Draw(tex, drawPos, frame, lightColor, 0, frame.Size() / 2, 1f, 0, 0f);
                    }
                    return;
            }
        }
    }

    public class HallowLensTileEntity : BaseActiveProducerTileEntity<HallowLensTile>
    {
        public override MagikeContainer GetStartContainer()
            => new HallowLensContainer();

        public override MagikeLinerSender GetStartSender()
            => new HallowLensSender();

        public override MagikeActiveProducer GetStartProducer()
            => new HallowProducer();
    }

    public class HallowLensContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    MagikeMaxBase = 0;
                    //AntiMagikeMaxBase = 0;
                    break;
                case MALevel.Hallow:
                    MagikeMaxBase = 630;
                    //AntiMagikeMaxBase = MagikeMaxBase * 3;
                    break;
                case MALevel.HolyLight:
                    MagikeMaxBase = 932;
                    //AntiMagikeMaxBase = MagikeMaxBase * 2;
                    break;
            }

            LimitMagikeAmount();
            //LimitAntiMagikeAmount();
        }
    }

    public class HallowLensSender : UpgradeableLinerSender
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MaxConnectBase = 1;
            ConnectLengthBase = 4 * 16;

            switch (incomeLevel)
            {
                default:
                    MaxConnectBase = 0;
                    UnitDeliveryBase = 0;
                    SendDelayBase = -1;
                    ConnectLengthBase = 0;
                    break;
                case MALevel.Hallow:
                    UnitDeliveryBase = 180;
                    SendDelayBase = 8;
                    break;
                case MALevel.HolyLight:
                    UnitDeliveryBase = 225;
                    SendDelayBase = 7;
                    break;
            }

            SendDelayBase *= 60;
            RecheckConnect();
        }
    }

    public class HallowProducer : UpgradeableProducerByBiome
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.HallowLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.HallowCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Hallow[tile.TileType] || TileID.Sets.HallowBiome[tile.TileType] > 0 || TileID.Sets.HallowBiomeSight[tile.TileType];

        public override bool CheckWall(Tile tile)
            => true;

        public override void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                    ProductionDelayBase = 1_0000_0000 / 60;//随便填个大数
                    ThroughputBase = 1;
                    break;
                case MALevel.Hallow:
                    ProductionDelayBase = 8;
                    ThroughputBase = 60;
                    break;
                case MALevel.HolyLight:
                    ProductionDelayBase = 7;
                    ThroughputBase = 75;
                    break;
            }

            ProductionDelayBase *= 60;
            Timer = ProductionDelayBase;
        }
    }
}
