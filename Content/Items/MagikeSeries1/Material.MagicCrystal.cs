using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class MagicCrystal : BaseMaterial, IMagikeRemodelable
    {
        public MagicCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0, 0, 1), ModContent.RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeSeries1Item)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magikeAmount = 25;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!MagikeSystem.learnedMagikeBase)
                tooltips.Add(new TooltipLine(Mod, "MoreDescription", this.GetLocalizedValue("MoreDescription")));
        }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<MagicCrystal, CrystallineMagike>(275, condition: HardModeCondition.Instance);
        }
    }
}
