using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0传入颜色，范围0-6
    /// </summary>
    public class NightmareEnergy : ModProjectile, INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float ColorState => ref Projectile.ai[0];
        private ref float State => ref Projectile.ai[1];

        private bool Init = true;

        public RotateTentacle tentacle;
        public Color drawColor;

        public Vector2 secondPos;

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (Init)
            {
                if (ColorState<0)
                    drawColor = NightmarePlantera.nightmareSparkleColor;
                else
                {
                    int c = Math.Clamp((int)ColorState, 0, 6);
                    drawColor = NightmarePlantera.phantomColors[c];
                }
                secondPos = Projectile.Center;
                Init = false;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            tentacle ??= new RotateTentacle(30, factor => drawColor
            , factor =>
            {
                if (factor > 0.6f)
                    return Helper.Lerp(20, 0, (factor - 0.6f) / 0.4f);

                return Helper.Lerp(0, 20, factor / 0.6f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, secondPos, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(secondPos, Projectile.Center) / 30);

            switch (State)
            {
                default:
                case 0: //自身冲向梦魇花
                    {
                        Vector2 center = np.Center;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 200, 0, 1) * 12;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.035f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.85f);
                        Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.3f);

                        if (dir.Length() < 30)
                        {
                            Projectile.velocity *= 0;
                            State++;
                            Projectile.Center = np.Center;
                        }
                    }
                    break;
                case 1://拖尾收拢
                    {
                        Vector2 dir = np.Center - secondPos;
                        secondPos += dir.SafeNormalize(Vector2.Zero) * 10;
                        if (dir.Length() < 30)
                        {
                            State++;
                        }
                    }
                    break;
                case 2:
                    Projectile.Kill();
                    return;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawTentacle()
        {
            tentacle?.DrawTentacle_NoEndBegin(i => 2 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));
        }
    }
}
