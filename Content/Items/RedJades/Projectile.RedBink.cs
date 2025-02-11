using Coralite.Content.Buffs;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.RedJades
{
    public class RedBink : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + Name;

        public Player Owner => Main.player[Projectile.owner];
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public float ReadyRotation;
        public bool rightClick;
        public int originDamage;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("小赤玉灵");

            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 25;
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
        private bool span;
        public void Initialize()
        {
            originDamage = Projectile.originalDamage;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            //1：无敌怪时在玩家周围围成一圈
            //2：找到敌人后不断向敌人位置冲刺
            //3：右键打断当前动作短暂蓄力后向敌人冲刺

            if (!CheckActive(Owner))
                return;

            //添加Buff
            Owner.AddBuff(BuffType<RedBinkBuff>(), 2);

            NPC target = Helper.FindClosestEnemy(Projectile.Center, 1200f, (n) =>
                   n.CanBeChasedBy() && !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1));

            if (Timer == 0 && target != null)
            {
                if (rightClick)
                    State = (int)AIStates.specialAttack;
                else
                    State = (int)AIStates.normalAttack;
            }

            switch (State)
            {
                default: break;
                case (int)AIStates.idle://回到玩家身边

                    Timer = 0;
                    Helper.GetMyProjIndexWithSameType(Type, Projectile.whoAmI, Projectile.owner, out int index, out int totalIndexes);
                    Vector2 idlePosition = Owner.Center + new Vector2(0, -48 - (totalIndexes * 2)).RotatedBy(6.282f * index / totalIndexes);

                    if (Vector2.Distance(idlePosition, Projectile.position) > 2000)
                        Projectile.Center = idlePosition;

                    //XY方向都渐进目标方向
                    Projectile.direction = Projectile.spriteDirection = idlePosition.X > Projectile.Center.X ? 1 : -1;
                    int directionY = idlePosition.Y > Projectile.Center.Y ? 1 : -1;

                    if (Math.Abs(idlePosition.X - Projectile.Center.X) < 1f)
                        Projectile.direction = 0;
                    if (Math.Abs(idlePosition.Y - Projectile.Center.Y) < 1f)
                        directionY = 0;

                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.X, Projectile.direction, 7f, 0.1f, 0.3f, 0.97f);
                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.Y, directionY, 5f, 0.1f, 0.3f, 0.97f);

                    Projectile.rotation = Projectile.velocity.ToRotation() * 0.03f;

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
                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 10f;
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

                        Projectile.originalDamage = (int)(originDamage * 1.5f);
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

                            Projectile.alpha += 4;
                            break;
                        }

                        if (Timer == 30)
                        {
                            if (target is null || !target.active)
                            {
                                ResetStates();
                                return;
                            }

                            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 14.5f;
                            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                            break;
                        }

                        if (Timer < 60)
                            break;

                        if (Timer > 59 && Timer < 80)
                        {
                            Projectile.velocity *= 0.96f;
                            Projectile.rotation = Helper.Lerp(Projectile.rotation, 0, 0.1f);
                            Projectile.alpha -= 25;
                            if (Projectile.alpha < 0)
                                Projectile.alpha = 0;
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
            Projectile.alpha = 0;
            Projectile.originalDamage = originDamage;
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (rightClick && Timer > 30 && Timer < 90)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBoom>(), hit.Damage, 5f);
        }

        #endregion

        #region Draw

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            if (rightClick)
            {
                if (Timer < 31)
                    Main.spriteBatch.Draw(mainTex, drawPos, null, new Color(248, 40, 24, (byte)Projectile.alpha), Projectile.rotation, origin, 1 + (Projectile.alpha / 255), SpriteEffects.None, 0f);
                else
                {
                    Texture2D rushTex = Request<Texture2D>(AssetDirectory.RedJadeProjectiles + "RedBinkRush").Value;
                    int color = Projectile.alpha;
                    Main.spriteBatch.Draw(rushTex, drawPos, null, new Color(color, color, color, color), Projectile.rotation, rushTex.Size() / 2, 0.8f, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(mainTex, drawPos, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        #endregion

        public enum AIStates : int
        {
            idle = 0,
            normalAttack = 1,
            specialAttack = 2
        }
    }
}
