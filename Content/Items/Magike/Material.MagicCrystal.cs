using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class MagicCrystal : BaseMaterial
    {
        public MagicCrystal() : base(Item.CommonMaxStack, Item.sellPrice(0,0,1), ModContent.RarityType<MagikeCrystalRarity>(), AssetDirectory.MagikeItems)
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.GetMagikeItem().magiteAmount = 25;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!MagikeSystem.learnedMagikeBase)
                tooltips.Add(new TooltipLine(Mod, "MoreDescription", this.GetLocalizedValue("MoreDescription")));
        }
    }
}
