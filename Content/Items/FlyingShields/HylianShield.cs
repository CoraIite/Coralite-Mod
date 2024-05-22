using Coralite.Content.Items.Nightmare;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    [AutoloadEquip(EquipType.Shield)]
    public class HylianShield : BaseFlyingShieldItem<HylianShieldGuard>, IEquipHeldItem, IDashable
    {
        public HylianShield() : base(Item.sellPrice(0, 20), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        {
        }

        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                flyingShieldGuard.State = 5;
                flyingShieldGuard.DistanceToOwner = flyingShieldGuard.GetWidth();
                flyingShieldGuard.Projectile.velocity = dashDirection.ToRotationVector2() * 9f;
                flyingShieldGuard.Projectile.rotation = dashDirection;

                Player.GetModPlayer<CoralitePlayer>().DashTimer = 75;
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;

                return true;
            }

            return false;

        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<HylianShieldProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 9;
            Item.damage = 225;
            Item.accessory = true;
        }

        public void UpdateEquipHeldItem(Player player)
        {
            player.statDefense += 25;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (player.ItemTimeIsZero && player.ownedProjectileCounts[Item.shoot] == 0)
                    cp.AddEffect(nameof(HylianShield));
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 25;
        }
    }

    public class HylianShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HylianShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
            Projectile.extraUpdates = 1;
        }

        public override void SetOtherValues()
        {
            flyingTime = 42;
            backTime = 17;
            backSpeed = 12;
            trailCachesLength = 12;
            trailWidth = 30 / 2;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.2f;
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.2f;
        }

        public override Color GetColor(float factor)
        {
            return Color.DarkBlue * factor;
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            for (int i = trailCachesLength - 1; i > 6; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.6f * ((i - 6) * 1 / 3f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);

            base.DrawTrails(lightColor);
        }
    }

    public class HylianShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HylianShield";

        private bool canDamage;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 48;
            Projectile.decidesManualFallThrough = true;
        }

        public override void SetOtherValues()
        {
            scalePercent = 1.2f;
            damageReduce = 0.6f;
            parryTime = 9;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == 5;
        }

        public override bool? CanDamage()
        {
            if (canDamage)
            {
                return base.CanDamage();
            }
            return false;
        }

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;
            if (State != 5)
                Projectile.velocity.X = Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            Projectile.timeLeft = 4;

            if (!Owner.active || Owner.dead)
                Projectile.Kill();

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                cp.FlyingShieldGuardIndex = Projectile.whoAmI;

            switch (State)
            {
                default: Projectile.Kill(); break;
                case (int)GuardState.Dashing:
                    {
                        SetPos();
                        OnHoldShield();

                        if (dashFunction != null)
                        {
                            dashFunction.OnDashing(this);
                        }
                        else
                            OnDashOver();

                        Timer--;
                        if (Timer < 1)
                            OnDashOver();
                    }
                    break;
                case (int)GuardState.Parry:
                    {
                        if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();
                        OnHoldShield();

                        if (CheckCollide(out _) > 0)
                        {
                            State = (int)GuardState.Guarding;
                            CompletelyHeldUpShield = true;
                            OnParry();
                        }

                        Timer--;
                        if (Timer < 1)
                        {
                            State = (int)GuardState.ParryDelay;
                            Timer = parryTime * 2;
                        }
                    }
                    break;
                case (int)GuardState.ParryDelay:
                    {
                        DistanceToOwner = Helper.Lerp(0, GetWidth(), Timer / (parryTime * 2));
                        SetPos();

                        Timer--;
                        if (Timer < 1)
                        {
                            State = (int)GuardState.Guarding;
                        }
                    }
                    break;
                case (int)GuardState.Guarding:
                    {
                        if (!Main.mouseRight)
                            TurnToDelay();

                        SetPos();
                        OnHoldShield();

                        if (DistanceToOwner < GetWidth())
                        {
                            DistanceToOwner += distanceAdder;
                            break;
                        }

                        CompletelyHeldUpShield = true;
                        int which = CheckCollide(out int index);
                        if (which > 0)
                        {
                            UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                            OnGuard();
                            if (which == (int)GuardType.Projectile)
                                OnGuardProjectile(index);
                            else if (which == (int)GuardType.NPC)
                                OnGuardNPC(index);
                        }
                    }
                    break;
                case (int)GuardState.Delay:
                    {
                        DistanceToOwner = Helper.Lerp(0, GetWidth(), Timer / delayTime);
                        SetPos();
                        Timer--;
                        if (Timer < 1)
                            Projectile.Kill();
                    }
                    break;
                case 5:
                    Sliding();
                    break;
            }

            //更新弹幕的无敌帧
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (localProjectileImmunity[i] > 0)
                {
                    localProjectileImmunity[i]--;
                    if (!Main.projectile[i].active || Main.projectile[i].friendly)
                        localProjectileImmunity[i] = 0;
                }
            }
        }

        public override void OnParry()
        {
            Owner.AddBuff(BuffID.ParryDamageBuff, 60 * 5);

            Helper.PlayPitched("TheLegendOfZelda/Shield_JustGird", 0.6f, 0, Projectile.Center);
            LightCiecleParticle.Spawn(Projectile.Center, Color.SkyBlue, 0.2f, Projectile.rotation, new Vector2(0.45f + Main.rand.NextFloat(-0.1f, 0.1f), 0.8f));
            Helper.PlayPitched($"TheLegendOfZelda/Guard_Metal_Metal_{Main.rand.Next(4)}", 0.4f, 0, Projectile.Center);

            float rot = Projectile.rotation + Main.rand.NextFloat(-0.6f, 0.6f);
            LightShotParticle.Spawn(Projectile.Center, Color.SkyBlue, rot
                , new Vector2(0.7f, 0.03f));

            Color c = new Color(0, 100, 255, 150);
            for (int i = 0; i < 6; i++)
            {
                rot = Main.rand.NextFloat(6.282f);
                LightShotParticle.Spawn(Projectile.Center, c, rot + Projectile.rotation
                    , new Vector2(Main.rand.NextFloat(0.4f, 0.6f) * Helper.EllipticalEase(rot, 1, 0.4f)
                    , 0.06f));
            }

            for (int i = 0; i < 5; i++)
            {
                rot = Main.rand.NextFloat(6.282f);
                LightShotParticle.Spawn(Projectile.Center, Color.SkyBlue, rot + Projectile.rotation
                    , new Vector2(Main.rand.NextFloat(0.6f, 0.7f) * Helper.EllipticalEase(rot, 1, 0.4f)
                    , 0.03f));
            }

            for (int i = 0; i < 6; i++)
            {
                LightTrailParticle.Spawn(Projectile.Center, Helper.NextVec2Dir(0.5f, 4f), c, Main.rand.NextFloat(0.3f, 0.6f));
            }
            for (int i = 0; i < 8; i++)
            {
                LightTrailParticle.Spawn(Projectile.Center, Helper.NextVec2Dir(3f, 5f), Color.White, Main.rand.NextFloat(0.1f, 0.3f));
            }

            if (VisualEffectSystem.HylianShieldScreenHighlight)
            {
                Color c2 = Color.Cyan;
                c2.A = 10;
                Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<ScreenLightParticle>(),
                  c2, 12);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 16 * 2;
            height = 16;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != 0)
            {
                Tile tile = Framing.GetTileSafely(Projectile.Center + new Vector2(0, 18));
                if (Main.tileBouncy[tile.TileType] || TileID.Sets.Platforms[tile.TileType])
                {
                    if (Math.Abs(Projectile.velocity.X) < 3)
                        Projectile.velocity.X *= 0.75f;
                    else
                        Projectile.velocity.X *= 0.98f;
                }
                else if (TileID.Sets.Grass[tile.TileType] || TileID.Sets.Snow[tile.TileType]
                      || TileID.Sets.isDesertBiomeSand[tile.TileType])
                {
                    if (Math.Abs(Projectile.velocity.X) < 3)
                        Projectile.velocity.X *= 0.75f;
                    else
                        Projectile.velocity.X *= 0.993f;
                }
                else if (TileID.Sets.Ices[tile.TileType] || tile.TileType == TileID.BreakableIce)
                {
                    if (Math.Abs(Projectile.velocity.X) < 3)
                        Projectile.velocity.X *= 0.8f;
                    else
                        Projectile.velocity.X *= 0.999f;
                }
                else
                    Projectile.velocity.X *= 0.7f;
            }
            return false;
        }

        public void Sliding()
        {
            if (!Main.mouseRight)
            {
                Owner.velocity = Projectile.velocity;
                Owner.velocity.Y += 0.0001f;
                TurnToDelay();
                return;
            }

            //设置玩家中心
            Projectile.tileCollide = true;
            Projectile.shouldFallThrough = Owner.controlDown;
            canDamage = false;
            Owner.velocity = new Vector2(0, -0.0001f);

            Projectile.rotation = Helper.Lerp(Projectile.rotation, 1.57f, 0.2f);

            //Projectile.Center = Owner.Center + Vector2.UnitY * DistanceToOwner;

            if (Projectile.velocity.Length() < 0.2f)
                Projectile.Kill();

            if (DistanceToOwner < GetWidth())
                DistanceToOwner += distanceAdder;

            Tile tile = Framing.GetTileSafely(Projectile.Center + new Vector2(0, 18));

            if (Projectile.velocity.Y == 0 && Owner.controlJump)
            {
                if (tile.HasTile && Main.tileBouncy[tile.TileType])
                    Projectile.velocity.Y -= 10f;
                else
                    Projectile.velocity.Y -= 6f;
            }

            if (Projectile.velocity.Y < 16)
                Projectile.velocity.Y += 0.4f;

            //判定敌怪
            int which = CheckCollide(out int index);
            if (which > 0)
            {
                if (which == 3)
                {
                    Projectile.velocity.Y = -12;
                    OnGuardProjectile(index);
                }
                else if (which == 4)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 10);
                    Projectile.velocity.Y = -12;
                    OnGuard();
                    OnGuardNPC(index);
                }
            }

            if (tile.HasTile)
            {
                if (Owner.controlLeft)
                {
                    if (Math.Abs(Projectile.velocity.X) > 8)
                        Projectile.velocity.X -= 0.1f;
                }
                else if (Owner.controlRight)
                {
                    if (Math.Abs(Projectile.velocity.X) > 8)
                        Projectile.velocity.X += 0.1f;
                }
            }

            if (Math.Abs(Projectile.velocity.X) > 9)
                Projectile.velocity.X = Math.Sign(Projectile.velocity.X) * Math.Abs(Projectile.velocity.X);

            Owner.Center = Projectile.Center + new Vector2(0, -30) + Projectile.velocity;
            Vector2 pos = Projectile.Center + new Vector2(-16, -8);
            float speed = 8f;
            Collision.StepUp(ref pos, ref Projectile.velocity, 32, 16, ref speed, ref Projectile.gfxOffY);
            if (speed != 8)
            {
                Projectile.velocity *= 1.1f;
            }
            Projectile.Center = pos + new Vector2(16, 8);

            if (Main.rand.NextBool(5))
                Collision.HitTiles(pos, Projectile.velocity, 32, 16);
        }

        public override int CheckCollide(out int index)
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                //检测脚下是否有炸弹
                if (proj.active && State == 5 && CoraliteSets.ProjectileExplosible[proj.type] && proj.Colliding(proj.getRect(), rect))
                {
                    proj.timeLeft = 2;
                    index = i;
                    return 3;
                }

                if (!proj.IsActiveAndHostile() || i == Projectile.whoAmI || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {

                    float damageR = damageReduce;
                    if (proj.penetrate < 0)//对于无限穿透的弹幕额外减伤
                        damageR += Main.rand.NextFloat(0, strongGuard / 3);

                    OnGuard_DamageReduce(damageR);

                    //如果不应该瞎JB乱该这东西的速度那就跳过
                    if (proj.aiStyle == 4 || proj.aiStyle == 38 || proj.aiStyle == 84 || proj.aiStyle == 148 ||
                        (proj.aiStyle == 7 && proj.ai[0] == 2f) || ((proj.type == 440 || proj.type == 449 ||
                        proj.type == 606) && proj.ai[1] == 1f) || (proj.aiStyle == 93 && proj.ai[0] < 0f) ||
                        proj.type == 540 || proj.type == 756 || proj.type == 818 || proj.type == 856 ||
                        proj.type == 961 || proj.type == 933 || ProjectileID.Sets.IsAGolfBall[proj.type])
                        goto over;

                    if (!ProjectileLoader.ShouldUpdatePosition(proj))
                        goto over;

                    //修改速度
                    proj.velocity *= -1;
                    float angle = proj.velocity.ToRotation();
                    proj.velocity = angle.AngleLerp(Projectile.rotation, 0.5f).ToRotationVector2() * proj.velocity.Length();

                over:
                    float percent = MathHelper.Clamp(strongGuard, 0, 1);
                    if (Main.rand.NextBool((int)(percent * 100), 100) && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                        OnStrongGuard();
                    }
                    localProjectileImmunity[i] = Projectile.localNPCHitCooldown;
                    index = i;
                    return (int)GuardType.Projectile;
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.immortal || !Projectile.localNPCImmunity.IndexInRange(i) || Projectile.localNPCImmunity[i] > 0)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    OnGuard_DamageReduce(damageReduce);

                    Projectile.localNPCImmunity[i] = Projectile.localNPCHitCooldown;
                    if (!npc.dontTakeDamage)
                        npc.SimpleStrikeNPC(Projectile.damage, Projectile.direction, knockBack: Projectile.knockBack, damageType: DamageClass.Melee);

                    if (State == 5 && npc.Top.Y > Projectile.Top.Y)
                    {
                        index = i;
                        return 4;
                    }

                    index = i;
                    return (int)GuardType.NPC;
                }
            }

            index = -1;
            return (int)GuardType.notGuard;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
        }

        public override void OnGuardNPC(int npcIndex)
        {
            NPC n = Main.npc[npcIndex];
            if (n.HitSound == CoraliteSoundID.Metal_NPCHit4)
                Helper.PlayPitched($"TheLegendOfZelda/Guard_Metal_Metal_{Main.rand.Next(4)}", 0.4f, 0, Projectile.Center);
            else if (n.HitSound == CoraliteSoundID.Bone_NPCHit2)
                Helper.PlayPitched($"TheLegendOfZelda/Guard_Bone_Metal_{Main.rand.Next(4)}", 0.4f, 0, Projectile.Center);
            else
                Helper.PlayPitched($"TheLegendOfZelda/Guard_Wood_Metal_{Main.rand.Next(4)}", 0.4f, 0, Projectile.Center);
        }

        public override void OnGuardProjectile(int projIndex)
        {
            Helper.PlayPitched($"TheLegendOfZelda/Guard_Metal_Metal_{Main.rand.Next(4)}", 0.4f, 0, Projectile.Center);
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    public class LightCiecleParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Circle3";

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 15)
            {
                color = Color.Lerp(color, new Color(0, 100, 250, 0), 0.35f);
                Scale += 0.07f;
            }
            else
            {
                Scale += 0.025f;
            }

            if (color.A < 2)
                active = false;
        }

        public static Particle Spawn(Vector2 center, Color newcolor, float baseScale, float rotation, Vector2 circleScale)
        {
            Particle p = NewParticle<LightCiecleParticle>(center, Vector2.Zero, newcolor, baseScale);

            p.Rotation = rotation;
            p.oldCenter = [circleScale];
            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;

            Vector2 pos = Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;
            Vector2 scale = oldCenter[0] * Scale;
            Color c = color;

            spriteBatch.Draw(mainTex, pos
                , null, c, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, pos
                , null, c, Rotation, origin, scale, SpriteEffects.None, 0f);

            Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "Circle2").Value;
            origin = exTex.Size() / 2;
            scale *= 0.6f;

            spriteBatch.Draw(exTex, pos
                , null, c, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(exTex, pos
                , null, c, Rotation, origin, scale * 0.95f, SpriteEffects.None, 0f);
            spriteBatch.Draw(exTex, pos
                , null, c, Rotation, origin, scale * 1.05f, SpriteEffects.None, 0f);
        }
    }

    public class LightShotParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "LightGlow";
        public override bool ShouldUpdateCenter() => false;

        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 15)
            {
                color = Color.Lerp(color, new Color(0, 60, 200, 0), 0.15f);
                Velocity.X *= 0.86f;
            }
            else if (fadeIn < 4)
            {
                Velocity *= 1.2f;
            }
            else
            {
                Velocity.X *= 0.98f;
            }

            if (color.A < 2)
                active = false;
        }

        /// <summary>
        /// 使用速度来充当缩放，虽然不太好但先就这样
        /// </summary>
        /// <param name="center"></param>
        /// <param name="newcolor"></param>
        /// <param name="rotation"></param>
        /// <param name="circleScale"></param>
        /// <returns></returns>
        public static Particle Spawn(Vector2 center, Color newcolor, float rotation, Vector2 circleScale)
        {
            Particle p = NewParticle<LightShotParticle>(center, Vector2.Zero, newcolor, 1);

            p.Rotation = rotation;
            p.Velocity = circleScale;
            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            //Vector2 pos = center - Main.screenPosition;
            //Vector2 origin = new Vector2(0, mainTex.Height / 2);
            Vector2 scale = Velocity * 0.3f;
            Color c = color;

            //spriteBatch.Draw(mainTex, pos
            //    , null, c, rotation, origin, scale, SpriteEffects.None, 0f);
            //c = Color.White * (c.A / 255f);

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            //List<CustomVertexInfo> bar2 = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar3 = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bar4 = new List<CustomVertexInfo>();

            Vector2 dir = Rotation.ToRotationVector2();
            Vector2 normal = dir.RotatedBy(1.57f);
            float width = mainTex.Width * scale.X / 25;
            float height = mainTex.Height * scale.Y;

            for (int i = 0; i < 25; i++)
            {
                float factor = i / 25f;
                Vector2 Center = this.Center + i * dir * width;
                Vector2 Top = Center - Main.screenPosition + normal * height * (1 - factor);
                Vector2 Bottom = Center - Main.screenPosition - normal * height * (1 - factor);

                Vector2 Top2 = Center - Main.screenPosition + normal * height * 6 * (1 - factor);
                Vector2 Bottom2 = Center - Main.screenPosition - normal * height * 6 * (1 - factor);

                var Color2 = Color.Lerp(color, Color.DarkBlue, factor);
                bars.Add(new(Top, Color2, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, Color2, new Vector3(factor, 1, 1)));
                //Color2.A = (byte)(c.A * 0.2f);
                //bar2.Add(new(Top, Color2, new Vector3(factor, 0, 1)));
                //bar2.Add(new(Bottom, Color2, new Vector3(factor, 1, 1)));
                Color2 = Color.White * (c.A / 255f);
                Color2.A = (byte)(c.A * 0.4f);
                bar3.Add(new(Bottom2, Color.Transparent, new Vector3(factor, 1, 1)));
                bar3.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Center - Main.screenPosition, Color2, new Vector3(factor, 0.5f, 1)));
                bar4.Add(new(Top2, Color.Transparent, new Vector3(factor, 0, 1)));
            }

            spriteBatch.GraphicsDevice.BlendState = BlendState.Additive;
            spriteBatch.GraphicsDevice.Textures[0] = mainTex;
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            //spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar2.ToArray(), 0, bar2.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar3.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bar4.ToArray(), 0, bar3.Count - 2);
            spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;


            //spriteBatch.Draw(Texture2D.Value, pos
            //    , null,c, rotation, origin, scale, SpriteEffects.None, 0f);
            //scale.Y *= 2f;
            //spriteBatch.Draw(Texture2D.Value, pos
            //    , null, c*0.5f, rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }

    public class ScreenLightParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LightBall";

        public override void OnSpawn()
        {
            shouldKilledOutScreen = false;
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn > 8)
            {
                color = Color.Lerp(color, new Color(0, 60, 250, 0), 0.05f);
            }
            else
            {
                color *= 1.03f;
                color.A += 3;
            }

            if (color.A < 2)
                active = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = GetTexture().Value;
            Vector2 origin = mainTex.Size() / 2;

            spriteBatch.Draw(mainTex, Center - Main.screenPosition,
                null, color, 0, origin, new Vector2(1.4f, 1) * Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(mainTex, Center - Main.screenPosition,
                null, color, 0, origin, new Vector2(1.4f, 1) * Scale / 2, SpriteEffects.None, 0f);
        }
    }
}
