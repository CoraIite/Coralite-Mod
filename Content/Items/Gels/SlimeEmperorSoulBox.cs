using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class SlimeEmperorSoulBox : BaseBossBag
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override bool NoGravity => true;
        public override Color OutLineColor => new Color(50, 152, 225, 50);

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<EmperorSabre>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EmperorGel>(), 1, 20, 40));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<SlimeEmperor>()));
            itemLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 45, 150));

            IItemDropRule[] weaponTypes = [
                ItemDropRule.Common(ModContent.ItemType<SlimeEruption>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<GelWhip>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<RoyalClassics>(), 1, 1, 1),
                ItemDropRule.Common(ModContent.ItemType<SlimeSceptre>(), 1, 1, 1),
            ];

            itemLoot.Add(new OneFromRulesRule(1, weaponTypes));
        }

        public override void DrawOutline(SpriteBatch spriteBatch, float rotation, float scale, Texture2D texture, Rectangle frame, Vector2 frameOrigin, Vector2 drawPos, float time, float timer)
        {
            Helper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255),
                time, 0, 0.3f, 0.7f, 1, timer * MathHelper.TwoPi, (timer * MathHelper.TwoPi).ToRotationVector2() * 4, Vector2.One);
            Helper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255),
                0.4f + (time * 0.2f), 0, 0.3f, 0.7f, 1, -timer * MathHelper.Pi, new Vector2(2, 2), Vector2.One * 2);

            Helper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(50, 152, 225),
                0.4f + (time * 0.2f), 0, 0.5f, 0.5f, 1, 0, Vector2.One * 3, Vector2.One * 1.5f);

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + (new Vector2(0f, 8f).RotatedBy(radians) * time), frame, new Color(50, 152, 225, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }
        }
    }
}
