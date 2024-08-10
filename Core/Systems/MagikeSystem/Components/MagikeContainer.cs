using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using System;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeContainer : MagikeComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeContainer;

        /// <summary> 当前内部的魔能量 </summary>
        public int Magike { get; set; }

        /// <summary> 自身魔能基础容量，可以通过升级来变化 </summary>
        public int MagikeMaxBase { get; protected set; }
        /// <summary> 额外魔能量，通过扩展膜附加的魔能容量 </summary>
        public int MagikeMaxExtra { get; set; }

        /// <summary> 当前的魔能上限 </summary>
        public int MagikeMax { get => MagikeMaxBase + MagikeMaxExtra; }

        /// <summary> 有魔能就为<see langword="true"/> </summary>
        public virtual bool HasMagike => Magike > 0;
        /// <summary> 魔能满了后为true </summary>
        public virtual bool FullMagike => Magike >= MagikeMax;

        public override void Update(IEntity entity) { }

        #region 魔能操作相关

        public void LimitMagikeAmount() => Magike = Math.Clamp(Magike, 0, MagikeMax);

        /// <summary>
        /// 直接向魔能容器内添加魔能
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void AddMagike(int amount)
        {
            Magike += amount;
            LimitMagikeAmount();
        }

        /// <summary>
        /// 直接减少，请一定在执行这个操作前检测能否减少
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void ReduceMagike(int amount)
        {
            Magike -= amount;
            LimitMagikeAmount();
        }

        /// <summary>
        /// 一键充满
        /// </summary>
        public void FullChargeMagike()
        {
            Magike = MagikeMax;
        }

        /// <summary>
        /// 一键清空
        /// </summary>
        public void ClearMagike()
        {
            Magike = 0;
        }

        /// <summary>
        /// 将传入的数值限制在能接受的魔能数量
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool LimitReceiveOverflow(ref int amount)
        {
            if (Magike + amount > MagikeMax)
            {
                amount = MagikeMax - Magike;
                return true;
            }

            return false;
        }

        #endregion

        #region UI

        public override void ShowInUI(UIElement parent)
        {
            UIList list = new UIList();
            list.Width.Set(0, 1);
            list.Height.Set(0, 1);

            AddText(list, c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeAmount) + c.Magike, parent);
            AddText(list, c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeMax) + c.MagikeMax, parent);
            AddText(list, c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeMaxBase) + c.MagikeMaxBase, parent);
            AddText(list, c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeMaxExtra) + c.MagikeMaxExtra, parent);
            AddText(list, c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikePercent) + MathF.Round(100*c.Magike / (float)c.MagikeMax, 1)+"%", parent);

            parent.Append(list);
        }

        public void AddText(UIList list, Func<MagikeContainer, string> textFunc, UIElement parent)
        {
            list.Add(new ComponentUIElementText<MagikeContainer>(textFunc, this, parent));
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
