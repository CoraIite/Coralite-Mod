using System;
using System.Collections.Generic;
using Terraria;

namespace Coralite.lib
{
    public class AI_156_BatOfLight
    {
        private AI_156_BatOfLight()
        {

        }

        //这是个NPC目标黑名单。
        //它确确实实是能起到防止特定NPC被追踪的作用的，但是并没有在这个AI中将任何一个NPC加入到列表中
        //简单讲就是有用，但这里没用到
        public List<int> _ai156_blacklistedTargets = new List<int>();

        //泰拉棱镜和血蝙蝠的AI
        public void AI_BatOfLight(Projectile Projectile)
        {
            List<int> ai156_blacklistedTargets = _ai156_blacklistedTargets;
            Player player = Main.player[Projectile.owner];
            bool batOfLight = Projectile.type == 755;
            bool terraprisma = Projectile.type == 946;
            #region 血蝙蝠部分
            if (batOfLight)
            {
                if (player.dead)
                    player.batsOfLight = false;

                if (player.batsOfLight)
                    Projectile.timeLeft = 2;

                DelegateMethods.v3_1 = AI_156_GetColor(Projectile).ToVector3();
                Point point = Projectile.Center.ToTileCoordinates();
                DelegateMethods.CastLightOpen(point.X, point.Y);
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
            }
            #endregion
            if (terraprisma)    //泰拉棱镜部分
            {
                if (player.dead)
                    player.empressBlade = false;

                if (player.empressBlade)
                    Projectile.timeLeft = 2;

                DelegateMethods.v3_1 = AI_156_GetColor(Projectile).ToVector3(); //有啥用？我也不到啊
                Point point2 = Projectile.Center.ToTileCoordinates();
                DelegateMethods.CastLightOpen(point2.X, point2.Y);
            }

            ai156_blacklistedTargets.Clear();
            AI_156_Think(Projectile, ai156_blacklistedTargets);
        }

        public Color AI_156_GetColor(Projectile Projectile)
        {
            if (Projectile.aiStyle != 156)
                return Color.Transparent;

            bool num = Projectile.type == 755;
            _ = Projectile.type;
            if (num)
                return Color.Crimson;

            return Color.Transparent;
        }

        /// <summary>
        /// 让弹幕think think它该干什么，主体AI
        /// ai0作为计时器和状态控制变量
        /// ai1存储目标NPC的索引
        /// localAI 分别存储弹幕中心的位置
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="blacklist"></param>
        public void AI_156_Think(Projectile Projectile, List<int> blacklist)
        {
            bool batOfLight = Projectile.type == 755;
            bool terraprisma = Projectile.type == 946;
            int halfTime = 60;
            int halfTime_less1 = halfTime - 1;
            int startTime = halfTime * 2;
            int startTime_less1 = startTime - 1;
            int halfTime_Plus1 = halfTime + 1;
            #region 血蝙蝠
            if (batOfLight)
                halfTime = 66;
            #endregion
            if (terraprisma)        //泰拉棱镜的攻击Timer
            {
                halfTime = 40;
                halfTime_less1 = halfTime - 1;
                startTime = halfTime * 2;
                startTime_less1 = startTime - 1;
                halfTime_Plus1 = halfTime + 1;
            }

            Player player = Main.player[Projectile.owner];
            #region 距离玩家太远直接进入尝试开始攻击的状态（该状态下会急速回到idle点位上）
            if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
            {
                Projectile.ai[0] = 0f;
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            #endregion
            #region 回到玩家身边，回到玩家身边后将ai0设为0，进入尝试攻击阶段
            if (Projectile.ai[0] == -1f)
            {
                AI_GetMyGroupIndexAndFillBlackList(Projectile, out var index, out var totalIndexesInGroup);
                AI_156_GetIdlePosition(Projectile, index, totalIndexesInGroup, out var idleSpot, out var idleRotation);
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation, 0.2f);
                if (Projectile.Distance(idleSpot) < 2f)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }
            #endregion
            #region  尝试开始攻击
            if (Projectile.ai[0] == 0f)
            {
                #region 血蝙蝠部分
                if (batOfLight)
                {
                    AI_GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out var totalIndexesInGroup2);
                    AI_156_GetIdlePosition(Projectile, index2, totalIndexesInGroup2, out var idleSpot2, out var _);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot2, 0.45f);
                    if (Main.rand.NextBool(20))
                    {
                        int num6 = AI_156_TryAttackingNPCs(Projectile, blacklist);
                        if (num6 != -1)
                        {
                            AI_156_StartAttack(Projectile);
                            Projectile.ai[0] = halfTime;
                            Projectile.ai[1] = num6;
                            Projectile.netUpdate = true;
                            return;
                        }
                    }
                }
                #endregion
                if (!terraprisma)
                    return;

                AI_GetMyGroupIndexAndFillBlackList(Projectile, out var thisProjIndex, out var totalIndexesInGroup3);
                AI_156_GetIdlePosition(Projectile, thisProjIndex, totalIndexesInGroup3, out var idleSpot3, out var idleRotation3);
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.SmoothStep(Projectile.Center, idleSpot3, 0.45f);
                Projectile.rotation = Projectile.rotation.AngleLerp(idleRotation3, 0.45f);
                if (Main.rand.NextBool(20))
                {
                    int targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, blacklist);
                    if (targetNPCIndex != -1)
                    {
                        AI_156_StartAttack(Projectile);
                        Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                        Projectile.ai[0] = startTime;            //什么玩意 不知为何重复赋了2次值 ？？？？？
                        Projectile.ai[1] = targetNPCIndex;
                        Projectile.netUpdate = true;
                    }
                }

                return;
            }
            #endregion

            #region 血蝙蝠的攻击部分AI
            if (batOfLight)
            {
                int num8 = (int)Projectile.ai[1];
                if (!Main.npc.IndexInRange(num8))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }

                NPC nPC = Main.npc[num8];
                if (!nPC.CanBeChasedBy(this))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    return;
                }

                Projectile.ai[0] -= 1f;
                if (Projectile.ai[0] >= halfTime_less1)
                {
                    Projectile.velocity *= 0.8f;
                    if (Projectile.ai[0] == halfTime_less1)
                    {
                        Projectile.localAI[0] = Projectile.Center.X;
                        Projectile.localAI[1] = Projectile.Center.Y;
                    }

                    return;
                }

                float lerpValue = Utils.GetLerpValue(halfTime_less1, 0f, Projectile.ai[0], clamped: true);
                Vector2 vector = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                if (lerpValue >= 0.5f)
                    vector = Main.player[Projectile.owner].Center;

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
                if (Projectile.ai[0] == 0f)
                {
                    int num13 = AI_156_TryAttackingNPCs(Projectile, blacklist);
                    if (num13 != -1)
                    {
                        Projectile.ai[0] = halfTime;
                        Projectile.ai[1] = num13;
                        AI_156_StartAttack(Projectile);
                        Projectile.netUpdate = true;
                        return;
                    }

                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            #endregion

            if (!terraprisma)       //以下是泰拉棱镜的攻击AI
                return;

            bool skipBodyCheck = true;

            int AttackState = 0;
            int onStartOrHalfTime = halfTime_less1;
            int zeroOrHalf = 0;
            if (Projectile.ai[0] >= halfTime_Plus1)
            {
                AttackState = 1;
                onStartOrHalfTime = startTime_less1;
                zeroOrHalf = halfTime_Plus1;
            }

            int targetNPCIndex_ = (int)Projectile.ai[1];

            #region 各种防止NPC不存在或是不可攻击所做的...预防？
            //如果目标NPC不存在重新寻找一次，如果仍然未找到的话就进入回到玩家身边的阶段（ai0设为-1）
            if (!Main.npc.IndexInRange(targetNPCIndex_))
            {
                int _targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, blacklist, skipBodyCheck);
                if (_targetNPCIndex != -1)
                {
                    Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                    Projectile.ai[1] = _targetNPCIndex;
                    AI_156_StartAttack(Projectile);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }

            NPC target = Main.npc[targetNPCIndex_];

            //如果目标NPC不能被攻击到，那么重新寻找，具体内容和上面的一模一样 就这么喜欢复制黏贴是吧？
            if (!target.CanBeChasedBy(this))
            {
                int _targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, blacklist, skipBodyCheck);
                if (_targetNPCIndex != -1)
                {
                    Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                    AI_156_StartAttack(Projectile);
                    Projectile.ai[1] = _targetNPCIndex;
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }

                return;
            }
            #endregion

            //计时器递减
            Projectile.ai[0] -= 1f;
            //这部分仅有在ai0等于startTime以及HalfTime，还有他们的下一帧的时候才会执行，什么傻逼写法啊，看了好久才看明白！！！
            if (Projectile.ai[0] >= onStartOrHalfTime)
            {
                Projectile.direction = (Projectile.Center.X < target.Center.X) ? 1 : (-1);
                if (Projectile.ai[0] == onStartOrHalfTime)      //这里是上面写的“他们的下一帧”的时候才会执行的
                {
                    Projectile.localAI[0] = Projectile.Center.X;
                    Projectile.localAI[1] = Projectile.Center.Y;
                }
            }

            //这玩意返回值是 (Projectile.ai[0] - onStartOrHalfTime) / (zeroOrHalf - onStartOrHalfTime) 额 ......总之是非常神必的算法 真是难看懂啊
            //翻译一下可以写成    当前状态下已经过去了的时间 / 一半的时间，本质上还是个从0到1的插值...只是写法神奇
            //然鹅整的这么复杂不如直接吧clamped设为默认的false，然后把两个时间反过来就好了（我认为是这样）
            float lerpValue2 = Utils.GetLerpValue(onStartOrHalfTime, zeroOrHalf, Projectile.ai[0], clamped: true);
            #region 泰拉棱镜的挥舞
            if (AttackState == 0)
            {
                Vector2 originCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                if (lerpValue2 >= 0.5f)     //如果该状态时间已经过去一半
                    originCenter = Vector2.Lerp(target.Center, Main.player[Projectile.owner].Center, 0.5f);

                Vector2 targetCenter = target.Center;
                float center2TargetRotate = (targetCenter - originCenter).ToRotation();
                float Pi = (Projectile.direction == 1) ? (-(float)Math.PI) : ((float)Math.PI);

                //nnd 这一大堆是什么玩意 反正我是真没看懂
                float num22 = Pi + (0f - Pi) * lerpValue2 * 2f;
                Vector2 spinningPoint2 = num22.ToRotationVector2();
                spinningPoint2.Y *= 0.5f;
                spinningPoint2.Y *= 0.8f + (float)Math.Sin(Projectile.identity * 2.3f) * 0.2f;
                spinningPoint2 = spinningPoint2.RotatedBy(center2TargetRotate);
                float target2CenterLengthOver2 = (targetCenter - originCenter).Length() / 2f;
                Vector2 realCenter = Vector2.Lerp(originCenter, targetCenter, 0.5f) + spinningPoint2 * target2CenterLengthOver2;
                Projectile.Center = realCenter;
                float num24 = MathHelper.WrapAngle(center2TargetRotate + num22 + 0f);
                Projectile.rotation = num24 + (float)Math.PI / 2f;
                Vector2 vector4 = num24.ToRotationVector2() * 10f;
                Projectile.velocity = vector4;
                Projectile.position -= Projectile.velocity;
            }
            #endregion
            #region 泰拉棱镜的冲刺
            if (AttackState == 1)
            {
                //饿啊 太复杂了 实在是太难看懂了
                //有需要的自己尝试分析分析吧
                //在一般时间之前先缓慢地靠近
                //之后快速突刺到目标身后
                Vector2 originCenter = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                originCenter += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerpValue2, clamped: true) * -100f);
                Vector2 v = target.Center - originCenter;
                Vector2 vector6 = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
                Vector2 value = target.Center + vector6;
                float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, clamped: true);
                float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, clamped: true);
                float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation() + (float)Math.PI / 2f;
                Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f);
                Projectile.Center = Vector2.Lerp(originCenter, target.Center, lerpValue3);
                if (lerpValue4 > 0f)
                    Projectile.Center = Vector2.Lerp(target.Center, value, lerpValue4);
            }
            #endregion
            #region 上一个状态结束时的重新寻找NPC，以及随机状态
            if (Projectile.ai[0] == zeroOrHalf)
            {
                int targetNPCIndex = AI_156_TryAttackingNPCs(Projectile, blacklist, skipBodyCheck);
                if (targetNPCIndex != -1)
                {
                    //随机一个攻击状态，冲刺或是挥砍
                    Projectile.ai[0] = Main.rand.NextFromList(halfTime, startTime);
                    Projectile.ai[1] = targetNPCIndex;
                    AI_156_StartAttack(Projectile);
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.ai[0] = -1f;
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
            }
            #endregion
        }

        /// <summary>
        /// 说是开始攻击，然鹅实际上是清空自身所有的本地NPC无敌帧
        /// </summary>
        /// <param name="Projectile"></param>
        public void AI_156_StartAttack(Projectile Projectile)
        {
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
        public int AI_156_TryAttackingNPCs(Projectile Projectile, List<int> blackListedTargets, bool skipBodyCheck = false)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;
            //如果有锁定的NPC那么就用锁定的，没有或不符合条件在从所有NPC里寻找
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(this))
            {
                bool flag = true;
                if (!ownerMinionAttackTargetNPC.boss && blackListedTargets.Contains(ownerMinionAttackTargetNPC.whoAmI))
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
                if (nPC.CanBeChasedBy(Projectile) && (nPC.boss || !blackListedTargets.Contains(i)))
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
            bool batOfLight = Projectile.type == 755;
            bool terraprisma = Projectile.type == 946;
            idleRotation = 0f;
            idleSpot = Vector2.Zero;
            #region 血蝙蝠的
            if (batOfLight)
            {
                float num2 = (totalIndexes - 1f) / 2f;
                idleSpot = player.Center + -Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 40f;
                idleRotation = 0f;
            }
            #endregion
            if (terraprisma)
            {
                //呃呃 太复杂了
                int num3 = stackedIndex + 1;
                idleRotation = num3 * ((float)Math.PI * 2f) * (1f / 60f) * player.direction + (float)Math.PI / 2f;
                idleRotation = MathHelper.WrapAngle(idleRotation);
                int num4 = num3 % totalIndexes;
                Vector2 vector = new Vector2(0f, 0.5f).RotatedBy((player.miscCounterNormalized * (2f + num4) + num4 * 0.5f + player.direction * 1.3f) * ((float)Math.PI * 2f)) * 4f;
                idleSpot = idleRotation.ToRotationVector2() * 10f + player.MountedCenter + new Vector2(player.direction * (num3 * -6 - 16), player.gravDir * -15f);
                idleSpot += vector;
                idleRotation += (float)Math.PI / 2f;
            }
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
    }
}
