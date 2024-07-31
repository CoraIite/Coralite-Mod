using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.EnchantPools
{
    public class CrystalEnchantPool : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public CrystalEnchantPool() : base(TileType<CrystalEnchantPoolTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeEnchantPools)
        { }

        public override int MagikeMax => 300;
        public string WorkTimeMax => "10";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagicCrystal>(10)
                .AddIngredient<Basalt>(10)
                .AddCondition(MagikeSystem.Instance.LearnedMagikeBase, () => MagikeSystem.learnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class CrystalEnchantPoolTile : BaseEnchantPool
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrystalEnchantPoolEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.Instance.MagicCrystalPink);
            DustType = DustID.CrystalSerpent_Pink;
        }
    }

    public class CrystalEnchantPoolEntity : MagikeFactory_EnchantPool
    {
        public CrystalEnchantPoolEntity() : base(300, 10 * 60) { }

        public override ushort TileType => (ushort)TileType<CrystalEnchantPoolTile>();

        public override Color MainColor => Coralite.Instance.MagicCrystalPink;
    }
}
