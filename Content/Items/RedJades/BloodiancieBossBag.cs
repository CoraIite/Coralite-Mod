using Coralite.Content.Bosses.Rediancie;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace Coralite.Content.Items.RedJades
{
    public class BloodiancieBossBag : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            ItemID.Sets.ItemNoGravity[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.maxStack = 999;

            Item.consumable = true;
            Item.expert = true;

            Item.rare = ItemRarityID.Expert;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.4f);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Whistle>()));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodJade>(), 1, 26, 30));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Rediancie>()));
        }

        public override void PostUpdate(WorldItem item)
        {
            Lighting.AddLight(item.Center, Color.Red.ToVector3() * 0.5f);

            if (item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = item.Center + new Vector2(0f, item.height * -0.1f);
                Vector2 direction = Main.rand.NextVector2CircularEdge(item.width * 0.6f, item.height * 0.6f);
                float distance = 0.3f + (Main.rand.NextFloat() * 0.5f);
                Vector2 velocity = new(0f, (-Main.rand.NextFloat() * 0.3f) - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + (direction * distance), DustID.SilverFlame, velocity);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(WorldItem item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[item.type] != null)
                frame = Main.itemAnimations[item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new((item.width / 2) - frameOrigin.X, item.height - frame.Height);
            Vector2 drawPos = item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = (item.timeSinceItemSpawned / 240f) + (time * 0.04f);

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = (time * 0.5f) + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + (new Vector2(0f, 8f).RotatedBy(radians) * time), frame, new Color(232, 37, 98, 100) * 0.25f, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + (new Vector2(0f, 4f).RotatedBy(radians) * time), frame, new Color(232, 37, 98, 100) * 0.25f, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (int i = 0; i < 5; i++)
            {
                float starRot = (i * MathHelper.TwoPi / 5) - MathHelper.PiOver2;
                Vector2 dir = starRot.ToRotationVector2();
                Helper.DrawPrettyStarSparkle(1, 0, drawPos + (dir * 16), new Color(255, 192, 192), Color.Red * 0.5f,
                    0.5f + (time * 0.3f), 0, 0.3f, 0.7f, 1, starRot, new Vector2(1, 0.5f), Vector2.One);
                Helper.DrawPrettyStarSparkle(1, 0, drawPos + (dir * 20), new Color(255, 192, 192), Color.Red * 0.5f,
                    0.5f + (time * 0.3f), 0, 0.3f, 0.7f, 1, starRot, new Vector2(1, 0.5f), Vector2.One);
            }

            return true;
        }

    }
}
