using Coralite.Content.Items.CoreKeeper;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallSlash : BaseSwingProj/*, IDrawWarp*/
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + "Shadurion";

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];
        public ref float StartAngle => ref Projectile.ai[2];

        public override int OwnerDirection
        {
            get
            {
                if (!GetOwner(out NPC owner))
                    return 0;

                return owner.direction;
            }
        }

        public static Asset<Texture2D> trailTexture;
        public static Asset<Texture2D> GradientTexture;

        public ShadowBallSlash() : base(new Vector2(66, 70).ToRotation(), trailLength: 48) { }

        public int delay;
        public int alpha;

        public float minScale;
        public float maxScale;
        public float extraScaleAngle;
        protected float recordStartAngle;
        protected float recordTotalAngle;

        public enum ComboType
        {
            /// <summary>
            /// 向下冲刺，
            /// </summary>
            SmashDown_SmashDown,
            SmashDown_Shouryuukenn,
            SmashDown_Rolling,
            /// <summary>
            /// 横砍
            /// </summary>
            VerticalRolling,

            /// <summary>
            /// 跳跃下砸，向上跳
            /// </summary>
            SkyJump_JumpUp,
        }

        /// <summary>
        /// 生成弹幕
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="damage"></param>
        /// <param name="comboType"></param>
        /// <param name="startAngle"></param>
        /// <returns></returns>
        public static int Spawn(NPC owner, int damage, ComboType comboType, float startAngle)
        {
            return owner.NewProjectileInAI<ShadowBallSlash>(owner.Center, Vector2.Zero, damage, 0, owner.target,
                owner.whoAmI, (int)comboType, startAngle);
        }

        public override void SetDefs()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = 40;
            Projectile.height = 90;

            trailTopWidth = 16;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 4;
            useSlashTrail = true;
            useTurnOnStart = false;

            alpha = 255;
        }

        #region 杂项

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "HTrail");
            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowCastleItems + "ShaduraGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            GradientTexture = null;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 65 * Projectile.scale;
        }

        protected override float GetStartAngle() => StartAngle;

        protected override void AIBefore()
        {
            Lighting.AddLight(Owner.Center, new Vector3(0.8f, 0.3f, 0.8f));
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + RotateVec2 * (Projectile.scale * Projectile.height / 2 + trailTopWidth);
            Bottom = Projectile.Center - RotateVec2 * (Projectile.scale * Projectile.height / 2);//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (useShadowTrail || useSlashTrail)
                UpdateCaches();
        }

        public bool GetOwner(out NPC owner)
        {
            if (!Main.npc.IndexInRange((int)OwnerIndex))
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)OwnerIndex];
            if (!npc.active || npc.type != NPCType<ShadowBall>())
            {
                Projectile.Kill();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        protected override Vector2 OwnerCenter()
        {
            if (!GetOwner(out NPC owner))
                return Vector2.Zero;
            return owner.Center;
        }

        #endregion

        protected override void Initializer()
        {
            Projectile.extraUpdates = 4;
            bool setScale = true;
            switch (Combo)
            {
                default:
                case (int)ComboType.SmashDown_SmashDown://向下冲刺
                    startAngle = 0f;
                    totalAngle = 0.02f;
                    minTime = 33 * 5;
                    maxTime = minTime + 25 * 5;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 1f;
                    maxScale = 1f;
                    useSlashTrail = false;
                    distanceToOwner = -40;

                    Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                    break;
                case (int)ComboType.SmashDown_Shouryuukenn://升龙
                    startAngle = 1.6f;
                    totalAngle = 4.5f;
                    Projectile.extraUpdates = 6;
                    minTime = 28 * 7;
                    maxTime = minTime + 18 * 7;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 0.8f;
                    maxScale = 1.3f;

                    Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                    setScale = false;
                    break;
                case (int)ComboType.SmashDown_Rolling://旋转
                    startAngle = 2.2f;
                    totalAngle = 36f;
                    minTime = 33 * 5;
                    maxTime = minTime + 80 * 5;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    minScale = 0.8f;
                    maxScale = 1.4f;

                    //Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);
                    setScale = false;
                    break;
                case (int)ComboType.VerticalRolling:
                    {
                        int rand = Main.rand.NextFromList(-1, 1);
                        startAngle = rand * (2.2f + Main.rand.NextFloat(-0.2f, 0.2f));
                        totalAngle = rand * (4.6f + Main.rand.NextFloat(-0.2f, 0.2f));
                        minTime = 30 * 5;
                        maxTime = minTime + 14 * 5;
                        delay = 30;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        extraScaleAngle = Main.rand.NextFloat(-0.4f, 0.4f);
                        minScale = 0.7f;
                        maxScale = 1.1f;
                        setScale = false;
                    }
                    break;
                    case (int)ComboType.SkyJump_JumpUp:{
                    startAngle = 0f;
                    totalAngle = 0.02f;
                    minTime = 30 * 5;
                    maxTime = minTime + 20 * 5;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 1f;
                    maxScale = 1f;
                    useSlashTrail = false;
                    distanceToOwner = -40;
                    }
                    break;
            }

            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            if (setScale)
                Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), minScale, maxScale);
            else
                Projectile.scale = 0.01f;
            base.Initializer();
            //extraScaleAngle *= Math.Sign(totalAngle);
        }

        protected override void BeforeSlash()
        {
            if (!GetOwner(out NPC owner))
                return;

            switch (Combo)
            {
                default:
                    break;
                case (int)ComboType.SmashDown_SmashDown:
                case (int)ComboType.SkyJump_JumpUp:
                    {
                        spriteRotation += MathHelper.TwoPi * 3 / minTime;
                        distanceToOwner = Helper.Lerp(-40, 8, (Timer / minTime));
                        if ((int)Timer == minTime)
                        {
                            spriteRotation = new Vector2(66, 70).ToRotation();
                            startAngle = owner.rotation;
                        }
                    }
                    break;
                case (int)ComboType.SmashDown_Shouryuukenn:
                case (int)ComboType.SmashDown_Rolling:
                    {
                        Projectile.scale = Helper.Lerp(0, 0.8f, Coralite.Instance.SqrtSmoother.Smoother(Timer / minTime));
                    }
                    break;
                case (int)ComboType.VerticalRolling:
                    {
                        Projectile.scale = Helper.Lerp(0, 1f, Coralite.Instance.SqrtSmoother.Smoother(Timer / minTime));
                    }
                    break;

            }

            _Rotation = startAngle;
            Slasher();
            if ((int)Timer == minTime)
            {
                if (useSlashTrail)
                    InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            if (!GetOwner(out _))
                return;

            int timer = (int)Timer - minTime;

            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime), minScale, maxScale);

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Projectile.scale = Helper.Lerp(Projectile.scale, 0, 0.1f);
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            lightColor = new Color(80,0,120,0);

            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            base.DrawSelf(mainTex, origin, lightColor*0.9f, extraRot);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / count;
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["StarsTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
                effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                effect.Parameters["uExchange"].SetValue(0.87f + 0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly));

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    public class ShadowBallSlash2 : ShadowBallSlash
    {
        public static int Spawn(NPC owner, int damage, float startAngle)
        {
            return owner.NewProjectileInAI<ShadowBallSlash2>(owner.Center, Vector2.Zero, damage, 0, owner.target,
                owner.whoAmI, -1, startAngle);
        }

        protected override void Initializer()
        {
            Projectile.extraUpdates = 4;
            alpha = 0;
            startAngle = 0f;
            totalAngle = 38.5f;
            minTime = 20 * 4;
            maxTime = minTime+90 * 4;
            Smoother = Coralite.Instance.BezierEaseSmoother;
            delay = 20*4;
            Projectile.localNPCHitCooldown = 60;
            Projectile.scale = 0.01f;

            Projectile.velocity *= 0f;
            if (Owner.whoAmI == Main.myPlayer)
            {
                _Rotation = startAngle = GetStartAngle() - OwnerDirection * startAngle;//设定起始角度
                totalAngle *= OwnerDirection;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailLength];
                oldDistanceToOwner = new float[trailLength];
                oldLength = new float[trailLength];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        protected override void BeforeSlash()
        {
            if (!GetOwner(out _))
                return;

            Projectile.scale = Helper.Lerp(0, 1f, Coralite.Instance.SqrtSmoother.Smoother(Timer / minTime));

            _Rotation = startAngle;
            Slasher();
            if ((int)Timer == minTime)
            {
                if (useSlashTrail)
                    InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            if (!GetOwner(out _))
                return;

            if (alpha<255)
            {
                alpha += 5;
            }
            _Rotation = startAngle + totalAngle * Smoother.Smoother((int)Timer - minTime, maxTime - minTime);
            Slasher();
        }

        protected override void AfterSlash()
        {
            Projectile.scale = Helper.Lerp(1f, 0, Coralite.Instance.SqrtSmoother.Smoother((Timer-maxTime) / delay));

            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 80 * Projectile.scale;
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new List<VertexPositionColorTexture>();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < oldRotate.Length; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - i / count;
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]);
                Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]);

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = Filters.Scene["NoHLGradientTrail"].GetShader().Shader;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(RuneSongSlash.trailTexture.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }

                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
