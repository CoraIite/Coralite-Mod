using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart
{
    /// <summary>
    /// 放生仙灵的弹幕<br></br>
    /// 使用ai0传入仙灵类型，ai1传入仙灵弹幕类型
    /// </summary>
    public class FairyFreeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float FairyType => ref Projectile.ai[0];
        public ref float FairyProjType => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[2];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://刚出现，如果是在微光中就向上飘
                    {
                        if (Projectile.shimmerWet)
                            Projectile.velocity = new Vector2(0, -2.5f);
                        else
                            State = 1;
                    }
                    break;
                case 1://简单减速
                    {
                        Projectile.velocity *= 0.98f;
                        Timer++;
                        if (Timer > 30)
                        {
                            State = Main.rand.Next(2,4);
                            Timer = Main.rand.Next(-20,20);
                        }
                    }
                    break;
                case 2://绕圈飞行
                    {
                        Timer++;
                        float t1 = MathF.Sin(Timer * 0.1f);
                        float t2 = MathF.Cos(Timer * 0.2f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(t1, t2) * 6, 0.1f);

                        if (Timer > 60 * 2)
                        {
                            State = 4;
                            Timer = 0;
                            Projectile.velocity = new Vector2(Main.rand.NextFromList(-1, 1) * 2, -Main.rand.NextFloat(1, 4));

                            SpawnFairyAura();
                        }
                    }
                    break;
                case 3://绕圈飞行
                    {
                        Timer++;
                        float t1 = MathF.Sin(Timer * 0.2f);
                        float t2 = MathF.Cos(Timer * 0.2f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(t1, t2) * 4, 0.1f);

                        if (Timer > 60 * 2)
                        {
                            State = 4;
                            Timer = 0;
                            Projectile.velocity = new Vector2(Main.rand.NextFromList(-1, 1) * 6, -Main.rand.NextFloat(1, 4));

                            SpawnFairyAura();
                        }
                    }
                    break;
                case 4://飞走喽
                    {
                        Timer++;

                        if (Timer > 60 * 4 || Vector2.Distance(Projectile.Center, Owner.Center) > 1500)
                            Projectile.Kill();
                    }
                    break;
            }

            SpawnFairyDust();
        }

        public void SpawnFairyAura()
        {
            Fairy fairy = FairyLoader.GetFairy((int)FairyType);

            if (fairy == null)
                return;

            FairyFree.SpawnFairyAura(Projectile.Center, fairy.Type, fairy.Rarity, Owner);

            ParticleOrchestrator.BroadcastParticleSpawn(ParticleOrchestraType.ShimmerTownNPC, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center
            });
        }

        public void SpawnFairyDust()
        {
            Projectile p = ContentSamples.ProjectilesByType[(int)FairyProjType];

            if (p.ModProjectile is BaseFairyProjectile fproj)
            {
                fproj.SpawnFairyDust(Projectile.Center,Projectile.velocity);
                Projectile.UpdateFrameNormally(6, fproj.FrameY - 1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Fairy fairy = FairyLoader.GetFairy((int)FairyType);

            if (fairy == null)
                return false;

            fairy.QuickDraw(Projectile.Center, Main.screenPosition, new Rectangle(0,Projectile.frame,1,1),lightColor, 0
                , Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            return false;
        }
    }
}
