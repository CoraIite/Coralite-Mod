using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeContainer : Component
    {
        public override int ID => MagikeComponentID.MagikeContainer;

        /// <summary> 当前内部的魔能量 </summary>
        public int Magike { get; set; }

        /// <summary> 自身魔能基础容量，可以通过升级来变化 </summary>
        public int MagikeMaxBase { get; private set; }
        /// <summary> 额外魔能量，通过扩展膜附加的魔能容量 </summary>
        public int MagikeMaxExtra { get; set; }

        /// <summary> 当前的魔能上限 </summary>
        public int MagikeMax { get => MagikeMaxBase + MagikeMaxExtra; }

        /// <summary> 有魔能就为<see langword="true"/> </summary>
        public bool HasMagike => Magike > 0;
        /// <summary> 魔能满了后为true </summary>
        public bool FullMagike => Magike >= MagikeMax;

        public override void Update(IEntity entity) { }

        /// <summary>
        /// 请自行判断传入的等级，返回<see langword="true"/>为能升级<br></br>
        /// 
        /// </summary>
        /// <param name="incomeLevel"></param>
        /// <returns></returns>
        public virtual bool ChangeLevel(MagikeApparatusLevel incomeLevel)
        {
            return false;
        }

        #region 魔能操作相关

        public void LimitMagikeAmount() => Magike = Math.Clamp(Magike, 0, MagikeMax);

        /// <summary>
        /// 直接向魔能容器内添加魔能
        /// </summary>
        /// <param name="container"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static MagikeContainer operator +(MagikeContainer container, int count)
        {
            container.Magike += count;
            container.LimitMagikeAmount();
            return container;
        }

        /// <summary>
        /// 直接减少，请一定在执行这个操作前检测能否减少
        /// </summary>
        /// <param name="container"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static MagikeContainer operator -(MagikeContainer container, int count)
        {
            container.Magike -= count;
            container.LimitMagikeAmount();
            return container;
        }

        /// <summary>
        /// 将传入的数值限制在能接受的魔能数量
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool LimitReceiveOverflow(ref int amount)
        {
            if (Magike + amount > MagikeMax)
            {
                amount = MagikeMax - Magike;
                return true;
            }

            return false;
        }

        #endregion

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(Magike), Magike);
            tag.Add(preName + nameof(MagikeMaxBase), MagikeMaxBase);
            tag.Add(preName + nameof(MagikeMaxExtra), MagikeMaxExtra);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            Magike = tag.GetInt(preName + nameof(Magike));
            MagikeMaxBase = tag.GetInt(preName + nameof(MagikeMaxBase));
            MagikeMaxExtra = tag.GetInt(preName + nameof(MagikeMaxExtra));
        }
    }
}
