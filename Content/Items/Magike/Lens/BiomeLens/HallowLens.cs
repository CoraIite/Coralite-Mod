using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Components.Producers;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                HallowLevel.ID,
                HolyLightLevel.ID,
            ];
        }

        public override Vector2 GetTexFrameSize(Texture2D tex, ushort level)
        {
            if (level == HallowLevel.ID)
                return tex.Frame(2, 10).Size();
            if (level == HolyLightLevel.ID)
                return tex.Frame(2, 18).Size();

            return base.GetTexFrameSize(tex, level);
        }

        public override void DrawTopTex(SpriteBatch spriteBatch, Texture2D tex, Vector2 drawPos, Color lightColor, ushort level, bool canProduce)
        {
            if (level == HallowLevel.ID)
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
                return;
            }

            if (level == HolyLightLevel.ID)
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
                return;
            }

            base.DrawTopTex(spriteBatch, tex, drawPos, lightColor, level, canProduce);
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

    public class HallowLensContainer : UpgradeableContainer<HallowLensTile>
    {
    }

    public class HallowLensSender : UpgradeableLinerSender<HallowLensTile>
    {
    }

    public class HallowProducer : UpgradeableProducerByBiome<HallowLensTile>
    {
        public override MagikeSystem.UITextID ApparatusName()
            => MagikeSystem.UITextID.HallowLensName;

        public override MagikeSystem.UITextID ProduceCondition()
            => MagikeSystem.UITextID.HallowCondition;

        public override bool CheckTile(Tile tile)
            => TileID.Sets.Hallow[tile.TileType] || TileID.Sets.HallowBiome[tile.TileType] > 0 || TileID.Sets.HallowBiomeSight[tile.TileType];

        public override bool CheckWall(Tile tile)
            => true;
    }
}
