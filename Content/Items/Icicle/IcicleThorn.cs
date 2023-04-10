using System;
using Coralite.Content.Buffs;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleThorn:ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public Player Owner => Main.player[Projectile.owner];
        public ref float State => ref Projectile.ai[0];
        public ref float ShootCount => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];
        public float alpha;
        public float ReadyRotation;
        public bool rightClick;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("冰魔刺");

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

            if (!CheckActive(Owner))
                return;

            //添加Buff
            Owner.AddBuff(BuffType<IcicleThornBuff>(), 2);

            NPC target = ProjectilesHelper.FindClosestEnemy(Projectile.Center, 1200f, (n) =>
                   n.CanBeChasedBy() && !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1));

            if (Timer == 0 && target != null)
            {
                if (rightClick&&ShootCount>2)
                    State = (int)AIStates.specialAttack;
               else if (rightClick)
                    State = (int)AIStates.specialAttack;
                else
                    State = (int)AIStates.normalAttack;
            }

            switch (State)
            {
                default: break;
                case (int)AIStates.idle://回到玩家身边
                    Timer = 0;
                    ProjectilesHelper.GetMyProjIndexWithSameType(Type, Projectile.whoAmI, Projectile.owner, out int index, out int totalIndexes);
                    Vector2 idlePosition = Owner.Center + new Vector2(0, -48 - totalIndexes * 2).RotatedBy(6.282 * index / totalIndexes);

                    if (Vector2.Distance(idlePosition, Projectile.position) > 2000)
                        Projectile.Center = idlePosition;

                    //XY方向都渐进目标方向
                    Projectile.direction = Projectile.spriteDirection = Owner.Center.X > Projectile.Center.X ? 1 : -1;
                    int directionY = Owner.Center.Y > Projectile.Center.Y ? 1 : -1;

                    if (Vector2.Distance(Owner.Center, Projectile.Center) > 50)
                    {
                        Helper.Movement_SimpleOneLine(ref Projectile.velocity.X, Projectile.direction, 7f, 0.2f, 0.3f, 0.97f);
                        Helper.Movement_SimpleOneLine(ref Projectile.velocity.Y, directionY, 5f, 0.2f, 0.3f, 0.97f);
                    }
                    else
                        Projectile.velocity *= 0.96f;

                    Projectile.rotation += Projectile.velocity.Length() * 0.05f + 0.1f;
                    return;
                case (int)AIStates.normalAttack://普通攻击：不断冲向敌人位置

                    if (target is null || !target.active)
                    {
                        ResetStates();
                        return;
                    }

                    do
                    {
                        if (Timer < 10)//原地旋转
                        {
                            Projectile.rotation = Helper.Lerp(Projectile.rotation, (target.Center - Projectile.Center).ToRotation(), 0.1f);
                            break;
                        }

                        if (Timer == 10)
                        {
                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 8f;
                            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                            break;
                        }

                        if (Timer < 40)
                            break;

                        if (Timer > 39 && Timer < 50)
                        {
                            Projectile.velocity *= 0.96f;
                            Projectile.rotation = Helper.Lerp(Projectile.rotation, 0, 0.1f);
                            break;
                        }

                        ResetStates();
                        return;

                    } while (false);

                    break;
                case (int)AIStates.specialAttack://特殊攻击

                    if (Timer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        ReadyRotation = Main.rand.NextFloat(-3.141f, 3.141f);
                        Projectile.netUpdate = true;
                    }

                    do
                    {
                        if (Timer < 30)//原地旋转
                        {
                            Projectile.rotation += 0.2f;
                            Projectile.velocity += ReadyRotation.ToRotationVector2() * 0.01f;
                            if (Projectile.velocity.Length() > 0.8f)
                                Projectile.velocity = ReadyRotation.ToRotationVector2() * 0.8f;

                            alpha += 4f;
                            break;
                        }

                        if (Timer == 30)
                        {
                            if (target is null || !target.active)
                            {
                                ResetStates();
                                return;
                            }

                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 9f;
                            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                            break;
                        }

                        if (Timer < 90)
                            break;

                        if (Timer > 89 && Timer < 110)
                        {
                            Projectile.velocity *= 0.96f;
                            Projectile.rotation = Helper.Lerp(Projectile.rotation, 0, 0.1f);
                            alpha -= 25;
                            if (alpha < 0)
                                alpha = 0;
                            break;
                        }

                        ResetStates();
                        rightClick = false;
                        return;

                    } while (false);
                    break;
            }

            Timer++;
        }

        public void ResetStates()
        {
            State = 0;
            Timer = 0;
            alpha = 0;
            Projectile.netUpdate = true;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<RedBinkBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<RedBinkBuff>()))
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