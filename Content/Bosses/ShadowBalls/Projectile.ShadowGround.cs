using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowGround : ModProjectile
    {
        public override string Texture => AssetDirectory.ShadowBalls + Name;

        protected ref float OwnerIndex => ref Projectile.ai[0];
        protected ref float Timer => ref Projectile.ai[1];
        protected ref float FadeTime => ref Projectile.ai[2];

        protected ref float Alpha => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!GetOwner(out NPC owner))
                return;

            Projectile.position.X = owner.Center.X - Projectile.width / 2;

            if (Timer < 8)
            {
                Alpha += 1 / 8f;
                Timer++;
            }
            else if (FadeTime == 1)
            {
                Alpha -= 1 / 20f;
                if (Timer > 20 + 8)
                    Projectile.Kill();
                Timer++;
            }
        }

        public void Fade()
        {
            FadeTime = 1;
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != ModContent.NPCType<ShadowBall>())
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Top - Main.screenPosition;
            var origin = new Vector2(mainTex.Width / 2, 0);

            Color c = new Color(104,54,192,0)*Alpha;

            Main.spriteBatch.Draw(mainTex, pos, null, c, 0, origin, 0.2f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.75f, 0, origin, 0.2f, 0, 0);

            return false;
        }
    }
}
