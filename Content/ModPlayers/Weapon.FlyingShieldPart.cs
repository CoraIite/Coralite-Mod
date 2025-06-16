using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary> +1个额外盾牌 </summary>
        public bool ExtraShield1 { get; internal set; }
        /// <summary> +1个额外盾牌 </summary>
        public bool ExtraShield2 { get; internal set; }
        /// <summary> +1个额外盾牌 </summary>
        public bool ExtraShield3 { get; internal set; }

        /// <summary>
        /// 同时存在的飞盾弹幕数量
        /// </summary>
        public int MaxFlyingShield;
        /// <summary>
        /// 是否能够同时使用左右键
        /// </summary>
        public bool FlyingShieldLRMeantime;
        /// <summary>
        /// 回归时是否会逐渐加速
        /// </summary>
        public bool FlyingShieldAccBack;
        /// <summary>
        /// 飞盾格挡后获得的伤害减免
        /// </summary>
        public float FlyingShieldDamageReduce;
        /// <summary>
        /// 防御时间，
        /// </summary>
        public int FlyingShieldGuardTime;
        /// <summary>
        /// 冲刺时的伤害减少
        /// </summary>
        public int FlyingShieldDashDamageReduce;
        /// <summary>
        /// 飞盾饰品的list
        /// </summary>
        public List<IFlyingShieldAccessory> FlyingShieldAccessories = new();

        /// <summary>
        /// 飞盾右键防御的手持弹幕
        /// </summary>
        public int FlyingShieldGuardIndex;

        /// <summary>
        /// 在<see cref="ResetEffects"/>中调用
        /// </summary>
        public void ResetFlyingShieldSets()
        {
            //最大飞盾数为1
            MaxFlyingShield = 1;
            //无法同时左右键使用
            FlyingShieldLRMeantime = false;
            FlyingShieldAccBack = false;
            //更新飞盾提供的伤害减免
            if (FlyingShieldGuardTime > 0)
            {
                FlyingShieldGuardTime--;
                if (FlyingShieldGuardTime <= 25)
                    FlyingShieldDamageReduce -= FlyingShieldDamageReduce / FlyingShieldGuardTime;
                if (FlyingShieldGuardTime == 0)
                    FlyingShieldDamageReduce = 0;
            }

            //清空飞盾饰品的list
            FlyingShieldAccessories ??= new List<IFlyingShieldAccessory>();
            if (FlyingShieldAccessories.Count != 0)
                FlyingShieldAccessories.Clear();

            //查看飞盾右键弹幕是否还存在
            if (Main.projectile.IndexInRange(FlyingShieldGuardIndex))
            {
                Projectile p = Main.projectile[FlyingShieldGuardIndex];
                if (!p.active || !p.friendly || p.ModProjectile is not BaseFlyingShieldGuard)
                    FlyingShieldGuardIndex = -1;
            }
        }

        public void UpdateFlyingShield()
        {
            if (ExtraShield1)
                MaxFlyingShield++;
            if (ExtraShield2)
                MaxFlyingShield++;
            if (ExtraShield3)
                MaxFlyingShield++;
        }

        /// <summary>
        /// 防御效果，为玩家添加伤害减免
        /// </summary>
        /// <param name="damageReduce">伤害减免</param>
        public void Guard(float damageReduce)
        {
            FlyingShieldGuardTime = 35;
            FlyingShieldDamageReduce = damageReduce;
        }

        /// <summary>
        /// 尝试获得当前的飞盾防御手持弹幕
        /// </summary>
        /// <param name="flyingShieldGuard"></param>
        /// <returns></returns>
        public bool TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
        {
            flyingShieldGuard = null;

            if (!Main.projectile.IndexInRange(FlyingShieldGuardIndex))
                return false;

            if (Main.projectile[FlyingShieldGuardIndex].ModProjectile is BaseFlyingShieldGuard flyingShieldGuard1)
            {
                flyingShieldGuard = flyingShieldGuard1;
                return true;
            }

            return false;
        }

        public void OnFlyingShieldDashOver()
        {
            FlyingShieldDashDamageReduce = 0;
        }

        public void FlyingShieldHurt(ref Player.HurtModifiers modifiers)
        {
            if (FlyingShieldDashDamageReduce > 0)
            {
                modifiers.DisableSound();
                modifiers.DisableDust();

                modifiers.FinalDamage.Flat -= FlyingShieldDashDamageReduce;
                FlyingShieldDashDamageReduce = 0;

                Helpers.Helper.PlayPitched(CoraliteSoundID.Metal_NPCHit4, Player.Center);
            }
        }

        public void SaveFlyingShield(TagCompound tag)
        {
            if (ExtraShield1)
                tag.Add(nameof(ExtraShield1), ExtraShield1);
            if (ExtraShield2)
                tag.Add(nameof(ExtraShield2), ExtraShield2);
            if (ExtraShield3)
                tag.Add(nameof(ExtraShield3), ExtraShield3);
        }

        public void LoadFlyingShield(TagCompound tag)
        {
            ExtraShield1 = tag.ContainsKey(nameof(ExtraShield1));
            ExtraShield2 = tag.ContainsKey(nameof(ExtraShield2));
            ExtraShield3 = tag.ContainsKey(nameof(ExtraShield3));
        }
    }
}
