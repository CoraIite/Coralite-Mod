using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.EnchantPools
{
    public class SoulEnchantPool : BaseMagikePlaceableItem, IMagikeFactoryItem
    {
        public SoulEnchantPool() : base(TileType<SoulEnchantPoolTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<CrystallineMagikeRarity>(), 300, AssetDirectory.MagikeEnchantPools)
        { }

        public override int MagikeMax => 2500;
        public string WorkTimeMax => "4";
        public string WorkCost => "?";

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BrilliantEnchantPool>()
                .AddIngredient(ItemID.Ectoplasm, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class SoulEnchantPoolTile : BaseEnchantPool
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<SoulEnchantPoolEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.CrystallineMagikePurple);
            DustType = DustID.PurpleTorch;
        }
    }

    public class SoulEnchantPoolEntity : MagikeFactory_EnchantPool
    {
        public SoulEnchantPoolEntity() : base(2500, 4 * 60) { }

        public override ushort TileType => (ushort)TileType<SoulEnchantPoolTile>();

        public override Color MainColor => Coralite.CrystallineMagikePurple;
    }
}
