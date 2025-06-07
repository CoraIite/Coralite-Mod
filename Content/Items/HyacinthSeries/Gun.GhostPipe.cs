using Coralite.Content.Dusts;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class GhostPipe : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        private int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(24, 2);
            Item.DefaultToRangedWeapon(ProjectileType<QueenOfNightSpilitProj>(), AmmoID.Bullet, 13, 11.5f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.Gun_Item11;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -6);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, velocity, ProjectileType<GhostPipeHeldProj>(), 0, knockback, player.whoAmI);

            shootCount++;
            if (shootCount > 3)
                shootCount = 0;
            else
            {
                Vector2 dir = -velocity.SafeNormalize(Vector2.Zero);
                Dust.NewDustPerfect(player.Center, DustType<GhostPipePetal>()
                    , dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(1, 3)
                    , Scale: Main.rand.NextFloat(0.8f, 1.2f));
                type = ProjectileType<GhostPipeBullet>();
            }

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlintlockPistol)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.CursedFlame, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class GhostPipeHeldProj : BaseGunHeldProj
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public GhostPipeHeldProj() : base(0.1f, 24, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex GhostPipeFire { get; private set; }
        public static ATex GhostPipeChain { get; private set; }

        protected override float HeldPositionY => -2;

        private int FrameX;

        public override void InitializeGun()
        {
            FrameX = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, Color.Lime.ToVector3() / 3);
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 3 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Vector2 chainPos = Projectile.Center
                - rot.ToRotationVector2() * 34
                 - Main.screenPosition;

            Texture2D chain = GhostPipeChain.Value;
            Main.spriteBatch.Draw(chain, chainPos, null, lightColor
                , Owner.velocity.X / 15, new Vector2(chain.Width / 2, 0), Projectile.scale, 0, 0f);

            base.PreDraw(ref lightColor);

            if (Projectile.frame > 2)
                return false;

            Texture2D effect = GhostPipeFire.Value;
            Rectangle frameBox = effect.Frame(4, 3, FrameX, Projectile.frame);
            //SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 32 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class GhostPipeBullet : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex GhostPipeGradient { get; private set; }

        public Trail trail;
        private bool init = true;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public int trailCount = 8;
        public int trailWidth = 10;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates * 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 5;
        }

        public override void AI()
        {
            Initialize();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;

            UpdateOldPos();
            SpawnDust();
        }

        public void Initialize()
        {
            if (init)
            {
                init = false;

                if (!VaultUtils.isServer)
                {
                    Projectile.InitOldPosCache(trailCount);
                    trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new EmptyMeshGenerator()
                        , f => trailWidth, f => new Color(255, 255, 255, 170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                }
            }
        }

        private void UpdateOldPos()
        {
            if (Timer % 2 == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Projectile.UpdateOldPosCache();

                    Vector2[] pos2 = new Vector2[trailCount + 4];

                    //延长一下拖尾数组，因为使用的贴图比较特别
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                        pos2[i] = Projectile.oldPos[i] + Projectile.velocity;

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    int exLength = 4;

                    for (int i = 1; i < 5; i++)
                        pos2[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;

                    trail.TrailPositions = pos2;
                }
            }
        }

        private void SpawnDust()
        {
            if (Main.rand.NextBool(2))
            {
                int width = 8;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), DustID.CursedTorch
                    , Projectile.velocity * Main.rand.NextFloat(-0.4f, 0.8f), 75, Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        public static Color RandomColor()
        {
            return Main.rand.Next(3) switch
            {
                0 => new Color(190, 170, 251),
                1 => new Color(134, 229, 251),
                _ => new Color(125, 190, 255)
            };
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile p = Main.projectile.FirstOrDefault(p => p.active && p.friendly
                && (int)p.ai[2] == target.whoAmI && p.type == ProjectileType<GhostPipeParasitic>() && p.ai[0] < 6, null);

            if (p == null)//没有寄生弹幕，生成一个
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, target.Center, 0.25f);

                //pos += (pos - target.Center).SafeNormalize(Vector2.Zero) * 32;
                Vector2 dir = pos - target.Center;
                dir = dir.RotateByRandom(-MathHelper.Pi, MathHelper.Pi);


                Projectile.NewProjectileFromThis<GhostPipeParasitic>(pos, dir, Projectile.damage, Projectile.knockBack, ai2: target.whoAmI);
            }
            else//有弹幕，加能量
            {
                (p.ModProjectile as GhostPipeParasitic).GetEnergy();
            }
        }

        public override void OnKill(int timeLeft)
        {
            Vector2 dir = -Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, dir.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 5)
                    , Scale: Main.rand.NextFloat(1, 1.5f));

                d.noGravity = true;
            }
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(GhostPipeGradient.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceArrow.TurbulenceFlow.Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            trail?.DrawTrail(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    /// <summary>
    /// 使用ai0判断能量
    /// </summary>
    public class GhostPipeParasitic : ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public ref float Energy => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];
        public ref float Alpha => ref Projectile.localAI[1];
        public ref float Scale => ref Projectile.localAI[2];

        private bool init = false;
        private Vector2 Offset => Projectile.velocity;
        private bool canDamage;
        private float exRot;
        private float exAlpha;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 50;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
        {
            if (canDamage)
                return null;

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI == (int)Target)
                return base.CanHitNPC(target);

            return false;
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC npc, Projectile.Kill))
                return;

            if (init)
                Initialize();

            Projectile.Center = npc.Center + Offset;
            Projectile.rotation = Offset.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3());

            switch (State)
            {
                default:
                case 0://出现
                    {
                        Timer++;

                        Alpha += 0.15f;
                        if (Alpha > 1)
                            Alpha = 1;

                        const int MaxTimer = 15;
                        float factor = Timer / MaxTimer;
                        Scale = 0.5f * Helper.HeavyEase(factor);

                        exRot = factor * MathF.Sin(factor * MathHelper.TwoPi * 1.5f) * 0.35f;

                        BallDust();

                        if (Timer > MaxTimer)
                        {
                            Timer = 0;
                            State = 1;
                        }
                    }
                    break;
                case 1://等待开放
                    {
                        Timer++;

                        Scale = Helper.Lerp(Scale, 0.5f + Energy / 6 * 0.5f, 0.25f);
                        exRot = MathF.Sin((int)Main.timeForVisualEffects * 0.1f) * 0.2f;

                        //生成一些花雾粒子
                        if (Main.rand.NextBool(3))
                            FogParticle();

                        if (Timer == 60 * 15)
                        {
                            canDamage = true;
                            Projectile.damage /= 2;
                            for (int i = 0; i < 8; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.NextVec2Dir(2, 6)
                                    , Scale: Main.rand.NextFloat(1, 1.5f));
                                d.noGravity = true;
                            }

                            for (int i = 0; i < 4; i++)
                                FogParticle();
                        }
                        if (Timer > 60 * 15)//15秒都没开就消失了
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
                case 2://开放，并生成火球
                    {
                        Timer++;

                        if (Timer < 20)
                        {
                            Scale += 0.005f;
                            BallDust();
                        }
                        else
                            Scale -= 0.01f;

                        exAlpha += 0.1f;
                        if (exAlpha > 1)
                            exAlpha = 1;

                        const int MaxTimer = 45;
                        float factor = Timer / MaxTimer;

                        exRot = factor * MathF.Sin(factor * MathHelper.TwoPi * 1.5f) * 0.35f;

                        if (Timer == MaxTimer - 1)
                        {
                            Helper.PlayPitched(CoraliteSoundID.FireBallHit_NPCHit3, Projectile.Center);

                            canDamage = true;
                            for (int i = 0; i < 16; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.NextVec2Dir(2, 6)
                                    , Scale: Main.rand.NextFloat(1, 1.5f));
                                d.noGravity = true;
                            }

                            for (int i = 0; i < 4; i++)
                                FogParticle();
                        }
                        if (Timer > MaxTimer)
                        {
                            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                            Projectile.NewProjectileFromThis<GhostPipeFireBall>(Projectile.Center + dir * 25, dir * 14
                                , Projectile.damage * 3, Projectile.knockBack);
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        private void FogParticle()
        {
            PRTLoader.NewParticle(Projectile.Center, Offset.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(1, 2)
                , CoraliteContent.ParticleType<Fog>(), new Color(209, 174, 255, 80), Main.rand.NextFloat(0.5f, 0.7f));
        }

        private void BallDust()
        {
            Color c = Main.rand.Next(3) switch
            {
                0 => new Color(209, 174, 255),
                1 => Color.LimeGreen,
                _ => new Color(95, 91, 176),
            };
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustType<GlowBall>(),
                (Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(1, 3), 0, c, 0.25f);
        }

        private void Initialize()
        {
            init = false;

            Projectile.rotation = Offset.ToRotation();
        }

        public void GetEnergy()
        {
            Energy++;
            if (Energy >= 6)//6个能量直接开放
            {
                Energy = 6;
                State = 2;

                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustType<GhostPipePetal>()
                        , Offset.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.5f, 3)
                        , Scale: Main.rand.NextFloat(0.8f, 1.2f));
                }
            }

            Timer = 0;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Energy >= 6)
                target.AddBuff(BuffID.CursedInferno, 60 * 4);

            modifiers.SourceDamage += Energy * 0.4f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Color c = lightColor * Alpha;

            if (State < 2)
                DrawNotBloom(mainTex, pos, c);
            else
                DrawBloom(mainTex, pos, c);

            return false;
        }

        public void DrawNotBloom(Texture2D tex, Vector2 pos, Color c)
        {
            c *= 0.75f;
            float rotation = Projectile.rotation + exRot;

            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 4, 1, 5), pos, c, rotation, Scale);
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 4, 1, 5), pos, (c * 0.25f) with { A = 0 }, rotation, Scale);
        }

        public void DrawBloom(Texture2D tex, Vector2 pos, Color c)
        {
            Color c2 = c * 0.5f * exAlpha;
            c2.A /= 2;

            //绘制花瓣
            float rotation = Projectile.rotation + exRot;
            float t = (int)Main.timeForVisualEffects * 0.1f;
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 0, 1, 5)
                , pos, c2, rotation + MathF.Sin(t + MathHelper.PiOver2) * 0.2f, Scale);
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 1, 1, 5)
                , pos, c2, rotation + MathF.Sin(t) * 0.2f, Scale);

            //绘制本体
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 2, 1, 5)
                , pos, c * 0.75f, rotation, Scale);
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 2, 1, 5)
                , pos, (c * 0.25f) with { A = 0 }, rotation, Scale);

            //绘制上面的花瓣
            tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 3, 1, 5)
                , pos, c2, rotation + MathF.Sin(t - MathHelper.PiOver2) * 0.2f, Scale);
        }
    }

    public class GhostPipeFireBall : ModProjectile, IDrawAdditive, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public static Color highlightC = Color.White;
        public static Color brightC = new(96, 248, 2);
        public static Color darkC = new(138, 130, 218);

        ref float State => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        ref float FlyingTime => ref Projectile.localAI[2];

        private PrimitivePRTGroup fireParticles;
        private Trail trail;
        private readonly int trailPoint = 12;

        private int chaseTime;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 26;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanDamage()
        {
            if (State > 1)
                return false;
            return base.CanDamage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;
            State = 2;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 60 * 4);

            chaseTime = 0;
            State = 2;
            Projectile.velocity *= 0;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (fireParticles == null)
            {
                fireParticles = new PrimitivePRTGroup();
                Projectile.InitOldPosCache(trailPoint);
                Projectile.localAI[1] = Main.rand.NextFloat(-0.01f, 0.01f);
                FlyingTime = 20 * 5;
            }

            trail ??= new Trail(Main.instance.GraphicsDevice, trailPoint, new EmptyMeshGenerator(), factor =>
            {
                if (factor < 0.8f)
                    return Helper.Lerp(4, 6, factor / 0.8f);

                return Helper.Lerp(10, 0, (factor - 0.8f) / 0.2f);
            }, ColorFunc1);

            switch (State)
            {
                default:
                case 0://下落
                    {
                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                        {
                            float selfAngle = Projectile.velocity.ToRotation();
                            float targetAngle = (target.Center - Projectile.Center).ToRotation();

                            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.2f + (0.8f * Math.Clamp((chaseTime - 30) / 30, 0, 1f))).ToRotationVector2() * Projectile.velocity.Length();
                        }

                        SpawnDusts(1 - (0.3f * Timer / FlyingTime));

                        Timer++;
                        chaseTime++;

                        if (Timer > FlyingTime)
                        {
                            State = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        Projectile.localAI[0] += Projectile.localAI[1];
                        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
                        Projectile.velocity *= 0.97f;

                        SpawnDusts(0.7f);

                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        Timer++;
                        if (Timer > 15)
                        {
                            Projectile.velocity *= 0;
                            State++;
                        }
                    }
                    break;
                case 2://他紫砂了
                    {
                        if (!fireParticles.Any())
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }

            Projectile.UpdateOldPosCache();
            trail.TrailPositions = Projectile.oldPos;
            fireParticles.Update();
        }

        public static Color ColorFunc1(Vector2 factor)
        {
            if (factor.X < 0.7f)
            {
                return Color.Lerp(new Color(0, 0, 0, 0), brightC, factor.X / 0.7f);
            }

            return Color.Lerp(brightC, highlightC, (factor.X - 0.7f) / 0.3f);
        }

        public void SpawnDusts(float factor)
        {
            Color c;
            int type;
            type = DustID.CursedTorch;
            c = Main.rand.Next(3) switch
            {
                1 => darkC,
                _ => brightC,
            };

            Projectile.SpawnTrailDust(type, Main.rand.NextFloat(0.2f, 0.6f), Scale: Main.rand.NextFloat(1f, 2f));

            float angle = Projectile.velocity.AngleFrom(Projectile.oldVelocity);
            float rate = MathHelper.Clamp(0.4f - (Math.Abs(angle) / 5), 0, 0.4f);
            if (Main.rand.NextBool())
            {
                var p = fireParticles.NewParticle<FireParticle>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                     (Projectile.velocity * factor * Main.rand.NextFloat(rate * 0.7f, (rate * 1.3f) + 0.001f)).RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f))
                    , c, Main.rand.NextFloat(0.2f, 0.5f));
                p.MaxFrameCount = 2;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            fireParticles?.Draw(Main.spriteBatch);
        }

        public void DrawPrimitives()
        {
            if (State == 2 || trail == null)
                return;

            Effect effect = Filters.Scene["Flow2"].GetShader().Shader;

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 5);
            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTextImage"].SetValue(Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaserFlow").Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            trail.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class GhostPipePetal : ModDust
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.frame = new Rectangle(0, Main.rand.Next(3), 1, 3);
            dust.color = Color.White * 0.75f;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.1f, 0.15f, 0.1f));
            dust.position += dust.velocity;
            dust.rotation += 0.1f;
            dust.velocity *= 0.99f;
            dust.velocity.Y += 0.02f;

            if (dust.fadeIn > 30)
                dust.color *= 0.84f;

            if (!dust.noGravity && dust.velocity.Y < 5)
            {
                dust.velocity.Y += 0.05f;
            }

            dust.fadeIn++;
            if (dust.fadeIn > 45)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color c = Lighting.GetColor(dust.position.ToTileCoordinates());
            c *= dust.color.A / 255f;
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.frame, dust.position - Main.screenPosition, c, dust.rotation, scale: dust.scale);

            return false;
        }
    }

}
