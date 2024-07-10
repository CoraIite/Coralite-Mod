using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria;
using Terraria.ID;
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

        /// <summary>
        /// 当前的魔能上限
        /// </summary>
        public int MagikeMax { get => MagikeMaxBase + MagikeMaxExtra; }

        public MagikeApparatusLevel Level { get; private set; }

        public override void Update(IEntity entity)
        {
        }


        public virtual bool Upgrage(MagikeApparatusLevel incomeLevel)
        {
            return false;
        }

        /// <summary>
        /// 返回<see langword="true"/>为成功移除
        /// </summary>
        /// <returns></returns>
        public bool RemoveCurrentExtendMembrane()
        {
            if (Level == MagikeApparatusLevel.MagicCrystal)
                return true;

            if (!CheckPlayerOwnedExtendMembrane())
                return false;

            //TODO：生成当前的膜物品

            return true;
        }

        /// <summary>
        /// 消耗
        /// </summary>
        /// <returns></returns>
        public static bool CheckPlayerOwnedExtendMembrane()
        {
            return Main.LocalPlayer.ConsumeItem(ItemID.Lens, includeVoidBag: true);
        }

        #region 魔能操作相关

        public void LimitMagikeAmount() => Magike = Math.Clamp(Magike, 0, MagikeMax);

        public bool HasMagike() => Magike > 0;

        /// <summary>
        /// 直接向魔能容器内添加魔能
        /// </summary>
        /// <param name="container"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static MagikeContainer operator +(MagikeContainer container,int count)
        {
            container.Magike += count;
            container.LimitMagikeAmount();
            return container;
        }

        /// <summary>
        /// 从魔能容器内取走魔能，返回值为最后能取走多少<br></br>
        /// 如果能取走给定的值，那么就返回这个值，如果不能则返回当前魔能容器中的魔能量
        /// </summary>
        /// <param name="container"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int operator -(MagikeContainer container, int count)
        {
            //如果没有魔能则直接返回0
            if (!container.HasMagike())
                return 0;

            //如果魔能量不够那么就返回剩余所有
            if (container.Magike < count)
            {
                int i = container.Magike;
                container.Magike = 0;
                return i;
            }
            
            //正常状态
            container.Magike -= count;
            container.LimitMagikeAmount();
            return count;
        }

        #endregion

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
        }
    }
}
