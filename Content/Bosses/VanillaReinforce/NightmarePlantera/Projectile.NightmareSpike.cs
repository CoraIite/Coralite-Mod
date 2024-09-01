using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0输入蓄力时间<br></br>
    /// 使用ai1传入颜色，-1为紫色，-2为红色，0-1时为对应的hue颜色<br></br>
    /// 使用ai2传入刺出的长度<br></br>
    /// 使用速度传入突刺角度
    /// </summary>
    public class NightmareSpike : BaseNightmareProj, IDrawNonPremultiplied, INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float ChannelTime => ref Projectile.ai[0];
        private ref float ColorState => ref Projectile.ai[1];
        private ref float SpurtLength => ref Projectile.ai[2];
        private ref float State => ref Projectile.localAI[1];
        public ref float Length => ref Projectile.localAI[0];

        public ref Vector2 SpikeTop => ref Projectile.velocity;

        public bool canHitPlayer = true;
        public int ShootTime = 30;
        private bool Init = true;
        private float Timer;
        private float spikeWidth;
        private float warningLineAlpha = 1;

        public NightmareTentacle spike;
        public Color drawColor;
        public static Asset<Texture2D> SpikeTex;
        public static Asset<Texture2D> FlowTex;
        public static Asset<Texture2D> WarningLineTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SpikeTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSpike");
            FlowTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSpikeFlow");
            WarningLineTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "WarningLine");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            SpikeTex = null;
            FlowTex = null;
            WarningLineTex = null;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 64;
            Projectile.timeLeft = 3000;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)State != 1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.velocity);
        }

        public override bool CanHitPlayer(Player target)
        {
            return canHitPlayer;
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
                    drawColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    drawColor = NightmarePlantera.nightmareRed;
                else
                    drawColor = Main.hslToRgb(new Vector3(Math.Clamp(ColorState, 0, 1f), 1f, 0.8f));

                Vector2 center = Projectile.Center;
                Projectile.width = Projectile.height = (int)SpurtLength;
                Projectile.Center = center;

                Init = false;
                Projectile.rotation = Projectile.velocity.ToRotation();
                SpikeTop = center;
            }

            spike ??= new NightmareTentacle(30, factor =>
            {
                if (factor > 0.2f)
                    return Color.Lerp(drawColor, Color.White, (factor - 0.2f) / 0.8f);

                return Color.Lerp(Color.Transparent, drawColor, factor / 0.2f);
            }, factor =>
            {
                return Helper.Lerp(0, spikeWidth, factor);
            }, NightmarePlantera.tentacleTex, FlowTex);


            switch ((int)State)
            {
                default:
                case 0: //伸出一个小头
                    {
                        warningLineAlpha = Helper.Lerp(1, 0, Timer / ChannelTime);

                        if (Timer < 3)
                        {
                            SpikeTop += Projectile.rotation.ToRotationVector2() * 25;
                            spikeWidth += 3f;
                        }

                        if (Timer > ChannelTime)
                        {
                            State++;
                            Timer = 0;
                            warningLineAlpha = 0;
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        if (Timer < 5)
                        {
                            float currentLength = Vector2.Distance(SpikeTop, Projectile.Center);
                            currentLength = Helper.Lerp(currentLength, SpurtLength, 0.7f);
                            SpikeTop = Projectile.Center + (Projectile.rotation.ToRotationVector2() * currentLength);
                            spikeWidth += 4f;
                        }

                        if (Timer > ShootTime)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://逐渐收回并减淡消失
                    {
                        SpikeTop = Vector2.Lerp(SpikeTop, Projectile.Center, 0.1f);
                        drawColor *= 0.9f;
                        if (drawColor.A < 10)
                            Projectile.Kill();
                    }
                    break;

            }

            spike.pos = Projectile.Center;
            spike.rotation = Projectile.rotation;
            spike.UpdateTentacle((SpikeTop - Projectile.Center).Length() / 30, i => MathF.Sin((i + Timer) * 0.314f) * 2);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            if (warningLineAlpha > 0)
            {
                Texture2D warningTex = WarningLineTex.Value;
                Vector2 pos = Projectile.Center - Main.screenPosition;
                Rectangle destination = new((int)pos.X, (int)pos.Y, (int)SpurtLength, warningTex.Height);

                Main.spriteBatch.Draw(warningTex, destination, null, new Color(190, 0, 101, (byte)(warningLineAlpha * 255)), Projectile.rotation, new Vector2(0, warningTex.Height / 2), 0, 0);
            }
        }

        public void DrawTentacle()
        {
            spike?.DrawTentacle_NoEndBegin();
        }
    }

    /// <summary>
    /// 就只是视觉效果，用于固定梦魇花
    /// 使用ai0输入刺出时间<br></br>
    /// 使用ai1传入刺出的长度<br></br>
    /// 使用速度传入突刺角度
    /// </summary>
    public class FantasySpike_Visual : ModProjectile, INightmareTentacle
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float ShootTime => ref Projectile.ai[0];
        private ref float SpurtLength => ref Projectile.ai[1];
        private ref float State => ref Projectile.localAI[1];
        public ref float Length => ref Projectile.localAI[0];

        public ref Vector2 SpikeTop => ref Projectile.velocity;

        public bool canHitPlayer = true;
        private bool Init = true;
        private float Timer;
        private float spikeWidth;

        public NightmareTentacle spike;
        public Color drawColor = FantasyGod.shineColor;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 64;
            Projectile.timeLeft = 3000;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool CanHitPlayer(Player target) => false;

        public override void AI()
        {
            if (!NightmarePlantera.NightmarePlanteraAlive(out NPC np))
            {
                Projectile.Kill();
                return;
            }

            if (Init)
            {
                Vector2 center = Projectile.Center;
                Projectile.width = Projectile.height = (int)SpurtLength;
                Projectile.Center = center;

                Init = false;
                Projectile.rotation = Projectile.velocity.ToRotation();
                SpikeTop = center;
            }

            spike ??= new NightmareTentacle(30, factor =>
            {
                if (factor < 0.3f)
                {
                    return Color.Lerp(Color.Transparent, drawColor, factor / 0.3f);
                }

                return Color.Lerp(drawColor, Color.White, (factor - 0.3f) / 0.7f);

            }, factor =>
            {
                return Helper.Lerp(0, spikeWidth, factor);
            }, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);


            switch ((int)State)
            {
                default:
                case 0: //伸出一个小头
                    {
                        if (Timer < 3)
                        {
                            SpikeTop += Projectile.rotation.ToRotationVector2() * 25;
                            spikeWidth += 3f;
                        }

                        if (Timer > 20)
                        {
                            State = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        if (Timer < 5)
                        {
                            float currentLength = Vector2.Distance(SpikeTop, Projectile.Center);
                            currentLength = Helper.Lerp(currentLength, SpurtLength, 0.7f);
                            SpikeTop = Projectile.Center + (Projectile.rotation.ToRotationVector2() * currentLength);
                            spikeWidth += 4f;
                        }

                        if (Timer > ShootTime)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://逐渐收回并减淡消失
                    {
                        SpikeTop = Vector2.Lerp(SpikeTop, Projectile.Center, 0.1f);
                        drawColor *= 0.9f;
                        if (drawColor.A < 10)
                            Projectile.Kill();
                    }
                    break;

            }

            spike.pos = Projectile.Center;
            spike.rotation = Projectile.rotation;
            spike.UpdateTentacle((SpikeTop - Projectile.Center).Length() / 30, i => MathF.Sin((i + Timer) * 0.314f) * 2);
            Timer++;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawTentacle()
        {
            spike?.DrawTentacle_NoEndBegin();
        }
    }
}
