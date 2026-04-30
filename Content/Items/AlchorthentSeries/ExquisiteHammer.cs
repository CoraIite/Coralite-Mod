using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
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

        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ProjectileType<ExquisiteHammerHeldProj>();

            Item.SetWeaponValues(30, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 2));
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<ExquisiteHammerHeldProj>(), damage * 2, knockback * 1.5f, player.whoAmI);

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
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteHammerHeldProj() : BaseSwingProj(1, 40)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + nameof(ExquisiteHammer);

        public ref float Combo => ref Projectile.ai[0];
        public ref float ChainedProjIndex => ref Projectile.ai[1];

        public Particle chainParticle;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public int delay;
        public int alpha;

        [VaultLoaden("{@classPath}" + "ExquisiteHammerGradient")]
        public static ATex GradientTexture { get; set; }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = 40;
            Projectile.height = 95;
            trailTopWidth = 0;
            distanceToOwner = 6;
            onHitFreeze = 8;
            Projectile.hide = true;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 50 * Projectile.scale;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 3;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0://召唤，连线
                    startAngle = 1.7f;
                    totalAngle = 4f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.2f);
                    break;
                case 1:
                    startAngle = 2.4f;
                    totalAngle = 3.5f;
                    maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 18;
                    extraScaleAngle = 0.3f;
                    ExtraInit();
                    Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 0.9f, 1.4f);

                    break;
            }

            base.InitializeSwing();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.5f, 0.25f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            base.BeforeSlash();
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            bool preSwing = timer < maxTime * 0.4f;
            int timePer = Projectile.MaxUpdates;
            if (preSwing)
                timePer = 2;

            //if (Main.rand.NextBool(3) || timer % timePer == 0)
            //{
            //    Dust d = Dust.NewDustPerfect(Projectile.Center + (RotateVec2 * Projectile.height * 0.4f) + Main.rand.NextVector2Circular(12, 12)
            //        , DustID.Clentaminator_Cyan, RotateVec2.RotatedBy(1.57f) * Main.rand.NextFloat(1, 2f)
            //        , Scale: Main.rand.NextFloat(0.5f, 1f));

            //    d.noGravity = true;
            //}

            if (alpha < 255)
                alpha += 8;

            if (Item.type == ItemType<ExquisiteHammer>())
                scale = Owner.GetAdjustedItemScale(Item);
            else
                Projectile.Kill();

            float angle = recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime));

            Projectile.scale = scale * Helper.EllipticalEase(angle, 0.9f, 1.4f);

            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            if (DownRight)
            {
                Projectile.Kill();
                Owner.itemAnimation = Owner.itemTime = 0;
                return;
            }
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

                var c = new Color(255, 255, 255, Helper.Lerp(alpha, 0, 1 - factor));
                bars.Add(new(Top.Vec3(), c, new Vector2(factor, 0)));
                bars.Add(new(Bottom.Vec3(), c, new Vector2(factor, 1)));
            }

            if (bars.Count > 2)
            {
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("NoHLGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.Split.Value);
                    effect.Parameters["gradientTexture"].SetValue(GradientTexture.Value);

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

        #endregion
    }

    public class ExquisiteAwlBuff : BaseAlchorthentBuff<ExquisiteAwl>
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";
    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class ExquisiteAwl : BaseAlchorthentMinion<FaintEagleBuff>
    {
        public int TexType { get; set; }

        public ref float Recorder => ref Projectile.ai[1];
        public ref float Recorder2 => ref Projectile.ai[2];
        public ref float Recorder3 => ref Projectile.localAI[1];
        public ref float Recorder4 => ref Projectile.localAI[2];

        public int WingFrame=-1;
        public int WingFrameCounter;

        /// <summary>
        /// 翅膀帧图
        /// </summary>
        public static ATex ExquisiteWing { get; private set; }

        public const int TexTypes = 14;
        public const int IdleFrame = 14;
        public const int FlyFrame = IdleFrame + 15;

        public const int AwlFrameMax = FlyFrame + 1;
        public const int WimgFrameMax = 14;

        public const float DrawOriginAdd = 10;

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
            /// <summary> 戳刺攻击 </summary>
            SpikeAttackHeavy,
            /// <summary> 强化版冲刺攻击 </summary>
            SpikeAttackWeak,
            /// <summary> 撞碎后重组自身 </summary>
            SpikeAttackSpecial
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 32;
            Projectile.localNPCHitCooldown = 20;
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
                    break;
                case (int)AIStates.BackToOwner:
                    break;
                case (int)AIStates.Idle:
                    break;
                case (int)AIStates.SpIdle:
                    break;
            }
        }

        public void OnSummon()
        {

        }

        public void BackToOwner()
        {

        }

        public void Idle()
        {

        }

        public void SpIdle()
        {

        }

        /// <summary>
        /// 被击打出去
        /// </summary>
        /// <param name="velocity"></param>
        public void BoostShoot(Vector2 velocity)
        {
            //生成粒子

            Projectile.velocity = velocity;
        }

        #endregion

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + (Projectile.spriteDirection > 0 ? 0 : MathHelper.Pi);
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (WingFrame>=0)
                DrawWing(lightColor, pos, rot, effect);

            DrawSelf(lightColor, pos, rot, effect);



            return false;
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
            Rectangle frame = tex.Frame(1, WimgFrameMax, 0, WingFrame);
            Vector2 origin = frame.Size() / 2;
            origin.X += DrawOriginAdd;

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
            Texture2D tex = ExquisiteWing.Value;
            Rectangle frame = tex.Frame(TexTypes, AwlFrameMax, TexType, Projectile.frame);
            Vector2 origin = frame.Size() / 2;
            origin.X += DrawOriginAdd;

            Main.EntitySpriteDraw(tex, pos, frame, lightColor, rot, origin, Projectile.scale, effect);
        }

        public void DrawEyeRedLine()
        {

        }

        #endregion
    }
}
