using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Coralite.Content.Buffs;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Projectiles.Projectile_Summon
{
    public class MushroomDragon : ModProjectile
    {
        public override string Texture => AssetDirectory.Projectiles_Summon + Name;

        internal ref float Timer => ref Projectile.ai[0];
        internal ref float PetAIState => ref Projectile.ai[0];
        internal ref float State => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        public bool rightClick;
        public float targetLocation_X;
        public int frameCounter;
        public int baseDamage;

        private Vector2[] oldPosi = new Vector2[10];

        public static float gravityAccel = 0.4f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("蘑菇幼龙");

            Main.projFrames[Type] = 7;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.alpha = 0;

            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 300;

            Projectile.minionSlots = 1;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;

        }

        /// <summary>
        /// 接触伤害,false没有,true有
        /// </summary>
        /// <returns></returns>
        public override bool MinionContactDamage() => true;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 20; //等效于普通无敌帧，但显示表示用于兼容多人模式
        }

        #region AI
        private enum AIStates
        {
            AIStates_Idle = 0,
            AIStates_Attack = 1,
            AIStates_RightClick = 2,
        }

        public override void AI()
        {
            // 玩家死亡会让召唤物消失
            if (!CheckActive(Owner))
            {
                return;
            }

            //添加Buff
            Owner.AddBuff(BuffType<MushroomDragonBuff>(), 2);
            Projectile.netUpdate = true;

            //寻敌
            NPC target = ProjectilesHelper.FindCloestEnemy(Projectile.Center, 1200f, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1);
            });

            StateController(target, Owner);
            //控制伤害 因为不知道如何比较好的控制所以写在这里了，而且只能在这里暴力更改数值来达到目的。。。
            if (State == (int)AIStates.AIStates_RightClick)
                Projectile.originalDamage = 12;
            else
                Projectile.originalDamage = 6;

            switch (State)
            {
                case (int)AIStates.AIStates_Idle:
                    float idleOffset_X = Projectile.minionPos * (Projectile.width + 16);
                    PetAI(idleOffset_X);
                    PetAIFrame();
                    break;
                case (int)AIStates.AIStates_Attack:
                    Attack(target, 8f, 100f);
                    AttackFrame(30);
                    break;
                case (int)AIStates.AIStates_RightClick:
                    RightClick(target, 10f, 150f);
                    AttackFrame(60);
                    break;
                default:
                    goto case (int)AIStates.AIStates_Idle;
            }
        }

        private void StateController(NPC target, Player owner)
        {
            if (target == null || Projectile.Distance(owner.Center) > 2000f)// 如果周围没有敌人
            {
                Timer = 0;
                rightClick = false;
                State = (int)AIStates.AIStates_Idle;
                return;
            }

            //如果有敌人
            if (rightClick == true)
            {
                if (State != (int)AIStates.AIStates_RightClick)
                    Timer = 0;
                State = (int)AIStates.AIStates_RightClick;
                return;
            }
            State = (int)AIStates.AIStates_Attack;
        }

        private void PetAI(float offset, int closedLenth = 500)
        {
            Player owner = Main.player[Projectile.owner];

            bool projInRight = false;
            bool projInLeft = false;
            bool FaceSoildTile = false;

            int ClosedLenth = closedLenth;
            int Lenth_85 = 85;

            // 判断弹幕在人物左还是右
            if (owner.Center.X - owner.direction * offset < Projectile.Center.X - Lenth_85)
            {
                projInRight = true;
            }
            else if (owner.Center.X - owner.direction * offset > Projectile.Center.X + Lenth_85)
            {
                projInLeft = true;
            }

            Vector2 Center = Projectile.Center;

            float DistanceToOwner_X = owner.Center.X - owner.direction * offset - Center.X;
            float DistanceToOwner_Y = owner.Center.Y - Center.Y;
            float DistanceToOwner = (float)Math.Sqrt(DistanceToOwner_X * DistanceToOwner_X + DistanceToOwner_Y * DistanceToOwner_Y);

            if (DistanceToOwner > 2000f)//距离太远直接传送
            {
                Projectile.Center = owner.Center;
            }
            else if (DistanceToOwner > ClosedLenth || Math.Abs(DistanceToOwner_Y) > 300f)//距离远了
            {
                if (DistanceToOwner_Y > 0f && Projectile.velocity.Y < 0f)
                    Projectile.velocity.Y = 0f;

                if (DistanceToOwner_Y < 0f && Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = 0f;

                PetAIState = 1;
            }

            if (PetAIState != 0)
            {
                Projectile.tileCollide = false;
                float accel = 0.2f;

                if (Projectile.velocity.X < DistanceToOwner_X)
                {
                    Projectile.velocity.X += accel;
                    if (Projectile.velocity.X < 0f)
                        Projectile.velocity.X += accel * 1.5f;
                }

                if (Projectile.velocity.X > DistanceToOwner_X)
                {
                    Projectile.velocity.X -= accel;
                    if (Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= accel * 1.5f;
                }

                if (Projectile.velocity.Y < DistanceToOwner_Y)
                {
                    Projectile.velocity.Y += accel;
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += accel * 1.5f;
                }

                if (Projectile.velocity.Y > DistanceToOwner_Y)
                {
                    Projectile.velocity.Y -= accel;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= accel * 1.5f;
                }

                if (Projectile.velocity.X > 0.5)
                    Projectile.spriteDirection = -1;
                else if (Projectile.velocity.X < -0.5)
                    Projectile.spriteDirection = 1;

                Projectile.rotation = 0f;
            }
            else
            {
                Projectile.rotation = 0f;
                Projectile.tileCollide = true;

                float accel = 0.08f;
                float speedmax = 6.5f;

                //向玩家移动
                if (projInRight)
                {
                    if (Projectile.velocity.X > -3.5)
                        Projectile.velocity.X -= accel;
                    else
                        Projectile.velocity.X -= accel * 0.25f;
                }
                else if (projInLeft)
                {
                    if (Projectile.velocity.X < 3.5)
                        Projectile.velocity.X += accel;
                    else
                        Projectile.velocity.X += accel * 0.25f;
                }
                else//距离玩家较近减速
                {
                    Projectile.velocity.X *= 0.9f;
                    if (Projectile.velocity.X >= 0f - accel && Projectile.velocity.X <= accel)
                        Projectile.velocity.X = 0f;
                }
                //判断面前是否有方块
                if (projInRight || projInLeft)
                {
                    int ProjToTile_X = (int)Projectile.Center.X / 16;
                    int ProjToTile_Y = (int)Projectile.Center.Y / 16;

                    if (projInRight)
                        ProjToTile_X--;

                    if (projInLeft)
                        ProjToTile_X++;

                    ProjToTile_X += (int)Projectile.velocity.X;
                    if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y))
                        FaceSoildTile = true;
                }

                //上台阶
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                bool LowerToOwner;
                if (owner.Bottom.Y - 8f < Projectile.Bottom.Y)
                    LowerToOwner = true;
                else
                    LowerToOwner = false;

                if (!LowerToOwner && (Projectile.velocity.X < 0f || Projectile.velocity.X > 0f))
                {
                    int ProjToTile_X = (int)Projectile.Center.X / 16;
                    int ProjToTile_YPlus1 = (int)Projectile.Center.Y / 16 + 1;
                    if (projInRight)
                        ProjToTile_X--;

                    if (projInLeft)
                        ProjToTile_X++;

                    WorldGen.SolidTile(ProjToTile_X, ProjToTile_YPlus1);
                }

                if (Projectile.velocity.Y == 0f)
                {
                    //面前有方块时起跳
                    if (FaceSoildTile)
                    {
                        int ProjToTile_X = (int)Projectile.Center.X / 16;
                        int ProjToTile_Y = (int)Projectile.Bottom.Y / 16;
                        if (WorldGen.SolidTileAllowBottomSlope(ProjToTile_X, ProjToTile_Y) || Main.tile[ProjToTile_X, ProjToTile_Y].IsHalfBlock || (byte)Main.tile[ProjToTile_X, ProjToTile_Y].Slope > 0)
                        {
                            try
                            {
                                ProjToTile_X = (int)Projectile.Center.X / 16;
                                ProjToTile_Y = (int)Projectile.Center.Y / 16;
                                if (projInRight)
                                    ProjToTile_X--;

                                if (projInLeft)
                                    ProjToTile_X++;

                                ProjToTile_X += (int)Projectile.velocity.X;
                                if (!WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 1) && !WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 2))
                                    Projectile.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 2))
                                    Projectile.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 5))
                                    Projectile.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(ProjToTile_X, ProjToTile_Y - 4))
                                    Projectile.velocity.Y = -10.1f;
                                else
                                    Projectile.velocity.Y = -9.1f;
                            }
                            catch
                            {
                                Projectile.velocity.Y = -9.1f;
                            }
                        }
                    }
                }
                else if (Projectile.velocity.Y != 0f && LowerToOwner)
                {
                    Projectile.velocity.Y -= 0.61f;

                    int ProjToTile_X = (int)Projectile.Top.X / 16;
                    int ProjToTilePlus1_Y = (int)Projectile.Top.Y / 16 - 1;
                    if (WorldGen.SolidTile(ProjToTile_X, ProjToTilePlus1_Y))
                        Projectile.tileCollide = false;
                    else
                        Projectile.tileCollide = true;
                }

                if (Projectile.velocity.X > speedmax)
                    Projectile.velocity.X = speedmax;

                if (Projectile.velocity.X < 0f - speedmax)
                    Projectile.velocity.X = 0f - speedmax;

                if (Projectile.velocity.X != 0f)
                    Projectile.direction = Math.Sign(Projectile.velocity.X);
                else
                    Projectile.direction = Math.Sign(owner.position.X - Projectile.position.X);

                if (Projectile.velocity.X > accel || projInLeft)
                    Projectile.direction = 1;

                if (Projectile.velocity.X < 0f - accel || projInRight)
                    Projectile.direction = -1;

                Projectile.spriteDirection = -Projectile.direction;

                //重力以及最大速度
                Gravity(gravityAccel);
            }
        }

        private void Attack(NPC target, float speed, float distance)
        {
            Timer++;
            if (Timer < 30)
            {
                Projectile.velocity.X *= 0.8f;
                Gravity(gravityAccel);
            }
            else if (Timer == 30)//瞄准
            {
                int targetDirection = Math.Sign(target.Center.X - Projectile.Center.X);
                targetLocation_X = target.Center.X + targetDirection * distance;
                Projectile.direction = Projectile.spriteDirection = -targetDirection;
                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0)
                    Projectile.velocity.Y += -3f;
                Projectile.netUpdate = true;
            }
            //开冲！
            else
            {
                float target_X = Math.Sign(targetLocation_X - Projectile.Center.X);
                Projectile.velocity.X = target_X * speed;
                Gravity(gravityAccel);
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

                //冲到位了
                if (Math.Abs(targetLocation_X - Projectile.Center.X) < 16f || Timer > 600)
                {
                    Projectile.velocity *= 0.5f;
                    Timer = 0;
                }
            }
        }

        private void RightClick(NPC target, float speed, float distance)
        {
            Timer++;
            if (Main.netMode != NetmodeID.Server && Main.rand.NextBool(5))
            {
                //生成粒子
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64f, 64f), DustID.Firework_Yellow, null, 0);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity;
            }
            //瞄准
            if (Timer < 60)
            {
                Projectile.velocity.X *= 0.8f;
            }
            else if (Timer == 60)
            {
                int targetDirection = Math.Sign(target.Center.X - Projectile.Center.X);
                targetLocation_X = target.Center.X + targetDirection * distance;
                Projectile.direction = Projectile.spriteDirection = -targetDirection;
                Projectile.rotation = 0f;
                if (Projectile.velocity.Y == 0)
                    Projectile.velocity.Y += -3f;
                Projectile.netUpdate = true;
            }
            //开冲！
            else
            {
                float target_X = Math.Sign(targetLocation_X - Projectile.Center.X);
                Projectile.velocity.X = target_X * speed;
                Gravity(gravityAccel);
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);


                for (int i = oldPosi.Length - 1; i > 0; i--)
                {
                    oldPosi[i] = oldPosi[i - 1];
                }
                oldPosi[0] = Projectile.Center;


                //冲到位了
                if (Math.Abs(targetLocation_X - Projectile.Center.X) < 16f || Timer > 600)
                {
                    rightClick = false;
                    Projectile.velocity.X *= 0.5f;
                    Timer = 0;
                }
            }
        }

        private void PetAIFrame()
        {
            frameCounter++;

            if (Projectile.velocity.Y != gravityAccel)//不在地面上时
            {
                if (frameCounter < 30)
                    Projectile.frame = 6;
                else if (frameCounter < 60)
                    Projectile.frame = 5;
                else
                    frameCounter = 0;
                return;
            }

            if (Projectile.velocity.X != 0)//水平方向有速度时
            {
                frameCounter += (int)(Projectile.velocity.X / 10f);
                if (frameCounter > 5)
                {
                    frameCounter = 0;
                    switch (Projectile.frame)
                    {
                        case 0:
                            Projectile.frame = 4;
                            break;
                        case 3:
                            Projectile.frame = 0;
                            break;
                        case 4:
                            Projectile.frame = 3;
                            break;
                        default:
                            goto case 0;
                    }
                }
                return;
            }

            if (frameCounter <= 120 || frameCounter >= 135)
            {
                Projectile.frame = 0;
                if (frameCounter >= 135)
                    frameCounter = 0;
            }
            else if (frameCounter <= 125 || frameCounter >= 130)
                Projectile.frame = 1;
            else
                Projectile.frame = 2;
        }

        private void AttackFrame(int TimerPoint)
        {
            if (Timer > TimerPoint)
                Projectile.frame = 3;
            else
                Projectile.frame = 0;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<MushroomDragonBuff>());

                return false;
            }

            if (owner.HasBuff(BuffType<MushroomDragonBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void Gravity(float gravityAccel)
        {
            Projectile.velocity.Y += gravityAccel;
            if (Projectile.velocity.Y > 10f)
                Projectile.velocity.Y = 10f;
        }
        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Request<Texture2D>(Texture).Value;

            int frameWidth = mainTex.Width;
            int frameHeight = mainTex.Height / Main.projFrames[Type];
            Rectangle frameBox = new Rectangle(0, Projectile.frame * frameHeight, frameWidth, frameHeight);

            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

            if (Projectile.spriteDirection != 1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, Color.White, Projectile.rotation, origin, Projectile.scale, effects, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (State == (int)AIStates.AIStates_RightClick && Timer > 61)
            {
                Texture2D mainTex = Request<Texture2D>(Texture).Value;

                int frameWidth = mainTex.Width;
                int frameHeight = mainTex.Height / Main.projFrames[Type];
                Rectangle frameBox = new Rectangle(0, Projectile.frame * frameHeight, frameWidth, frameHeight);

                SpriteEffects effects = SpriteEffects.None;
                Vector2 origin = new Vector2(frameWidth / 2, frameHeight / 2);

                if (Projectile.spriteDirection != 1)
                {
                    effects = SpriteEffects.FlipHorizontally;
                }

                for (int i = oldPosi.Length - 1; i > 0; i--)
                {
                    if (oldPosi[i] != Vector2.Zero)
                    {
                        Main.spriteBatch.Draw(mainTex, oldPosi[i] - Main.screenPosition, frameBox, Color.White * 1 * (1 - 0.1f * i), Projectile.rotation, origin, 1 * (0.5f - 0.05f * i), effects, 0);
                    }
                }
            }
        }
    }
}
