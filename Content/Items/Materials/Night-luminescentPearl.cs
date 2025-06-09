using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
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
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<Night_luminescentPearl>(), ItemID.Meowmere, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120), 2)
                .RegisterNewCraft(ItemID.Terrarian, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.StarWrath, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.SDMG, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.LastPrism, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.LunarFlareBook, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.RainbowCrystalStaff, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.MoonlordTurretStaff, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft<ConquerorOfTheSeas>(CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft<Aurora>(CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.Celeb2, CalculateMagikeCost(MALevel.SplendorMagicore, 4, 120))
                .Register();
        }
    }
}
