using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    /// <summary>
    /// ai[0]用于控制状态，为0时为默认发射
    /// 为1时为蓄力攻击
    /// </summary>
    public class RedJadeStaffHeldProj : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeStaff";

        public ref float State => ref Projectile.ai[0];
        protected ref float TargetRot => ref Projectile.ai[1];

        protected Player Owner => Main.player[Projectile.owner];

        private bool initialized;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Owner.itemAnimation = Owner.itemTime = 2;
            Owner.heldProj = Projectile.whoAmI;

            switch (State)
            {
                default:
                case 0: //默认
                    if (!initialized)
                        Initialize_Normal();

                    Projectile.Center = Owner.Center + Owner.direction * Projectile.rotation.ToRotationVector2() * 20;

                    break;
                case 1: //强化
                    if (!initialized)
                        Initialize_Special();

                    if (Projectile.alpha < 255)
                    {
                        Projectile.alpha += 50;
                        if (Projectile.alpha > 255)
                            Projectile.alpha = 255;
                    }

                    if (Main.myPlayer == Projectile.owner && Projectile.timeLeft == 24)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center,
                            (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX) * 12f, ModContent.ProjectileType<RedJadeBeam>(),
                            Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

                        Helper.PlayPitched("RedJade/RedJadeBeam", 0.16f, 0f, Projectile.Center);
                        float r = (Main.MouseWorld - Owner.Center).ToRotation();
                        Vector2 targetDir = r.ToRotationVector2();
                        Vector2 center = Projectile.Center + Projectile.velocity;
                        for (int i = 0; i < 20; i++)
                        {
                            r += 0.314f;
                            Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(1.85f + 0.314f * i, 1f, 3f);

                            //Vector2 dir = new Vector2((float)Math.Cos(1.57f + 0.314f*i ), (float)Math.Sin(1.57f + 0.314f*i +0.5f));       //<--这样不行  : (
                            Dust dust = Dust.NewDustPerfect(center, DustID.GemRuby, dir * 1.4f + targetDir * 6, Scale: 1.8f);
                            dust.noGravity = true;
                            Dust dust2 = Dust.NewDustPerfect(center, DustID.GemRuby, dir * 0.8f + targetDir * 3, Scale: 1.5f);
                            dust2.noGravity = true;
                        }
                    }

                    if (Main.myPlayer == Projectile.owner)
                        TargetRot = (Main.MouseWorld - Owner.Center).ToRotation();

                    Projectile.rotation = TargetRot + (Owner.direction > 0 ? 0f : MathHelper.Pi);
                    Projectile.velocity = TargetRot.ToRotationVector2() * 32;
                    Projectile.Center = Owner.Center + Owner.direction * Projectile.rotation.ToRotationVector2() * 20;
                    Owner.itemRotation = Projectile.rotation + Owner.direction * 0.3f;
                    Projectile.netUpdate = true;
                    break;
            }
        }

        public void Initialize_Normal()
        {
            Projectile.timeLeft = 24;
            if (Main.myPlayer == Projectile.owner)
                TargetRot = (Main.MouseWorld - Owner.Center).ToRotation() + (Owner.direction > 0 ? 0f : 3.141f);

            Projectile.rotation = TargetRot;
            Owner.itemRotation = Projectile.rotation + Owner.direction * 0.3f;
            Projectile.netUpdate = true;
            initialized = true;
        }

        public void Initialize_Special()
        {
            Projectile.timeLeft = 36;
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);

            initialized = true;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteEffects effects = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation + Owner.direction * 0.785f, mainTex.Size() / 2, Projectile.scale, effects, 0f);

            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 22)
                {
                    float factor = 1 - (36 - Projectile.timeLeft) / 14f;
                    ProjectilesHelper.DrawPrettyStarSparkle(1, SpriteEffects.None, center + Projectile.velocity, new Color(255, 255, 255, 0) * 0.8f,
                        Coralite.Instance.RedJadeRed, factor, 0f, 0.4f, 0.6f, 1f, 0f, new Vector2(6, 3f) * factor, Vector2.One);
                }
            }

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 22)
                {
                    Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Rediancie + "RedShield").Value;
                    float factor = 1 - (36 - Projectile.timeLeft) / 14f;
                    spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition + Projectile.velocity, null, Coralite.Instance.RedJadeRed * (Projectile.alpha / 255f), Projectile.timeLeft * 0.25f, mainTex.Size() / 2, MathF.Sin(factor * 3.141f) * 0.3f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
