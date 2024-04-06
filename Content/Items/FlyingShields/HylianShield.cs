using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class HylianShield : BaseFlyingShieldItem<HylianShieldGuard>, IEquipHeldItem, IDashable
    {
        public HylianShield() : base(Item.sellPrice(0, 5), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
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
            Item.shootSpeed = 15;
            Item.damage = 200;
        }

        public void UpdateEquipHeldItem(Player player)
        {
            player.statDefense += 20;
        }
    }

    public class HylianShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HylianShield";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 40;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18;
            backTime = 14;
            backSpeed = 15;
            trailCachesLength = 6;
            trailWidth = 12 / 2;
        }

        public override Color GetColor(float factor)
        {
            return Color.Gray * factor;
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
            scalePercent = 1.4f;
            damageReduce = 0.8f;
        }

        public override bool ShouldUpdatePosition()
        {
            return State==5;
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

                        if (CheckCollide() > 0)
                        {
                            State = (int)GuardState.Guarding;
                            CompletelyHeldUpShield = true;
                            OnParry();
                            UpdateShieldAccessory(accessory => accessory.OnParry(this));
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
                        int which = CheckCollide();
                        if (which > 0)
                        {
                            UpdateShieldAccessory(accessory => accessory.OnGuard(this));
                            OnGuard();
                            if (which == (int)GuardType.Projectile)
                                OnGuardProjectile();
                            else if (which == (int)GuardType.NPC)
                                OnGuardNPC();
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

            Projectile.rotation = 1.57f;

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
            int which = CheckCollide();
            if (which > 0)
            {
                if (which == 3)
                    Projectile.velocity.Y = -12;
                else if (which == 4)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 10);
                    Projectile.velocity.Y = -12;
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
            if (speed!=8)
            {
                Projectile.velocity *= 1.1f;
            }
            Projectile.Center = pos + new Vector2(16, 8);

            if (Main.rand.NextBool(5))
                Collision.HitTiles(pos, Projectile.velocity, 32, 16);
        }

        public override int CheckCollide()
        {
            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                //检测脚下是否有炸弹
                if (proj.active && State == 5 && CoraliteSets.ProjectileExplosible[proj.type]&&proj.Colliding(proj.getRect(), rect))
                {
                    proj.timeLeft = 2;
                    return 3;
                }

                if (!proj.active || !proj.hostile || localProjectileImmunity[i] > 0)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                {

                    float damageR = damageReduce;
                    if (proj.penetrate < 0)//对于无限穿透的弹幕额外减伤
                        damageR += Main.rand.NextFloat(0, strongGuard / 3);

                    OnGuard_DamageReduce(damageR);

                    float percent = MathHelper.Clamp(strongGuard, 0, 1);
                    if (Main.rand.NextBool((int)(percent * 100), 100) && proj.penetrate > 0)//削减穿透数
                    {
                        proj.penetrate--;
                        if (proj.penetrate < 1)
                            proj.Kill();
                        OnStrongGuard();
                    }
                    localProjectileImmunity[i] = Projectile.localNPCHitCooldown;
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

                    if ( State == 5 &&npc.Top.Y>Projectile.Top.Y)
                        return 4;

                    return (int)GuardType.NPC;
                }
            }

            return (int)GuardType.notGuard;
        }

        public override float GetWidth()
        {
            return Projectile.width / 2 / Projectile.scale;
        }
    }

    public class ExpandLightParticle:ModParticle
    {
        public override string Texture => AssetDirectory.OtherProjectiles+ "HorizontalLight";

        public override void OnSpawn(Particle particle)
        {

        }

        public override void Update(Particle particle)
        {
            if (particle.velocity.Y<4)
            {
                particle.velocity.Y += 0.3f;
            }
        }
    }
}
