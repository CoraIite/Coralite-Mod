using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Coralite.Content.Items.RedJades
{
    public class BloodiancieBossBag : BaseBossBag
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override bool PreHardmode => false;
        public override bool NoGravity => true;

        public override Color OutLineColor => new Color(232, 37, 98, 100) * 0.25f;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Whistle>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodJade>(), 1, 26, 30));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Rediancie>()));
        }

        public override void PostDrawOutline(Vector2 drawPos, float time)
        {
            for (int i = 0; i < 5; i++)
            {
                float starRot = (i * MathHelper.TwoPi / 5) - MathHelper.PiOver2;
                Vector2 dir = starRot.ToRotationVector2();
                Helper.DrawPrettyStarSparkle(1, 0, drawPos + (dir * 16), new Color(255, 192, 192), Color.Red * 0.5f,
                    0.5f + (time * 0.3f), 0, 0.3f, 0.7f, 1, starRot, new Vector2(1, 0.5f), Vector2.One);
                Helper.DrawPrettyStarSparkle(1, 0, drawPos + (dir * 20), new Color(255, 192, 192), Color.Red * 0.5f,
                    0.5f + (time * 0.3f), 0, 0.3f, 0.7f, 1, starRot, new Vector2(1, 0.5f), Vector2.One);
            }
        }
    }
}
