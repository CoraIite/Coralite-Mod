using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 使用ai0输入蓄力时间<br></br>
    /// 使用ai1传入颜色，-1为紫色，-2为红色，0-1时为对应的hue颜色<br></br>
    /// 使用ai2传入刺出的长度<br></br>
    /// 使用velocity传入目标方向
    /// </summary>
    public class ConfusionHole : BaseNightmareProj, IDrawNonPremultiplied,INightmareTentacle
    {
        public override string Texture => AssetDirectory.NightmarePlantera + Name;

        private ref float ChannelTime => ref Projectile.ai[0];
        private ref float ColorState => ref Projectile.ai[1];
        private ref float SpurtLength => ref Projectile.ai[2];

        private ref float State => ref Projectile.localAI[1];
        public ref float Length => ref Projectile.localAI[0];

        private float SelfScale;

        public ref Vector2 SpikeTop => ref Projectile.velocity;

        private bool Init = true;
        private float Timer;
        private float spikeWidth;
        private float warningLineAlpha = 1;

        public NightmareTentacle spike;
        public Color drawColor;

        public static Asset<Texture2D> SparkleTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SparkleTex = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NightmareSparkle");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            SparkleTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.width = Projectile.height = 16;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if ((int)State != 1)
                return false;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.velocity);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.friendly)
            {
                modifiers.SourceDamage += 10;
            }
        }

        public override void AI()
        {
            if (Init)
            {
                if (ColorState == -1)
                    drawColor = NightmarePlantera.lightPurple;
                else if (ColorState == -2)
                    drawColor = new Color(255, 20, 20, 130);
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
            }, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);


            switch ((int)State)
            {
                default:
                case 0: //伸出一个小头
                    {
                        float factor = MathHelper.Clamp(Timer / (ChannelTime * 0.5f), 0, 1);
                        warningLineAlpha = Helper.Lerp(1, 0, factor);
                        if (SelfScale < 0.95f)
                        {
                            SelfScale = Helper.Lerp(0, 1, Timer / (ChannelTime * 0.25f));
                        }

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

                            Helper.PlayPitched("Misc/Spike", 0.5f, 0.4f, Projectile.Center);
                            //SoundEngine.PlaySound(CoraliteSoundID.IceMist_Item120, Projectile.Center);
                        }
                    }
                    break;
                case 1://快速戳出
                    {
                        if (Timer < 5)
                        {
                            float currentLength = Vector2.Distance(SpikeTop, Projectile.Center);
                            currentLength = Helper.Lerp(currentLength, SpurtLength, 0.7f);
                            SpikeTop = Projectile.Center + Projectile.rotation.ToRotationVector2() * currentLength;
                            spikeWidth += 4f;
                        }

                        if (Timer > 30)
                        {
                            State++;
                            Timer = 0;
                        }
                    }
                    break;
                case 2://逐渐收回并减淡消失
                    {
                        SpikeTop = Vector2.Lerp(SpikeTop, Projectile.Center, 0.14f);
                        drawColor *= 0.9f;
                        SelfScale *= 0.9f;
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
            Vector2 pos = Projectile.Center - Main.screenPosition;

            if (warningLineAlpha > 0)
            {
                Texture2D warningTex = NightmareSpike.WarningLineTex.Value;
                Rectangle destination = new Rectangle((int)pos.X, (int)pos.Y, (int)SpurtLength, warningTex.Height);

                spriteBatch.Draw(warningTex, destination, null, new Color(190, 0, 101, (byte)(warningLineAlpha * 255)), Projectile.rotation, new Vector2(0, warningTex.Height / 2), 0, 0);
            }

            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D sparkleTex = SparkleTex.Value;
            Texture2D blackholeTex = BlackHole.BlackHoleTex.Value;

            float rot = Projectile.rotation + MathHelper.PiOver2;
            Color c = Color.White;
            c.A = 200;
            spriteBatch.Draw(mainTex, pos, null, c, rot, mainTex.Size() / 2, new Vector2(1, SelfScale), 0, 0);
            spriteBatch.Draw(blackholeTex, pos, null, c, rot + MathHelper.PiOver2, blackholeTex.Size() / 2, new Vector2(SelfScale, 1)*0.3f, 0, 0);

            var sparkleFrame = sparkleTex.Frame(1, 2 , 0, Projectile.ai[1] == -1 ? 0 : 1);
            var sparkleOrigin = sparkleFrame.Size() / 2;
            spriteBatch.Draw(sparkleTex, pos, sparkleFrame, c, Main.GlobalTimeWrappedHourly * 0.5f, sparkleOrigin, SelfScale / 3 + Main.rand.NextFloat(0, 0.02f), 0, 0);

            //Effect e = Filters.Scene["HurricaneTwistReverse"].GetShader().Shader;
            //e.Parameters["uColor"].SetValue(drawColor.ToVector3());
            //e.Parameters["uOpacity"].SetValue(0.5f);
            //e.Parameters["uRotateSpeed"].SetValue(0.2f);
            //e.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap,
            //                Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, e, Main.GameViewMatrix.TransformationMatrix);

            //spriteBatch.Draw(circleTex, pos, null, drawColor, rot, circleTex.Size() / 2, 2, 0, 0);

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public void DrawTectacle()
        {
            spike?.DrawTentacle_NoEndBegin(spike.perLength * 30 / 200);
        }
    }
}
