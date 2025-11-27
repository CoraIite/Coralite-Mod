using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Content.Items.Materials
{
    public class Night_luminescentPearl : BaseMaterial, IMagikeCraftable
    {
        public Night_luminescentPearl() : base(Item.CommonMaxStack, Item.sellPrice(0, 1), ItemRarityID.Red, AssetDirectory.Materials)
        {
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<Night_luminescentPearl>(), ItemID.Meowmere, CalculateMagikeCost<SplendorLevel>( 4, 120), 2)
                .RegisterNewCraft(ItemID.Terrarian, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.StarWrath, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SDMG, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LastPrism, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LunarFlareBook, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RainbowCrystalStaff, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.MoonlordTurretStaff, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft<ConquerorOfTheSeas>(CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft<Aurora>(CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Celeb2, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .Register();
        }
    }
}
