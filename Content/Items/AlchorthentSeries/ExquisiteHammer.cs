using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class ExquisiteHammer : BaseAlchorthentItem
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + "ExquisiteHammerItemSmall";

        public static Color ShineIronColor = new Color(250, 97, 105);

        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ProjectileType<ExquisiteAwl>();

            Item.SetWeaponValues(30, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 2));
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            Main.projectile[p].originalDamage = Item.damage;

            Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<ExquisiteHammerHeldProj>(), damage * 2, knockback * 1.5f, player.whoAmI, 0, p);
        }

        public override void MinionAim(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

        }

        public override void AddRecipes()
        {

        }

        public static LineDrawer NewIronAlchSymbol()
        {
            const float sqrt2D2 = 1.414f / 2;
            const float circleScale = 0.7f;

            Vector2 circleOrigin = new Vector2(sqrt2D2, -sqrt2D2) * circleScale;

            return new LineDrawer([
                //圆圈
             new LineDrawer.WarpLine(circleOrigin,36
                ,f => (-MathHelper.PiOver4+f * (MathHelper.TwoPi+0.1f)).ToRotationVector2()*circleScale,linwWidthScale:1.2f),
                //圆圈上的一条线
                    new LineDrawer.StraightLine(circleOrigin,new Vector2(sqrt2D2*2,-sqrt2D2*2),AlchorthentAssets.DoubleSideBigLine),
                    //箭头
                    new LineDrawer.StraightLine(new Vector2(sqrt2D2*2,-sqrt2D2*2),new Vector2(sqrt2D2*2-0.6f,-sqrt2D2*2+0.2f),linwWidthScale:0.7f),
                    new LineDrawer.StraightLine(new Vector2(sqrt2D2*2,-sqrt2D2*2),new Vector2(sqrt2D2*2-0.2f,-sqrt2D2*2+0.6f),linwWidthScale:0.7f),
                    ]);
        }
    }

    /// <summary>
    /// ai0传入combo，ai1传入牵引弹幕索引
    /// </summary>
    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteHammerHeldProj() : BaseSwingProj(1, 30)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + nameof(ExquisiteHammer);

        public ref float Combo => ref Projectile.ai[0];
        public ref float ChainedProjIndex => ref Projectile.ai[1];

        public Particle chainParticle;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        private float exRot;

        public int delay;
        public int alpha;

        public int StartDirection;
        public int ExDirection;

        public bool hitted = false;

        public Color highlightColor;
        public Vector2 lineMiddlePos;
        public float lineAlpha;

        public PRTGroup flameGroup;
        public int updateCount = 0;

        [VaultLoaden("{@classPath}" + "ExquisiteHammerGradient")]
        public static ATex GradientTexture { get; set; }
        [VaultLoaden("{@classPath}" + "ExquisiteHammer_Highlight")]
        public static ATex HighlightTexture { get; set; }
        [VaultLoaden("{@classPath}" + "ExquisiteHammerLine")]
        public static ATex LineTexture { get; set; }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 40;
            Projectile.height = 95;
            trailTopWidth = -10;
            distanceToOwner = -10;
            onHitFreeze = 60;
            Projectile.hide = true;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 60 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

            alpha = 0;
            flameGroup ??= new PRTGroup();
            switch (Combo)
            {
                default:
                case 0://召唤，连线
                    ExDirection = -1;
                    startAngle = -0.3f;
                    totalAngle = 3f;
                    minTime = 79;
                    maxTime = minTime + (int)(Owner.itemTimeMax) + 14 * 5;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 5 * 12;
                    extraScaleAngle = 0;
                    ExtraInit();
                    InitScale();

                    StartDirection = Owner.direction;

                    Projectile.velocity *= 0f;
                    if (Owner.whoAmI == Main.myPlayer)
                    {
                        _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度
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
                    return;
                case 1:
                    startAngle = 2.4f;
                    totalAngle = 3.5f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 18;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    InitScale();
                    base.InitializeSwing();

                    break;
            }
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
        }

        public void InitScale()
        {
            Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle, 1.1f, 1.3f);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.25f);
            base.AIBefore();
            updateCount++;
            if (updateCount>= Projectile.MaxUpdates)
            {
                updateCount = 0;
                if (flameGroup != null)
                {
                    flameGroup.Update();
                    Vector2 off = Owner.position - Owner.oldPosition;
                    foreach (var p in flameGroup)
                        p.Position += off;
                }
            }
        }

        protected override void BeforeSlash()
        {
            switch (Combo)
            {
                default:
                case 0:
                    Summon();
                    break;
            }
        }

        #region Summon

        public void Summon()
        {
            /*
             * 召唤
             * 先向下滑动一点点，然后提起来，再偏转到对应位置
             */

            const int DownTime = 15;
            const int UpTime = 20;
            const int ChannelTime = 26;

            const float StartAngle = -1.5f;
            const float downAngle = 0.3f;
            const float UpAngle = -1.3f;

            if (Timer <= DownTime)//放下，接一个自身自转一圈
            {
                Owner.direction = StartDirection;
                float f = Timer / DownTime;
                float x2f = Helper.X2Ease(f);
                _Rotation = (StartDirection > 0 ? 0 : MathHelper.Pi) + StartDirection * StartAngle + StartDirection * (downAngle - StartAngle) * x2f;

                exRot = MathHelper.TwoPi * f * StartDirection;
            }
            else if (Timer < DownTime + UpTime)//举起
            {
                if (lineAlpha < 1)
                {
                    lineAlpha += 0.2f;
                    if (lineAlpha > 1)
                        lineAlpha = 1;
                }

                float baseF = (Timer - DownTime) / UpTime;
                float f = Helper.BezierEase(baseF);

                //连线中点的位置
                lineMiddlePos = Top - RotateVec2 * MathF.Sin(Helper.X2Ease(baseF) * MathHelper.Pi * 3) * 50;

                Owner.direction = StartDirection;
                _Rotation = (StartDirection > 0 ? 0 : MathHelper.Pi) + StartDirection * downAngle + StartDirection * (UpAngle - downAngle) * f;
            }
            else if (Timer == DownTime + UpTime)
            {
                startAngle = downAngle - UpAngle;
                ExDirection = 1;
                exRot = 0;
            }
            else if (Timer < DownTime + UpTime + ChannelTime)//蓄力
            {
                if (lineAlpha > 0)
                {
                    lineAlpha -= 1f / ChannelTime;
                    if (lineAlpha < 0)
                        lineAlpha = 0;

                    if (ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile p))
                        lineMiddlePos = Vector2.Lerp(Top + RotateVec2 * 50, (Top + p.Center) / 2, lineAlpha);
                }

                float f = Helper.SqrtEase((Timer - DownTime - UpTime) / ChannelTime);

                float rotAdd = (1 - f) * 0.13f;
                startAngle += rotAdd;
                totalAngle += rotAdd * 1.4f;
                _Rotation = _Rotation.AngleLerp(GetStartAngle() - (DirSign * startAngle), 0.25f);

                ChannelParticle();
                highlightColor = Color.Lerp(Color.Transparent, new Color(246, 71, 99) * 0.8f, f);
            }
            else if (Timer == DownTime + UpTime + ChannelTime)//完成蓄力
            {
                lineAlpha = 0;
                var p = PRTLoader.NewParticle<ExquisiteHammerSparkle>(GetTop(), Vector2.Zero);
                p.owner = this;
            }

            Slasher();

            if (Timer == minTime)
            {
                ExtraInit();
                InitScale();

                _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
                totalAngle *= DirSign;

                InitializeCaches();
                Projectile.extraUpdates = 4;
            }

            void ChannelParticle()
            {
                if (Timer < DownTime + UpTime + 20 && Timer % 3 == 0)
                {
                    float rot = Timer / 3 * (MathHelper.TwoPi / 3 + 0.34372f);
                    Vector2 dir = rot.ToRotationVector2();
                    float currT = Timer - DownTime - UpTime;

                    var p = ExquisiteBurst.Spawn(GetTop(), dir * Main.rand.NextFloat(1, 2), (ExquisiteBurst.Scales)Math.Clamp((int)currT / 6, 0, (int)ExquisiteBurst.Scales.Big), 0, GetTop, dir * Main.rand.NextFloat(4, 8));

                    p.Rotation = rot;
                }
            }
        }

        public Vector2 GetTop()
            => Top - RotateVec2 * 14;

        public Vector2 GetStringPos()
        {
            Vector2 dir = RotateVec2.RotatedBy((CheckEffect() == SpriteEffects.None ? -1 : 1) * MathHelper.PiOver2);
            return Top - RotateVec2 * 28 + dir * 6;
        }

        #endregion

        protected override void OnSlash()
        {
            if (Projectile.extraUpdates != 4)
                Projectile.extraUpdates = 4;

            int timer = (int)Timer - minTime;

            bool preSwing = timer < maxTime * 0.3f;
            int timePer = Projectile.MaxUpdates;

            if (preSwing)
                SwingParticle(timer, timePer);

            if (Combo == 0)
            {
                if (!hitted && ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile chain))
                {
                    //检测和锥子的碰撞
                    if (Utils.CenteredRectangle(Top, new Vector2(100, 100)).Contains(chain.getRect()))
                    {
                        (chain.ModProjectile as ExquisiteAwl).BoostShoot();
                        onHitTimer = 1;
                        hitted = true;
                    }
                }
            }


            if (alpha < 255)
                alpha += 8;

            if (Item.type != ItemType<ExquisiteHammer>())
                Projectile.Kill();

            float angle = recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime));

            Projectile.scale = Helper.EllipticalEase(angle, 1.1f, 1.3f);

            base.OnSlash();
        }

        private void SwingParticle(int timer, int timePer)
        {
            if (Main.rand.NextBool(3) && timer % (timePer / 2) == 0)
            {
                flameGroup.NewParticle<ExquisiteParticle>(GetTop() + Main.rand.NextVector2Circular(20, 20), RotateVec2.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-3, 2), Color.White);
            }

            if (Main.rand.NextBool(2) && timer % (timePer / 2) == 0)
            {
                Color c = Main.rand.NextFromList(ExquisiteHammer.ShineIronColor, new Color(230, 48, 48)) * 0.65f;

                var p = flameGroup.NewParticle<ExquisiteFire>(GetTop()-RotateVec2*10 + Main.rand.NextVector2Circular(18, 18), RotateVec2.RotatedBy(Owner.direction * 1f) * Main.rand.NextFloat(1, 2), c, Main.rand.NextFloat(0.2f, 0.5f));

                p?.MaxFrameCount = 2;
            }
        }

        protected override void AfterSlash()
        {
            highlightColor *= 0.97f;
            if (alpha > 20)
                alpha -= 10;

            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);

            if (onHitTimer != 1 || !VisualEffectSystem.HitEffect_SpecialParticles)
                return;

            Owner.MinionAttackTargetNPC = target.whoAmI;
        }

        protected override float GetExRot()
        {
            int dir = Math.Sign(totalAngle);

            if (Timer < minTime && Combo == 0)
                dir = DirSign * ExDirection;

            float extraRot = DirSign < 0 ? MathHelper.Pi : 0;
            extraRot += DirSign == dir ? 0 : MathHelper.Pi;
            extraRot += spriteRotation * dir;

            return extraRot + exRot;
        }

        protected override SpriteEffects CheckEffect()
        {
            if (Timer < minTime && Combo == 0)
            {
                if (DirSign * ExDirection < 0)
                {
                    return SpriteEffects.FlipHorizontally;
                }
                return SpriteEffects.None;

            }
            if (DirSign * ExDirection < 0)
            {
                if (totalAngle > 0)
                    return SpriteEffects.None;
                return SpriteEffects.FlipHorizontally;
            }

            if (totalAngle > 0)
                return SpriteEffects.None;
            return SpriteEffects.FlipHorizontally;
        }

        #region 绘制

        public void DrawWarp()
        {
            if (oldRotate != null)
                WarpDrawer(0.75f);
        }

        protected override void DrawSlashTrail()
        {
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

                var c = new Color(255, 255, 255) * Helper.Lerp(alpha, 0, 1 - factor);
                bars.Add(new(Top.Vec3(), c, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), c, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("ExquisiteHammer");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
                    effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.01f);
                    effect.Parameters["udissolveS"].SetValue(0.8f);
                    effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Split2.Value);
                    effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
                    effect.Parameters["uGradient"].SetValue(GradientTexture.Value);
                    effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
                    effect.Parameters["uflowPercent"].SetValue(0.8f);

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

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Timer == 0)
                return;

            SpriteEffects effects = CheckEffect();
            Vector2 position = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation + extraRot;

            Main.spriteBatch.Draw(mainTex, position, null, lightColor, rotation, origin, Projectile.scale, effects, 0f);
            //Main.spriteBatch.Draw(HighlightTexture.Value, position, null, highlightColor * 1f, rotation, HighlightTexture.Size() / 2, Projectile.scale, effects, 0f);
            Main.spriteBatch.Draw(HighlightTexture.Value, position, null, highlightColor with { A = 0 }, rotation, HighlightTexture.Size() / 2, Projectile.scale, effects, 0f);

            DrawLine();
            flameGroup?.Draw(Main.spriteBatch);
        }

        public void DrawLine()
        {
            if (lineAlpha <= 0 || !ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile p))
                return;

            DrawLine_Inner(LineTexture.Value, p.Center - Main.screenPosition);
        }

        public void DrawLine_Inner(Texture2D lineTex, Vector2 endPos)
        {
            List<ColoredVertex> bars = new();

            float halfLineWidth = lineTex.Height / 2;
            Vector2 startPos = GetStringPos() - Main.screenPosition;

            Vector2 recordPos = startPos;
            float recordUV = 0;

            int lineLength = (int)(startPos - endPos).Length();   //链条长度
            int pointCount = lineLength / 16 + 3;
            Vector2 controlPos = lineMiddlePos - Main.screenPosition;

            //贝塞尔曲线
            for (int i = 0; i <= pointCount; i++)
            {
                float factor = (float)i / pointCount;

                Vector2 P1 = Vector2.Lerp(startPos, controlPos, factor);
                Vector2 P2 = Vector2.Lerp(controlPos, endPos, factor);

                Vector2 Center = Vector2.Lerp(P1, P2, factor);
                Vector2 p = Center + Main.screenPosition;
                var color = Lighting.GetColor((int)p.X / 16, (int)(p.Y / 16f), Color.White) * lineAlpha;

                if (factor < 0.1f)
                    color *= factor / 0.1f;
                if (factor > 0.8f)
                    color *= 1 - (factor - 0.8f) / 0.2f;

                Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                Vector2 Top = Center + normal * halfLineWidth;
                Vector2 Bottom = Center - normal * halfLineWidth;

                recordUV += (Center - recordPos).Length() / lineTex.Width;

                bars.Add(new(Top, color, new Vector3(recordUV, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(recordUV, 1, 1)));

                recordPos = Center;
            }

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = lineTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;

        }

        #endregion
    }

    public class ExquisiteAwlBuff : BaseAlchorthentBuff<ExquisiteAwl>
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteAwl : BaseAlchorthentMinion<ExquisiteAwlBuff>
    {
        public int TexType { get; set; }

        public ref float Recorder => ref Projectile.ai[1];
        public ref float Recorder2 => ref Projectile.ai[2];
        public ref float Recorder3 => ref Projectile.localAI[1];
        public ref float Recorder4 => ref Projectile.localAI[2];

        public int WingFrame = -1;
        public int WingFrameCounter;

        /// <summary>
        /// 为1时进行一个静止状态帧图更新，为2时进行飞行中帧图更新
        /// </summary>
        public int UpdateSeflFrame { get; private set; } = -1;

        /// <summary>
        /// 本体透明度
        /// </summary>
        public float alpha;
        /// <summary>
        /// 拖尾透明度
        /// </summary>
        public float eyeAlpha;
        public int waitTime;

        /// <summary>
        /// 翅膀帧图
        /// </summary>
        public static ATex ExquisiteWing { get; private set; }

        const int TeleportDistance = 2000;

        public const int TexTypes = 3;
        public const int IdleFrame = 14;
        public const int FlyFrame = IdleFrame + 15;

        public const int AwlFrameMax = FlyFrame + 1;
        public const int WingFrameMax = 14;

        public const float DrawOriginAdd = 10;

        public const int TrailCount = 16;

        private enum AIStates : byte
        {
            /// <summary> 刚召唤出来 </summary>
            OnSummon,
            /// <summary> 飞回玩家的过程 </summary>
            BackToOwner,
            /// <summary> 待机 </summary>
            Idle,
            /// <summary> 特殊待机 </summary>
            SpIdle,
            /// <summary> 特殊卡肉 </summary>
            SpecialWait,
            /// <summary> 弱化版戳刺攻击 </summary>
            SpikeAttackHeavy,
            /// <summary> 弱化版冲刺攻击 </summary>
            SpikeAttack,
            /// <summary> 将敌人钉在圈内 </summary>
            SpikeAttackSpecial,
            /// <summary> 撞到墙了 </summary>
            TileHit
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, TrailCount);
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 32;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool ShouldUpdatePosition()
        {
            return waitTime == 0;
        }

        #region AI

        public override void Initialize()
        {
            TexType = Main.rand.Next(0, 3);
        }

        public override void AIMoves()
        {
            Timer++;

            switch (State)
            {
                default:
                case (int)AIStates.OnSummon:
                    OnSummon();
                    break;
                case (int)AIStates.BackToOwner:
                    break;
                case (int)AIStates.Idle:
                    break;
                case (int)AIStates.SpIdle:
                    break;
                case (int)AIStates.SpecialWait:
                    break;
                case (int)AIStates.SpikeAttackHeavy:
                    break;
                case (int)AIStates.SpikeAttack:
                    break;
                case (int)AIStates.SpikeAttackSpecial:
                    break;
                case (int)AIStates.TileHit:
                    TileHit();
                    break;
            }

            UpdateWingFrame();
            UpdateSelfFrame();
        }

        private void UpdateWingFrame()
        {
            if (WingFrame < 0)
                return;

            if (++WingFrameCounter > 3 * Projectile.MaxUpdates)
            {
                WingFrameCounter = 0;
                WingFrame++;
                if (WingFrame > WingFrameMax)
                    WingFrame = -1;
            }
        }

        private void UpdateSelfFrame()
        {
            if (UpdateSeflFrame == 1)
            {
                int frame = Projectile.frame;
                Projectile.UpdateFrameNormally(3 * Projectile.MaxUpdates, IdleFrame + 1);
                if (Projectile.frame == 0 && frame != 0)
                    UpdateSeflFrame = -1;
            }
            else if (UpdateSeflFrame == 2)
            {
                int frame = Projectile.frame;
                Projectile.UpdateFrameNormally(3 * Projectile.MaxUpdates, FlyFrame + 1);
                if (Projectile.frame == 0 && frame != 0)
                    UpdateSeflFrame = -1;
            }
        }

        public void OnSummon()
        {
            Vector2 targetCenter = Owner.MountedCenter + new Vector2(Owner.direction * 60, 80);

            const float WaitTime = 22;
            const int UpTime = 35;

            Projectile.UpdateFrameNormally(3, IdleFrame + 1);

            if (Timer < WaitTime)
            {
                Projectile.Center = targetCenter;
                return;
            }

            if (alpha < 1)
            {
                alpha += 0.08f;
                if (alpha > 1)
                    alpha = 1;
            }

            if (Timer == WaitTime)
            {
                WingFrame = 0;
            }
            if (Timer < WaitTime + UpTime)//从地下向上飞出来
            {
                Projectile.spriteDirection = Owner.direction;
                //Projectile.rotation = MathHelper.PiOver2;

                float f = Helper.BezierEase((Timer - WaitTime) / UpTime);

                float rot;
                float length = 60 + 80 * MathF.Sin(f * MathHelper.Pi);
                if (Owner.direction > 0)
                {
                    rot = 1.2f - 1.5f * f;
                    Projectile.rotation = (Projectile.Center - Owner.MountedCenter).ToRotation() + MathHelper.PiOver2 * (1 - f);
                }
                else
                {
                    rot = MathHelper.PiOver2 + (MathHelper.PiOver2 - 1.2f) + 1.5f * f;
                    Projectile.rotation = (Projectile.Center - Owner.MountedCenter).ToRotation() - MathHelper.PiOver2 * (1 - f);
                }

                targetCenter = Owner.MountedCenter + rot.ToRotationVector2() * length;
                Projectile.Center = targetCenter;
            }
            else if (Timer == WaitTime + UpTime)//
            {
                Projectile.velocity = Vector2.Zero;
            }
            else
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    float currTime = Timer - WaitTime - UpTime;

                    targetCenter = Projectile.Center - Owner.MountedCenter;
                    targetCenter = Vector2.SmoothStep(targetCenter, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * 16 * 5, Helper.Clamp(currTime, 0, 15) / 15f * 0.5f);

                    Projectile.netUpdate = true;
                    Projectile.Center = Owner.MountedCenter + targetCenter;

                    if (Target.GetNPCOwner(out NPC target3))
                    {
                        //转向目标
                        Projectile.rotation = Projectile.rotation.AngleLerp((target3.Center - Projectile.Center).ToRotation(), 0.4f);

                        SpriteDirectionTo(target3.Center);
                    }
                    else
                    {
                        //转向鼠标位置
                        Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.4f);

                        Projectile.spriteDirection = Math.Sign(Main.MouseWorld.X - Owner.MountedCenter.X);

                        if (Timer % 3 == 0)
                            do
                            {
                                //第一次索敌，寻找距离鼠标最近的
                                if (Helper.TryFindClosestEnemy(Main.MouseWorld, 300, n => n.CanBeChasedBy() && Collision.CanHit(Owner, n), out NPC target))
                                {
                                    Target = target.whoAmI;
                                    break;
                                }

                                //第二次索敌，寻找距离玩家最近的
                                if (Helper.TryFindClosestEnemy(Owner.MountedCenter, 800, n => n.CanBeChasedBy() && Collision.CanHit(Owner, n), out NPC target2))
                                {
                                    Target = target2.whoAmI;
                                    break;
                                }
                            } while (false);
                    }
                }
            }
        }

        public void BackToOwner()
        {
            Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
            Vector2 aimPos = GetIdlePos(index, total);

            /*
             * 距离大于2000直接传送
             * 
             * 3折现运动后冲向目标位置，距离小于一定后直接返回
             */
            float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

            if (distanceToAimPos > TeleportDistance || Timer > 60 * 16)
            {
                Teleport(aimPos);
                SwitchState(AIStates.Idle);

                return;
            }

            switch (Recorder)
            {
                default:
                case 0://记录当前与目标点位的距离
                    Recorder2 = distanceToAimPos;
                    Recorder = 1;
                    break;
                case 1://3折线运动
                    break;
            }
        }

        public void Idle()
        {

        }

        public void SpIdle()
        {

        }

        public void TileHit()
        {
            if (Timer < 60)
            {
                float f = Timer / 60f;

                //减速并转两圈
                Projectile.velocity *= 0.93f;
                Projectile.rotation += 0.4f * (1 - Helper.SqrtEase(f));

                return;
            }

            //SwitchState()
        }

        public override Vector2 GetIdlePos(int selfIndex, int totalCount)
        {
            Vector2 center = Owner.MountedCenter + new Vector2(-Owner.direction * 20, -16);

            const float maxRot = MathHelper.PiOver4 + 0.4f;

            float f = (float)selfIndex / totalCount;

            float rot = f * maxRot;
            float dis = Helper.SqrtEase(f) * 40;

            return base.GetIdlePos(selfIndex, totalCount);
        }

        /// <summary>
        /// 被击打出去
        /// </summary>
        public void BoostShoot()
        {
            //生成粒子
            SwitchState(AIStates.SpikeAttackHeavy);

            Projectile.tileCollide = true;

            Projectile.velocity = Projectile.rotation.ToRotationVector2() * 15;
            Projectile.extraUpdates = 5;
            Projectile.InitOldPosCache(TrailCount, false);

            UpdateSeflFrame = 2;
            eyeAlpha = 1;

        }

        public void SpriteDirectionTo(Vector2 pos)
        {
            float dir = pos.X - Projectile.Center.X;
            if (Math.Abs(dir) > 8)
                Projectile.spriteDirection = Math.Sign(dir);
        }

        /// <summary>
        /// 传送到目标位置，生成炼金术符号
        /// </summary>
        /// <param name="teleportPos"></param>
        public void Teleport(Vector2 teleportPos)
        {
            Projectile.velocity *= 0;
            Projectile.Center = teleportPos;
            Recorder = 0;

            Projectile.InitOldPosCache(TrailCount, false);

            PRTLoader.NewParticle<AlchSymbolIron>(Main.MouseWorld, Vector2.Zero, ExquisiteHammer.ShineIronColor);

            SwitchState(AIStates.Idle);
        }

        #endregion

        #region 状态切换

        private void SwitchState(AIStates state)
        {
            State = (byte)(state);
            Timer = 0;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
        }


        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            switch (State)
            {
                default:
                    break;
                case (byte)AIStates.SpikeAttack:
                    break;
                case (byte)AIStates.SpikeAttackHeavy://强化刺击，额外伤害一次
                    target.SimpleStrikeNPC(Projectile.damage / 2, MathF.Sign(target.Center.X - Projectile.Center.X), false, 0, DamageClass.Summon);
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                default:
                    break;
                case (byte)AIStates.SpikeAttack:
                case (byte)AIStates.SpikeAttackHeavy:
                    SwitchState(AIStates.TileHit);
                    //反弹
                    Projectile.velocity = -oldVelocity.SafeNormalize(Vector2.Zero) * 5;
                    UpdateSeflFrame = 1;
                    break;
            }

            return false;
        }

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + (Projectile.spriteDirection > 0 ? 0 : MathHelper.Pi);
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            lightColor *= alpha;

            if (WingFrame >= 0)
                DrawWing(lightColor, pos, rot, effect);

            DrawSelf(lightColor, pos, rot, effect);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            DrawEyeRedLine();
        }

        /// <summary>
        /// 绘制翅膀
        /// </summary>
        /// <param name="lightColor"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="effect"></param>
        public void DrawWing(Color lightColor, Vector2 pos, float rot, SpriteEffects effect)
        {
            Texture2D tex = ExquisiteWing.Value;
            Rectangle frame = tex.Frame(1, WingFrameMax, 0, WingFrame);
            Vector2 origin = frame.Size() / 2;
            origin.X -= Projectile.spriteDirection * DrawOriginAdd;

            Main.EntitySpriteDraw(tex, pos, frame, lightColor, rot, origin, Projectile.scale, effect);
        }

        /// <summary>
        /// 绘制本体
        /// </summary>
        /// <param name="lightColor"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="effect"></param>
        public void DrawSelf(Color lightColor, Vector2 pos, float rot, SpriteEffects effect)
        {
            Texture2D tex = Projectile.GetTextureValue();
            Rectangle frame = tex.Frame(TexTypes, AwlFrameMax, TexType, Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            origin.X -= Projectile.spriteDirection * DrawOriginAdd;

            Main.EntitySpriteDraw(tex, pos, frame, lightColor, rot, origin, Projectile.scale, effect);
        }

        public void DrawEyeRedLine()
        {
            if (eyeAlpha == 0)
                return;

            Texture2D Texture = CoraliteAssets.Trail.CircleSPA.Value;

            List<ColoredVertex> bars = [];
            List<ColoredVertex> bars2 = [];

            Vector2 toCenter = Projectile.Size / 2;

            Color color = ExquisiteHammer.ShineIronColor* eyeAlpha;
            Color color2 = new Color(245, 233, 225) * 0.8f * eyeAlpha;

            for (int i = 0; i < TrailCount; i++)
            {
                float factor = (float)i / TrailCount;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition + toCenter;
                Vector2 normal;
                if (i > 0)
                    normal = (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                else
                    normal = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();

                Vector2 Top = Center + (normal * 5);
                Vector2 Bottom = Center - (normal * 5);
                Vector2 Top2 = Center + (normal * 1.25f);
                Vector2 Bottom2 = Center - (normal * 1.25f);

                Color c1 = color;
                Color c2 = color2;
                if (i > 0 && (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).LengthSquared() < 1)
                    c1 = c2 = Color.Transparent;

                bars.Add(new(Top, c1, new Vector3(1 - factor, 0, 1)));
                bars.Add(new(Bottom, c1, new Vector3(1 - factor, 1, 1)));
                bars2.Add(new(Top2, c2, new Vector3(1 - factor, 0, 1)));
                bars2.Add(new(Bottom2, c2, new Vector3(1 - factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
        }

        #endregion
    }

    public class ExquisiteHammerSparkle : Particle
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public ExquisiteHammerHeldProj owner;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
            Frame.Width = 1;
            Frame.Height = 7;
            Scale = 1.5f;
        }

        public override void AI()
        {
            if (owner != null)
            {
                Position = owner.Top - owner.RotateVec2 * 14;
            }

            if (Opacity % 3 == 0)
            {
                Frame.Y++;
                if (Frame.Y > 6)
                {
                    active = false;
                }
            }

            Opacity++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            TexValue.QuickCenteredDraw(spriteBatch, Frame, Position - Main.screenPosition, Color.White, 0, Scale);
            return false;
        }
    }

    public class ExquisiteHammerSparkle2 : Particle
    {
        public override string Texture => AssetDirectory.Sparkles + "ShotLineSPA";

        public Color addColor = Color.Transparent;
        public Vector2 scale1;
        public Vector2 scale2;

        public ExquisiteHammerHeldProj owner;

        public override void AI()
        {
            Opacity++;

            const int ShinyTime = 7;
            const int ShinyTime2 = 8;
            const int FadeTime = 20;

            Position = owner.GetTop();


            if (Opacity < ShinyTime)
            {
                float f = Helper.X2Ease(Opacity / ShinyTime);
                Color = Color.Lerp(new Color(255, 211, 132), new Color(244, 231, 222), f);
                addColor = Color.Lerp(Color.Transparent, Color.White, f);

                scale1 = Vector2.SmoothStep(Vector2.Zero, new Vector2(0.8f, 0.7f), f);
                scale2 = Vector2.SmoothStep(Vector2.Zero, new Vector2(0.4f, 0.5f), f);
            }
            else if (Opacity < ShinyTime + ShinyTime2)
            {
                float f = Helper.BezierEase((Opacity - ShinyTime) / ShinyTime2);
                addColor = Color.Lerp(Color.White, new Color(255, 211, 132), f);
                Color = Color.Lerp(new Color(244, 231, 222), new Color(246, 71, 99), f);
                scale1 = Vector2.SmoothStep(new Vector2(0.8f, 0.7f), new Vector2(0.4f, 0.5f), f);
                scale2 = Vector2.SmoothStep(new Vector2(0.4f, 0.5f), new Vector2(0.7f, 0.7f), f);
            }
            else if (Opacity < ShinyTime + ShinyTime2 + FadeTime)
            {
                float f = (Opacity - ShinyTime - ShinyTime2) / FadeTime;
                addColor = Color.Lerp(new Color(255, 211, 132), Color.Transparent, f);
                Color = Color.Lerp(new Color(246, 71, 99), Color.Transparent, f);

                scale1 = Vector2.SmoothStep(new Vector2(0.4f, 0.5f), Vector2.Zero, f);
                scale2 = Vector2.SmoothStep(new Vector2(0.7f, 0.7f), Vector2.Zero, f);
            }
            else
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            Vector2 pos = Position - Main.screenPosition;

            const float r1 = -1.1f;
            const float r2 = r1 + MathHelper.PiOver2;

            //绘制闪闪1层
            TexValue.QuickCenteredDraw(spriteBatch, pos, scale1, Color, r1);
            TexValue.QuickCenteredDraw(spriteBatch, pos, scale2, Color, r2);

            //绘制闪闪2层
            Color c = addColor;
            addColor.A = 0;
            TexValue.QuickCenteredDraw(spriteBatch, pos, scale1 * 0.4f, c, r1);
            TexValue.QuickCenteredDraw(spriteBatch, pos, scale2 * 0.4f, c, r2);

            return false;
        }
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteBurst : Particle
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public static ATex ExquisiteBurst_Smallest { get; set; }
        public static ATex ExquisiteBurst_SuperSmall { get; set; }
        public static ATex ExquisiteBurst_Middle { get; set; }
        public static ATex ExquisiteBurst_Big { get; set; }

        public Scales texScale;

        public Func<Vector2> GetPos;
        public Vector2 offset;

        public int frameCounterMax;
        public int frameXCount;

        public enum Scales
        {
            Smallest,
            SuperSmall,
            Small,
            Middle,
            Big
        }

        public override void SetProperty()
        {
            Frame = new Rectangle(0, 0, 0, 0);
            PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        }

        public override void AI()
        {
            if (GetPos != null)
            {
                Position = GetPos() + offset;
                offset += Velocity;
                Velocity *= 0.94f;
            }

            if (++Opacity > frameCounterMax)
            {
                Opacity = 0;
                if (++Frame.X >= frameXCount)
                    active = false;
            }
        }

        public virtual void Follow(Projectile proj)
        {
            Position += (proj.position - proj.oldPosition);
        }

        public static ExquisiteBurst Spawn(Vector2 pos, Vector2 vel, Scales texScale, int frameCounterMax, Func<Vector2> GetPos = null, Vector2? offset = null)
        {
            if (VaultUtils.isServer)
            {
                return null;
            }

            var p = PRTLoader.NewParticle<ExquisiteBurst>(pos, vel, Color.White);
            p.texScale = texScale;
            p.frameCounterMax = frameCounterMax;
            p.GetPos = GetPos;
            if (offset != null)
                p.offset = offset.Value;
            p.SetFrameXMax();

            return p;
        }

        public void SetFrameXMax()
        {
            frameXCount = 13;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = texScale switch
            {
                Scales.Smallest => ExquisiteBurst_Smallest.Value,
                Scales.SuperSmall => ExquisiteBurst_SuperSmall.Value,
                Scales.Small => TexValue,
                Scales.Middle => ExquisiteBurst_Middle.Value,
                Scales.Big => ExquisiteBurst_Big.Value,
                _ => TexValue,
            };

            var frameBox = tex.Frame(frameXCount, 1, Frame.X, 0);

            spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
                , Color, Rotation + MathHelper.PiOver2, new Vector2(frameBox.Width / 2, frameBox.Height * 0.8f), Scale, 0, 0);

            return false;
        }
    }

    public class ExquisiteParticle() : BaseFrameParticle(3, 12, 1, randRot: true)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;
    }

    public class ExquisiteFire() : FireParticleSPA
    {
        public override string Texture => AssetDirectory.Particles + "FireParticleSPA";

        public override void AI()
        {
            Opacity++;

            if (Opacity % MaxFrameCount == 0)
            {
                Frame.Y++;
                if (Frame.Y > 15)
                    active = false;
            }

            Velocity *= 0.95f;
            if (Opacity > 8)
                Color = Color.Lerp(Color, new Color(2, 13, 50) * 0.5f, 0.12f);
        }
    }

    public class AlchSymbolIron : BaseAlchSymbol
    {
        public override LineDrawer GetSymbolLine() => ExquisiteHammer.NewIronAlchSymbol();

        public override bool FadeLineOffset => false;
        public override bool FadeScale => false;
        public override bool FadeColor => true;

        public override void SetProperty()
        {
            base.SetProperty();
            maxScale = 18;
            fadeTime = 20;
            ShineTime = 14;
            disappearTime = 20;
        }
    }
}
