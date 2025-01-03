using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class FullMoon : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(44, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 21, 4f);

            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 3);

            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<FullMoonHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.DemonBow)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.TendonBow)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        break;
                    }
                default:
                    return false;
            }

            int mouseDir = Main.MouseWorld.X > Player.Center.X ? 1 : -1;

            if (mouseDir > 0)
            {
                if (dashDirection > 0)
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
                else
                    newVelocity = new Vector2(-1, 0);
            }
            else
            {
                if (dashDirection > 0)
                    newVelocity = new Vector2(1, 0);
                else
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 70;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 18;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;
            Player.AddImmuneTime(ImmunityCooldownID.General, 14);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(10, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<FullMoonHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }



                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(dashDirection, 0), ProjectileType<FullMoonHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 18);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FullMoonHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(FullMoon);

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;
        public int SPTimer;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override int GetItemType()
            => ItemType<FullMoon>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺

        public override void DashAttackAI()
        {
            switch (DashState)
            {
                default:
                case 0://朝向目标方向冲刺，并检测碰撞
                    if (Timer < DashTime + 2)
                    {
                        Rotation = RecordAngle;
                        Vector2 dir = Rotation.ToRotationVector2();

                        Owner.velocity = dir * 10;
                        SpawnDashingDust();

                        if (Dashing_CheckCollide())
                        {
                            DashState = 1;
                            Timer = 0;
                            Owner.velocity = -dir * 10;
                            return;
                        }
                    }
                    else
                        Projectile.Kill();

                    break;
                case 1://与目标产生碰撞，向后反射并旋转弓
                    const int maxTime = 20;
                    Rotation += 1f / maxTime * MathHelper.TwoPi;
                    SpawnBackingDust();

                    if (Timer > maxTime)
                    {
                        DashState = 2;
                        Timer = 0;

                        if (Projectile.IsOwnedByLocalPlayer())//生成弹幕
                        {
                            Projectile.NewProjectileFromThis<FullMoonStrike>(Projectile.Center, Vector2.Zero
                                , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 1.75f), 10, Projectile.whoAmI);
                        }
                        return;
                    }

                    break;
                case 2://射击阶段
                    if (DownLeft && SPTimer == 0)
                    {
                        if (Main.myPlayer == Projectile.owner)
                        {
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                        }

                        Projectile.timeLeft = 30;
                        LockOwnerItemTime();
                    }
                    else
                    {
                        if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                        {
                            Helper.PlayPitched(CoraliteSoundID.ShadowflameApparition_NPCDeath55, Projectile.Center, pitchAdjust: -0.5f);
                            Helper.PlayPitched(CoraliteSoundID.Bow_Item5, Projectile.Center);

                            for (int i = 0; i < Main.maxProjectiles; i++)//将弹幕设置为射出状态
                            {
                                Projectile p = Main.projectile[i];

                                if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<FullMoonStrike>())
                                {
                                    p.ai[1] = 1;
                                    break;
                                }
                            }

                            Rotation = ToMouseAngle;

                            Vector2 dir = Rotation.ToRotationVector2();

                            if (VisualEffectSystem.HitEffect_ScreenShaking)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, dir, 8, 7, 4, 800));

                            handOffset = -20;
                        }

                        if (Main.myPlayer == Projectile.owner)
                        {
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            if (SPTimer < 8)
                                Rotation -= Owner.direction * 0.05f;
                            else
                                Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                        }

                        handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                        SPTimer++;

                        if (SPTimer > 22)
                            Projectile.Kill();
                    }



                    //Projectile.Kill();
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public void SpawnDashingDust()
        {
            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 n = (Rotation + 1.57f).ToRotationVector2();

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir.RotatedBy(i * MathF.Sin((float)Main.timeForVisualEffects * 0.4f) * 0.5f) * Main.rand.NextFloat(2, 4);
                Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 28, DustID.GoldFlame, velocity
                    , Scale: Main.rand.NextFloat(1.5f, 2f));

                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + i * n * 20, DustID.Shadowflame, velocity
                    , Alpha: 150, Scale: Main.rand.NextFloat(1f, 2f));

                d.noGravity = true;
            }

            Dust d2 = Dust.NewDustPerfect(Projectile.Center + dir * 24, DustID.GoldCoin
                , -dir.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(1, 2)
                , Scale: Main.rand.NextFloat(1, 1.5f));

            //d2.noGravity = true;
        }

        public void SpawnBackingDust()
        {
            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 n = (Rotation + 1.57f).ToRotationVector2();

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 velocity = -dir * Main.rand.NextFloat(0.5f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + i * n * 28, DustID.GoldFlame, velocity
                    , Scale: Main.rand.NextFloat(1.5f, 2f));

                d.noGravity = true;
                d = Dust.NewDustPerfect(Projectile.Center + i * n * 20, DustID.Shadowflame, velocity
                    , Alpha: 150, Scale: Main.rand.NextFloat(1f, 2f));

                d.noGravity = true;
            }

            Dust d2 = Dust.NewDustPerfect(Projectile.Center + dir * 24, DustID.GoldCoin
                , -dir * Main.rand.NextFloat(0.5f, 1f)
                , Scale: Main.rand.NextFloat(1, 1.5f));
        }

        public bool Dashing_CheckCollide()
        {
            Rectangle rect = GetDashRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                    return true;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    JustCollideNPC(npc);
                    return true;
                }
            }

            return false;
        }

        public Rectangle GetDashRect()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(48, 48));
        }

        public void JustCollideNPC(NPC target)
        {
            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, Projectile.Center, pitchAdjust: -0.3f);

            if (target.CanBeChasedBy())//踢一脚
                target.SimpleStrikeNPC(Owner.GetWeaponDamage(Owner.HeldItem), Owner.direction, knockBack: 10, damageType: DamageClass.Ranged);

            if (!VisualEffectSystem.HitEffect_SpecialParticles)
                return;

        }



        #endregion

        public override Vector2 GetOffset()
            => new Vector2(20 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteBatch spriteBatch = Main.spriteBatch;

            bool spDraw = Special == 1 && Timer < DashTime + 3 && DashState < 2;
            if (spDraw)
            {
                Projectile.DrawShadowTrails(Color.White, 0.75f, 1 / 8f, 0, 8, 2, 1 / 8f, 0, 1.2f);
            }

            Vector2 origin = mainTex.Size() / 2;
            spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (spDraw)
            {
                spriteBatch.Draw(mainTex, center, null, new Color(30, 50, 50, 0), Projectile.rotation
                    , origin, 1.25f, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            return false;
        }
    }

    /// <summary>
    /// ai0传入拥有者，ai1控制状态
    /// </summary>
    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FullMoonStrike : BaseHeldProj, IDrawNonPremultiplied, IPostDrawAdditive, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public static ATex FullMoonGradient { get; private set; }

        private PrimitivePRTGroup group;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 12000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
            => State == 1;

        public override void AI()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;

            if (!VaultUtils.isServer)
                group ??= [];

            Projectile.Center = owner.Center;

            switch (State)
            {
                default:
                case 0://捏在手里的阶段
                    if (!VaultUtils.isServer && Timer % 18 == 0 && Main.rand.NextBool())
                    {
                        float r = 32 + Main.rand.Next(2) * 12;
                        float startRot = Main.rand.NextFloat(-0.6f, -0.4f);
                        //总旋转路程除以转速
                        float time = (Main.rand.NextFloat(3.6f, 5.2f) - startRot) / TurbulenceCircle.MaxRotateSpeed;

                        var p = FullMoonCircle.Spawn(Projectile.Center,
                                 r, time, startRot, Main.rand.NextFromList(0.6f, MathHelper.Pi - 0.6f) + Main.rand.NextFloat(-0.4f, 0.4f)
                                 , owner.rotation + 1.57f + Main.rand.NextFloat(-0.4f, 0.4f)
                                 , owner.rotation.ToRotationVector2(), Projectile);
                        p.Color = new Color(50, 75, 75);

                        group.Add(p);
                    }

                    if (Timer > 1000)
                    {
                        Projectile.Kill();
                    }
                    break;
                case 1://释放
                    break;
            }

            if (!VaultUtils.isServer)
                group?.Update();

            Projectile.rotation = owner.rotation;
            Timer++;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D tex = CoraliteAssets.Trail.LightShot.Value;
            Vector2 pos = Projectile.Center;
            Vector2 scale = new Vector2(0.5f, 1f);

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(tex);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(FullMoonGradient.Value);
            effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.Tunnel.Value);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, effect, Main.GameViewMatrix.TransformationMatrix);

            Vector2 origin = new Vector2(tex.Width * 5 / 6, tex.Height / 2);
            spriteBatch.Draw(tex, pos, null
                , Color.White, Projectile.rotation + MathHelper.Pi, origin, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(tex, pos - Main.screenPosition, null
                , Color.White * 0.75f, Projectile.rotation + MathHelper.Pi, origin, scale * 1.1f, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.BoosterASP.Value;



            return base.PreDraw(ref lightColor);
        }

        public void DrawPrimitives()
        {
            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            group?.DrawPrimitive();
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class FullMoonCircle : TrailParticle
    {
        public override string Texture => AssetDirectory.Particles + "PurpleLine";

        static BasicEffect effect;
        public Projectile proj;
        public float zRot;
        public float alpha = 0;
        public float exRot;
        public float startRot;

        public static float MaxRotateSpeed = 0.25f;
        public Vector2 velocity2;

        public FullMoonCircle()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                effect = new BasicEffect(Main.instance.GraphicsDevice);
                effect.VertexColorEnabled = true;
                effect.TextureEnabled = true;
            });
        }

        public override void SetProperty()
        {
            MaxRotateSpeed = 0.3f;
            int trailCount = 10;
            trail = new Trail(Main.instance.GraphicsDevice, trailCount, new EmptyMeshGenerator(), factor => 10 * Scale, factor =>
            {
                return new Color(Color.R, Color.G, Color.B, (byte)(255 * alpha));
            });

            oldPositions = new Vector2[trailCount];
            float r = Rotation - trailCount * MaxRotateSpeed;
            for (int i = 0; i < oldPositions.Length; i++)
            {
                float length2 = Helper.EllipticalEase(r, zRot, out float overrideAngle2) * Velocity.X;

                oldPositions[i] = (overrideAngle2 + exRot).ToRotationVector2() * length2;
                r += MaxRotateSpeed;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Rotation += MaxRotateSpeed;

            Vector2 pos = proj.Center + proj.rotation.ToRotationVector2() * 16 + velocity2 * Opacity;//特殊判定，需要手动给这个oldPos赋值

            Velocity.X *= 1.015f;
            float length = Helper.EllipticalEase(Rotation, zRot, out float overrideAngle) * Velocity.X;

            for (int i = 0; i < oldPositions.Length - 1; i++)
                oldPositions[i] = oldPositions[i + 1];

            oldPositions[^1] = (overrideAngle + exRot).ToRotationVector2() * length;

            //for (int i = 0; i < oldPositions.Length; i++)
            //    oldPositions[i] += offset;

            float time = Velocity.Y;

            if (Opacity < (int)(time * 0.4f))
            {
                float factor = Opacity / (time * 0.4f);
                alpha = factor;
            }
            else if (Opacity == (int)(time * 0.4f))
            {
                alpha = 1;
            }

            if (Opacity > (int)(time * 0.9f))
            {
                alpha *= 0.9f;

                if (alpha < 0.02f)
                {
                    active = false;
                }
            }

            Opacity++;

            Vector2[] pos2 = new Vector2[oldPositions.Length];
            for (int i = 0; i < pos2.Length; i++)
                pos2[i] = pos + oldPositions[i];

            trail.TrailPositions = pos2;
        }

        public static FullMoonCircle Spawn(Vector2 center, float r, float time, float startRot, float zRot, float exRot, Vector2 velocity2, Projectile proj)
        {
            if (VaultUtils.isServer)
                return null;

            FullMoonCircle p = PRTLoader.PRT_IDToInstances[CoraliteContent.ParticleType<FullMoonCircle>()].Clone() as FullMoonCircle;
            p.Position = center;
            p.Velocity = new Vector2(r, time);
            p.Rotation = startRot;
            p.active = true;
            p.ShouldKillWhenOffScreen = false;
            p.Scale = 1;
            p.proj = proj;
            p.zRot = zRot;
            p.exRot = exRot;
            p.velocity2 = velocity2;

            p.SetProperty();

            return p;
        }

        public override void DrawPrimitive()
        {
            if (effect == null)
                return;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.Texture = TexValue;

            trail?.DrawTrail(effect);
        }
    }

}
