using Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Gels
{
    public class SlimeEmperorSoulBox : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Type] = true;
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 999;


            Item.consumable = true;
            Item.expert = true;

            Item.rare = ItemRarityID.Expert;
        }

        public override bool CanRightClick() => true;

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.4f);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<EmperorSabre>()));
            //itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<RedJade>(), 1, 26, 30));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<SlimeEmperor>()));
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);
                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = time * 0.5f + 0.5f;

            ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255) ,
                time, 0, 0.3f, 0.7f, 1, timer * MathHelper.TwoPi, (timer * MathHelper.TwoPi).ToRotationVector2() * 4, Vector2.One);
            ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(158, 222, 255),
                0.4f + time * 0.2f, 0, 0.3f, 0.7f, 1, -timer * MathHelper.Pi, new Vector2(2,2), Vector2.One*2);

            ProjectilesHelper.DrawPrettyStarSparkle(1, 0, drawPos, new Color(38, 104, 185) * 0.7f, new Color(50, 152, 225),
                0.4f + time * 0.2f, 0, 0.5f, 0.5f, 1, 0, Vector2.One * 3, Vector2.One * 1.5f);

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(50, 152, 225, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

    }
}
