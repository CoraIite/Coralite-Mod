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

        public int alpha;
        private float recordStartAngle;
        private float recordTotalAngle;
        /// <summary>
        /// 需要设置
        /// </summary>
        protected float extraScaleAngle;
        protected float xScale;
        protected float yScale;

        private bool UseBeforeAngle;
        private float BeforeAngle;


        protected override void InitializeSwing()
        {
            if (UseBeforeAngle)
            {
                base.InitializeSwing();
                InitScale();
                return;
            }

            if (Owner.whoAmI == Main.myPlayer)
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
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
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

            int timer = (int)Timer - minTime;

            Projectile.scale = scale * Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime)), yScale, xScale);
        }

        protected override void BeforeSlash()
        {
            if (UseBeforeAngle)
            {
                float f = Timer / minTime;
                _Rotation =_Rotation.AngleLerp( GetStartAngle() - (DirSign * startAngle),f);
                Slasher();
                {
                    startAngle += BeforeAngle / minTime;
                    recordStartAngle = Math.Abs(startAngle);
                    SetScale();
                }
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

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
            CoraliteSystem.InitBars();
            List<ColoredVertex> bars = CoraliteSystem.Vertexes;
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var c = new Color(255, 255, 255) * Helper.Lerp(alpha, 0, 1 - factor);
                bars.Add(new(Top, c, new Vector2(factor, 0)));
                bars.Add(new(Bottom, c, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ApplyShader();

                    foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                    {
                        pass.Apply();
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                        Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    }
                }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            }
        }

        public abstract Texture2D GetGradient();

        public virtual Effect ApplyShader()
        {
            Effect effect = ShaderLoader.GetShader("ExquisiteHammer");

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.01f);
            effect.Parameters["udissolveS"].SetValue(0.8f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Split2.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(GetGradient());
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
            effect.Parameters["uflowPercent"].SetValue(0.8f);

            return effect;
        }
    }
}
