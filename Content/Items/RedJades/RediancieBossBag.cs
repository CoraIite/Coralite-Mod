using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.Items.RedJades
{
    public class RediancieBossBag : BaseBossBag
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override Color OutLineColor => new Color(90, 70, 255, 50);
        public override Color OutLineColor2 => new Color(140, 120, 255, 77);

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RedJadePendant>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RedJade>(), 1, 32, 38));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Rediancie>()));
        }
    }
}
