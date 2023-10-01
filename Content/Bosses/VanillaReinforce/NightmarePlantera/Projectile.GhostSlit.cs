using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai1传入颜色，-1为紫色，-2为红色，0-1为对应的颜色
    /// </summary>
    public class GhostSlit : BaseNightmareProj,INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        public Vector2 originCenter;
        public ref float State => ref Projectile.ai[0];
        public ref float ColorState => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];

        private NightmareTentacle tentacle;
        public float tentacleWidth = 15;
        public Color tencleColor;
        private bool Init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1000;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, originCenter);
        }

        public override void OnSpawn(IEntitySource source)
        {
            originCenter = Projectile.Center;
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (Init)
            {
                if (ColorState == -1)
                    tencleColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    tencleColor = new Color(255, 20, 20, 130);
                else
                    tencleColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Init = false;
            }

            tentacle ??= new NightmareTentacle(30, factor => tencleColor, factor =>
            {
                if (factor > 0.5f)
                    return Helper.Lerp(tentacleWidth, 0, (factor - 0.5f) / 0.5f);

                return Helper.Lerp(0, tentacleWidth, factor / 0.5f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);

            Vector2 dir = Projectile.Center - originCenter;
            tentacle.rotation = dir.ToRotation();
            tentacle.pos = originCenter;
            tentacle.UpdateTentacle(dir.Length() / 30, (i) => 2 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            switch ((int)State)
            {
                default:
                case 0://跟踪梦魇花中心
                    {
                        Projectile.Center = np.Center;
                    }
                    break;
                case 1:
                    break;
                case 2://射出鬼手
                    {
                        do
                        {
                            if (Timer < 60 * 2)
                            {
                                if ((int)Timer % 60 == 0)
                                {
                                    float factor = Timer / (60 * 2);
                                    float length = dir.Length() * factor;

                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), originCenter + dir.SafeNormalize(Vector2.Zero) * length,
                                        new Vector2(Timer % 20 == 0 ? -1 : 1, 0) * 14, ModContent.ProjectileType<GhostHand>(), Projectile.damage, 0, Projectile.owner, Projectile.ai[1]);
                                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, Projectile.Center);
                                }

                                break;
                            }

                            tencleColor *= 0.95f;
                            if (tencleColor.A < 20)
                                Projectile.Kill();
                        } while (false);

                        Timer++;
                    }
                    break;
            }

        }

        public void StopTracking()
        {
            Projectile.ai[0] = 1;
        }

        public static void Exposion()
        {
            foreach (var proj in Main.projectile.Where(p => p.active && p.hostile && p.type == ModContent.ProjectileType<GhostSlit>() && p.ai[0] == 1))
            {
                proj.ai[0] = 2;
                (proj.ModProjectile as GhostSlit).tentacleWidth += 15;

                Vector2 selfCenter = (proj.ModProjectile as GhostSlit).originCenter;
                Vector2 targetCenter = proj.Center;

                float maxLength = Vector2.Distance(selfCenter, targetCenter);
                Vector2 dir = (selfCenter - targetCenter).SafeNormalize(Vector2.UnitY);

                for (int i = 0; i < maxLength; i += 8)
                {
                    Vector2 pos = targetCenter + dir * i;
                    for (int j = -1; j < 2; j += 2)
                    {
                        Dust dust = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(16, 16), DustID.SpookyWood,
                            new Vector2(j, 0) * Main.rand.NextFloat(1, 8), Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }

            SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
            st.Pitch = -0.5f;
            SoundEngine.PlaySound(st);

        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawTentacle()
        {
            tentacle.DrawTentacle_NoEndBegin();
        }
    }
}
