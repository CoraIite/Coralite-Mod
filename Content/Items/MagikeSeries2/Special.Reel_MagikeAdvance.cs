using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Reel_MagikeAdvance : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public void AddMagikeCraftRecipe()
        {
            //MagikeSystem.AddRemodelRecipe<CrystallineMagike, Reel_MagikeAdvance>(300, selfStack: 5, condition: HardModeCondition.Instance);
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
