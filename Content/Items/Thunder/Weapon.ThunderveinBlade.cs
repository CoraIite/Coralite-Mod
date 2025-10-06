using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Gels;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinBlade : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public int useCount;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 65;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 2f;
            Item.crit = 10;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<ThunderveinBladeSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 2)
                    useCount = 0;

                switch (useCount)
                {
                    default:
                    case 0:
                    case 1:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), damage, knockback, player.whoAmI, useCount);
                        break;
                    case 2:
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(), (int)(damage * 1.35f), knockback, player.whoAmI, useCount);

                        break;
                }
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(4)
                .AddIngredient<InsulationCortex>(4)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        DashDir = (int)dashDirection;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = new Vector2(DashDir * 30, Player.velocity.Y);
            Player.direction = DashDir;
            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, Player.Center);

                for (int i = 0; i < 1000; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.ModProjectile is ThunderveinBladeSlash)
                    {
                        proj.Kill();
                        break;
                    }
                }

                //生成手持弹幕
                Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));

                int damage = Player.GetWeaponDamage(Player.HeldItem);
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                    damage, Player.HeldItem.knockBack, Player.whoAmI, 3);
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                    damage, Player.HeldItem.knockBack, Player.whoAmI, 10, DashDir);
            }

            return true;
        }
    }

    public class ThunderveinBladeSlash : BaseSwingProj, IDrawWarp
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderveinBlade";

        public ref float Combo => ref Projectile.ai[0];

        public static Asset<Texture2D> WarpTexture;
        public static Asset<Texture2D> GradientTexture;

        public ThunderveinBladeSlash() : base(new Vector2(48, 46).ToRotation() - 0.08f, trailCount: 48) { }

        public int delay;
        public int alpha;

        private float recordStartAngle;
        private float recordTotalAngle;
        private float extraScaleAngle;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            WarpTexture = Request<Texture2D>(AssetDirectory.OtherProjectiles + "WarpTex");
            GradientTexture = Request<Texture2D>(AssetDirectory.ThunderItems + "ThunderveinBladeGradient");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            WarpTexture = null;
            GradientTexture = null;
        }

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 80;
            Projectile.hide = true;
            trailTopWidth = -8;
            distanceToOwner = 8;
            minTime = 0;
            onHitFreeze = 12;
            useSlashTrail = true;
        }

        protected override float ControlTrailBottomWidth(float factor)
        {
            return 66 * Projectile.scale;
        }

        protected override float GetStartAngle()
        {
            if (Combo == 3)
                return Owner.velocity.ToRotation();
            return base.GetStartAngle();
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Combo < 3)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            Projectile.extraUpdates = 5;
            alpha = 0;
            switch (Combo)
            {
                default:
                case 0: //下挥，较为椭圆
                    startAngle = 1.6f;
                    totalAngle = 3.8f;
                    minTime = 12;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 82;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = 0.4f;
                    break;
                case 1://下挥，圆
                    startAngle = 2.2f;
                    totalAngle = 3.4f;
                    minTime = 0;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 82;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 14;
                    extraScaleAngle = -0.4f;
                    break;
                case 2://上挥 较圆
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    minTime = 24;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 90 + 24;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    break;
                case 3://上挥 较圆
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    minTime = 5;
                    maxTime = (int)(Owner.itemTimeMax * 0.6f) + 80 + 5;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    delay = 12;
                    break;
            }

            ExtraInit();
            base.InitializeSwing();
        }

        private void ExtraInit()
        {
            recordStartAngle = Math.Abs(startAngle);
            recordTotalAngle = Math.Abs(totalAngle);
            Projectile.scale = 0.7f;
        }

        protected override void AIBefore()
        {
            Lighting.AddLight(Projectile.Center, 1f, 1f, 0.3f);
            base.AIBefore();
        }

        protected override void BeforeSlash()
        {
            Slasher();
            Projectile.scale = Helper.Lerp(Projectile.scale, Helper.EllipticalEase(recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(0, maxTime - minTime)), 1.2f, 1.7f), 0.1f);
            if ((int)Timer == minTime)
            {
                switch (Combo)
                {
                    default:
                    case 0:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 1:
                        SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1);
                        break;
                    case 2:
                        Helper.PlayPitched("Misc/Slash", 0.4f, 0.4f, Owner.Center);

                        break;
                }
                InitializeCaches();
            }
        }

        protected override void OnSlash()
        {
            int timer = (int)Timer - minTime;
            float scale = 1f;

            if (Item.type == ItemType<ThunderveinBlade>())
                scale = Owner.GetAdjustedItemScale(Item);

            if (timer % 7 == 0 && timer < maxTime * 0.5f)
            {
                Vector2 pos = Projectile.Center + (RotateVec2 * Main.rand.NextFloat(-10, 40) * Projectile.scale);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.05f, 0.2f));
                }
            }

            switch (Combo)
            {
                default:
                case 0:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1f, 1.3f);
                    break;
                case 1:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 1.1f, 1.4f);
                    break;
                case 2:
                case 3:
                    Projectile.scale = scale * Helper.EllipticalEase(
                        recordStartAngle + extraScaleAngle - (recordTotalAngle
                        * Smoother.Smoother(timer, maxTime - minTime)), 0.8f, 1.8f);
                    break;
            }
            alpha = (int)(Helper.X2Ease(timer, maxTime - minTime) * 50) + 200;
            base.OnSlash();
        }

        protected override void AfterSlash()
        {
            Projectile.scale *= 0.99f;
            if (alpha > 20)
                alpha -= 5;
            Slasher();
            if (Timer > maxTime + delay)
                Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                Owner.immuneTime += 10;
                if (VaultUtils.isServer)
                    return;

                float strength = 2;
                float baseScale = 1;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, (Projectile.width * Projectile.scale) - Projectile.localAI[1]);
                Vector2 pos = Bottom + (RotateVec2 * offset);
                if (VisualEffectSystem.HitEffect_Lightning)
                {
                    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                    dust = Dust.NewDustPerfect(pos, DustType<EmperorSabreStrikeDust>(),
                             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.2f, 0.2f);
                }

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = RotateVec2.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
                        dust = Dust.NewDustPerfect(pos, DustID.PortalBoltTrail, dir * Main.rand.NextFloat(2f, 12f), newColor: Coralite.ThunderveinYellow, Scale: Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        public void DrawWarp()
        {
            WarpDrawer(0.75f);
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
                Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
                {
                    Effect effect = ShaderLoader.GetShader("SimpleGradientTrail");

                    effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                    effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.SlashFlatFade.Value);
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
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }

    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入冲刺角度，ai2传入冲刺次数
    /// </summary>
    public class ThunderveinBladeDash : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float DashDir => ref Projectile.ai[1];
        public ref float DashCount => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public float fade = 0;

        private bool ExtraDash = true;

        const int DelayTime = 30;

        protected ThunderTrail[] thunderTrails;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (Timer > DashTime + (DelayTime / 2))
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.velocity, Projectile.Center, Projectile.width, ref a);
        }

        public override void Initialize()
        {
            if (VisualEffectSystem.HitEffect_ScreenShaking)
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Helper.NextVec2Dir(), 12, 8, 5, 1000));
        }

        public override float GetAlpha(float factor)
        {
            if (factor < fade)
                return 0;

            return ThunderAlpha * (factor - fade) / (1 - fade);
        }

        public override void AI()
        {
            InitThunderTrail();

            if (Timer < DashTime)
            {
                SpawnDusts();
                Projectile.Center = Owner.Center
                    + ((Owner.Center - Projectile.velocity).SafeNormalize(Vector2.Zero) * Timer * 12);
                if (DashCount < 3)
                {
                    Owner.velocity = new Vector2(DashDir * 30, Owner.velocity.Y);
                    Owner.direction = (int)DashDir;
                }
                Owner.immuneTime = 20;
                Owner.immune = true;

                UpdateTrailPoints();

                ThunderWidth = 20;
                ThunderAlpha = Timer / DashTime;
            }
            else if ((int)Timer == (int)DashTime)
            {
                if (DashCount < 3)
                    Owner.velocity *= 0.1f;
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                if (DashCount < 2 && ExtraDash && Timer < DashTime + (DelayTime / 3))
                {
                    if (DashCount == 0)
                    {
                        if (Owner.controlLeft && DashDir == 1)
                        {
                            float count = DashCount + 1;
                            Owner.velocity = new Vector2(-30, Owner.velocity.Y);
                            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));

                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 3);
                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 10, -1, count);
                            ExtraDash = false;
                        }
                        else if (Owner.controlRight && DashDir == -1)
                        {
                            float count = DashCount + 1;
                            Owner.velocity = new Vector2(30, Owner.velocity.Y);
                            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));

                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 3);
                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 10, 1, count);
                            ExtraDash = false;
                        }
                    }
                    else
                    {
                        if (Owner.controlLeft)
                        {
                            float count = DashCount + 1;
                            Owner.velocity = new Vector2(-30, Owner.velocity.Y);
                            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));

                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 3);
                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 10, -1, count);
                            ExtraDash = false;
                        }
                        else if (Owner.controlRight)
                        {
                            float count = DashCount + 1;
                            Owner.velocity = new Vector2(30, Owner.velocity.Y);
                            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 50));

                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeSlash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 3);
                            Projectile.NewProjectile(Owner.GetSource_ItemUse(Item), Owner.Center, Vector2.Zero, ProjectileType<ThunderveinBladeDash>(),
                                Projectile.damage, Projectile.knockBack, Owner.whoAmI, 10, 1, count);
                            ExtraDash = false;
                        }
                    }
                }

                SpawnDusts();

                float factor = (Timer - DashTime) / DelayTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + (sinFactor * 30);
                ThunderAlpha = 1 - Helper.X2Ease(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 8 + (sinFactor * 16 / 2)));
                    trail.SetExpandWidth((1 - factor) * 14 / 3);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                fade = Helper.X2Ease((int)(Timer - DashTime), DelayTime);

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        private void InitThunderTrail()
        {
            if (thunderTrails == null)
            {
                Projectile.Resize(32, 40);
                Projectile.velocity = Projectile.Center;
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBodyF");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].UseNonOrAdd = true;
                    thunderTrails[i].SetRange((0, 6));
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }
        }

        private void UpdateTrailPoints()
        {
            Vector2 pos2 = Projectile.velocity;
            List<Vector2> pos = new()
                {
                    Projectile.velocity
                };

            if (Vector2.Distance(Projectile.velocity, Projectile.Center) < 32)
                pos.Add(Projectile.Center);
            else
                for (int i = 0; i < 40; i++)
                {
                    pos2 = pos2.MoveTowards(Projectile.Center, 32);
                    if (Vector2.Distance(pos2, Projectile.Center) < 32)
                    {
                        pos.Add(Projectile.Center);
                        break;
                    }
                    else
                        pos.Add(pos2);
                }

            foreach (var trail in thunderTrails)
            {
                pos[0] = Projectile.velocity + Main.rand.NextVector2CircularEdge(12, 12);
                pos[^1] = Projectile.Center + Main.rand.NextVector2CircularEdge(12, 12);
                trail.BasePositions = [.. pos];
                trail.SetExpandWidth(4);
            }

            if (Timer % 4 == 0)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
        }

        public virtual void SpawnDusts()
        {
            if (Main.rand.NextBool(7))
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                {
                    PRTLoader.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.7f, 1.1f));
                }
                else
                {
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.3f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
            {
                foreach (var trail in thunderTrails)
                {
                    trail?.DrawThunder(Main.instance.GraphicsDevice);
                }
            }
            return false;
        }
    }
}
