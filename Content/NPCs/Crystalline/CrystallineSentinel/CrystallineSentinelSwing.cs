using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    /// <summary>
    /// 二阶段挥刀：从选定的手掌出发、沿出手方向前伸最多 480 px 的大幅挥砍（带渐变刀光）。
    /// 挂在哪只手由本体的 <see cref="CrystallineSentinel.SwingHandSign"/> 决定。
    /// </summary>
    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineSentinelSwing : BaseSwingProj
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public ref float OwnerIndex => ref Projectile.ai[0];

        [VaultLoaden("{@classPath}" + "CrystallineSentinelSwing_Glow")]
        public static ATex GlowTex { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradient")]
        public static ATex GradientTexture { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientThin")]
        public static ATex GradientTextureThin { get; set; }
        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientThin2")]
        public static ATex GradientTextureThin2 { get; set; }

        public CrystallineSentinelSwing() : base(0.785f, trailCount: 62) { }

        public int delay;
        public int alpha;

        public float dir;
        public float offsetLength;
        public float maxLength;
        public Vector2 velocity;

        public override void SetSwingProperty()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.width = 40;
            Projectile.height = 85;
            trailTopWidth = 20;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 0;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 30 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                dir = Projectile.velocity.ToRotation();//(Projectile.Center - npc.Center).ToRotation();
                maxLength = Vector2.Distance(Main.player[npc.target].Center, npc.Center) + 140;

                if (maxLength > 480)
                    maxLength = 480;
            }

            Projectile.extraUpdates = 2;
            alpha = 0;
            startAngle = 0f;
            totalAngle = 40.5f;
            maxTime = 90 * 3;
            Smoother = Coralite.Instance.BezierEaseSmoother;
            delay = 20;
            distanceToOwner = -Projectile.height / 2;
            Projectile.localNPCHitCooldown = 60;
            Projectile.InitOldPosCache(62);

            base.InitializeSwing();
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.3f, 0.3f, 1f);
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;

            if (timer % 30 == 0)
                onHitTimer = 0;

            alpha = (int)(Helper.SinEase(timer, maxTime) * 255);
            if (timer < maxTime / 2)
                offsetLength = Helper.SqrtEase(timer, maxTime / 2) * maxLength;
            else
                offsetLength = Helper.SinEase(timer, maxTime) * maxLength;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (alpha > 20)
                alpha -= 10;
            if (Projectile.scale > 0.8f)
            {
                Projectile.scale *= 0.999f;
            }

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void AIAfter()
        {
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

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
        protected override Vector2 OwnerCenter()
        {
            if (OwnerIndex.GetNPCOwner<CrystallineSentinel>(out NPC npc, Projectile.Kill))
            {
                CrystallineSentinel cs = npc.ModNPC as CrystallineSentinel;
                // 旧代码直接读 ai[2]（那时的 Recorder），那个槽现在归基座；挥刀用哪只手改从本体的声明取，随热字段过线
                Vector2 pos = (cs.SwingHandSign > 0 ? cs.P2LeftHandPos : cs.P2RightHandPos);

                return pos + dir.ToRotationVector2() * offsetLength;
            }

            return base.OwnerCenter();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            base.DrawSelf(GlowTex.Value, origin, Color.White * (alpha / 255f), extraRot);
        }

        protected override void DrawSlashTrail()
        {
            if (oldRotate == null)
                return;

            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

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
                    effect.Parameters["gradientTexture"].SetValue(GradientTextureThin.Value);

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
