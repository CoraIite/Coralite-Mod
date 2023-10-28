using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class NightmareRaven : ModProjectile, INightmareMinion
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmareCrow";

        private Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float PowerfulAttackCount => ref Projectile.ai[2];

        public Color drawColor;

        public int AttackState;
        private Vector2 exVec2;

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
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

        public override void OnSpawn(IEntitySource source)
        {
            drawColor = NightmarePlantera.lightPurple;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (PowerfulAttackCount > 0)
            {
                modifiers.SourceDamage += 0.25f;
            }
        }

        public override bool MinionContactDamage() => Timer > 0f && AttackState == 0;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!CheckActive(Owner))
                return;

            Projectile.timeLeft = 2;
            Owner.AddBuff(BuffType<NightmareRavenBuff>(), 2);

            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type] - 1)
                    Projectile.frame = 0;
            }

            int num2 = player.direction;
            if (Projectile.velocity.X != 0f)
                num2 = Math.Sign(Projectile.velocity.X);

            Projectile.spriteDirection = num2;

            AI_156_Think(Projectile);
        }

        /// <summary>
        /// 让弹幕think think它该干什么，主体AI
        /// ai0作为计时器和状态控制变量
        /// ai1存储目标NPC的索引
        /// localAI 分别存储弹幕中心的位置
        /// </summary>
        /// <param name="Projectile"></param>
        public void AI_156_Think(Projectile Projectile)
        {
            int attackTime = 55;
            int attackTime_less1 = attackTime - 1;

            attackTime = 61;

            if (PowerfulAttackCount > 0)
            {
                attackTime = 51;
                attackTime_less1 = 44;
            }

            Player player = Main.player[Projectile.owner];
            #region 距离玩家太远直接进入尝试开始攻击的状态（该状态下会急速回到idle点位上）
            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                Timer = 0f;
                Target = 0f;
                Projectile.netUpdate = true;
            }
            #endregion
            #region 回到玩家身边，回到玩家身边后将ai0设为0，进入尝试攻击阶段
            if (Timer == -1f)
            {
                AI_GetMyGroupIndexAndFillBlackList(Projectile, out var index, out var totalIndexesInGroup);

                Vector2 idleSpot = CircleMovement(48 + totalIndexesInGroup * 4, 28, accelFactor: 0.4f, angleFactor: 0.2f, baseRot: index * MathHelper.TwoPi / totalIndexesInGroup);
                if (Projectile.Distance(idleSpot) < 2f)
                {
                    Timer = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }
            #endregion
            #region  尝试开始攻击
            if (Projectile.ai[0] == 0f)
            {
                AI_GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out var totalIndexesInGroup2);
                CircleMovement(48 + totalIndexesInGroup2 * 4, 28, accelFactor: 0.4f, angleFactor: 0.2f, baseRot: index2 * MathHelper.TwoPi / totalIndexesInGroup2);
                if (Main.rand.NextBool(20))
                {
                    int num6 = AI_156_TryAttackingNPCs(Projectile);
                    if (num6 != -1)
                    {
                        AI_156_StartAttack(Projectile);
                        Timer = attackTime;
                        Target = num6;
                        Projectile.netUpdate = true;
                        AttackState = Main.rand.Next(0, 2);
                        exVec2 = Helper.NextVec2Dir() * Main.rand.Next(250, 300);
                        return;
                    }
                }

                return;
            }
            #endregion

            int targetIndex = (int)Target;
            if (!Main.npc.IndexInRange(targetIndex))
            {
                Timer = 0f;
                Projectile.netUpdate = true;
                return;
            }

            NPC nPC = Main.npc[targetIndex];
            if (!nPC.CanBeChasedBy(this))
            {
                Timer = 0f;
                Projectile.netUpdate = true;
                return;
            }

            Timer -= 1f;
            if (Timer >= attackTime_less1)
            {
                Projectile.velocity *= 0.8f;
                if (Timer == attackTime_less1)
                {
                    Projectile.localAI[0] = Projectile.Center.X;
                    Projectile.localAI[1] = Projectile.Center.Y;
                }

                return;
            }

            float lerpValue = Utils.GetLerpValue(attackTime_less1, 0f, Timer, clamped: true);
            Vector2 vector = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);

            if (lerpValue >= 0.5f)
                vector = Main.player[Projectile.owner].Center;

            switch (AttackState)
            {
                default:
                case 0://就只是飞过去碰撞伤害
                    {
                        Vector2 center = nPC.Center;
                        float num9 = (center - vector).ToRotation();
                        float num10 = (center.X > vector.X) ? (-(float)Math.PI) : ((float)Math.PI);
                        float num11 = num10 + (0f - num10) * lerpValue * 2f;
                        Vector2 spinningPoint = num11.ToRotationVector2();
                        spinningPoint.Y *= (float)Math.Sin(Projectile.identity * 2.3f) * 0.5f;
                        spinningPoint = spinningPoint.RotatedBy(num9);
                        float num12 = (center - vector).Length() / 2f;
                        Vector2 center2 = Vector2.Lerp(vector, center, 0.5f) + spinningPoint * num12;
                        Projectile.Center = center2;
                        Vector2 vector2 = MathHelper.WrapAngle(num9 + num11 + 0f).ToRotationVector2() * 10f;
                        Projectile.velocity = vector2;
                        Projectile.position -= Projectile.velocity;
                        if (Timer == 0f)
                        {
                            int num13 = AI_156_TryAttackingNPCs(Projectile);
                            if (num13 != -1)
                            {
                                Timer = attackTime;
                                Target = num13;
                                AI_156_StartAttack(Projectile);
                                Projectile.netUpdate = true;
                                AttackState = Main.rand.Next(0, 2);
                                exVec2 = Helper.NextVec2Dir() * Main.rand.Next(250, 300);
                                return;
                            }

                            Target = 0f;
                            Projectile.netUpdate = true;
                        }
                    }

                    break;
                case 1://飞到附近射弹幕
                    {
                        Vector2 center = nPC.Center + exVec2;
                        float num9 = (center - vector).ToRotation();
                        float num10 = (center.X > vector.X) ? (-(float)Math.PI) : ((float)Math.PI);
                        float num11 = num10 + (0f - num10) * lerpValue * 2f;
                        Vector2 spinningPoint = num11.ToRotationVector2();
                        spinningPoint.Y *= (float)Math.Sin(Projectile.identity * 2.3f) * 0.5f;
                        spinningPoint = spinningPoint.RotatedBy(num9);
                        float num12 = (center - vector).Length() / 2f;
                        Vector2 center2 = Vector2.Lerp(vector, center, 0.5f) + spinningPoint * num12;
                        Projectile.Center = center2;
                        Vector2 vector2 = MathHelper.WrapAngle(num9 + num11 + 0f).ToRotationVector2() * 10f;
                        Projectile.velocity = vector2;
                        Projectile.position -= Projectile.velocity;

                        if (Timer >= attackTime_less1 / 2 && Timer % (attackTime_less1 / 6) == 0)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (nPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero),
                                ProjectileType<RavenFeather>(), (int)(Projectile.damage * 0.7f), 2, Projectile.owner, ai1: PowerfulAttackCount > 0 ? 1 : 0);
                        }

                        if (Timer == 0f)
                        {
                            int num13 = AI_156_TryAttackingNPCs(Projectile);
                            if (num13 != -1)
                            {
                                Timer = attackTime;
                                Target = num13;
                                AI_156_StartAttack(Projectile);
                                Projectile.netUpdate = true;
                                AttackState = 0;
                                return;
                            }

                            Target = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                    break;
            }

            if (lerpValue >= 0.5f)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustType<GlowBall>(),
                        -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.NextFloat(0.1f, 0.25f), 0, drawColor, Main.rand.NextFloat(0.2f, 0.45f));
                }

            }

        }

        /// <summary>
        /// 说是开始攻击，然鹅实际上是清空自身所有的本地NPC无敌帧
        /// </summary>
        /// <param name="Projectile"></param>
        public void AI_156_StartAttack(Projectile Projectile)
        {
            if (PowerfulAttackCount > 0)
                PowerfulAttackCount--;
            drawColor = PowerfulAttackCount > 0 ? NightmarePlantera.nightmareRed : NightmarePlantera.lightPurple;

            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }

        /// <summary>
        /// 获取目标NPC的索引
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="blackListedTargets"></param>
        /// <param name="skipBodyCheck"></param>
        /// <returns></returns>
        public int AI_156_TryAttackingNPCs(Projectile Projectile, bool skipBodyCheck = false)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;
            //如果有锁定的NPC那么就用锁定的，没有或不符合条件在从所有NPC里寻找
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                bool flag = true;
                if (!ownerMinionAttackTargetNPC.boss)
                    flag = false;

                if (ownerMinionAttackTargetNPC.Distance(ownerCenter) > 1000f)
                    flag = false;

                if (!skipBodyCheck && !Projectile.CanHitWithOwnBody(ownerMinionAttackTargetNPC))
                    flag = false;

                if (flag)
                    return ownerMinionAttackTargetNPC.whoAmI;
            }

            for (int i = 0; i < 200; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float npcDistance2Owner = nPC.Distance(ownerCenter);
                    if (npcDistance2Owner <= 1000f && (npcDistance2Owner <= num || num == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(nPC)))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 计算出在玩家身边的位置
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="stackedIndex"></param>
        /// <param name="totalIndexes"></param>
        /// <param name="idleSpot"></param>
        /// <param name="idleRotation"></param>
        public void AI_156_GetIdlePosition(Projectile Projectile, int stackedIndex, int totalIndexes, out Vector2 idleSpot, out float idleRotation)
        {
            Player player = Main.player[Projectile.owner];

            float num2 = (totalIndexes - 1f) / 2f;
            idleSpot = player.Center + -Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 40f;
            idleRotation = 0f;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<NightmareRavenBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<NightmareRavenBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        public Vector2 CircleMovement(float distance, float speedMax, float accelFactor = 0.25f, float rollingFactor = 5f, float angleFactor = 0.08f, float baseRot = 0f)
        {
            Vector2 center = Owner.Center + (baseRot + Main.GlobalTimeWrappedHourly / rollingFactor * MathHelper.TwoPi).ToRotationVector2() * distance;
            Vector2 dir = center - Projectile.Center;

            float velRot = Projectile.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = Projectile.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 200f, 0, 1) * speedMax;

            Projectile.velocity = velRot.AngleTowards(targetRot, angleFactor).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
            return center;
        }


        /// <summary>
        /// 获取自身是第几个召唤物弹幕
        /// 非常好的东西，建议稍微改改变成静态帮助方法
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        public void AI_GetMyGroupIndexAndFillBlackList(Projectile Projectile, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.type == Projectile.type && (projectile.type != 759 || projectile.frame == Main.projFrames[projectile.type] - 1))
                {
                    if (Projectile.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        public void GetPower(int howMany)
        {
            PowerfulAttackCount += howMany;
            if (PowerfulAttackCount > Owner.GetModPlayer<CoralitePlayer>().nightmareEnergyMax)
                PowerfulAttackCount = Owner.GetModPlayer<CoralitePlayer>().nightmareEnergyMax;

            if (PowerfulAttackCount > 0)
                drawColor = NightmarePlantera.nightmareRed;

            if (howMany > 0)
            {
                float angle = Main.rand.NextFloat(6.282f);
                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, angle.ToRotationVector2() * Main.rand.NextFloat(1f, 4f), newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                    angle += MathHelper.TwoPi / 12;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 origin = frameBox.Size() / 2;
            SpriteEffects effect = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //绘制残影
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 7; i += 1)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (0.5f - i * 0.5f / 7), Projectile.oldRot[i], frameBox.Size() / 2, 0.7f, effect, 0);

            //向上下左右四个方向绘制一遍
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (i * MathHelper.PiOver2).ToRotationVector2() * 4, frameBox, drawColor, Projectile.rotation, origin, 0.7f,
                   effect, 0);
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, Projectile.rotation, origin, 0.7f, effect, 0);
            return false;
        }
    }

    public class RavenFeather : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public bool init = true;
        private Color drawColor;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2400;
            Projectile.extraUpdates = 1;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (init)
            {
                if (Projectile.ai[1] == 1)
                {
                    drawColor = NightmarePlantera.nightmareRed;
                }
                else
                    drawColor = NightmarePlantera.nightPurple;
            }

            //Projectile.rotation += Projectile.ai[0];
            //if (Projectile.ai[0] < 0.3f)
            //{
            //    Projectile.ai[0] += 0.03f;
            //}

            if (Projectile.velocity.Length() < 14)
            {
                Projectile.velocity += Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.125f;
            }

            float dir2 = ((Projectile.timeLeft % 30) > 15 ? -1 : 1) * 0.0125f;
            Projectile.velocity = Projectile.velocity.RotatedBy(dir2);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.rand.NextBool(8))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, drawColor, Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 32), DustID.VilePowder, Helper.NextVec2Dir() * Main.rand.NextFloat(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            //Rectangle frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            Vector2 origin = mainTex.Size() / 2;
            //绘制残影
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 10; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (0.5f - i * 0.5f / 10), Projectile.oldRot[i], origin, 1, 0, 0);

            //向上下左右四个方向绘制一遍
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(mainTex, pos + (i * MathHelper.PiOver2).ToRotationVector2() * 2, null, drawColor, Projectile.rotation, origin, 1,
                   0, 0);
            }

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, null, Color.Gray, Projectile.rotation, origin, 1, 0, 0);
            return false;
        }
    }
}
