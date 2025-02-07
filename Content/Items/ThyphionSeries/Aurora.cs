using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Aurora : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(136, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 22, 16f);

            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 10);

            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
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
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AuroraHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            if (player.ownedProjectileCounts[ProjectileType<AuroraHeldProj>()] == 0)
            {
                Projectile.NewProjectile(source, player.Center
                    , velocity.SafeNormalize(Vector2.Zero) * 14.5f, ProjectileType<AuroraArrow>(), damage, knockback, player.whoAmI);
                for (int i = -1; i < 2; i += 2)
                {
                    float r = i * 0.8f;
                    Projectile.NewProjectile(source, player.Center + velocity.SafeNormalize(Vector2.Zero).RotateByRandom(r - 0.2f, r + 0.2f) * 8, velocity * Main.rand.NextFloat(0.75f, 0.9f), type, damage, knockback, player.whoAmI);
                }

                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            }

            return false;
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
                        newVelocity.X = dashDirection * 13;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 85;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immune = true;
            Player.AddImmuneTime(ImmunityCooldownID.General, 30);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<AuroraHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<AuroraHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class AuroraHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(Aurora);

        public ref float ArrowLength => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        [AutoLoadTexture(Name = "Aurora_Glow")]
        public static ATex Flow { get; private set; }

        public static ATex AuroraArrow { get; private set; }

        public float handOffset = 0;
        public int SPTimer;

        public override int GetItemType()
            => ItemType<Aurora>();

        public override Vector2 GetOffset()
            => new(12 + handOffset, 0);

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺部分

        public override void DashAttackAI()
        {
            Lighting.AddLight(Projectile.Center, ThyphionSeries.AuroraArrow.GetColor(MathF.Sin((int)Main.timeForVisualEffects * 0.05f) / 2 + 0.5f).ToVector3()*0.5f);

            if (Timer < DashTime + 2)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Owner.itemTime = Owner.itemAnimation = 2;
                    Rotation = Helper.Lerp(RecordAngle, DirSign > 0 ? 0 : 3.141f, Timer / DashTime);
                }

                var p = PRTLoader.NewParticle<AuroraParticle>(Owner.Bottom, new Vector2(0, Main.rand.NextFloat(-1, 1)), Scale: Main.rand.NextFloat(0.8f, 1.3f));
                p.MaxTime = Main.rand.Next(10, 14);
                p.Rotation = Owner.velocity.ToRotation() + (Owner.velocity.X > 0 ? 0 : MathHelper.Pi);

                Owner.itemTime = Owner.itemAnimation = 2;
            }
            else
            {
                if (DownLeft && SPTimer == 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp(ToMouseAngle, 0.15f);
                    }

                    Projectile.timeLeft = 20;
                    Owner.itemTime = Owner.itemAnimation = 2;
                }
                else
                {
                    if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                    {
                        Helper.PlayPitched(CoraliteSoundID.AetheriumBlock, Projectile.Center, pitchAdjust: -0.5f);
                        Helper.PlayPitched(CoraliteSoundID.Bow2_Item102, Projectile.Center);

                        Projectile.NewProjectileFromThis<AuroraArrow>(Owner.Center, ToMouse.SafeNormalize(Vector2.Zero) * 16
                            , (int)(Owner.GetWeaponDamage(Owner.HeldItem) * 1.5f), Projectile.knockBack, 1);

                        Rotation = ToMouseAngle;

                        Vector2 dir = Rotation.ToRotationVector2();
                        PRTLoader.NewParticle<AuroraFlow>(Projectile.Center, dir * 8, Color.White, 0.9f);

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

                    if (SPTimer > 20)
                        Projectile.Kill();
                }
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(Flow.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            if (Special == 1 && SPTimer == 0)
            {
                Texture2D arrowTex = AuroraArrow.Value;
                Main.spriteBatch.Draw(arrowTex, center, null, lightColor, Projectile.rotation + 1.57f
                    , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);
            }

            return false;
        }
    }

    public class AuroraArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public Player Owner => Main.player[Projectile.owner];

        public bool SpAttack => Projectile.ai[0] == 1;
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];
        public ref float Random => ref Projectile.localAI[0];

        private bool init = true;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 14);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (init)
                Initialize();

            Lighting.AddLight(Projectile.Center, new Color(181, 255, 149).ToVector3() * 0.75f);

            if (Timer % (8 * Projectile.MaxUpdates) == 0)
            {
                NPC n = Helper.FindClosestEnemy(Projectile.Center, 400, n => n.CanBeChasedBy());
                if (n != null)
                    Target = n.whoAmI;
            }

            if (Target.GetNPCOwner(out NPC npc))
            {
                float num481 = 20f;
                Vector2 center = Projectile.Center;
                Vector2 targetCenter = npc.Center;
                Vector2 dir = targetCenter - center;
                float length = dir.Length();
                if (length < 100f)
                    num481 = 14f;

                float r = 64;
                if (SpAttack)
                    r = 34;

                length = num481 / length;
                dir *= length;
                Projectile.velocity.X = ((Projectile.velocity.X * r) + dir.X) / (r + 1);
                Projectile.velocity.Y = ((Projectile.velocity.Y * r) + dir.Y) / (r + 1);
            }
            else
                Target = -1;

            if (Timer > 0)
            {
                var p = PRTLoader.NewParticle<AuroraParticle>(Projectile.Center, new Vector2(0, -Random * Main.rand.NextFloat(1, 3)), Scale: Main.rand.NextFloat(0.8f, 1.3f));
                p.MaxTime = Main.rand.Next(9, 12);
                p.Rotation = Projectile.rotation + (Projectile.velocity.X > 0 ? 0 : MathHelper.Pi);
                p.powerful = SpAttack;
            }

            if (Main.rand.NextBool())
            {
                Vector2 dir = (Projectile.rotation + (Projectile.velocity.X > 0 ? -MathHelper.PiOver2 : MathHelper.PiOver2)).ToRotationVector2();
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.ShimmerSpark, dir * Main.rand.NextFloat(2, 5)
                    , newColor: GetColor(Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.5f, 2f));
                d.noGravity = true;
                d.noLight = true;
            }

            Random += Main.rand.NextFloat(-0.1f, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
        }

        public void Initialize()
        {
            init = false;
            Target = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (SpAttack)
            {
                //击杀所有的标记弹幕
                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Projectile.owner && proj.type == ProjectileType<AuroraArrowTag>()
                                     select proj)
                {
                    proj.ai[2] = 1;
                    break;
                }

                //生成标记弹幕
                Projectile.NewProjectileFromThis<AuroraArrowTag>(target.Center, Vector2.Zero, Projectile.damage, 0, ai1: target.whoAmI);
            }
            else
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];

                    if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<AuroraArrowTag>())
                    {
                        if (p.ai[0] < 15)
                            p.ai[0]++;
                        break;
                    }
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24)+Projectile.velocity*Main.rand.NextFloat(-3,1), DustType<PixelPoint>(), dir * Main.rand.NextFloat(4, 7)
                        ,0, GetColor(Main.rand.NextFloat()), Main.rand.NextFloat(1f, 2f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 14, 0, 14, 2, 1.57f, -1);

            Projectile.QuickDraw(lightColor, 1.57f);

            return false;
        }

        public static Color GetColor(float factor)
        {
            return Color.Lerp(new Color(181, 255, 149), new Color(202, 80, 129), factor);
        }
    }

    /// <summary>
    /// 极光的标记弹幕，极光箭命中标记弹幕标记的NPC后会额外造成一次伤害并积累标记，标记一定时间后根据层数爆炸
    /// </summary>
    public class AuroraArrowTag : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "AuroraTrail";

        public ref float HitCount => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];

        public ref float Length => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanDamage()
        {
            if (State == 2)
                return null;

            return false;
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC npc, Projectile.Kill))
                return;

            Projectile.Center = npc.Center;
            Lighting.AddLight(Projectile.Center, new Color(202, 80, 129).ToVector3() * 0.75f);

            if (Main.rand.NextBool())
            {
                Vector2 dir = -Vector2.UnitY;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16, 45), DustID.ShimmerSpark, dir * Main.rand.NextFloat(2, 5)
                    , newColor: AuroraArrow.GetColor(Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.5f, 2f));
                d.noGravity = true;
                d.noLight = true;
            }

            if (State == 0)
            {
                if (Length < 4)
                    Length += 0.25f;
            }
            else if (State == 1)
            {
                Length = Helper.Lerp(Length, 0, 0.2f);
                if (Length < 0.2f)
                {
                    //造成伤害
                    Attack();
                }
            }

            if (Timer > 60 * 8)
            {
                State = 1;
                Timer = 0;
            }

            Timer++;
        }

        public void Attack()
        {
            State = 2;
            Projectile.timeLeft = 3;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += HitCount * 0.15f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = CoraliteAssets.Trail.LightShotSPA.Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 pos2 = pos + new Vector2(0, -200);

            for (int i = 0; i < 12; i++)
            {
                float factor = i / 12f;

                int k = 10 - i;
                float y = k * MathF.Sin(k * 0.8f + (float)(Main.timeForVisualEffects) * 0.1f) / 20f;
                Vector2 spDir = Vector2.UnitY.RotatedBy(-y) * k * Length;
                Vector2 targetPos = pos - spDir;
                spDir.X *= -0.5f;

                float rot = (pos2 - targetPos).ToRotation();

                float scaleX = 0.22f - i * 0.013f;
                scaleX *= 2;
                float scaleY = 12 * 0.02f - i * 0.02f;
                scaleY *=2.5f* Length/4;

                Color c = AuroraArrow.GetColor(1 - factor) * 0.5f;

                Main.spriteBatch.Draw(mainTex, targetPos, null, c, rot + MathHelper.Pi, new Vector2(mainTex.Width, mainTex.Height / 2)
                    , new Vector2(scaleX, scaleY), 0, 0);
                Main.spriteBatch.Draw(mainTex, pos - spDir, null, c, rot + MathHelper.Pi, new Vector2(mainTex.Width, mainTex.Height / 2)
                    , new Vector2(scaleX*0.5f, scaleY), 0, 0);

                c.A = 10;

                Main.spriteBatch.Draw(mainTex, targetPos, null, c, rot + MathHelper.Pi, new Vector2(mainTex.Width, mainTex.Height / 2)
                    , new Vector2(scaleX * 0.3f, scaleY), 0, 0);
            }

            return false;
        }
    }

    public class AuroraParticle : Particle
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "AuroraTrail";

        public bool powerful;
        public float alpha;
        public Vector2 scale = new Vector2(0, 0.2f);
        public int MaxTime = 20;
        public float endX = Main.rand.NextFloat(0.125f, 0.35f);

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;

            Color = powerful ? GetColorPowerful(Opacity / MaxTime) : GetColor(Opacity / MaxTime);

            if (Opacity < MaxTime / 4)
            {
                float f = Opacity / (MaxTime / 4f);
                scale = Vector2.Lerp(new Vector2(0, 0.2f), new Vector2(endX, 0.4f), f);
            }
            else
            {
                scale = Vector2.Lerp(scale, new Vector2(0.02f, 0.6f), 0.1f);
            }

            if (Opacity < MaxTime / 2)
            {
                float f = Opacity / (MaxTime / 2f);
                alpha = 1 - f;
            }
            else
            {
                alpha *= 0.9f;
            }

            if (Opacity > MaxTime)
            {
                active = false;
            }
        }

        public Color GetColorPowerful(float factor)
        {
            return Color.Lerp(new Color(239, 213, 172), new Color(250, g: 45, 112), factor);
        }

        public Color GetColor(float factor)
        {
            return Color.Lerp(new Color(181, 255, 180), new Color(202, g: 60, 109), factor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D trail = TexValue;
            Vector2 trailOrigin = new Vector2(trail.Width, trail.Height / 2);
            Vector2 trailPos = Position - Main.screenPosition;

            Color c = Color * alpha * 0.6f;
            if (c.A < 2)
            {
                return false;
            }
            Vector2 s = scale * Scale;

            float rot = Rotation + 1.57f;
            Main.spriteBatch.Draw(trail, trailPos, null, c, rot, trailOrigin, s, 0, 0);
            s.X *= 0.35f;
            Main.spriteBatch.Draw(trail, trailPos + Velocity * 2, null, c, rot, trailOrigin, s, 0, 0);

            c.A = 35;
            Main.spriteBatch.Draw(trail, trailPos, null, c, rot, trailOrigin, s, 0, 0);

            return false;
        }
    }

    public class AuroraFlow : RadiantSunFlow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "AuroraFlow";
    }
}
