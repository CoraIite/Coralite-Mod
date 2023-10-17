using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入旋转量，ai1传入旋转方向
    /// </summary>
    public class IllusionSpikeHell : ModProjectile, IDrawNonPremultiplied,INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Target => Main.player[Projectile.owner];

        public RotateTentacle[] rotateTentacles;
        public float alpha = 0.75f;
        public ref float BaseRot => ref Projectile.ai[0];
        public ref float RotDir => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1800;
        }

        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            rotateTentacles ??= new RotateTentacle[3]
            {
                new RotateTentacle(20, TentacleColor,  NightmarePlantera.TentacleWidth, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex)
                {
                    pos = Projectile.Center,
                    targetPos = Projectile.Center
                },
                new RotateTentacle(20, TentacleColor,NightmarePlantera.TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex)
                {
                    pos = Projectile.Center,
                    targetPos = Projectile.Center
                },
                new RotateTentacle(20, TentacleColor,NightmarePlantera.TentacleWidth,NightmarePlantera.tentacleTex,NightmarePlantera.waterFlowTex)
                {
                    pos = Projectile.Center,
                    targetPos = Projectile.Center
                },
            };

            const float RollingTime = 120f;
            if (Timer < RollingTime)
            {
                float currentRot = BaseRot + RotDir * Timer / RollingTime * MathHelper.TwoPi;

                Vector2 center = Target.Center + currentRot.ToRotationVector2() * 800;
                Vector2 dir = center - Projectile.Center;

                float velRot = Projectile.velocity.ToRotation();
                float targetRot = dir.ToRotation();

                float speed = Projectile.velocity.Length();
                float aimSpeed = Math.Clamp(dir.Length() / 300f, 0, 1) * 56;

                Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.3f);

                if (Timer % 5 == 0)
                {
                    Vector2 dir2 = (Target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - dir2 * 150, dir2,
                        ModContent.ProjectileType<ConfusionHole>(), Projectile.damage, 0, Projectile.owner, 35, -2, 555);
                }
            }
            else
            {
                Projectile.velocity *= 0.8f;
                alpha -= 0.05f;
                if (alpha < 0)
                    Projectile.Kill();
            }

            Vector2 center2 = Projectile.Center - Projectile.velocity * 2;
            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                float factor = MathF.Sin((float)Main.timeForVisualEffects / 12 + i * 1.5f);
                float targetRot = tentacle.rotation.AngleLerp(Projectile.rotation + factor * 1.3f, 0.4f);
                Vector2 selfPos = Vector2.Lerp(tentacle.pos,
                    center2 + (i * 30 + 140) * (Projectile.rotation + factor * 0.65f + MathHelper.Pi).ToRotationVector2(), 0.2f);
                tentacle.SetValue(selfPos, Projectile.Center, targetRot);
            }

            for (int i = 0; i < 3; i++)
            {
                RotateTentacle tentacle = rotateTentacles[i];
                tentacle.UpdateTentacle(Vector2.Distance(tentacle.pos, tentacle.targetPos) / 20, 0.7f);
            }

            Timer++;
        }

        public Color TentacleColor(float factor)
        {
            return Color.Lerp(NightmarePlantera.nightmareRed, Color.Transparent, factor) * alpha;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
                return false;

            Texture2D mainTex = TextureAssets.Npc[np.type].Value;
            Rectangle frameBox = mainTex.Frame(4, Main.npcFrameCount[np.type], np.frame.X, np.frame.Y);
            Vector2 origin = frameBox.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float selfRot = Projectile.rotation + MathHelper.PiOver2;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White * 0.75f*alpha, selfRot, origin, 1, 0, 0);

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (rotateTentacles != null)
            {
                Texture2D sparkleTex = ConfusionHole.SparkleTex.Value;
                var frameBox = sparkleTex.Frame(1, 2, 0, 1);
                Vector2 origin = frameBox.Size() / 2;
                //float rot = Main.GlobalTimeWrappedHourly * 0.5f;
                Color c = Color.White;
                c.A = (byte)(200 * alpha);
                for (int j = 0; j < 3; j++) //绘制触手上的三个小星星
                {
                    Vector2 pos = rotateTentacles[j].pos - Main.screenPosition;
                    spriteBatch.Draw(sparkleTex, pos, frameBox, c, rotateTentacles[j].rotation, origin, 0.3f + Main.rand.NextFloat(0, 0.02f), 0, 0);
                }
            }
        }

        public void DrawTentacle()
        {
            if (rotateTentacles != null)
                for (int j = 0; j < 3; j++)
                    rotateTentacles[j]?.DrawTentacle_NoEndBegin(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly), 2);
        }
    }
}
