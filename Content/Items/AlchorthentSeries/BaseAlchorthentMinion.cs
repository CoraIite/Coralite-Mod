using Coralite.Content.Buffs;
using Coralite.Core;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public abstract class BaseAlchorthentMinion<TBuff> : ModProjectile where TBuff:ModBuff
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        /// <summary>
        /// 状态
        /// </summary>
        public byte State { get; set; }
        public int Timer { get; set; }
        /// <summary>
        /// 目标索引
        /// </summary>
        public int Target { get; set; }

        public bool CanDamageNPC { get; set; }

        private bool init = true;

        public Player Owner => Main.player[Projectile.owner];

        public sealed override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            SetOtherDefault();
        }

        /// <summary>
        /// 已设置伤害类型，友好，启用本地无敌帧
        /// </summary>
        public virtual void SetOtherDefault()
        {

        }

        public override bool? CanDamage()
        {
            if (CanDamageNPC)
                return null;

            return false;
        }

        public override void AI()
        {
            if (init)
            {
                init = false;
                Target = -1;
                Initialize();
            }

            if (!CheckActive())
                return;

            Owner.AddBuff(BuffType<TBuff>(), 2);

            AIMoves();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 具体的AI动作
        /// </summary>
        public virtual void AIMoves()
        {

        }

        /// <summary>
        /// 重力
        /// </summary>
        /// <param name="maxYSpeed"></param>
        /// <param name="gravityAccel"></param>
        public void Gravity(float maxYSpeed, float gravityAccel)
        {
            Projectile.velocity.Y += gravityAccel;
            if (Projectile.velocity.Y > maxYSpeed)
                Projectile.velocity.Y = maxYSpeed;
        }

        /// <summary>
        /// 检测玩家存活和BUFF
        /// </summary>
        /// <returns></returns>
        public bool CheckActive()
        {
            if (Owner.dead || !Owner.active)
            {
                Owner.ClearBuff(BuffType<TBuff>());
                return false;
            }

            if (Owner.HasBuff(BuffType<TBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
    }
}
