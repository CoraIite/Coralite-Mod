using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai1传入追踪的角度<br></br>
    /// 使用ai2传入蓄力时间
    /// </summary>
    public class FantasySpike : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;
        private FantasyTentacle tentacle;

        private Player Owner => Main.player[Projectile.owner];

        public ref float State => ref Projectile.ai[0];
        public ref float Angle => ref Projectile.ai[1];
        public ref float ChannelTime => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 1000;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)State == 1 && NightmarePlantera.FantasyGodAlive(out NPC fg))
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, fg.Center);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.type == ModContent.NPCType<NightmarePlantera>();
        }

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np) || !NightmarePlantera.FantasyGodAlive(out NPC fg))
            {
                Projectile.Kill();
                return;
            }

            tentacle ??= new FantasyTentacle(30, FantasyGod.TentacleColor, factor =>
            {
                if (factor > 0.6f)
                    return Helper.Lerp(25, 0, (factor - 0.6f) / 0.4f);

                return Helper.Lerp(0, 25, factor / 0.6f);
            }, NightmarePlantera.tentacleTex, NightmarePlantera.waterFlowTex);

            tentacle.SetValue(Projectile.Center, fg.Center, Projectile.rotation + MathHelper.Pi);
            tentacle.UpdateTentacle(Vector2.Distance(fg.Center, Projectile.Center) * 1.5f / 30);

            switch ((int)State)
            {
                default:
                case 0: //跟踪梦魇花
                    {
                        float angle = Angle + 0.2f * MathF.Sin(Timer * 0.0314f);
                        Vector2 center = np.Center + angle.ToRotationVector2() * 650;
                        Vector2 dir = center - Projectile.Center;

                        float velRot = Projectile.velocity.ToRotation();
                        float targetRot = dir.ToRotation();

                        float speed = Projectile.velocity.Length();
                        float aimSpeed = Math.Clamp(dir.Length() / 600f, 0, 1) * 24;

                        Projectile.velocity = velRot.AngleTowards(targetRot, 0.08f).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, 0.25f);
                        Projectile.rotation = Projectile.rotation.AngleTowards((np.Center - Projectile.Center).ToRotation(), 0.2f);

                        if (Timer > ChannelTime)
                        {
                            Vector2 dir2 = Helper.NextVec2Dir();
                            var modifyer = new PunchCameraModifier(Projectile.Center, dir2, 26, 7, 10, 1000);
                            Main.instance.CameraModifiers.Add(modifyer);

                            Timer = 0;
                            State++;

                            Helper.PlayPitched("Misc/Spike", 1f, 0.6f, np.Center);
                        }
                    }
                    break;
                case 1://刺出
                    {
                        Vector2 dir = np.Center - fg.Center;
                        float length = dir.Length() + 800;
                        float rot = dir.ToRotation();
                        Projectile.rotation = Projectile.rotation.AngleLerp(rot, 0.4f);
                        Projectile.Center = Vector2.Lerp(Projectile.Center, fg.Center + dir.SafeNormalize(Vector2.Zero) * length, 0.4f);


                        if (Timer > 32)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://收回
                    {
                        Projectile.Center = Vector2.Lerp(Projectile.Center, fg.Center, 0.15f);
                        if (Timer > 20)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            tentacle?.DrawTentacle(i => 4 * MathF.Sin(i / 2 * Main.GlobalTimeWrappedHourly));

            Texture2D mainTex = Projectile.GetTexture();
            Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 selforigin = frameBox.Size() / 2;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White * 0.8f, Projectile.rotation + MathHelper.PiOver2, selforigin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
