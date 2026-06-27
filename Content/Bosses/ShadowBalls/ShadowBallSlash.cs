using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.BossSystem;
using Coralite.Helpers;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class ShadowBallSlash : BaseSwingProj/*, IDrawWarp*/
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + "Shadurion";

        public ref float OwnerIndex => ref Projectile.ai[0];
        public ref float Combo => ref Projectile.ai[1];
        public ref float StartAngle => ref Projectile.ai[2];

        public override int DirSign
        {
            get
            {
                if (!GetOwner(out NPC owner))
                    return 0;

                return owner.direction;
            }
        }

        public static Asset<Texture2D> GradientTexture;

        public ShadowBallSlash() : base(new Vector2(66, 70).ToRotation(), trailCount: 48) { }

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
            NightmareKingDash,
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
            // 仅权威端生成，随后由原版弹幕同步 + BaseSwingProj 手持同步把起手角下发给各端，避免多人重复生成。
            if (VaultUtils.isClient)
            {
                return -1;
            }

            return owner.NewProjectileInAI<ShadowBallSlash>(owner.Center, Vector2.Zero, damage, 0, owner.target,
                owner.whoAmI, (int)comboType, startAngle);
        }

        public override void SetSwingProperty()
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

        /// <summary>
        /// 挥舞初始化权威：单人/专用服走服务端；联机客户端仅本地拥有端（房主即目标玩家）可 init，其余等 NetHeldReceive。
        /// </summary>
        protected bool IsSwingInitAuthority() => !VaultUtils.isClient || Projectile.IsOwnedByLocalPlayer();

        /// <summary>从主球已同步的 AttackSeed 派生本斩击专用 RNG，专用服与客户端结果一致。</summary>
        private Random CreateSlashRandom()
        {
            if (!GetOwner(out NPC owner))
            {
                return new Random(Projectile.identity);
            }

            int seed = (int)owner.ai[CoraliteBossContext.AttackSeedAiSlot];
            if (seed == 0)
            {
                seed = owner.whoAmI + 1;
            }

            unchecked
            {
                seed ^= Projectile.identity * 397;
                seed ^= (int)Combo * 7919;
                seed ^= Projectile.whoAmI;
            }

            return new Random(seed);
        }

        private static float RandFloat(Random rng, float min, float max)
            => min + (float)(rng.NextDouble() * (max - min));

        private static int RandSign(Random rng) => rng.Next(2) == 0 ? -1 : 1;

        /// <summary>
        /// 专用服上 BaseSwingProj 的 onStart 门控依赖 IsOwnedByLocalPlayer，需在生成帧由权威端预初始化。
        /// </summary>
        public override void OnSpawn(IEntitySource source)
        {
            if (!VaultUtils.isClient)
            {
                InitBasicValues();
                InitializeSwing();
            }
        }

        protected void FinalizeSwingInit()
        {
            if (!IsSwingInitAuthority())
            {
                return;
            }

            _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);
            totalAngle *= DirSign;
            Projectile.netUpdate = true;
            onStart = false;
            netSendBasicValues = true;
            init = false;
        }

        #region 杂项

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GradientTexture = Request<Texture2D>(AssetDirectory.ShadowCastleItems + "ShaduraGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

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
            Top = Projectile.Center + (RotateVec2 * ((Projectile.scale * Projectile.height / 2) + trailTopWidth));
            Bottom = Projectile.Center - (RotateVec2 * (Projectile.scale * Projectile.height / 2));//弹幕的底端和顶端计算，用于检测碰撞以及绘制

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

        // BaseSwingProj 仅同步 startAngle/totalAngle 等基础量，这里补充同步缩放角与缩放曲线用的角度记录，
        // 保证 VerticalRolling / SmashDown_Rolling 等带随机/位置相关缩放的招式在各端 hitbox 完全一致。
        // record* 由本地拥有端在 InitializeSwing 内（base 变换前）算出，单机不走网络、行为与旧代码一致。
        public override void NetHeldSend(BinaryWriter writer)
        {
            base.NetHeldSend(writer);
            writer.Write(extraScaleAngle);
            writer.Write(recordStartAngle);
            writer.Write(recordTotalAngle);
        }

        public override void NetHeldReceive(BinaryReader reader)
        {
            base.NetHeldReceive(reader);
            extraScaleAngle = reader.ReadSingle();
            recordStartAngle = reader.ReadSingle();
            recordTotalAngle = reader.ReadSingle();
        }

        #endregion

        protected override void InitializeSwing()
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
                    maxTime = minTime + (25 * 5);
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 1f;
                    maxScale = 1f;
                    useSlashTrail = false;
                    distanceToOwner = -40;

                    break;
                case (int)ComboType.SmashDown_Shouryuukenn://升龙
                    startAngle = 1.6f;
                    totalAngle = 4.5f;
                    Projectile.extraUpdates = 6;
                    minTime = 28 * 7;
                    maxTime = minTime + (18 * 7);
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    extraScaleAngle = 0f;
                    minScale = 0.8f;
                    maxScale = 1.3f;

                    setScale = false;
                    break;
                case (int)ComboType.SmashDown_Rolling://旋转
                    startAngle = 2.2f;
                    totalAngle = 36f;
                    minTime = 33 * 5;
                    maxTime = minTime + (80 * 5);
                    Smoother = Coralite.Instance.NoSmootherInstance;
                    minScale = 0.8f;
                    maxScale = 1.4f;

                    if (IsSwingInitAuthority() && GetOwner(out NPC rollingOwner))
                    {
                        Player rollingTarget = Main.player[rollingOwner.target];
                        extraScaleAngle = MathHelper.Clamp((rollingTarget.Center.X - rollingOwner.Center.X) / 300, -1, 1) * 0.25f;
                    }

                    setScale = false;
                    break;
                case (int)ComboType.VerticalRolling:
                    {
                        if (IsSwingInitAuthority())
                        {
                            Random rng = CreateSlashRandom();
                            int rand = RandSign(rng);
                            startAngle = rand * (2.2f + RandFloat(rng, -0.2f, 0.2f));
                            totalAngle = rand * (4.6f + RandFloat(rng, -0.2f, 0.2f));
                            extraScaleAngle = RandFloat(rng, -0.4f, 0.4f);
                        }
                        minTime = 30 * 5;
                        maxTime = minTime + (14 * 5);
                        delay = 30;
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        minScale = 0.7f;
                        maxScale = 1.1f;
                        setScale = false;
                    }
                    break;
                case (int)ComboType.SkyJump_JumpUp:
                    {
                        startAngle = 0f;
                        totalAngle = 0.02f;
                        minTime = 30 * 5;
                        maxTime = minTime + (20 * 5);
                        Smoother = Coralite.Instance.BezierEaseSmoother;
                        extraScaleAngle = 0f;
                        minScale = 1f;
                        maxScale = 1f;
                        useSlashTrail = false;
                        distanceToOwner = -40;
                    }
                    break;
                case (int)ComboType.NightmareKingDash:
                    {
                        startAngle = 2.4f;
                        totalAngle = 4.8f + MathHelper.TwoPi;
                        minTime = 28 * 5;
                        maxTime = minTime + (30 * 5);
                        Smoother = Coralite.Instance.NoSmootherInstance;
                        minScale = 0.8f;
                        maxScale = 1.4f;
                        extraScaleAngle = 0;

                        setScale = false;
                    }
                    break;
            }

            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            if (setScale)
                Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), minScale, maxScale);
            else
                Projectile.scale = 0.01f;

            Projectile.velocity *= 0f;
            FinalizeSwingInit();
            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
            {
                oldRotate ??= new float[trailCount];
                oldDistanceToOwner ??= new float[trailCount];
                oldLength ??= new float[trailCount];
                InitializeCaches();
            }
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
                    {
                        spriteRotation += MathHelper.TwoPi * 3 / minTime;
                        distanceToOwner = Helper.Lerp(-40, 8, Timer / minTime);
                        if ((int)Timer == minTime)
                        {
                            spriteRotation = new Vector2(66, 70).ToRotation();
                            startAngle = owner.rotation;
                            Helper.PlayPitched("Misc/SwingWave", 0.4f, 0f, Owner.Center);
                        }
                    }
                    break;
                case (int)ComboType.SmashDown_Shouryuukenn:
                case (int)ComboType.SmashDown_Rolling:
                    {
                        Projectile.scale = Helper.Lerp(0, 0.8f, Helper.SqrtEase(Timer / minTime));
                        if ((int)Timer == minTime)
                        {
                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                        }

                    }
                    break;
                case (int)ComboType.VerticalRolling:
                    {
                        Projectile.scale = Helper.Lerp(0, 1f, Helper.SqrtEase(Timer / minTime));
                        if ((int)Timer == minTime)
                        {
                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
                        }
                    }
                    break;
                case (int)ComboType.SkyJump_JumpUp:
                    {
                        spriteRotation += MathHelper.TwoPi * 3 / minTime;
                        distanceToOwner = Helper.Lerp(-40, 8, Timer / minTime);
                        if ((int)Timer == minTime)
                        {
                            spriteRotation = new Vector2(66, 70).ToRotation();
                            startAngle = owner.velocity.ToRotation();
                            Helper.PlayPitched("Misc/SwingWave", 0.4f, 0f, Owner.Center);
                        }
                    }
                    break;
                case (int)ComboType.NightmareKingDash:
                    {
                        Projectile.scale = Helper.Lerp(0, 0.8f, Helper.SqrtEase(Timer / minTime));
                        if ((int)Timer == minTime)
                        {
                            Helper.PlayPitched("Misc/Swing", 0.4f, 0f, Owner.Center);
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
            if (!GetOwner(out NPC owner))
                return;

            int timer = (int)Timer - minTime;

            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime)), minScale, maxScale);

            switch (Combo)
            {
                default:
                    break;
                case (int)ComboType.SmashDown_Rolling:
                case (int)ComboType.NightmareKingDash:
                    {
                        Vector2 dir = _Rotation.ToRotationVector2();

                        owner.spriteDirection = owner.direction = MathF.Sign(dir.X);
                        if (timer % 60 == 0)
                            Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);
                    }
                    break;
            }

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
            lightColor = new Color(80, 0, 120, 0);

            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            base.DrawSelf(mainTex, origin, lightColor * 0.9f, extraRot);
        }

        protected override void DrawSlashTrail()
        {
            RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("StarsTrail");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Highlight.Value);
                effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);
                effect.Parameters["worldSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 5);
                effect.Parameters["uExchange"].SetValue(0.87f + (0.05f * MathF.Sin(Main.GlobalTimeWrappedHourly)));

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
            if (VaultUtils.isClient)
            {
                return -1;
            }

            return owner.NewProjectileInAI<ShadowBallSlash2>(owner.Center, Vector2.Zero, damage, 0, owner.target,
                owner.whoAmI, -1, startAngle);
        }

        protected override void InitializeSwing()
        {
            Projectile.extraUpdates = 4;
            alpha = 0;
            minTime = 20 * 4;
            maxTime = minTime + (90 * 4);
            Smoother = Coralite.Instance.BezierEaseSmoother;
            delay = 20 * 4;
            Projectile.localNPCHitCooldown = 60;
            Projectile.scale = 0.01f;
            Projectile.velocity *= 0f;

            if (IsSwingInitAuthority())
            {
                startAngle = 0f;
                totalAngle = 38.5f;
                FinalizeSwingInit();
            }

            Slasher();
            Smoother.ReCalculate(maxTime - minTime);

            if (!VaultUtils.isServer && (useShadowTrail || useSlashTrail))
            {
                oldRotate = new float[trailCount];
                oldDistanceToOwner = new float[trailCount];
                oldLength = new float[trailCount];
                InitializeCaches();
            }
        }

        protected override void BeforeSlash()
        {
            if (!GetOwner(out _))
                return;

            Projectile.scale = Helper.Lerp(0, 1f, Helper.SqrtEase(Timer / minTime));

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
            if (!GetOwner(out NPC owner))
                return;

            if (alpha < 255)
            {
                alpha += 5;
            }

            if (Timer % 60 == 0)
                Helper.PlayPitched("Misc/SwingFlow", 0.4f, 0f, Owner.Center);

            _Rotation = startAngle + (totalAngle * Smoother.Smoother((int)Timer - minTime, maxTime - minTime));
            Slasher();
        }

        protected override void AfterSlash()
        {
            Projectile.scale = Helper.Lerp(1f, 0, Helper.SqrtEase((Timer - maxTime) / delay));

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
            List<VertexPositionColorTexture> bars = new();
            GetCurrentTrailCount(out float count);

            for (int i = 0; i < count; i++)
            {
                if (oldRotate[i] == 100f)
                    continue;

                float factor = 1f - (i / count);
                Vector2 Center = GetCenter(i);
                Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
                Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

                var topColor = Color.Lerp(new Color(238, 218, 130, alpha), new Color(167, 127, 95, 0), 1 - factor);
                var bottomColor = Color.Lerp(new Color(109, 73, 86, alpha), new Color(83, 16, 85, 0), 1 - factor);
                bars.Add(new(Top.Vec3(), topColor, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), bottomColor, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

                Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatBlurSmall.Value);
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
