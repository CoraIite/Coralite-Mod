using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Altars
{
    public class BasicAltar() : MagikeApparatusItem(TileType<BasicAltarTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeAltars)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(35)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class BasicAltarTile() : BaseCraftAltarTile
        (5, 5, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeAltarTiles + Name;
        public override int DropItemType => ItemType<BasicAltar>();

        public override MagikeTP GetEntityInstance() => GetInstance<BasicAltarTileEntity>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.Shadow,
                MALevel.CrystallineMagike,
                MALevel.Hallow,
                MALevel.HolyLight,
            ];
        }

        public override Vector2 GetFloatingOffset(float rotation, MALevel level)
        {
            return level switch
            {
                MALevel.MagicCrystal 
                or MALevel.Glistent
                or MALevel.Hallow 
                => rotation.ToRotationVector2() * 8,
                MALevel.CrystallineMagike
                or MALevel.Shadow
                => rotation.ToRotationVector2() * 12,
                MALevel.HolyLight => rotation.ToRotationVector2() * 4,
                _ => Vector2.Zero
            };
        }

        public override Vector2 GetRestOffset(float rotation, MALevel level)
        {
            return level switch
            {
                MALevel.MagicCrystal
                or MALevel.Glistent
                or MALevel.Shadow
                or MALevel.CrystallineMagike
                => Vector2.Zero,
                MALevel.Hallow => -6 * rotation.ToRotationVector2(),
                MALevel.HolyLight => -6*rotation.ToRotationVector2(),
                _ => Vector2.Zero
            };
        }
    }

    public class BasicAltarTileEntity : BaseCraftAltar<BasicAltarTile>
    {
        public override int ExtendFilterCapacity => 5;

        public override CraftAltar GetStartAltar()
            => new BasicAltarAltar();

        public override MagikeContainer GetStartContainer()
            => new BasicAltarContainer();

        public override GetOnlyItemContainer GetStartGetOnlyItemContainer()
            => new ()
            {
                CapacityBase = 4
            };


        public override ItemContainer GetStartItemContainer()
            => new()
            {
                CapacityBase = 1
            };

        public override CheckOnlyLinerSender GetStartSender()
            => new BasicAltarSender();
    }

    public class BasicAltarContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => MagikeHelper.CalculateMagikeCost(MALevel.MagicCrystal, 32, 60 * 10),
                MALevel.Glistent => MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 32, 60 * 10),
                MALevel.Shadow => MagikeHelper.CalculateMagikeCost(MALevel.Shadow, 32, 60 * 10),
                MALevel.CrystallineMagike => MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 32, 60 * 10),
                MALevel.Hallow => MagikeHelper.CalculateMagikeCost(MALevel.Hallow, 32, 60 * 10),
                MALevel.HolyLight => 1000_0000,
                MALevel.SplendorMagicore => 35000,
                _ => 0,
            };
            LimitMagikeAmount();

            AntiMagikeMaxBase = MagikeMaxBase * 2;
            LimitAntiMagikeAmount();
        }
    }

    public class BasicAltarSender : CheckOnlyLinerSender
    {
        public BasicAltarSender()
        {
            ConnectLengthBase = 16 * 8;
            MaxConnectBase = 12;
        }
    }

    public class BasicAltarAltar : CraftAltar
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            var second = incomeLevel switch
            {
                MALevel.MagicCrystal => 15,
                MALevel.Glistent => 13,
                MALevel.Shadow => 11,
                MALevel.CrystallineMagike => 9,
                MALevel.Hallow => 7,
                MALevel.HolyLight => 5,
                _ => (float)(10_0000_0000 / 60),
            };

            WorkTimeBase = (int)(second * 60);
        }
    }
}
