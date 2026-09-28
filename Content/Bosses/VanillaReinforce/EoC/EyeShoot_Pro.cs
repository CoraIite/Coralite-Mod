using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.VanillaReinforce.EoC
{
    public class EyeShoot_Pro : ModProjectile
    {
        public override string Texture => AssetDirectory.Sparkles+ "EX98";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 20);
        }

        public override void SetDefaults()
        {
            Projectile.width =   Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            if (Projectile.ai[0]==0)
            {
                SoundEngine.PlaySound(SoundID.Item33, Projectile.Center);
                Color color = Color.Lerp(Color.Blue, Color.Purple, 0.5f);
                for (int i = 0; i < 8; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center - Projectile.velocity * 2f, 0, 0, ModContent.DustType<GlowDust_Prismatic>(), 0, 0, 100, color, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Normalize(Projectile.velocity).RotateRandom(0.15f) * Main.rand.NextFloat(6f, 9f);
                }

                Projectile.ai[0] = 1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + 0.1f);

            Projectile.ai[1]++;
        }

        private void Trail()
        {
            Texture2D Tex = TextureAssets.Projectile[Projectile.type].Value;
            int TexY = Tex.Height / Main.projFrames[Projectile.type];
            Rectangle Rect = new(0, TexY * Projectile.frame, Tex.Width, TexY);
            int y = 8;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Color color = Color.Purple;
            color.A = 0;
            int Max = (int)MathHelper.Min(Projectile.oldPos.Length, Projectile.ai[1]);
            Vector2 VecOffset = Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            for (int k = 1; k < Max; k++)
            {
                float Rotation = Projectile.oldRot[k];
                for (int i = 0; i < y; i++)
                {
                    float scale = 1f * ((i + k * (float)y) / (Projectile.oldPos.Length * (float)y));
                    if (k > Projectile.oldPos.Length / 2) scale = 1f * (1f - (i + k * (float)y) / (Projectile.oldPos.Length * (float)y));
                    Vector2 S = new Vector2(1f, 0.2f) * scale;
                    Vector2 vec = Vector2.Lerp(Projectile.oldPos[k - 1], Projectile.oldPos[k], 1f / y * i);
                    Vector2 value3 = vec + VecOffset;
                    Main.spriteBatch.Draw(Tex, value3, new Rectangle?(Rect), color, Rotation, Rect.Size() / 2f, S, spriteEffects, 0f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Trail();
            return false;
        }
    }
}
