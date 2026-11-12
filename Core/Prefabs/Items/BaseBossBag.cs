using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseBossBag:ModItem
    {
        /// <summary>
        /// 是否是困难模式前的宝藏袋，困难模式后能开出开发者时装，默认true
        /// </summary>
        public virtual bool PreHardmode => true;
        /// <summary>
        /// 是否有重力
        /// </summary>
        public virtual bool NoGravity => false;

        public abstract Color OutLineColor { get; }
        public virtual Color OutLineColor2 { get=> OutLineColor; }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = PreHardmode;
            ItemID.Sets.ItemNoGravity[Type] = NoGravity;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;

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

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Red.ToVector3() * 0.5f);

            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);
                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
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

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new((Item.width / 2) - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = (Item.timeSinceItemSpawned / 240f) + (time * 0.04f);

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
                time = 2f - time;

            time = (time * 0.5f) + 0.5f;
            DrawOutline(spriteBatch, rotation, scale, texture, frame, frameOrigin, drawPos, time, timer);

            PostDrawOutline(drawPos, time);
            return true;
        }

        public virtual void DrawOutline(SpriteBatch spriteBatch, float rotation, float scale, Texture2D texture, Rectangle frame, Vector2 frameOrigin, Vector2 drawPos, float time, float timer)
        {
            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + (new Vector2(0f, 8f).RotatedBy(radians) * time), frame, OutLineColor, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + (new Vector2(0f, 4f).RotatedBy(radians) * time), frame, OutLineColor2, rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }
        }

        public virtual void PostDrawOutline(Vector2 drawPos,float time)
        {
        }
    }
}
