using Coralite.Content.CoraliteNotes.MagikeChapter2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Reel_MagikeAdvance : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

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
            KnowledgeSystem.CheckForUnlock<MagikeS2Knowledge>(player.Top, Coralite.CrystallinePurple);

            return base.CanUseItem(player);
        }
    }
}
