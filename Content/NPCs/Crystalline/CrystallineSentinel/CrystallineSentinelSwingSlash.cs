using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 二阶段旋风斩的两把绕身刀刃：先在身侧蓄力旋转、再沿半径外扩做整圈（短版半圈）挥砍。
    /// </summary>
    public class CrystallineSentinelSwingSlash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + "CrystallineSentinelSwing";

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Offset => ref Projectile.ai[1];
        public ref float TargetRot => ref Projectile.ai[2];
        public ref float MaxRange => ref Projectile.localAI[0];
        public ref float LightVer => ref Projectile.localAI[1];

        public CrystallineSentinelSwingSlash() : base(1.047f, trailCount: 62) { }
        public int delay;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            trailBottomWidth = 40;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
            useTurnOnStart = false;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (LightVer > 0 && Timer > minTime * 0.7f)
            {
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Bottom, Top, Projectile.width / 2, ref Projectile.localAI[1]);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        protected override void InitializeSwing()
        {
            if (!OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
                return;

            float angleFromOwner = (Projectile.Center - npc.Center).ToRotation();

            Projectile.extraUpdates = 2;
            int euMult = 1 + Projectile.extraUpdates;
            minTime = (80 + 7) * euMult;
            maxTime = minTime + 35 * euMult;
            Smoother = Coralite.Instance.BezierEaseSmoother;/*new CustomSmoother(Helper.EaseOutQuad);*/
            delay = 20 * euMult;

            distanceToOwner = -Projectile.height / 2;
            Projectile.InitOldPosCache(62);

            base.InitializeSwing();
            totalAngle = MathHelper.TwoPi;
            _Rotation = startAngle = angleFromOwner;

            if (LightVer > 0)
            {
                maxTime = minTime + 20 * euMult;
                totalAngle = MathHelper.Pi;
            }
        }

        protected override void AIBefore()
        {
            //RotateVec2 = _Rotation.ToRotationVector2();
            //Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            //Projectile.rotation = _Rotation;
        }

        protected override void BeforeSlash()
        {
            if (LightVer > 0 && Timer < 2)
                Timer = (int)(minTime * 0.5f);
            //_Rotation += 0.01f;
            float maxRange = MaxRange;
            float distCharge =
                Utils.MultiLerp(MathHelper.Clamp(Timer / (minTime * 2 / 3), 0, 1f), -Projectile.height, Projectile.height * 0.75f, Projectile.height / 4);
            float distFadein = Helper.EaseOutCubic(Utils.Remap(Timer, minTime * 2 / 3, minTime, 0f, 1f));
            float dist = maxRange * distFadein;
            distanceToOwner = distCharge + dist;
            float a = 0.01f, b = 0.07f / minTime;

            float rotFactor = Helper.EaseOutQuad(Utils.Remap(Timer, 0, minTime / 3, 1f, 0f));
            float indexFactor = Offset > 0 ? -1 : 1;
            float finalRot = (TargetRot - _Rotation) * Utils.Remap(Timer, 0, minTime * 0.55f, 0f, 1f);
            float extraRot = (float)(_Rotation
                + finalRot
                + a * Timer + b * Timer * (Timer - 1) / 2f)
                + MathHelper.Pi * Utils.Remap(Timer, minTime * 0.7f, minTime, 0f, 1f)
            ;

            RotateVec2 = extraRot.ToRotationVector2();
            Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            Projectile.rotation = extraRot - ((MathHelper.Pi) * rotFactor) * indexFactor;

            if (Timer == minTime)
            {
                if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
                {
                    startAngle = (Projectile.Center - npc.Center).ToRotation();
                    //Projectile.ai[2] = distanceToOwner;
                }
            }
        }

        protected override void OnSlash()
        {
            float fadein = Utils.Remap(Timer, 0, minTime, 0f, 1f);
            float fadeout = Utils.Remap(Timer, maxTime, maxTime + delay, 1f, 0f);
            Projectile.Opacity = fadein * fadeout;
            float factor = Utils.Remap(Timer, minTime, maxTime, 0, 1f);
            float extraRot = Utils.Remap(factor, 0, 1f, 0.3f, 0.01f) + 0.1f;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            _Rotation += 0.01f;
            float maxRange = MaxRange;
            float distFadeout = Helper.BezierEase(Utils.Remap(Timer, maxTime, maxTime + delay, 1, 0f));
            float dist = distFadeout * maxRange;
            distanceToOwner = Projectile.height / 4 + dist;
            Projectile.Opacity = distFadeout;

            RotateVec2 = _Rotation.ToRotationVector2();
            Projectile.Center = OwnerCenter() + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + distanceToOwner));
            Projectile.rotation = _Rotation;

            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void AIAfter()
        {
            RotateVec2 = Projectile.rotation.ToRotationVector2();
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * (Projectile.height / 2 + trailBottomWidth)));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

            if (useShadowTrail || useSlashTrail)
            {
                UpdateCaches();
                UpdateOldPosCaches();
            }
        }

        public void UpdateOldPosCaches()
        {
            for (int i = oldRotate.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            }
            Projectile.oldPos[0] = Top;
        }

        public static float EaseInOutQuint(float time, float duration)
        {
            if ((time /= duration * 0.5f) < 1f)
                return 0.5f * time * time * time * time * time;

            return 0.5f * ((time -= 2f) * time * time * time * time + 2f);
        }

        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                return npc.Center;
            }
            return base.OwnerCenter();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            bool ret = base.PreDraw(ref lightColor);

            if (useSlashTrail && VisualEffectSystem.DrawKniefLight && Timer <= minTime)
                DrawSlashTrail();

            return ret;
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor * Projectile.Opacity, extraRot);
            base.DrawSelf(CrystallineSentinelSwing.GlowTex.Value, origin, Color.White * Projectile.Opacity, extraRot);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            float alpha = Projectile.Opacity * 255f;
            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = Projectile.oldPos[i];
                Vector2 Top = Center /*+ (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]))*/;
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]))
                    - (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Vanilla.Value);
                    effect.Parameters["gradientTexture"].SetValue(CrystallineSentinelSwing.GradientTextureThin.Value);

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
    }
}
