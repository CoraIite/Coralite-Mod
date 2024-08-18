using Coralite.Content.Items.Glistent;
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
    public class DemoniteEnchantPool : BaseMagikePlaceableItem, IMagikePolymerizable, IMagikeFactoryItem
    {
        public DemoniteEnchantPool() : base(TileType<DemoniteEnchantPoolTile>(), Item.sellPrice(0, 0, 50, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeEnchantPools)
        { }

        public override int MagikeMax => 450;
        public string WorkTimeMax => "8";
        public string WorkCost => "?";

        public void AddMagikePolymerizeRecipe()
        {
            MagikeCraftRecipe.CreateRecipe<DemoniteEnchantPool>(150)
                .SetMainItem<CrystalEnchantPool>()
                .AddIngredient<GlistentBar>(4)
                .AddIngredient(ItemID.ShadowScale, 4)
                .Register();
        }
    }

    public class DemoniteEnchantPoolTile : BaseEnchantPool
    {
        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
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
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<DemoniteEnchantPoolEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Purple);
            DustType = DustID.Demonite;
        }
    }

    public class DemoniteEnchantPoolEntity : MagikeFactory_EnchantPool
    {
        public DemoniteEnchantPoolEntity() : base(450, 8 * 60) { }

        public override ushort TileType => (ushort)TileType<DemoniteEnchantPoolTile>();

        public override Color MainColor => Color.Purple;
    }
}
