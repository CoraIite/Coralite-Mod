using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NightmareSlit : BaseNightmareProj, INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        public Vector2 originCenter;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        //private static NPC NightmareOwner => Main.npc[NightmarePlantera.NPBossIndex];

        private NightmareTentacle tentacle;
        public float tentacleWidth = 30;
        public Color tencleColor = NightmarePlantera.lightPurple;

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

            tentacle ??= new NightmareTentacle(30, factor => tencleColor, factor =>
            {
                if (factor > 0.5f)
                    return Helper.Lerp(tentacleWidth, 0, (factor - 0.5f) / 0.5f);

                return Helper.Lerp(0, tentacleWidth, factor / 0.5f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);

            Vector2 dir = Projectile.Center - originCenter;
            tentacle.rotation = dir.ToRotation();
            tentacle.pos = originCenter;
            tentacle.UpdateTentacle(dir.Length() / 30, (i) => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

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
                case 2://开始从上至下射出黑暗叶弹幕
                    {
                        do
                        {
                            if (Timer < 9 * 12)
                            {
                                if ((int)Timer % 9 == 0)
                                {
                                    float factor = Timer / (9 * 12);
                                    float length = dir.Length() * factor;

                                    Vector2 targetDirection = dir.SafeNormalize(Vector2.Zero);
                                    for (int i = -1; i < 2; i += 2)
                                    {
                                        Vector2 position = originCenter + targetDirection * length;
                                        Vector2 velDir = targetDirection.RotatedBy(i * MathHelper.PiOver2);
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), position,
                                           velDir * 15, ModContent.ProjectileType<DarkLeaf>(), Projectile.damage, 0);

                                        for (int j = 0; j < 5; j++)
                                        {
                                            Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<NightmarePetal>(),
                                                    velDir.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(2, 10), newColor: NightmarePlantera.nightPurple);
                                            dust.noGravity = true;
                                        }
                                    }

                                    Vector2 slitCenter = (originCenter + Projectile.Center) / 2;
                                    float angle = factor * MathHelper.TwoPi;
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), slitCenter, (angle + i * MathHelper.Pi).ToRotationVector2() * 12,
                                             ModContent.ProjectileType<DarkLeaf>(), Projectile.damage, 0, ai0: 1);
                                    }

                                    SoundEngine.PlaySound(CoraliteSoundID.NoUse_BlowgunPlus_Item65, Projectile.Center);
                                }
                                break;
                            }

                            tencleColor *= 0.95f;
                            tentacleWidth *= 0.95f;
                            if (tencleColor.A < 20)
                                Projectile.Kill();
                        } while (false);

                        Timer++;
                    }
                    break;
            }

        }

        public static void StopTracking()
        {
            foreach (var proj in Main.projectile.Where(p => p.active && p.hostile && p.type == ModContent.ProjectileType<NightmareSlit>() && p.ai[0] == 0))
                proj.ai[0] = 1;
        }

        public static void Exposion()
        {
            foreach (var proj in Main.projectile.Where(p => p.active && p.hostile && p.type == ModContent.ProjectileType<NightmareSlit>() && p.ai[0] == 1))
            {
                proj.ai[0] = 2;
                (proj.ModProjectile as NightmareSlit).tentacleWidth += 30;

                SoundStyle st = CoraliteSoundID.BigBOOM_Item62;
                st.Pitch = -0.5f;
                SoundEngine.PlaySound(st, proj.Center);

                Vector2 selfCenter = (proj.ModProjectile as NightmareSlit).originCenter;
                Vector2 targetCenter = proj.Center;
                var modifyer = new PunchCameraModifier((targetCenter + selfCenter) / 2, Vector2.One, 20, 8, 20, 1000);
                Main.instance.CameraModifiers.Add(modifyer);

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
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawTentacle()
        {
            tentacle.DrawTentacle_NoEndBegin();
        }
    }
}
