using Coralite.Content.Items.Fairies;
using Coralite.Content.Items.Fairies.ColorSeries;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.CircleCoreAccessories
{
    public class SunflowerRing : BaseCircleCoreAccessory<SunflowerCore>, IFairyAccessory
    {
        public override int CircleRadiusBonus => 16 * 3;

        public override void SetDefaultsSafely()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(silver: 30));
        }

        public override void UpdateAccessorySafely(Player player, FairyCatcherPlayer fcp, bool hideVisual)
        {
            fcp.AddFairyAccessory(this);
            player.AddBuff(BuffID.Sunflower, 60);
            fcp.fairyCatchPowerBonus.Flat += 3;
        }

        public void ModifyFairySpawn(ref FairyAttempt attempt)
        {
            FairyAttempt.AddFairySpawn(CoraliteContent.FairyType<Sunniry>(), 150000);
        }
    }

    public class SunflowerCore : FairyCircleCore
    {
        public override Color? EdgeColor => Color.Gold;

        public override Color? InnerColor => Color.SaddleBrown * 0.4f;
    }
}
