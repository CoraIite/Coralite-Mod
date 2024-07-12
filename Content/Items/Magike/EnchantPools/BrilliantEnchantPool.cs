using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.EnchantPools
{
    public class BrilliantEnchantPool : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public BrilliantEnchantPool() : base(TileType<BrilliantEnchantPoolTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeEnchantPools)
        { }

        public override int MagikeMax => 1500;
        public string WorkTimeMax => "6";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrimtaneEnchantPool>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();

            CreateRecipe()
                .AddIngredient<DemoniteEnchantPool>()
                .AddIngredient<CrystallineMagike>(10)
                .AddIngredient(ItemID.SoulofLight, 8)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeAdvanced, () => MagikeSystem.learnedMagikeAdvanced)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BrilliantEnchantPoolTile : BaseEnchantPool
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] {
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<BrilliantEnchantPoolEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class BrilliantEnchantPoolEntity : MagikeFactory_EnchantPool
    {
        public BrilliantEnchantPoolEntity() : base(1500, 6 * 60) { }

        public override ushort TileType => (ushort)TileType<BrilliantEnchantPoolTile>();

        public override Color MainColor => Coralite.Instance.CrystallineMagikePurple;
    }
}
