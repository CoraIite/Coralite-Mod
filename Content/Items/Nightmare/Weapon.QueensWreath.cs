using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class QueensWreath : ModItem, IDashable, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int Combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(239, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 22;
        }

        public override bool AltFunctionUse(Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);
                ClearOtherHeldProj(player);

                if (Combo > 3)//射出能获得梦魇光能的箭矢
                {
                    Combo = 0;
                    Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockback, player.whoAmI, ai1: 1);

                    Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, player.whoAmI, 1);
                    SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, position);
                }
                else//就只是普通地射箭
                {
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)), type, damage, knockback, player.whoAmI);

                    Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, player.whoAmI, 0);
                    SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, position);
                }

                Combo++;
            }
            return false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            IEntitySource source = Player.GetSource_ItemUse(Player.HeldItem);
            Vector2 position = Player.Center;

            switch (DashDir)
            {
                default: return false;
                case CoralitePlayer.DashLeft://向左右方向冲刺，直接冲
                case CoralitePlayer.DashRight:
                    ClearOtherHeldProj(Player);

                    Projectile p = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Player.whoAmI, 2);
                    float dashDirection;

                    if (Math.Sign(Main.MouseWorld.X - Player.Center.X) == (DashDir == CoralitePlayer.DashRight ? 1 : -1))
                        dashDirection = (Main.MouseWorld - Player.Center).ToRotation();
                    else
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : MathHelper.Pi;

                    (p.ModProjectile as QueensWreathHeldProj).dashDir = dashDirection;

                    newVelocity = dashDirection.ToRotationVector2() * 16;

                    for (int i = -3; i < 4; i++)
                    {
                        float rot = dashDirection + MathHelper.Pi + i * 0.35f;
                        Dust dust = Dust.NewDustPerfect(Player.Center + rot.ToRotationVector2() * 12, DustType<NightmareStar>(),
                            rot.ToRotationVector2() * (4f - Math.Abs(i) * 1f), newColor: NightmarePlantera.nightPurple, Scale: 1.8f - Math.Abs(i) * 0.3f);
                        dust.rotation = rot + MathHelper.PiOver2;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Particle.NewParticle(Player.Center + Main.rand.NextVector2Circular(24, 24) + i * newVelocity, -newVelocity * 0.75f, CoraliteContent.ParticleType<SpeedLine>(), NightmarePlantera.nightmareSparkleColor,
                            Main.rand.NextFloat(0.2f, 0.6f));
                    }

                    Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;

                    break;
                case CoralitePlayer.DashUp://向上冲，需要消耗1个梦魇光能
                    if (Player.TryGetModPlayer(out CoralitePlayer cp2) && cp2.nightmareEnergy > 0)
                    {
                        cp2.nightmareEnergy--;
                        //遍历一下，如果有npc在下方一定范围内那么就朝着那个npc的位置冲刺
                        float dashDir = -1.57f;//默认向正上方冲

                        ClearOtherHeldProj(Player);

                        Projectile p2 = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Player.whoAmI, 3);
                        (p2.ModProjectile as QueensWreathHeldProj).dashDir = dashDir;

                        newVelocity = dashDir.ToRotationVector2() * 11;
                        Player.GetModPlayer<CoralitePlayer>().DashTimer = 40;

                        for (int i = -3; i < 4; i++)
                        {
                            float rot = dashDir + MathHelper.Pi + i * 0.35f;
                            Dust dust = Dust.NewDustPerfect(Player.Center + rot.ToRotationVector2() * 12, DustType<NightmareStar>(),
                                rot.ToRotationVector2() * (4f - Math.Abs(i) * 1f), newColor: NightmarePlantera.nightPurple, Scale: 1.8f - Math.Abs(i) * 0.3f);
                            dust.rotation = rot + MathHelper.PiOver2;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            Particle.NewParticle(Player.Center + Main.rand.NextVector2Circular(24, 24) + i * newVelocity, -newVelocity * 0.75f, CoraliteContent.ParticleType<SpeedLine>(), NightmarePlantera.nightmareSparkleColor,
                                Main.rand.NextFloat(0.2f, 0.6f));
                        }
                    }
                    else
                        return false;
                    break;
                case CoralitePlayer.DashDown://向下冲，需要消耗2个梦魇光能
                    if (Player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 1)
                    {
                        cp.nightmareEnergy -= 2;
                        //遍历一下，如果有npc在下方一定范围内那么就朝着那个npc的位置冲刺
                        float dashDir = 1.57f;//默认向正下方冲
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];

                            if (npc.active && !npc.friendly
                                && npc.Distance(Player.Center) < 16 * 24 && npc.Top.Y > Player.Bottom.Y)
                            {
                                dashDir = (npc.Center - Player.Center).ToRotation();
                                break;
                            }
                        }

                        ClearOtherHeldProj(Player);

                        Projectile p2 = Projectile.NewProjectileDirect(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Player.whoAmI, 4);
                        (p2.ModProjectile as QueensWreathHeldProj).dashDir = dashDir;

                        newVelocity = dashDir.ToRotationVector2() * 16;
                        Player.GetModPlayer<CoralitePlayer>().DashTimer = 60;

                        for (int i = -3; i < 4; i++)
                        {
                            float rot = dashDir + MathHelper.Pi + i * 0.35f;
                            Dust dust = Dust.NewDustPerfect(Player.Center + rot.ToRotationVector2() * 12, DustType<NightmareStar>(),
                                rot.ToRotationVector2() * (4f - Math.Abs(i) * 1f), newColor: NightmarePlantera.nightPurple, Scale: 1.8f - Math.Abs(i) * 0.3f);
                            dust.rotation = rot + MathHelper.PiOver2;
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            Particle.NewParticle(Player.Center + Main.rand.NextVector2Circular(24, 24) + i * newVelocity, -newVelocity * 0.75f, CoraliteContent.ParticleType<SpeedLine>(), NightmarePlantera.nightmareSparkleColor,
                                Main.rand.NextFloat(0.2f, 0.6f));
                        }
                    }
                    else
                        return false;

                    break;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 60;
            Player.AddImmuneTime(ImmunityCooldownID.General, 30);
            Player.immune = true;
            Player.velocity = newVelocity;

            return true;
        }

        public static void ClearOtherHeldProj(Player Player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == Player.whoAmI && proj.type == ProjectileType<QueensWreathHeldProj>())
                {
                    proj.Kill();
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ai0控制状态，0普通发射，1普通射能获得梦魇光能的，2强化射，3向上冲刺吊射，4向下冲刺瞄准射击<br></br>
    /// ai2控制是否右键过
    /// </summary>
    public class QueensWreathHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.NightmareItems + "QueensWreath";

        public ref float State => ref Projectile.ai[0];
        public ref float Rotation => ref Projectile.ai[1];
        public bool NotRightClicked => Projectile.ai[2] == 0;

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public bool fadeIn = true;
        public bool init = true;

        public int shootTime;
        public int delayTime;
        public int dashHitNPCIndex = -1;
        public float dashDir;//TODO: 这个以后需要同步
        public bool dashHited;
        public bool shooted;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;

            if (init)
            {
                shootTime = Owner.itemTimeMax;
                delayTime = NotRightClicked ? 8 : 0;

                Owner.itemTime = shootTime + delayTime;
                if (NotRightClicked)
                {
                    switch (State)
                    {
                        default:
                        case 0:
                        case 1:
                            Projectile.timeLeft = shootTime + delayTime;
                            Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
                            break;
                        case 3:
                            Projectile.timeLeft = 120;
                            Projectile.rotation = -1.57f;
                            break;
                        case 2:
                        case 4:
                            Projectile.timeLeft = 120;
                            Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
                            break;
                    }
                }
                else
                {
                    Projectile.timeLeft = shootTime;
                    Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
                }

                init = false;
            }

            switch (State)
            {
                default: Projectile.Kill(); break;
                case 0: //普普通通的射箭
                    {
                        Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;
                        Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : 3.141f);

                        //如果满足条件且没有右键过 那么就再次射击
                        if (NotRightClicked && Main.mouseRight && Main.mouseRightRelease
                            && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)
                        {
                            if (Owner.PickAmmo(Owner.HeldItem, out int type, out float speed, out int damage, out float knockBack, out _))
                            {
                                cp.nightmareEnergy -= 1;

                                IEntitySource source = Projectile.GetSource_FromAI();
                                Vector2 position = Owner.Center;

                                Vector2 velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * speed;

                                for (int i = 0; i < 3; i++)
                                    Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)), type, (int)(damage * 0.9f), knockBack, Projectile.owner);

                                Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Projectile.owner, 0, ai2: 1);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);
                                //生成粒子

                            }

                            Projectile.Kill();
                        }
                    }

                    break;
                case 1: //射出能够获得梦魇光能的
                    {
                        Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;
                        Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : 3.141f);

                        //如果满足条件且没有右键过 那么就再次射击
                        if (NotRightClicked && Main.mouseRight && Main.mouseRightRelease
                            && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)
                        {
                            if (Owner.PickAmmo(Owner.HeldItem, out int type, out float speed, out int damage, out float knockBack, out _))
                            {
                                cp.nightmareEnergy -= 1;

                                IEntitySource source = Projectile.GetSource_FromAI();
                                Vector2 position = Owner.Center;

                                Vector2 velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * speed;

                                Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.Zero) * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner, ai1: 1);

                                Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Projectile.owner, 0, ai2: 1);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);
                                //生成粒子
                                for (int i = 0; i < 6; i++)
                                    Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Helper.NextVec2Dir() * 2);
                            }

                            Projectile.Kill();
                        }
                    }

                    break;
                case 2://横向冲刺
                    do
                    {
                        Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : 3.141f);
                        if (Timer < 20)
                        {
                            Owner.itemTime = Owner.itemAnimation = 20;
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            if (Timer < 14)
                            {
                                Owner.velocity = dashDir.ToRotationVector2() * 14;

                                Projectile.rotation = Projectile.rotation.AngleLerp(-1.57f - Owner.direction * 0.3f, 0.09f);
                                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * Helper.Lerp(4, 24, Timer / 14f);
                            }
                            else
                            {
                                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.Center).ToRotation(), 0.15f);
                                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * Helper.Lerp(24, 16, (Timer - 14) / 6f);
                            }

                            CheckDashHited();

                            //特效部分
                            for (int i = 0; i < 2; i++)
                            {
                                Particle.NewParticle(Owner.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, CoraliteContent.ParticleType<Fog>(),
                                    newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.2f));
                            }

                            Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(32, 32), DustType<NightmareStar>(), Vector2.Zero, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.5f, 1.2f));
                            break;
                        }

                        Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;
                        Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);

                        if (!shooted)
                        {
                            if (Owner.controlUseItem)
                            {
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                    Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                                    Projectile.netUpdate = true;

                                    if (Main.rand.NextBool(20))
                                    {
                                        Vector2 dir = Rotation.ToRotationVector2();
                                        Particle.NewParticle(Owner.Center + dir * 16 + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), NightmarePlantera.nightmareRed, Main.rand.NextFloat(0.1f, 0.15f));
                                    }
                                }

                                if (Projectile.timeLeft < 3)
                                {
                                    ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                    {
                                        if (dashHited)//冲刺途中有碰撞到东西
                                        {
                                            if (Owner.HeldItem.ModItem is QueensWreath qw)
                                                qw.Combo = 4;

                                            for (int i = 0; i < 3; i++)
                                                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * Main.rand.NextFloat(7f, 9f), ProjectileType<QueensWreathArrow>(), (int)(damage * 1.5f), knockBack, Projectile.owner);
                                        }
                                        else//啥也没碰到那就普通射一箭
                                            Projectile.NewProjectile(source, position, velocity * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);

                                        SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                    });

                                    Projectile.Kill();
                                    return;
                                }
                                //Projectile.timeLeft = 2;
                                Owner.itemTime = Owner.itemAnimation = 20;
                            }
                            else
                            {
                                ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                {
                                    if (dashHited)//冲刺途中有碰撞到东西
                                    {
                                        if (Owner.HeldItem.ModItem is QueensWreath qw)
                                            qw.Combo = 4;

                                        for (int i = 0; i < 3; i++)
                                            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * Main.rand.NextFloat(7f, 9f), ProjectileType<QueensWreathArrow>(), (int)(damage * 1.5f), knockBack, Projectile.owner);
                                    }
                                    else//啥也没碰到那就普通射一箭
                                        Projectile.NewProjectile(source, position, velocity * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);

                                    SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                });

                                Projectile.timeLeft = 20;
                                shooted = true;
                            }

                            break;
                        }

                        //射完后检测右键
                        if (NotRightClicked && Main.mouseRight && Main.mouseRightRelease
                            && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)
                        {
                            ShootArrow((source, position, velocity, type, knockBack, damage) =>
                            {
                                cp.nightmareEnergy -= 1;

                                if (dashHited)//冲刺途中有碰撞到东西
                                {
                                    for (int i = 0; i < 3; i++)
                                        Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)) * Main.rand.NextFloat(7f, 9f), ProjectileType<QueensWreathArrow>(), (int)(damage * 1.5f), knockBack, Projectile.owner);
                                }
                                else//啥也没碰到那就普通射一箭
                                    Projectile.NewProjectile(source, position, velocity * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);

                                Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Projectile.owner, 0, ai2: 1);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);

                                //生成粒子
                                for (int i = 0; i < 6; i++)
                                    Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Helper.NextVec2Dir() * 2);
                            });

                            Projectile.Kill();
                        }

                    } while (false);

                    break;
                case 3://向上冲刺
                    do
                    {
                        Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : 3.141f);
                        if (Timer < 20)
                        {
                            Projectile.rotation += MathHelper.TwoPi / 20;
                            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                            if (Timer < 14)
                            {
                                Owner.velocity = dashDir.ToRotationVector2() * 11;
                                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * Helper.Lerp(4, 24, Timer / 14f);
                            }
                            else
                                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * Helper.Lerp(24, 16, (Timer - 14) / 6f);

                            //特效部分
                            for (int i = 0; i < 2; i++)
                            {
                                Particle.NewParticle(Owner.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, CoraliteContent.ParticleType<Fog>(),
                                    newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.2f));
                            }

                            Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(32, 32), DustType<NightmareStar>(), Vector2.Zero, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.5f, 1.2f));

                            break;
                        }

                        Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;
                        Projectile.rotation = -1.57f + Math.Clamp(((Main.MouseWorld.X - Owner.Center.X) / 400f) * 0.3f, -0.4f, 0.4f);

                        if (!shooted)
                        {
                            if (Owner.controlUseItem)
                            {
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                    Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                                    Projectile.netUpdate = true;

                                    if (Main.rand.NextBool(20))
                                    {
                                        Vector2 dir = Rotation.ToRotationVector2();
                                        Particle.NewParticle(Owner.Center + dir * 16 + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), NightmarePlantera.nightmareRed, Main.rand.NextFloat(0.1f, 0.15f));
                                    }
                                }

                                if (Projectile.timeLeft < 3)
                                {
                                    UpShoot();
                                    Projectile.Kill();
                                    return;
                                }
                                //Projectile.timeLeft = 2;
                                Owner.itemTime = Owner.itemAnimation = 20;
                            }
                            else
                            {
                                UpShoot();
                                Projectile.timeLeft = 20;
                                shooted = true;
                            }

                            break;
                        }

                        //射完后检测右键
                        if (NotRightClicked && Main.mouseRight && Main.mouseRightRelease
                            && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)
                        {
                            ShootArrow((source, position, velocity, type, knockBack, damage) =>
                            {
                                cp.nightmareEnergy -= 1;

                                Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-150, 150), -1200);
                                velocity = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);

                                for (int i = 0; i < 3; i++)
                                    Projectile.NewProjectile(source, pos + Main.rand.NextVector2Circular(32, 32),
                                        velocity.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f)) * Main.rand.NextFloat(7f, 9f),
                                        ProjectileType<QueensWreathArrow>(), (int)(damage * 1.5f), knockBack, Projectile.owner, 1);

                                Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Projectile.owner, 0, ai2: 1);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);

                                for (int i = 0; i < 6; i++)
                                    Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Helper.NextVec2Dir() * 2);
                            });

                            Projectile.Kill();
                        }

                    } while (false);

                    break;
                case 4://向下冲刺
                    {
                        do
                        {
                            Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : 3.141f);
                            if (Timer < 20)
                            {
                                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                Projectile.rotation += MathHelper.TwoPi / 20;

                                if (Timer < 15)
                                    Owner.velocity = dashDir.ToRotationVector2() * 16;

                                Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;

                                CheckDashHited(npc => npc.SimpleStrikeNPC(Owner.GetWeaponDamage(Owner.HeldItem), Math.Sign(Owner.Center.X - npc.Center.X),
                                    damageType: DamageClass.Ranged));

                                if (dashHited)//冲刺过程中命中了什么东西
                                {
                                    Timer = 20;
                                    Projectile.timeLeft = 100;
                                    Owner.velocity = new Vector2(dashDir.ToRotationVector2().X, -0.5f).SafeNormalize(-Vector2.UnitY) * 14;
                                    Owner.AddImmuneTime(ImmunityCooldownID.General, 60);
                                    Owner.immune = true;
                                }

                                //特效部分
                                for (int i = 0; i < 2; i++)
                                {
                                    Particle.NewParticle(Owner.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, CoraliteContent.ParticleType<Fog>(),
                                        newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.2f));
                                }

                                Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(32, 32), DustType<NightmareStar>(), Vector2.Zero, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.5f, 1.2f));

                                break;
                            }

                            Projectile.Center = Owner.Center + Projectile.rotation.ToRotationVector2() * 16;

                            if (shooted)
                            {
                                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);

                                //射完后检测右键
                                if (NotRightClicked && Main.mouseRight && Main.mouseRightRelease
                                    && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy > 0)
                                {
                                    ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                    {
                                        cp.nightmareEnergy -= 1;

                                        position = Owner.Center;
                                        velocity *= 8.5f;

                                        if (dashHited)//冲刺途中有碰撞到东西
                                        {
                                            Projectile.NewProjectile(source, position, velocity, ProjectileType<QueensWreathArrow>(), damage * 2, knockBack, Projectile.owner, ai1: 1);
                                        }
                                        else//啥也没碰到那就普通射一箭
                                            Projectile.NewProjectile(source, position, velocity, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);

                                        Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<QueensWreathHeldProj>(), 1, 0, Projectile.owner, 0, ai2: 1);
                                        SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);

                                        //生成粒子

                                    });

                                    Projectile.Kill();
                                }

                                break;
                            }

                            if (dashHited)//冲刺过程命中了
                            {
                                if (Main.npc.IndexInRange(dashHitNPCIndex))
                                    Projectile.rotation = (Main.npc[dashHitNPCIndex].Center - Owner.Center).ToRotation();
                                else
                                    Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);

                                //跳起并做一个圆弧形运动，之后逐渐射出一些箭
                                do
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        Particle.NewParticle(Owner.Center + Main.rand.NextVector2Circular(16, 16), Vector2.Zero, CoraliteContent.ParticleType<Fog>(),
                                            newColor: NightmarePlantera.nightPurple, Scale: Main.rand.NextFloat(1f, 1.2f));
                                    }

                                    Dust.NewDustPerfect(Owner.Center + Main.rand.NextVector2Circular(32, 32), DustType<NightmareStar>(), Vector2.Zero, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(0.5f, 1.2f));

                                    Owner.itemTime = Owner.itemAnimation = 20;

                                    if (Timer < 40)
                                    {
                                        float factor = (Timer - 20) / 20;
                                        Owner.velocity = -dashDir.ToRotationVector2() * Helper.Lerp(16, 10, factor);
                                        break;
                                    }

                                    if (Timer < 70)
                                        Owner.velocity.X = -Math.Sign(dashDir.ToRotationVector2().X) * 10f;

                                    if (Timer == 50)//射出普通的箭
                                    {
                                        ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                        {
                                            Vector2 dir = velocity;
                                            if (Main.npc.IndexInRange(dashHitNPCIndex))
                                                dir = (Main.npc[dashHitNPCIndex].Center - Owner.Center).SafeNormalize(velocity);

                                            for (int i = 0; i < 3; i++)
                                                Projectile.NewProjectile(source, position, dir.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 18, type, damage, knockBack, Projectile.owner);

                                            SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                        });
                                    }

                                    if (Timer == 60 || Timer == 70)//射出特殊箭
                                    {
                                        ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                        {
                                            Vector2 dir = velocity;
                                            if (Main.npc.IndexInRange(dashHitNPCIndex))
                                                dir = (Main.npc[dashHitNPCIndex].Center - Owner.Center).SafeNormalize(velocity);

                                            Projectile.NewProjectile(source, position, dir * 9.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);
                                            SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                            //生成粒子
                                            for (int i = 0; i < 6; i++)
                                                Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Helper.NextVec2Dir() * 2);

                                        });
                                    }

                                    if (Timer == 80)//射出能够获得梦魇光能的特殊箭
                                    {
                                        ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                        {
                                            Vector2 dir = velocity;
                                            if (Main.npc.IndexInRange(dashHitNPCIndex))
                                                dir = (Main.npc[dashHitNPCIndex].Center - Owner.Center).SafeNormalize(velocity);

                                            Projectile.NewProjectile(source, position, dir * 9.5f, ProjectileType<QueensWreathArrow>(), damage * 3, knockBack, Projectile.owner, ai1: 1);
                                            SoundEngine.PlaySound(CoraliteSoundID.Bow2_Item102, Owner.Center);
                                            //生成粒子
                                            for (int i = 0; i < 6; i++)
                                                Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Helper.NextVec2Dir() * 2);

                                        });
                                        Projectile.timeLeft = 20;
                                        shooted = true;
                                    }
                                } while (false);

                                break;
                            }

                            if (Owner.controlUseItem)//冲刺过程没命中但没松手
                            {
                                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);

                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                    Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                                    Projectile.netUpdate = true;

                                    if (Main.rand.NextBool(20))
                                    {
                                        Vector2 dir = Rotation.ToRotationVector2();
                                        Particle.NewParticle(Owner.Center + dir * 16 + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), NightmarePlantera.nightmareRed, Main.rand.NextFloat(0.1f, 0.15f));
                                    }
                                }

                                if (Projectile.timeLeft < 3)
                                {
                                    ShootArrow((source, position, velocity, type, knockBack, damage) =>
                                    {
                                        Projectile.NewProjectile(source, position, velocity * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);
                                        SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                        //生成粒子

                                    });

                                    Projectile.Kill();
                                    return;
                                }
                                //Projectile.timeLeft = 2;
                                Owner.itemTime = Owner.itemAnimation = 20;

                                break;
                            }

                            Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                            ShootArrow((source, position, velocity, type, knockBack, damage) =>
                            {
                                Projectile.NewProjectile(source, position, velocity * 8.5f, ProjectileType<QueensWreathArrow>(), damage, knockBack, Projectile.owner);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                                //生成粒子

                            });

                            Projectile.timeLeft = 20;
                            shooted = true;

                        } while (false);
                    }
                    break;
            }

            Timer += 1f;
        }

        public void CheckDashHited(Action<NPC> onDashHitNPC = null)
        {
            if (!dashHited)//检测和npc或弹幕的碰撞
            {
                Rectangle rect = Projectile.getRect();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (!proj.active || proj.friendly || proj.whoAmI == Projectile.whoAmI)
                        continue;

                    if (proj.Colliding(proj.getRect(), rect))
                    {
                        DashHited();
                        break;
                    }
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (!npc.active || npc.friendly)
                        continue;

                    if (Projectile.Colliding(rect, npc.getRect()))
                    {
                        DashHited();
                        onDashHitNPC?.Invoke(npc);
                        dashHitNPCIndex = i;
                        break;
                    }
                }
            }
        }

        public void ShootArrow(Action<IEntitySource, Vector2, Vector2, int, float, int> shoot = null)
        {
            if (Owner.PickAmmo(Owner.HeldItem, out int type, out float _, out int damage, out float knockBack, out _))
            {
                IEntitySource source = Projectile.GetSource_FromAI();
                Vector2 position = Owner.Center;
                Vector2 velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);

                shoot?.Invoke(source, position, velocity, type, knockBack, damage);
            }
        }

        public void UpShoot()
        {
            ShootArrow((source, position, velocity, type, knockBack, damage) =>
            {
                Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-150, 150), -1200);
                velocity = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);

                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectile(source, pos + Main.rand.NextVector2Circular(32, 32),
                        velocity.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f)) * Main.rand.NextFloat(7f, 9f),
                        ProjectileType<QueensWreathArrow>(), (int)(damage * 1.5f), knockBack, Projectile.owner, 1);

                SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
            });
        }

        public void DashHited()
        {
            dashHited = true;
            //冲刺冲到了之后的特效
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1.1f, OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }
    }

    /// <summary>
    /// ai0控制状态，0普通发射，1吊射
    /// ai1控制是否能获得梦魇光能，为1时可以
    /// </summary>
    public class QueensWreathArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public ref float State => ref Projectile.ai[0];
        public bool CanGetNightmareEnergy => Projectile.ai[1] == 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.alpha = 0;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 4;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CanGetNightmareEnergy && Main.player[Projectile.owner].TryGetModPlayer(out CoralitePlayer cp))
                cp.GetNightmareEnergy(1);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(6);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                if (State == 1)
                    for (int i = 0; i < 6; i++)
                        Dust.NewDustPerfect(Projectile.Center, DustType<NightmareStar>(), Vector2.Zero, newColor: Color.Pink, Scale: 0.6f + i * 0.2f);
                Projectile.localAI[0] = 1;
            }

            if (++Projectile.frameCounter > 16)//帧图
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (++Projectile.frame > Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (CanGetNightmareEnergy)
            {
                if (Projectile.timeLeft % 8 > 3)//生成一条粒子
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Vector2.Zero, newColor: NightmarePlantera.lightPurple, Scale: 1.2f);
                    d.noGravity = true;

                }
                else//生成2条粒子
                {
                    Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
                    for (int i = -1; i < 2; i += 2)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center + dir * i * 6, DustID.ViciousPowder, Vector2.Zero, newColor: NightmarePlantera.lightPurple, Scale: 1.2f);
                        d.noGravity = true;

                    }
                }
            }
            else if (State == 1)
            {
                if (Projectile.timeLeft % 8 > 3)//生成一条粒子
                {
                    Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(Projectile.Center + dir * 4, DustID.ViciousPowder, Vector2.Zero, newColor: NightmarePlantera.lightPurple, Scale: 1.2f);
                    d.noGravity = true;
                }
                else//生成2条粒子
                {
                    Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(Projectile.Center + dir * -4, DustID.ViciousPowder, Vector2.Zero, newColor: NightmarePlantera.lightPurple, Scale: 1.2f);
                    d.noGravity = true;
                }
            }
            else
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.ViciousPowder, Vector2.Zero, newColor: NightmarePlantera.lightPurple, Scale: 1.2f);
                d.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.ViciousPowder, Helper.NextVec2Dir() * Main.rand.NextFloat(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 6, 0, Projectile.frame);
            Vector2 origin = frameBox.Size() / 2;
            //绘制残影
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 2; i < 14; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    Color.Pink * (0.75f - i * 0.75f / 14), Projectile.oldRot[i], frameBox.Size() / 2, 1f - i * 0.4f / 14, 0, 0);

            //向上下左右四个方向绘制一遍
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (i * MathHelper.PiOver2).ToRotationVector2() * 2, frameBox, Color.Pink, Projectile.rotation, origin, 1,
                   0, 0);
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White * 0.75f, Projectile.rotation, origin, 1, 0, 0);
            return false;
        }
    }
}
