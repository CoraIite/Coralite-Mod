using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace Coralite.Content.Items.Icicle
{
    /// <summary>
    /// ai0用于判断玩家的手持方向,ai1用于控制是否是特殊的弹幕
    /// </summary>
    public class IcicleBowHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleBow";

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public bool fadeIn = true;

        public override int GetItemType()
            => ModContent.ItemType<IcicleBow>();

        public override void AIBefore()
        {
            if (Alpha == 0)
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                }

                Projectile.rotation = Rotation;
                Alpha = 0.0001f;
                if (Projectile.ai[1] == 0)
                    Projectile.timeLeft = Owner.itemTime;
                else
                {
                    Projectile.timeLeft = Owner.itemTimeMax;
                }

                if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                {
                    DashTime = cp.DashTimer;
                }
            }
        }

        public override void DashAttackAI()
        {
            if (fadeIn)
            {
                Alpha += 0.02f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            Lighting.AddLight(Owner.Center, Coralite.IcicleCyan.ToVector3() * Alpha);

            do
            {
                if (Timer < DashTime + 2)
                {
                    Rotation += 0.3141f; //1/10 Pi
                    Projectile.timeLeft = 4;

                    Owner.itemTime = Owner.itemAnimation = 2;
                    break;
                }

                if (!DownLeft)
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                        Projectile.netUpdate = true;

                        if (Main.rand.NextBool(20))
                        {
                            Vector2 dir = Rotation.ToRotationVector2();
                            PRTLoader.NewParticle(Owner.Center + (dir * 16) + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), Coralite.IcicleCyan, Main.rand.NextFloat(0.1f, 0.15f));
                        }
                    }
                    Projectile.timeLeft = 4;
                    Owner.itemTime = Owner.itemAnimation = 2;
                }
                else
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One) * 9.5f
                            , ModContent.ProjectileType<IcicleStarArrow>(), (int)(Owner.GetDamageWithAmmo(Item) * 2.3f), Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                    }

                    Owner.itemTime = Owner.itemAnimation = 2;
                    if (Projectile.localAI[2] > 10)
                        Projectile.Kill();

                    Projectile.localAI[2]++;
                }

            } while (false);

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override Vector2 GetOffset()
            => new(12, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1.1f, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Alpha > 0.001f)
            {
                Texture2D starTex = ModContent.Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleStarArrow").Value;
                float factor = Timer % 80 / 80;
                float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);
                Vector2 dir = Rotation.ToRotationVector2();
                Main.spriteBatch.Draw(starTex, center + (dir * 6), null, Color.White * Alpha, Projectile.rotation + 1.57f, starTex.Size() / 2, 1.4f, SpriteEffects.None, 0f);

                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center + (dir * 18), new Color(255, 255, 255, 0) * num3 * 0.5f, Coralite.IcicleCyan,
                    factor, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1.3f, 1.3f), Vector2.One);
            }

            return false;
        }
    }
}