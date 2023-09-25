using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Materials
{
    public class SteelBar:BaseMaterial,IMagikePolymerizable
    {
        public SteelBar() : base(9999, Item.sellPrice(0, 0, 10), ItemRarityID.LightRed, AssetDirectory.Materials) { }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59;

            ItemTrader.ChlorophyteExtractinator.AddOption_OneWay(Type, 5, ItemID.ChlorophyteBar, 2);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<SteelBarTile>());
        }
        
        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<SteelBar>(300)
                .SetMainItem(ItemID.IronBar, 12)
                .AddIngredient(ItemID.Coal, 6)
                .AddIngredient<HeatanInABottle>()
                .AddCondition(HardModeCondition.Instance)
                .Register();
        }
    }

    public class SteelBarTile:ModTile
    {
        public override string Texture => AssetDirectory.Materials+Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            DustType = DustID.PlatinumCoin;

            AddMapEntry(new Color(150, 150, 150), Language.GetText("MapObject.MetalBar")); // localized text for "Metal Bar"
        }
    }
}
