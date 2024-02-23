using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Buffs;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThorn : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float ShootCount => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];
        public bool rightClick;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("冰魔刺");

            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.alpha = 0;
            Projectile.timeLeft = 300;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool MinionContactDamage() => true;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        #region AI

        public override void AI()
        {
            //1：返回玩家身边
            //2：找到敌人后不断向敌人位置冲刺
            //3：右键打断当前动作并向周围射击冰刺
            //4: 射3次冰刺后再右键会爆炸

            Player Owner = Main.player[Projectile.owner];

            if (!CheckActive(Owner))
                return;

            //添加Buff
            Owner.AddBuff(BuffType<IcicleThornBuff>(), 2);

            NPC target = Helper.FindClosestEnemy(Projectile.Center, 1200f, (n) =>
                   n.CanBeChasedBy() && !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1));

            if (Timer == 0)
            {
                if (rightClick)
                {
                    if (ShootCount > 2)
                        State = (int)AIStates.explosion;
                    else
                        State = (int)AIStates.specialAttack;
                }
                else if (target != null)
                    State = (int)AIStates.normalAttack;
            }

            switch (State)
            {
                default: break;
                case (int)AIStates.idle:    //回到玩家身边
                    Timer = 0;
                    Helper.GetMyProjIndexWithSameType(Type, Projectile.whoAmI, Projectile.owner, out int index, out int totalIndexes);
                    Vector2 idlePosition = Owner.Center + new Vector2(0, -16 - totalIndexes * 2).RotatedBy(6.282f * index / totalIndexes);

                    if (Vector2.Distance(idlePosition, Projectile.position) > 2000)
                        Projectile.Center = idlePosition;

                    //XY方向都渐进目标方向
                    Projectile.direction = Projectile.spriteDirection = idlePosition.X > Projectile.Center.X ? 1 : -1;
                    int directionY = idlePosition.Y > Projectile.Center.Y ? 1 : -1;

                    if (Vector2.Distance(idlePosition, Projectile.Center) > 80)
                    {
                        Helper.Movement_SimpleOneLine(ref Projectile.velocity.X, Projectile.direction, 7f, 0.2f, 0.3f, 0.97f);
                        Helper.Movement_SimpleOneLine(ref Projectile.velocity.Y, directionY, 5f, 0.2f, 0.3f, 0.97f);
                    }
                    else if (Projectile.velocity.Length() > 4)
                        Projectile.velocity *= 0.96f;
                    else
                    {
                        Projectile.velocity = Projectile.localAI[1].ToRotationVector2();
                        Vector2 dir = idlePosition - Projectile.Center;
                        if (dir.Length() > 50)
                            Projectile.localAI[1] = dir.ToRotation();
                        Projectile.localAI[1] += 0.01f;
                    }

                    Projectile.rotation += Projectile.velocity.Length() * 0.05f + 0.02f;
                    return;
                case (int)AIStates.normalAttack:    //普通攻击：不断冲向敌人位置

                    if (target is null || !target.active)
                    {
                        ResetStates();
                        return;
                    }

                    Projectile.rotation += Projectile.velocity.Length() * 0.05f + 0.05f;

                    if (Timer == 0)
                    {
                        Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 16f;
                        Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                        break;
                    }

                    if (Timer < 30)
                        break;

                    if (Timer < 40)
                    {
                        Projectile.velocity *= 0.96f;
                        Projectile.rotation = Helper.Lerp(Projectile.rotation, 0, 0.1f);
                        break;
                    }

                    ResetStates();
                    return;
                case (int)AIStates.specialAttack:   //特殊攻击
                    Projectile.velocity *= 0.96f;
                    Projectile.rotation += Projectile.velocity.Length() * 0.05f;

                    if (Timer < 25)
                    {
                        if (Timer % 15 == 0)
                            Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Scale: 0.15f);
                        break;
                    }

                    if (Timer < 35)
                        break;

                    if (Timer == 35 && Main.myPlayer == Projectile.owner)
                    {
                        //生成冰块弹幕
                        Vector2 dir = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.One);
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir * 14, ProjectileType<IcicleSpurt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 30);
                    }

                    if (Timer < 60)
                        break;

                    ShootCount += 1f;
                    rightClick = false;
                    ResetStates();
                    return;
                case (int)AIStates.explosion:   //爆炸
                    Projectile.velocity *= 0.94f;
                    Projectile.rotation += Projectile.velocity.Length() * 0.05f;

                    if (Timer < 25)
                        break;

                    if (Timer == 25)
                    {
                        Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1.5f);
                        Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceBurstHalo_Reverse>(), Scale: 1f);
                        Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.Instance.IcicleCyan, 0.8f);
                    }

                    if (Timer >= 40)
                    {
                        Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Scale: 0.6f);
                        Particle.NewParticle(Projectile.Center, Vector2.Zero, CoraliteContent.ParticleType<IceHalo>(), Scale: 0.4f);
                        for (int j = 0; j < 8; j++)
                        {
                            Dust.NewDustPerfect(Projectile.Center, DustType<CrushedIceDust>(), -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(2f, 5f),
                                Scale: Main.rand.NextFloat(1f, 1.4f));
                        }
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileType<IcicleThornExplosion>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                        Projectile.Kill();
                    }

                    break;
            }

            Timer++;
        }

        public void ResetStates()
        {
            State = 0;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<IcicleThornBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<IcicleThornBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        #endregion

        public enum AIStates : int
        {
            idle = 0,
            normalAttack = 1,
            specialAttack = 2,
            explosion = 3
        }
    }
}