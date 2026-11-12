using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    public class BabyIceDragonBossBag : BaseBossBag
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override Color OutLineColor => new Color(90, 70, 255, 50);
        public override Color OutLineColor2 => new Color(140, 120, 255, 77);

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<IcicleCoccyx>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IcicleCrystal>(), 1, 4, 6));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IcicleScale>(), 1, 3, 5));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IcicleBreath>(), 1, 5, 8));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<BabyIceDragon>()));
        }
    }
}