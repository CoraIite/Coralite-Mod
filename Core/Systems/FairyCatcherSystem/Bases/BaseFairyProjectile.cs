using Coralite.Content.DamageClasses;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyProjectile : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        /// <summary>
        /// 生成时间，生成仙灵时传入
        /// </summary>
        public ref float SpawnTime => ref Projectile.ai[0];
        /// <summary>
        /// 当前使用技能的索引
        /// </summary>
        public ref float UseSkillIndex => ref Projectile.ai[1];
        /// <summary>
        /// 目标敌怪的索引
        /// </summary>
        public ref float TargetIndex => ref Projectile.ai[2];

        public AIStates State { get; set; }
        public int Timer { get; set; }

        /// <summary>
        /// 受击后无敌时间
        /// </summary>
        public int ImmuneTimer { get; set; }
        /// <summary>
        /// 使用技能的次数，如果大于 <see cref="FairyIV.Stamina"/> 就会返回玩家
        /// </summary>
        public int UseSkillCount { get; set; }

        /// <summary>
        /// 由本地端同步给其他端的速度，无需使用仙灵物品获取速度个体值
        /// </summary>
        public float IVSpeed { get; set; }
        /// <summary>
        /// 由本地端同步给其他端的技能等级，无需使用仙灵物品获取速度个体值
        /// </summary>
        protected int IVSkillLevel { get; set; }

        /// <summary>
        /// 在开始攻击时设置，经过玩家饰品等增强过的技能等级
        /// </summary>
        public int SkillLevel { get; set; }

        protected virtual int FrameX => 1;
        protected virtual int FrameY => 4;
        /// <summary>
        /// 能够开始攻击的距离
        /// </summary>
        public float AttackDistance = 400;

        private bool init = true;
        protected bool canDamage = true;

        /// <summary>
        /// 存储所有的仙灵技能
        /// </summary>
        private FairySkill[] _skills;

        /// <summary>
        /// 仅本地端可用！用它获取仙灵个体值以及执行仙灵掉血等操作
        /// </summary>
        public BaseFairyItem FairyItem { get; set; }

        private NetState _netState;

        public enum AIStates:byte
        {
            /// <summary>
            /// 刚从仙灵捕手中射出的时候
            /// </summary>
            Shooting,
            /// <summary>
            /// 执行自身的特定AI
            /// </summary>
            Skill,
            /// <summary>
            /// 释放技能后的后摇
            /// </summary>
            Rest,
            /// <summary>
            /// 返回自身，此时无敌
            /// </summary>
            Backing,
        }

        private enum NetState:byte
        {
            None,
            Init,
            Skill,
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.width = Projectile.height = 16;

            Projectile.penetrate = -1;
        }

        public sealed override void AI()
        {
            if (init)
                Initilize();

            if (Vector2.Distance(Projectile.Center,Owner.Center)>4000)
                Projectile.Kill();

            switch (State)
            {
                default:
                    Projectile.Kill();
                    break;
                case AIStates.Shooting:
                    Projectile.timeLeft = 2;
                    Shooting();
                    SpawnFairyDust();
                    break;
                case AIStates.Skill:
                    Projectile.timeLeft = 2;
                    Skill();
                    break;
                case AIStates.Rest:
                    Projectile.timeLeft = 2;
                    Timer++;
                    if (Timer > 60 * 3 - IVSpeed * 4)
                        RestartAttack();

                    Rest();
                    SpawnFairyDust();
                    break;
                case AIStates.Backing:
                    Backing();
                    SpawnFairyDust();
                    break;
            }

            AIAfter();
        }

        /// <summary>
        /// 用于发光等特殊效果
        /// </summary>
        public virtual void AIAfter() { }

        public override void PostAI()
        {
            SetDirectionNormally();
            UpdateFrameY(6);

            //检测与弹幕间的碰撞
            if (Projectile.IsOwnedByLocalPlayer() && ImmuneTimer <= 0)
                foreach (var proj in Main.projectile.Where(p => p.active && p.hostile))
                {
                    if (proj.Colliding(proj.getRect(), Projectile.getRect()))
                    {
                        int damage = proj.damage;

                        foreach (var skill in _skills)
                            skill.ModifyHitByProj(this, proj, ref damage);

                        FairyItem?.HurtByProjectile(this, proj, damage);

                        if (State is AIStates.Rest or AIStates.Shooting)//仅在休息阶段会有受击的击退效果
                            Projectile.velocity += (Projectile.Center - proj.Center).SafeNormalize(Vector2.Zero) * proj.knockBack;

                        break;
                    }
                }

            if (ImmuneTimer > 0)
            {
                ImmuneTimer--;
            }
        }

        public void Initilize()
        {
            init = false;
            TargetIndex = -1;

            if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null)
            {
                Projectile.Resize((int)(Projectile.width * FairyItem.FairyIV.Scale)
                    , (int)(Projectile.height * FairyItem.FairyIV.Scale));

                Projectile.scale = FairyItem.FairyIV.Scale;
                IVSpeed = FairyItem.FairyIV.Speed;
                IVSkillLevel = FairyItem.FairyIV.SkillLevel;
                AttackDistance = 325 + IVSkillLevel * 75;

                _netState = NetState.Init;
                Projectile.netUpdate = true;
            }

            //初始化技能
            var skill = InitSkill();
            if (skill != null && skill.Length > 0)
                _skills = skill;
            else
                "仙灵必须有至少一个技能！！".Dump();

            OnInitialize();
        }

        /// <summary>
        /// 初始化仙灵技能
        /// </summary>
        /// <returns></returns>
        public abstract FairySkill[] InitSkill();

        /// <summary>
        /// 帮助方法，获取技能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FairySkill NewSkill<T>() where T : FairySkill
            => ModContent.GetInstance<T>().NewInstance();

        public virtual void OnInitialize() { }

        /// <summary>
        /// 生成粒子，使用<see cref="State"/>判断当前的状态
        /// </summary>
        public virtual void SpawnFairyDust()
        {

        }

        /// <summary>
        /// 在刚射出时执行，飞行指定时间后开始攻击
        /// </summary>
        public virtual void Shooting()
        {
            Timer++;
            if (Timer > SpawnTime)
                TryExchangeToAttack();

            float speed = Projectile.velocity.Length();
            if (speed>IVSpeed)
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Helper.Lerp(speed, IVSpeed, 0.08f);
        }

        /// <summary>
        /// 执行技能
        /// </summary>
        public virtual void Skill()
        {
            FairySkill skill = _skills[(int)UseSkillIndex];

            //初始化技能
            if (Timer == 0)
            {
                //设置增幅后的技能等级
                SkillLevel = IVSkillLevel;
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                    SkillLevel = fcp.FairySkillBonus[skill.Type].ModifyLevel(IVSkillLevel);

                Timer = 1;
                Projectile.StartAttack();
                skill.SpawnSkillText(Projectile.Top);
                skill.OnStartAttack(this);
                _netState = NetState.Skill;
                Projectile.netUpdate = true;
            }

            //更新招式
            if (skill.Update(this))
            {
                _netState = NetState.None;
                ExchangeToRest();
            }
        }

        public virtual void Rest()
        {
            Vector2 restSpeed = GetRestSpeed();
            if (TargetIndex.GetNPCOwner(out NPC target, () => TargetIndex = -1)
                && Vector2.Distance(target.Center, Projectile.Center) > AttackDistance)
            {
                restSpeed += (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * IVSpeed;
            }

            float slowTime = 20;

            Projectile.velocity = ((Projectile.velocity * slowTime) + restSpeed) / (slowTime + 1);
        }

        /// <summary>
        /// 获取后摇时的原地转圈速度
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetRestSpeed()
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 返回玩家，根据仙灵的速度个体值，一段时间后逐渐加速
        /// </summary>
        public virtual void Backing()
        {
            Projectile.tileCollide = false;
            float speed = IVSpeed;
            Vector2 dir = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero);

            Timer++;

            //速度越快加速回归的速度越快
            if (Timer > 60 * 4 - IVSpeed * 4)
            {
                speed = Projectile.velocity.Length();
                speed *= 1.01f;
                Projectile.velocity = dir * speed;
            }
            else
            {
                int slowTime = 20 - (int)(20 * Timer / (60 * 4f));
                if (slowTime<1)
                    slowTime = 1;

                Projectile.velocity = (Projectile.velocity*slowTime+ dir * speed)/(slowTime+1);
            }

            if (Vector2.Distance(Projectile.Center, Owner.Center) < speed * 2)
            {
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp)
                     && fcp.TryGetFairyBottle(out BaseFairyBottle bottle)
                     && FairyItem != null)
                    bottle.OnFairyBack(Owner, FairyItem);

                Projectile.Kill();
            }
        }

        public void UpdateFrameY(int spacing)
        {
            Projectile.UpdateFrameNormally(spacing, FrameY - 1);
        }

        public void SetDirectionNormally()
        {
            Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        }

        public override bool? CanDamage()
        {
            if (canDamage)
                return null;

            return false;
        }

        #region 切换状态相关

        /// <summary>
        /// 切换至返回玩家的状态
        /// </summary>
        public virtual void ExchangeToBack()
        {
            Timer = 0;
            State = AIStates.Backing;

            canDamage = false;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
        }

        /// <summary>
        /// 切换至招式后摇状态，同时在此更新使用招式的索引
        /// </summary>
        public virtual void ExchangeToRest()
        {
            Timer = 0;
            State = AIStates.Rest;

            Projectile.tileCollide = false;

            UseSkillIndex++;
            if (UseSkillIndex >= _skills.Length)
                UseSkillIndex = 0;

            UseSkillCount++;
        }

        /// <summary>
        /// 尝试开始攻击，如果没有那么就进入休息阶段
        /// </summary>
        public virtual void TryExchangeToAttack()
        {
            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance
                , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                TargetIndex = target.whoAmI;
                State = AIStates.Skill;
                Timer = 0;

                OnStartUseSkill(target);
                canDamage = true;
            }
            else
                ExchangeToRest();
        }

        /// <summary>
        /// 重新开始攻击，在此检测使用招式数量是否超过耐力
        /// </summary>
        public virtual void RestartAttack()
        {
            if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null
                && UseSkillCount >= FairyItem.FairyIV.Stamina)
            {
                ExchangeToBack();
                return;
            }

            //持续索敌，妹找到那就尝试换个目标
            if (TargetIndex.GetNPCOwner(out NPC target1,()=>TargetIndex=-1))
            {
                State = AIStates.Skill;
                Timer = 0;

                OnStartUseSkill(target1);
                return;
            }

            if (Helper.TryFindClosestEnemy(Projectile.Center, AttackDistance
                , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
            {
                TargetIndex = target.whoAmI;
                State = AIStates.Skill;
                Timer = 0;

                OnStartUseSkill(target);
            }
            else
                ExchangeToBack();
        }

        /// <summary>
        /// 在开始使用技能时调用
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnStartUseSkill(NPC target) { }

        #endregion

        #region 受击部分

        public sealed override void OnKill(int timeLeft)
        {
            if (FairyItem != null && FairyItem.IsDead)
            {
                foreach (var skill in _skills)
                    skill.OnFairyKill(this);

                OnKill_DeadEffect();
            }

            OnKill_OtherEffect(timeLeft);
        }

        public virtual void OnKill_DeadEffect() { }

        public virtual void OnKill_OtherEffect(int timeleft) { }

        /// <summary>
        /// 在仙灵血量减为0后执行，用于生成死亡效果
        /// </summary>
        /// <param name="target"></param>
        public virtual void OnKillByNPC(NPC target) { }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //自身受伤
            //使用默认模式的伤害量，防止一些特殊修改把NPC伤害改的太夸张
            int damage = ContentSamples.NpcsByNetId[target.type].damage;

            if (State == AIStates.Skill)
                _skills[(int)UseSkillIndex].ModifyHitNPC_Active(this, target, ref modifiers, ref damage);

            foreach (var skill in _skills)
                skill.ModifyHitNPC_Inactive(this, target, ref modifiers, ref damage);

            FairyItem?.HurtByNPC(this, target, modifiers, damage);
            if (State is AIStates.Rest or AIStates.Shooting)//仅在休息阶段会有受击的击退效果
                Projectile.velocity += (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 2;
        }

        #endregion

        #region Net

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)State);
            writer.Write((byte)_netState);
            writer.Write(UseSkillCount);
            writer.Write(ImmuneTimer);

            switch (_netState)
            {
                default:
                case NetState.None:
                    break;
                case NetState.Init:
                    writer.Write(IVSpeed);
                    writer.Write(IVSkillLevel);
                    break;
                case NetState.Skill:
                    _skills[(int)UseSkillIndex].SendExtra(writer);
                    break;
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            State = (AIStates)reader.ReadByte();
            _netState = (NetState)reader.ReadByte();
            UseSkillCount = reader.ReadInt32();
            ImmuneTimer = reader.ReadInt32();

            switch (_netState)
            {
                default:
                case NetState.None:
                    break;
                case NetState.Init:
                    IVSpeed = reader.ReadSingle();
                    IVSpeed = reader.ReadSingle();
                    break;
                case NetState.Skill:
                    _skills[(int)UseSkillIndex].ReceiveExtra(reader);
                    break;
            }
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (State == AIStates.Skill)
                _skills[(int)UseSkillIndex].OnTileCollide(this, oldVelocity);

            return false;
        }

        #region 绘制部分

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == AIStates.Skill)
                _skills[(int)UseSkillIndex].PreDrawSpecial(ref lightColor);

            DrawSelf(Main.spriteBatch, Main.screenPosition, lightColor);

            if (State == AIStates.Skill)
                _skills[(int)UseSkillIndex].PostDrawSpecial(lightColor);

            if (Projectile.IsOwnedByLocalPlayer() && FairyItem != null)
                DrawHealthBar(Projectile.Bottom + new Vector2(0, 12), FairyItem.Life, FairyItem.FairyIV.LifeMax);
            
            return false;
        }

        /// <summary>
        /// 绘制自己
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="screenPos"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawSelf(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frame = mainTex.Frame(FrameX, FrameY, 0, Projectile.frame);

            if (ImmuneTimer != 0 && (ImmuneTimer % 8) < 4)
                lightColor *= 0.4f;

            spriteBatch.Draw(mainTex, Projectile.Center - screenPos, frame, lightColor, Projectile.rotation, frame.Size() / 2,
                Projectile.scale, Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public void DrawHealthBar(Vector2 center, float Health, float MaxHealth, float alpha = 0.9f)
        {
            if (Health <= 0)
                return;

            float factor = Health / (float)MaxHealth;
            if (factor > 1f)
                factor = 1f;

            Color backColor = Color.Lerp(Color.DarkRed, Color.DarkGreen, factor);
            Color barColor = Color.Lerp(Color.OrangeRed, Color.LawnGreen, factor);
            float totalBarLength = 12 * 3f;

            Texture2D tex = CoraliteAssets.Sparkle.BarSPA.Value;

            float scale = totalBarLength / tex.Width * Projectile.scale;
            backColor *= alpha;
            barColor *= alpha * factor;

            barColor.A = 0;

            center -= Main.screenPosition;

            //绘制底部条，固定绘制一个横杠
            tex.QuickCenteredDraw(Main.spriteBatch, center, backColor, scale: scale);

            //绘制顶部，裁剪矩形绘制
            Rectangle rect = new Rectangle(0, 0, (int)(factor * tex.Width), tex.Height);
            Main.spriteBatch.Draw(tex, center + new Vector2(-scale * tex.Width / 2, 0), rect, barColor, 0, new Vector2(0, tex.Height / 2), scale, 0, 0);

            //绘制指针
            Texture2D tex2 = CoraliteAssets.Sparkle.ShotLineSPA.Value;
            //scale = totalBarLength / tex.Width;

            Vector2 pos = center + new Vector2(-scale * tex.Width / 2 + factor * scale * tex.Width, 0);

            Vector2 scale1 = new Vector2(scale * tex2.Height / tex2.Width * 0.75f, scale) * 0.55f;
            Main.spriteBatch.Draw(tex2, pos, null, backColor
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);
            Main.spriteBatch.Draw(tex2, pos, null, barColor
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);
        }

        #endregion
    }
}
