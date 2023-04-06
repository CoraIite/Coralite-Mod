using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Projectiles_Shoot
{
    public class ArethusaExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Shoot + Name;

        public ref float Scale => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 100;
            Projectile.timeLeft = 14;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (Alpha == 0f)
            {
                Scale = 0.3f;
                Alpha = 1f;
            }

            Alpha -= 0.068f;
            Scale += 0.068f;
            Projectile.rotation += 0.04f;
            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 0.8f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White*Alpha, Projectile.rotation, new Vector2(35, 45), Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}