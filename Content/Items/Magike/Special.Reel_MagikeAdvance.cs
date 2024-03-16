using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Magike
{
    public class Reel_MagikeAdvance : ModItem, IMagikeRemodelable
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<CrystallineMagike, Reel_MagikeAdvance>(300, selfStack: 5, condition: HardModeCondition.Instance);
        }

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;
        }

        public override bool CanUseItem(Player player)
        {
            MagikeSystem.learnedMagikeAdvanced = true;
            //NPCLoader.CanTownNPCSpawn(4);
            return base.CanUseItem(player);
        }
    }
}
