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
        public override string Texture => AssetDirectory.ShadowCastleItems + "Shadura";

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
        private float recordStartAngle;
        private float recordTotalAngle;

        public enum ComboType
        {
            /// <summary>
            /// 向下冲刺，
            /// </summary>
            SmashDown_SmashDown,
            SmashDown_Shouryuukenn,
            SmashDown_Rolling,
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

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "HLightSlashTrail");
            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowBalls + "SlashGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTexture = null;
            GradientTexture = null;
        }

        public override void SetDefs()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.width = 40;
            Projectile.height = 90;

            trailTopWidth = 2;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 4;
            useSlashTrail = true;
            useTurnOnStart = false;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 75 * Projectile.scale;
        }

        protected override float GetStartAngle() => StartAngle;

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 4;
            alpha = 0;
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
                    minScale = 1.2f;
                    maxScale = 1.5f;
                    useSlashTrail = false;
                    distanceToOwner = -40;

                    Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                    break;
                case (int)ComboType.SmashDown_Shouryuukenn://升龙
                    startAngle = 1.6f;
                    totalAngle = 4.5f;
                    //minTime = 12;
                    Projectile.extraUpdates = 10;
                    maxTime = 18 * 10;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 1f;
                    maxScale = 1.7f;

                    Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);

                    break;
                case (int)ComboType.SmashDown_Rolling://旋转
                    startAngle = 2.2f;
                    totalAngle = 48f;
                    //minTime = 32;
                    maxTime = 80*5;
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    minScale = 0.8f;
                    maxScale = 1.5f;

                    //Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);
                    break;
            }

            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(0, maxTime - minTime), minScale, maxScale);

            base.Initializer();
            //extraScaleAngle *= Math.Sign(totalAngle);
        }

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

        protected override void BeforeSlash()
        {
            if (!GetOwner(out NPC owner))
                return;
            
            switch (Combo)
            {
                default:
                    break;
                case (int)ComboType.SmashDown_SmashDown:
                    {
                        spriteRotation += MathHelper.TwoPi * 3 / minTime;
                        distanceToOwner =Helper.Lerp(-40,8, (Timer / minTime));
                        if ((int)Timer == minTime)
                        {
                            spriteRotation = new Vector2(66, 70).ToRotation();
                            startAngle = owner.rotation;
                        }
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
            if (!GetOwner(out _ )) 
                return;
            
            int timer = (int)Timer - minTime;
            float scale = 1f;

            alpha = (int)(Coralite.Instance.X2Smoother.Smoother(timer, maxTime - minTime) * 140) + 100;

            Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime), minScale, maxScale);


            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 5;
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
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
}
