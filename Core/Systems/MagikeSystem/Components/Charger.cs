using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class Charger : MagikeFactory
    {
        /// <summary>
        /// 为顶部的物品充能
        /// </summary>
        public bool ChargeItemsOnUp { get; set; } = true;

        /// <summary>
        /// 为顶部的玩家身上的物品充能
        /// </summary>
        public bool ChargePlayerItemsOnUp { get; set; } = true;

        /// <summary>
        /// 单个物品每次能充能多少
        /// </summary>
        public int MagikePerCharge { get; set; }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            var items = GetChargeItems();

            if (items.Count < 1)
            {
                text = MagikeSystem.GetUIText(MagikeSystem.UITextID.ChargerItemNotFound);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获取所有的充能物品
        /// </summary>
        /// <returns></returns>
        public List<MagikeItem> GetChargeItems()
        {
            List<MagikeItem> items = new List<MagikeItem>();

            if (Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                foreach (Item item in container.Items)
                {
                    if (item.IsAir || !item.IsMagikeChargable())
                        continue;

                    items.Add(item.GetMagikeItem());
                }
            }

            if (ChargeItemsOnUp)//遍历物品，检测是否在上方
            {
                MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);

                Rectangle selfRect = new Rectangle(Entity.Position.X * 16, (Entity.Position.Y - 2) * 16, data.Width * 16, 16 * 2);

                for (int i = 0; i < Main.maxItems; i++)
                {
                    Item item = Main.item[i];
                    if (item == null || item.IsAir || !item.IsMagikeChargable())
                        continue;

                    if (item.getRect().Intersects(selfRect))
                        items.Add(item.GetMagikeItem());
                }
            }

            if (ChargePlayerItemsOnUp)//遍历玩家，检测是否在上方
            {
                MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);

                Rectangle selfRect = new Rectangle(Entity.Position.X * 16, (Entity.Position.Y - 2) * 16, data.Width * 16, 16 * 2);

                for (int i = 0; i < Main.maxItems; i++)
                {
                    Player p = Main.player[i];

                    if (p == null || p.Alives())
                        continue;

                    if (p.getRect().Intersects(selfRect))
                    {
                        foreach (var item in p.inventory)
                            if (!item.IsAir && item.IsMagikeChargable())
                                items.Add(item.GetMagikeItem());

                        if (p.useVoidBag())
                            foreach (var item in p.bank4.item)
                                if (!item.IsAir && item.IsMagikeChargable())
                                    items.Add(item.GetMagikeItem());
                    }
                }
            }

            return items;
        }

        public override bool DuringWork()
        {
            OnWorking();

            //每隔固定时间充能一次
            if (UpdateTime())
            {
                if (ChargeAll())
                    return false;
                else
                {
                    Timer = WorkTime;
                    return true;
                }
            }

            return true;
        }

        /// <summary>
        /// 充能所有的物品，并检测所有的物品是否充满了，返回<see langword="true"/>就是全部充满了
        /// </summary>
        /// <returns></returns>
        public bool ChargeAll()
        {
            var items=GetChargeItems();
            var container=Entity.GetMagikeContainer();

            int currentMagike = container.Magike;
            int cosumeMagike = 0;
            bool hasItemNotFillUp=false;

            foreach (MagikeItem item in items)
            {
                if (item.FillUp)
                    continue;

                int charge = MagikePerCharge;
                int magikeLeast = currentMagike - cosumeMagike;//自身还剩多少魔能
                int magikeCanCharge = item.MagikeMax - item.Magike;//物品还能容纳多少魔能

                //检测当前的魔能是否足够去充能，不够就充当前的
                if (charge > magikeLeast)
                    charge = magikeLeast;

                if (charge > magikeCanCharge)
                    charge = magikeCanCharge;

                item.Charge(charge);//充能物品，并记录是否有充满
                if (!item.FillUp)
                    hasItemNotFillUp = true;

                if (magikeLeast < 1)//自身莫得了那就跳出循环
                    break;

                cosumeMagike += charge;
            }

            container.ReduceMagike(cosumeMagike);

            if (hasItemNotFillUp)
                return false;

            return true;
        }

        #region 同步

        #endregion

        #region 存储

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            if (ChargeItemsOnUp)
                tag.Add(nameof(ChargeItemsOnUp), true);
            if (ChargePlayerItemsOnUp)
                tag.Add(nameof(ChargePlayerItemsOnUp), true);

            tag.Add(nameof(MagikePerCharge), MagikePerCharge);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            if (tag.ContainsKey(nameof(ChargeItemsOnUp)))
                ChargeItemsOnUp = true;
            if (tag.ContainsKey(nameof(ChargePlayerItemsOnUp)))
                ChargePlayerItemsOnUp = true;

            MagikePerCharge = tag.GetInt(nameof(MagikePerCharge));
        }

        #endregion
    }
}
