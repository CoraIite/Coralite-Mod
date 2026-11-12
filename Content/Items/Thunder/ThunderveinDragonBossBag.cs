using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinDragonBossBag : BaseBossBag
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override bool PreHardmode => false;

        public override Color OutLineColor => new Color(219, 114, 22, 50) * 0.1f;
        public override Color OutLineColor2 => new Color(255, 202, 101, 77)*0.1f;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ThunderveinNecklace>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZapCrystal>(), 1, 8, 12));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElectrificationWing>(), 1, 4, 5));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<InsulationCortex>(), 1, 10, 14));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<ThunderveinDragon>()));
        }
    }
}