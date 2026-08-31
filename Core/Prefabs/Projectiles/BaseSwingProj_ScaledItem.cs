using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 使用ai0传入物品类型
    /// </summary>
    /// <param name="spriteRotation"></param>
    /// <param name="trailCount"></param>
    public abstract class BaseSwingProj_ScaledItem(float spriteRotation = 0.785f, short trailCount = 15) : BaseSwingProj(spriteRotation, trailCount)
    {
        public ref float ItemType => ref Projectile.ai[0];
        public ref float Follow => ref Projectile.ai[1];
        public ref float Combo => ref Projectile.ai[2];

        public int alpha;
        public int beforeTime;
        public int ExDirection;
        public int Delay;
        public ISmoother beforeSmoother=Coralite.Instance.NoSmootherInstance;
        private float recordStartAngle;
        private float recordStartAngleInn;
        /// <summary>
        /// 需要设置
        /// </summary>
        protected float extraScaleAngle;
        protected float xScale = 1;
        protected float yScale = 1;

        protected float? BeforeAngle;

        public void InitDirection()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction= ExDirection = InMousePos.X > Owner.Center.X ? 1 : -1;
        }

        protected override void InitializeSwing()
        {
            Follow = -1;
            InitScale();
            recordStartAngleInn = startAngle;

            if (!BeforeAngle.HasValue)
            {
                base.InitializeSwing();
                return;
            }

            if (Owner.whoAmI == Main.myPlayer&&Combo==0)
            {
                _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                //totalAngle *= OwnerDirection;
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (useShadowTrail || useSlashTrail)
            {
                oldRotate = new float[trailCount];
                oldDistanceToOwner = new float[trailCount];
                oldLength = new float[trailCount];
                InitializeCaches();
            }

            onStart = false;
            Projectile.netUpdate = true;
        }

        public void InitScale()
        {
            //extraScaleAngle = Main.rand.NextFloat(-0.4f, 0.4f);
            recordStartAngle = GetStartAngle();
            SetScale();
        }

        /// <summary>
        /// 根据近战范围和角度缩放尺寸
        /// </summary>
        public void SetScale()
        {
            float scale = 1f;

            if (Item.type == ItemType)
                scale = Owner.GetAdjustedItemScale(Item);
            else
                Projectile.Kill();

            Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle-_Rotation, yScale, xScale);
        }

        protected override void BeforeSlash()
        {
            if (BeforeAngle.HasValue)
            {
                float f = beforeSmoother.Smoother(Helper.Clamp(Timer / beforeTime, 0, 1));

                _Rotation = _Rotation.AngleLerp(GetStartAngle() - (DirSign * startAngle), Helper.X3Ease( Timer / minTime));
                
                startAngle = recordStartAngleInn + BeforeAngle.Value * f;
                InitScale();

                if (Timer == minTime)
                {
                    _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                    totalAngle *= DirSign;
                    InitScale();

                    Smoother.ReCalculate(maxTime - minTime);

                    if (useShadowTrail || useSlashTrail)
                    {
                        oldRotate = new float[trailCount];
                        oldDistanceToOwner = new float[trailCount];
                        oldLength = new float[trailCount];
                        InitializeCaches();
                    }
                }
            }

            Slasher();
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + Delay)
            {
                if (DownLeft)
                {
                    Combo++;
                    Timer = 0;
                    InitializeSwing();
                }
                else
                    Projectile.Kill();
            }
        }

        public override void PostDraw(Color lightColor)
        {
            if (onStart)
                return;

            Texture2D mainTex = TextureAssets.Item[(int)ItemType].Value;
            Vector2 origin = new(mainTex.Width / 2, mainTex.Height / 2);

            float extraRot = GetExRot();

            if (useShadowTrail && Timer > minTime)
                DrawShadowTrail(mainTex, origin, lightColor, extraRot);

            if (canDrawSelf)
                DrawSelf(mainTex, origin, lightColor, extraRot);
        }

        protected override float GetExRot()
        {
            int dir = Math.Sign(totalAngle);

            if (Timer < minTime)
                dir =  ExDirection;

            float extraRot = DirSign < 0 ? MathHelper.Pi : 0;
            extraRot += DirSign == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            return extraRot;
        }

        protected override SpriteEffects CheckEffect()
        {
            if (Timer < minTime)
            {
                if (ExDirection < 0)
                    return SpriteEffects.FlipHorizontally;
                return SpriteEffects.None;
            }
            return base.CheckEffect();
        }
        
        protected override void DrawSlashTrail()
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            CoraliteSystem.InitBars2();
            List<ColoredVertex> bars2 = CoraliteSystem.Vertexes2;
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                SetBars(bars, bars2, i, factor);
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ApplyBottomColorShader();

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                    effect = ApplyHighlightColor();
                    Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        ApplyHighlight(bars2);
                    }

                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public virtual void ApplyHighlight(List<ColoredVertex> bars2)
        {
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
        }

        public virtual void SetBars(List<ColoredVertex> bars, List<ColoredVertex> bars2, int i, float factor)
        {
            Vector2 Center = GetCenter(i);
            Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
            Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

            var c = new Color(255, 255, 255) * Helper.Lerp(alpha, 0, 1 - factor);

            Color c2 = AdditiveColor(factor);

            bars.Add(new(Top, c, new Vector2(factor, 0)));
            bars.Add(new(Bottom, c, new Vector2(factor, 1)));
            bars2.Add(new(Top, c2, new Vector2(factor, 0)));
            bars2.Add(new(Bottom, c2, new Vector2(factor, 1)));
        }

        public abstract Texture2D GetGradient();

        public virtual Color AdditiveColor(float f)
            =>new Color(255, 255, 255) * Helper.Lerp(alpha, 0, 1 - f);

        public virtual Effect ApplyBottomColorShader()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split2.Value);
            effect.Parameters["gradientTexture"].SetValue(GetGradient());
            return effect;
        }

        public virtual Effect ApplyHighlightColor()
        {
            Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split2.Value);
            effect.Parameters["gradientTexture"].SetValue(GetGradient());
            return effect;
        }
    }
}
