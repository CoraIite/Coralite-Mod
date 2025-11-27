using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    public class BasicCharger() : MagikeApparatusItem(TileType<BasicChargerTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeFactories)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient<MagicCrystalBlock>(5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicChargerTile() : BaseChargerTile
        (3, 1, Coralite.MagicCrystalPink, DustID.CorruptionThorns, topSoild: true)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<BasicCharger>();

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;

            TileID.Sets.IgnoredInHouseScore[Type] = true;
            CoraliteSetsSystem.MagikeTileTypes.Add(Type, CoraliteSetsSystem.MagikeTileType.None);

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.CoordinateHeights = [18];

            //默认防岩浆
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.Allowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.MagicCrystalPink);
            DustType = DustID.CorruptionThorns;

            List<ushort> levels = GetAllLevels();
            if (levels == null || levels.Count == 0)
                return;
        }

        public override List<ushort> GetAllLevels()
        {
            return
            [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                BrilliantLevel.ID,
                SplendorLevel.ID,
            ];
        }
    }

    public class BasicChargerTileEntity() : MagikeCharger<BasicChargerTile>()
    {
        public override MagikeContainer GetStartContainer()
            => new BasicChargerContainer();

        public override Charger GetStartCharger()
            => new BasicChargerFactory();

        public override ItemContainer GetStartItemContainer()
            => new ItemContainer()
            {
                CapacityBase = 1,
            };
    }

    public class BasicChargerContainer : UpgradeableContainer<BaseChargerTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    MagikeMaxBase = CalculateMagikeCost(incomeLevel, 6);
        //    LimitMagikeAmount();
        //}
    }

    public class BasicChargerFactory : UpgradeableCharger<BaseChargerTile>
    {
        //public override void Upgrade(ushort incomeLevel)
        //{
        //    float t = incomeLevel switch
        //    {
        //        MALevel.MagicCrystal => 1,
        //        MALevel.Glistent => 0.9f,
        //        MALevel.CrystallineMagike => 0.7f,
        //        MALevel.SplendorMagicore => 0.5f,
        //        _ => 10_0000_0000 / 60,
        //    };

        //    WorkTimeBase = (int)(60 * t);
        //    MagikePerCharge = CalculateMagikeCost(incomeLevel, 2, 20);
        //}
    }
}
