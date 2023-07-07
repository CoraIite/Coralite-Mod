using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaRareItems : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //凝胶
            AddRemodelRecipe(ItemID.Gel, 5, ItemID.PinkGel);
            AddRemodelRecipe(ItemID.Gel, 150, ItemID.SlimeStaff,selfRequiredNumber:999);

            //土块
            AddRemodelRecipe(ItemID.DirtBlock, 450, ItemID.DirtiestBlock, selfRequiredNumber: 9999);

            //骨头
            AddRemodelRecipe(ItemID.Bone, 5000, ItemID.BoneFeather, selfRequiredNumber: 99, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 1000, ItemID.BoneKey, selfRequiredNumber: 999, condition: HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BonePickaxe, selfRequiredNumber: 20);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BoneSword, selfRequiredNumber: 20);

            //琥珀
            AddRemodelRecipe(ItemID.Amber, 150, ItemID.AmberMosquito, selfRequiredNumber: 99);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, 400, ItemID.ShadowKey, selfRequiredNumber: 99,condition:DownedSkeletronCondition.Instance);

            //冰冻箱子
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.Fish, selfRequiredNumber: 25);
        }
    }
}
