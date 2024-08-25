using Coralite.Helpers;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class CraftAltar : MagikeFactory, IUpgradeable
    {
        public MagikeCraftRecipe ChosenResipe { get; set; }

        public override void Initialize()
        {
            Upgrade(MagikeApparatusLevel.None);
        }

        public virtual void Upgrade(MagikeApparatusLevel incomeLevel) { }

        public virtual bool CanUpgrade(MagikeApparatusLevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Work()
        {
            //如果不指定一个合成表那么就自动合成第一个可合成的
        }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            //获取物品容器
            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer,out ItemContainer container))
            {
                text = MagikeSystem.Error.Value;
                return false;
            }

            bool noMainItem = false;//没有在祭坛内放入主物品
            bool itemCantCraft = false;//该物品无法进行魔能合成
            bool noRequiredItem = false;//其他物品不足
            MagikeCraftRecipe chosenRecipe = ChosenResipe;

            if (chosenRecipe != null)//有选择的合成表，就检测这一个就行
             {

            }

            Item[] items= container.Items;
            //检测物品，搜索合成表
            foreach (Item item in items)
            {
                //没有就直接跳过
                if (item.IsAir)
                {
                    noMainItem = true;
                    continue;
                }

                if (!MagikeSystem.TryGetMagikeCraftRecipes(item.type, out List<MagikeCraftRecipe> recipes))
                {
                    noMainItem = false;
                    itemCantCraft = true;
                    continue;
                }

                //检测当前连接的物品容器内的物品是否符合某一合成表
            }

            text = "";

            if (noMainItem) { }
            else if (itemCantCraft)
            {
            }
            else if (noRequiredItem) { }


            return false;
        }
    }
}
