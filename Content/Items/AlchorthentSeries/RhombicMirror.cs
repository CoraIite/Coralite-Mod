using Coralite.Content.Particles;
using Coralite.Content.Prefixes.FairyWeaponPrefixes;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class RhombicMirror : BaseAlchorthentItem
    {
        public static Color ShineCorruptionColor = new Color(180, 120, 220);
        public static Color CopperGreen = new Color(70, 90, 100);

        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<RhombicMirrorProj>();

            Item.SetWeaponValues(24, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.useTurn = false;
        }

        public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Projectile.NewProjectile(source, player.Center + new Vector2(player.direction * 20, 0), new Vector2(player.direction * 4, -8), type, damage, knockback, player.whoAmI, 1);

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<RhombicMirrorHeldProj>(), damage, knockback, player.whoAmI, 0);

            //player.AddBuff(ModContent.BuffType<FaintEagleBuff>(), 60);

            Helper.PlayPitched(CoraliteSoundID.Swing_Item1, player.Center);
            //Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_DD2_BetsyFireballImpact, player.Center, pitchAdjust: 0.4f);
        }

        public override void MinionAim(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            PRTLoader.NewParticle<TestAlchSymbol>(Main.MouseWorld, Vector2.Zero, RhombicMirror.ShineCorruptionColor);

            //Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 0);
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.CheckMana(30, true, true))
                return;

            player.manaRegenDelay = 40;

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<CorruptionMirror>(), damage, knockback, player.whoAmI);
        }

        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient(ItemID.CopperBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.VilePowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
            //CreateRecipe()
            //    .AddIngredient(ItemID.CopperBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.ViciousPowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();

            //CreateRecipe()
            //    .AddIngredient(ItemID.TinBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.VilePowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
            //CreateRecipe()
            //    .AddIngredient(ItemID.TinBar, 12)
            //    .AddIngredient<MagicalPowder>(3)
            //    .AddIngredient(ItemID.ViciousPowder, 12)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
        }

        public static LineDrawer NewCorruptAlchSymbol()
        {
            Vector2 left = new Vector2(-0.9f, -0.8f);
            Vector2 right = new Vector2(0.8f, -1);

            return new LineDrawer([
                 new LineDrawer.StraightLine(new Vector2(0,-0.9f),new Vector2(0, 1),AlchorthentAssets.OneSideBigLine,1.4f),
                 new LineDrawer.WarpLine(left,30
                    ,f => Helper.TwoHandleBezierEase(f,left,right,new Vector2(-0.7f,0.7f), new Vector2(1,-0.1f))),
                 new LineDrawer.StraightLine(new Vector2(-1.2f, -0.6f), new Vector2(-0.6f, -0.8f),linwWidthScale:0.7f),
                 //对号的两个箭头
                 new LineDrawer.StraightLine(new Vector2(0.5f, -0.7f), new Vector2(0.9f, -1),linwWidthScale:0.7f),
                 new LineDrawer.StraightLine(new Vector2(1f, -0.5f), new Vector2(0.9f, -1),linwWidthScale:0.7f),
                 ]);
        }
    }

    public class RhombicMirrorBuff : BaseAlchorthentBuff<RhombicMirrorProj>
    {
        public override string Texture => AssetDirectory.MinionBuffs + "DefaultAlchorthentSeries";
    }

    /// <summary>
    /// 召唤和右键时出现的手持弹幕<br></br>
    /// ai0传入类型，0表示召唤，1表示锁定
    /// </summary>
    public class RhombicMirrorHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + "RhombicMirror";

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
            Projectile.hide = true;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ModContent.ItemType<RhombicMirror>())
            {
                Projectile.Kill();
                return;
            }

            SetHeld();

            if (State == 0)
                Summon();
            else
                AimTarget();
        }

        /// <summary>
        /// 光效展开后召唤物在人物背后缓慢旋转出现
        /// </summary>
        public void Summon()
        {
            Projectile.Center = Owner.Center + new Vector2(Owner.direction * 16.5f, -4 + Owner.gfxOffY);
            Owner.itemTime = Owner.itemAnimation = 2;

            if (Timer == 0)//生成光效
            {
                var p = PRTLoader.NewParticle<RhombicMirrorSummonParticle>(Projectile.Center, Vector2.Zero);
                p.OwnerProjIndex = Projectile.whoAmI;
            }

            if (Timer > 45)
            {
                Projectile.Kill();
            }

            Timer++;
        }

        public void AimTarget()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 0, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }

    /// <summary>
    /// 菱花镜召唤物，ai0控制是否强化形态
    /// </summary>
    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class RhombicMirrorProj : BaseAlchorthentMinion<RhombicMirrorBuff>
    {
        /*
         * 神秘身体部分贴图
         * 1~17帧为伴随着攻击变得生锈，18~37帧为特殊攻击清除生锈
         */
        /// <summary> 下面的头，上面的尾巴，右上后腿，左上后腿 </summary>
        public static ATex RhombicMirrorProjPart1 { get; set; }
        /// <summary> 右下前腿，左下前腿 </summary>
        public static ATex RhombicMirrorProjPart2 { get; set; }

        /// <summary>
        /// 攻击次数
        /// </summary>
        public short attackCount;
        public float xScale = 1;
        /// <summary> 控制身体部件距离中心点的长度 </summary>
        public float bodyPartLength = 0;
        /// <summary> 是否绘制身体部件 </summary>
        public bool canDrawBodyPart = false;

        const int totalFrameY = 37;

        private enum AIStates : byte
        {
            /// <summary> 刚召唤出来 </summary>
            OnSummon,
            /// <summary> 飞回玩家的过程 </summary>
            BackToOwner,
            /// <summary> 在玩家身边 </summary>
            Idle,
            /// <summary> 特殊待机动作1 </summary>
            IdleMove1,
            /// <summary> 特殊待机动作2 </summary>
            IdleMove2,
            /// <summary> 射光束 </summary>
            Shoot,
            /// <summary> 经过一定攻击后变得腐化 </summary>
            Corrupt,
            /// <summary> 腐蚀光束 </summary>
            CorruptedShoot,
        }

        public override void SetOtherDefault()
        {
            Projectile.tileCollide = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.width = Projectile.height = 46;
            Projectile.decidesManualFallThrough = true;
            Projectile.localNPCHitCooldown = 10;
        }

        #region AI

        public override void Initialize()
        {

        }

        public override void AIMoves()
        {

        }

        public void OnSummon()
        {

        }

        #endregion

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            if (canDrawBodyPart)
                DrawBodyuParts(lightColor);

            return false;
        }

        public void DrawBodyuParts(Color lightcolor)
        {

        }

        /// <summary>
        /// 绘制一层
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="xFrame"></param>
        /// <param name="totalXFrame"></param>
        /// <param name="posOffset"></param>
        /// <param name="color"></param>
        public void DrawLayer(Texture2D tex, int xFrame, int totalXFrame,Vector2 posOffset,Color color)
        {
            var frameBox = tex.Frame(totalXFrame, totalXFrame, xFrame, Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + posOffset,frameBox, color,Projectile.rotation, frameBox.Size()/2,new Vector2(xScale,1)*Projectile.scale,0,0);
        }

        #endregion
    }

    public class CorruptionMirror : ModProjectile
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Recorder => ref Projectile.ai[2];
        public ref float Recorder2 => ref Projectile.localAI[0];
        public int HitCount;
        public Player Owner => Main.player[Projectile.owner];

        private LineDrawer CorruptionEffect;

        const int channelTime = 40;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            this.LoadGore(3);
        }

        public override bool? CanDamage()
        {
            if (State == 1)
                return null;

            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return Recorder == 0;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            /*
             * 先在头上蓄力，生成动画和腐化的符号
             * 然后丢出，命中后产生切割音效
             */
            switch (State)
            {
                default:
                case 0:
                    Channel();
                    UpdateCorruptionEffect();
                    break;
                case 1://飞出
                    {
                        Shoot();
                        if (Recorder > 0)
                            Recorder--;
                    }
                    break;
                case 2:
                    {
                        Projectile.Kill();
                    }
                    break;
            }
        }

        private void Channel()
        {
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = -MathHelper.PiOver2 + (Owner.direction > 0 ? 0 : MathHelper.Pi);
            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;

            //一开始举起在玩家头上，之后来到中心点，再之后伸到身前
            Vector2 exOffset = new Vector2(0, -45);

            if (Timer < channelTime)
            {
                if (Timer == 0)
                    Helper.PlayPitched("AlchSeries/FaintEagleExplosion", 0.01f, -0.2f, Projectile.Center);
            }
            else if (Timer == channelTime)
            {
                Helper.PlayPitched("AlchSeries/CorruptionMirrorChargeComplete", 0.08f, 1, Projectile.Center);
            }
            else if (Timer < channelTime + 16)
            {
                float f = (Timer - channelTime) / 16;
                exOffset = new Vector2(0, -45 + 35 * Helper.HeavyEase(f));
            }
            else
            {
                float f = (Timer - channelTime - 16) / 6;
                Owner.itemRotation = (-MathHelper.PiOver2).AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), f) + (Owner.direction > 0 ? 0 : MathHelper.Pi);

                if (Projectile.IsOwnedByLocalPlayer())
                    exOffset = Vector2.Lerp(new Vector2(0, -10), (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 24, Helper.HeavyEase(f));
                Projectile.netUpdate = true;
            }

            Projectile.Center = Owner.Center + new Vector2(0, Owner.gfxOffY) + exOffset;

            Projectile.rotation = Helper.Lerp(MathHelper.TwoPi, 0, Helper.BezierEase(Timer / 60));

            Timer++;
            if (Projectile.frame < 19)
                Projectile.UpdateFrameNormally(2, 20);

            if (Timer > channelTime + 16 + 6)
            {
                State = 1;
                Timer = 0;
                Projectile.hide = false;
                Projectile.tileCollide = true;

                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 8;
                    Projectile.extraUpdates = 1;
                }
            }
        }

        public void Shoot()
        {
            Timer++;
            if (Recorder == 0)
            {
                Projectile.rotation -= MathF.Sign(Projectile.velocity.X) * Projectile.velocity.LengthSquared() / 35;
            }
        }

        public void SwitchToBreak()
        {
            State = 2;
            Timer = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Recorder == 0)
                Recorder = 5 * Projectile.MaxUpdates;

            HitVisualEffect(target);
            HitCount++;
        }

        public void HitVisualEffect(NPC target)
        {
            Helper.PlayPitched("Misc/BloodySlash2", 0.03f, -0.6f, Projectile.Center);

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                //菱形粒子
                var p2 = PRTLoader.NewParticle<MagikeLozengeParticleSPA>(Projectile.Center, Vector2.Zero, RhombicMirror.ShineCorruptionColor, 0.4f);

                float normalRot = (target.Center - Projectile.Center).ToRotation() ;
                p2.Rotation = normalRot + Main.rand.NextFloat(-0.2f, 0.2f);
                p2.XScale = 0.9f;

                normalRot += MathHelper.PiOver2;
                //两侧亮线
                Vector2 dir = normalRot.ToRotationVector2();
                for (int i = -3; i < 3; i++)
                {
                    PRTLoader.NewParticle<SpeedLine>(Projectile.Center, (i < 0 ? -1 : 1) * dir.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(2, 6), Main.rand.NextFromList(RhombicMirror.CopperGreen, RhombicMirror.ShineCorruptionColor), Scale: Main.rand.NextFloat(0.2f, 0.3f));
                }
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 8; i++)
                {
                    Vector2 dir2 = Helper.NextVec2Dir();
                    PRTLoader.NewParticle<CorruptionMirrorParticle>(Projectile.Center + dir2 * Main.rand.NextFloat(12, 20), dir2 * Main.rand.NextFloat(0.3f, 1.4f), Color.White);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (MathF.Abs(Projectile.velocity.X) < MathF.Abs(oldVelocity.X))
                Projectile.velocity.X = -oldVelocity.X;
            if (MathF.Abs(Projectile.velocity.Y) < MathF.Abs(oldVelocity.Y))
                Projectile.velocity.Y = -oldVelocity.Y;

            Recorder2++;
            if (Recorder2 > 6)
                SwitchToBreak();

            return false;
        }

        public void UpdateCorruptionEffect()
        {
            if (CorruptionEffect == null)
            {
                CorruptionEffect = RhombicMirror.NewCorruptAlchSymbol();
                CorruptionEffect.SetLineWidth(20);
            }

            if (Timer > channelTime)
                return;

            if (Timer < channelTime / 3)
            {
                float factor = Timer / (channelTime / 3);
                CorruptionEffect.SetScale(35 * factor);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 0 && CorruptionEffect != null)
                DrawCorruptEffect();

            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 20), lightColor, 0);

            return false;
        }

        private void DrawCorruptEffect()
        {
            float factor=1;
            Color c=Color.Transparent;

            if (Timer < channelTime / 3)
            {
                factor = Timer / (channelTime / 3);
                c = Color.Lerp(Color.Transparent, RhombicMirror.CopperGreen, factor);
            }
            else if (Timer < channelTime * 2 / 3)
            {
                factor = (Timer - channelTime / 3) / (channelTime / 3);
                c = Color.Lerp(RhombicMirror.CopperGreen, RhombicMirror.ShineCorruptionColor, factor);
            }
            else if (Timer < channelTime)
            {
                factor = (Timer - channelTime * 2 / 3) / (channelTime / 3);
                c = Color.Lerp(RhombicMirror.ShineCorruptionColor, Color.Transparent, factor);
            }

            float f = 1;
            if (Timer< channelTime / 2)
                f = Helper.BezierEase(Timer / (channelTime / 2));

            RhombicMirrorProj.DrawLine(shader =>
                {
                    shader.CurrentTechnique.Passes["MyNamePass"].Apply();
                    CorruptionEffect.Draw(Projectile.Center);
                }, CoraliteAssets.Laser.TwistLaser.Value
                   , (int)Main.timeForVisualEffects * 0.02f, 4, f, c, 0.2f, 0.5f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class RhombicMirrorSummonParticle : RhombicMirrorLaserParticle
    {
        public override void SetProperty()
        {
            base.SetProperty();
            LaserWidth = 8;
            LaserAngleOffset = 0.4f;
            LaserLength = 30;
        }

        public override void AI()
        {
            if (!OwnerProjIndex.GetProjectileOwner(out Projectile owner))
            {
                active = false;
                return;
            }

            Player p = Main.player[owner.owner];

            Position = owner.Center;
            Rotation = -MathHelper.PiOver2 + (p.direction > 0 ? -1 : 1) * 0.9f;

            Opacity++;
            if (Opacity < 15)
            {
                float f = Helper.HeavyEase(Opacity / 15);
                LaserAngleOffset = Helper.Lerp(0.4f, -0.4f, f);
                LaserLength = Helper.Lerp(30, 90, f);
                Color = Color.Lerp(Color.Transparent, new Color(180, 120, 220), f);
            }
            else if (Opacity < 45)
            {
                float f = Helper.X2Ease((Opacity - 15) / 30);
                LaserLength = Helper.Lerp(90, 60, f);
                Color = Color.Lerp(new Color(180, 120, 220), Color.Transparent, f);
            }
            else
            {
                active = false;
            }
        }
    }

    public class CorruptionMirrorParticle() : BaseFrameParticle(5, 8, 2, randRot: true)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;
    }

    public class CorruptionMirrorRotParticle() : BaseFrameParticle(1, 8, 1, randRot: true)
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public override void Follow(Projectile proj)
        {
            Position = proj.Center;
        }

        public override Color GetColor()
        {
            return Color;
        }
    }

    /// <summary>
    /// 可以变成扇形的激光粒子
    /// </summary>
    public abstract class RhombicMirrorLaserParticle : Particle
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems+ "EdgeSPA2";

        public int OwnerProjIndex;

        /// <summary>
        /// 激光长度
        /// </summary>
        public float LaserLength;
        /// <summary>
        /// 激光的扇形张角
        /// </summary>
        public float LaserAngleOffset;
        /// <summary>
        /// 激光宽度，建议不变
        /// </summary>
        public float LaserWidth;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public virtual Color GetColor(float f)
        {
            return Color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            CoraliteSystem.InitBars();

            Texture2D Texture = TexValue;

            Vector2 pos = Position - Main.screenPosition;
            Vector2 normal = (Rotation + MathHelper.PiOver2).ToRotationVector2();
            pos -= normal * LaserWidth;
            Vector2 dir = Rotation.ToRotationVector2();

            for (int i = 0; i <= 24; i++)
            {
                float factor = (float)i / 24;

                Vector2 Top = pos + normal * LaserWidth * 2f * factor;
                Vector2 Bottom = Top + dir.RotatedBy(Helper.Lerp(LaserAngleOffset, -LaserAngleOffset, factor)) * LaserLength;
                CoraliteSystem.Vertexes.Add(new(Top, Color, new Vector3(1, factor, 0)));
                CoraliteSystem.Vertexes.Add(new(Bottom, Color, new Vector3(0, factor, 0)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            var arr = CoraliteSystem.Vertexes.ToArray();
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);

            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            return false;
        }
    }
}
