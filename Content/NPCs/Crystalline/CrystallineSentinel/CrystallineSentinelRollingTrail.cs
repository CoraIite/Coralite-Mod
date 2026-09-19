using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 二阶段螺旋冲刺的双螺旋星芒拖尾（无伤害，纯视觉），贴在本体前方 114 px。
    /// </summary>
    public class CrystallineSentinelRollingTrail : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float FadeoutFactor => ref Projectile.ai[1];
        public ref float MaxtimeLeft => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Helper.QuickTrailSets(Type, Helper.TrailingMode.RecordAll, 20);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!OwnerIndex.GetNPCOwner(out NPC owner, Projectile.Kill))
                return;
            if (Projectile.localAI[0] == 0)
            {
                MaxtimeLeft = Projectile.timeLeft;
                Projectile.localAI[0] = 1;
            }
            Vector2 dir = (Vector2.UnitX * owner.spriteDirection).RotatedBy(owner.rotation);
            Vector2 pos = new Vector2(owner.spriteDirection * 114, 0).RotatedBy(owner.rotation) + owner.Center;
            Projectile.Center = pos;

            float fadein = Utils.Remap(Projectile.timeLeft, MaxtimeLeft - 20, MaxtimeLeft, 1f, 0f);
            float fadeout = Utils.Remap(Projectile.timeLeft, 0, 20, 0f, 1f);
            Projectile.Opacity = fadein * fadeout;
            FadeoutFactor = fadeout;
            Projectile.rotation = owner.velocity.ToRotation();
        }

        public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int j = -1; j < 2; j += 2)
            {
                Vector2 lastTrailPos = Vector2.Zero;
                float innerRot = 0;
                float mult = 24;
                float maxRadius = 32f;
                float minRadius = 6f;
                int total = (int)(Projectile.oldPos.Length * mult - mult);
                Vector2 scale = new Vector2(0.4f, 0.2f) * 0.25f * Projectile.scale;
                for (int i = 0; i < total - 1; i++)
                {
                    var roundI = (int)(i / mult);
                    if (Projectile.oldPos[roundI] == Vector2.Zero || Projectile.oldPos[roundI + 1] == Vector2.Zero)
                        continue;

                    float factor = 1 - (float)i / total;
                    float lerpFactor = Utils.Remap(i % mult, 0, mult - 1, 1 / mult, 1f);
                    float radius = Utils.Remap(factor, 0, 1, minRadius, maxRadius);
                    Vector2 oldpos = Vector2.Lerp(Projectile.oldPos[roundI], Projectile.oldPos[roundI + 1], lerpFactor);
                    float oldrot = MathHelper.Lerp(Projectile.oldRot[roundI], Projectile.oldRot[roundI + 1], lerpFactor);
                    float phase = (float)(-i * 0.025f - Projectile.timeLeft * 0.35f + Main.timeForVisualEffects * (0.04f + j * 0f));
                    float phaseoffset = phase + (j > 0 ? MathHelper.Pi : 0);
                    float fake3dAlpha = phaseoffset % MathHelper.TwoPi < MathHelper.Pi ? Utils.Remap(MathF.Abs(MathF.Cos(phaseoffset)), 0f, 1f, 0f, 1f) : 1f;
                    float y = MathF.Cos(phase) * j;

                    Vector2 dir = (oldrot + MathHelper.PiOver2).ToRotationVector2() * y;

                    float fadein = Utils.Remap(factor, 0.7f, 1f, 1f, 0f);
                    float fadeinFactor = MathHelper.Lerp(1f, fadein, FadeoutFactor);
                    Vector2 trailPos = oldpos + dir * radius * fadeinFactor;
                    var normalDir = lastTrailPos - trailPos;
                    innerRot -= 0.018f;
                    lastTrailPos = trailPos;


                    if (i == 0)
                        continue;

                    float alpha = factor * Projectile.Opacity * fake3dAlpha;
                    Color drawColor = j < 0 ? Coralite.CrystallinePurple : new Color(134, 156, 255);
                    Main.spriteBatch.Draw(star, trailPos - Main.screenPosition, null, drawColor with { A = 0 } * alpha, normalDir.ToRotation() + MathHelper.PiOver2, star.Size() / 2, scale, 0, 0);
                }
            }

            return false;
        }
    }
}
