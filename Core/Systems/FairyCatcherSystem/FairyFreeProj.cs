
using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    /// <summary>
    /// 放生仙灵的弹幕<br></br>
    /// 使用ai0传入仙灵类型
    /// </summary>
    public class FairyFreeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float FairyType => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

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
                            Projectile.velocity = new Vector2(0, -8);
                        else
                            State = 1;
                    }
                    break;
                case 1://简单减速
                    {
                        Projectile.velocity *= 0.9f;
                        Timer++;
                        if (Timer > 20)
                        {
                            State = 2;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://绕圈飞行
                    {
                        Timer++;
                        float t1 = MathF.Sin(Timer * 0.4f);
                        float t2 = MathF.Cos(Timer * 0.4f);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(t1, t2), 0.1f);

                        if (Timer > 60)
                        {
                            State = 3;
                            Timer = 0;
                            Projectile.velocity = new Vector2(Main.rand.NextFromList(-1, 1) * 10, Main.rand.NextFloat(2, 4));

                            SpawnFairyLight();
                        }
                    }
                    break;
                case 3://飞走喽
                    {

                    }
                    break;
            }
        }

        public void SpawnFairyLight()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
