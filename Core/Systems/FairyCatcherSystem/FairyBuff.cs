using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using System;
using System.Collections.Generic;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class FairyBuff : ModType
    {
        public int Type { get; internal set; }

        /// <summary>
        /// BUFF剩余时间
        /// </summary>
        public int TimeRemain;

        protected override void Register()
        {
            ModTypeLookup<FairyBuff>.Register(this);

            FairyLoader.buffs ??= new List<FairyBuff>();
            FairyLoader.buffs.Add(this);

            Type = FairyLoader.ReserveFairyBuffID();
        }

        public virtual FairyBuff NewInstance()
        {
            var inst = (FairyBuff)Activator.CreateInstance(GetType(), true);
            inst.Type = Type;
            return inst;
        }

        /// <summary>
        /// 判断另一个是否和本身的是同一个BUFF，保证不会重复上同个BUFF<br></br>
        /// 在调用之前已经判断过 <see cref="Type"/>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool IsSame(FairyBuff other);

        #region 捕捉环内操作

        /// <summary>
        /// 在捕捉环内的更新，仅会在捕捉状态调用
        /// </summary>
        /// <param name="fairy"></param>
        public virtual void UpdateInCatcher(Fairy fairy)
        {

        }

        /// <summary>
        /// 调整捕捉力，该数值是经过玩家饰品加成后的
        /// </summary>
        /// <param name="fairy"></param>
        /// <param name="catchPower"></param>
        public virtual void ModifyCatchPower(Fairy fairy,ref int catchPower)
        {

        }

        #endregion

        #region 仙灵弹幕内操作

        /// <summary>
        /// 在仙灵弹幕内更新
        /// </summary>
        /// <param name="baseFairyProj"></param>
        public virtual void UpdateInProj(BaseFairyProjectile baseFairyProj)
        {

        }

        /// <summary>
        /// 自定义仙灵受到的伤害
        /// <br></br>该调整在最后
        /// </summary>
        /// <param name="baseFairyProjectile"></param>
        /// <param name="damage"></param>
        public virtual void DamageReduce(BaseFairyProjectile baseFairyProjectile,ref int damage)
        {

        }

        #endregion

        /// <summary>
        /// 在捕捉环内和弹幕种均会调用
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public virtual void PreDraw(Vector2 center, Vector2 size, ref Color drawColor, float alpha)
        {

        }

        /// <summary>
        /// 在捕捉环内和弹幕种均会调用
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        public virtual void PostDraw(Vector2 center, Vector2 size, Color drawColor, float alpha)
        {

        }
    }
}
