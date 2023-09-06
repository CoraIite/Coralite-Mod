using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    internal class FantasyBall : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 80;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.type == ModContent.NPCType<NightmarePlantera>();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawPrimitives()
        {

        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {

        }
    }
}
