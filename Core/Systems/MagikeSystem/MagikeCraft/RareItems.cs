using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class RareItems : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //火焰花
            AddRemodelRecipe(ItemID.Fireblossom, ItemID.LivingFireBlock, CalculateMagikeCost(CrystallineMagike, 2, 20)
                , 5, conditions: Condition.Hardmode);

            //凝胶
            AddRemodelRecipe(ItemID.Gel, ItemID.PinkGel, CalculateMagikeCost(MagicCrystal, 2, 20));
            AddRemodelRecipe(ItemID.Gel, ItemID.SlimeStaff, CalculateMagikeCost(MagicCrystal, 6, 120), 99 * 3);

            //土块
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.DirtiestBlock, CalculateMagikeCost(Glistent, 6, 120), mainStack: 9999);

            //骨头
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneFeather, CalculateMagikeCost(Soul, 12, 120), mainStack: 99, conditions: Condition.DownedPlantera);
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneKey, CalculateMagikeCost(Bone, 6, 120), mainStack: 666, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.Bone, ItemID.BonePickaxe, CalculateMagikeCost(Bone, 3, 120), mainStack: 20, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneSword, CalculateMagikeCost(Bone, 3, 60), mainStack: 20);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, ItemID.ShadowKey, CalculateMagikeCost(Bone, 6, 60), mainStack: 49, conditions: Condition.DownedSkeletron);
            AddRemodelRecipe(ItemID.Obsidian, ItemID.ObsidianRose, CalculateMagikeCost(Bone, 6, 120), mainStack: 99, conditions: Condition.DownedSkeletron);

            //青蛙
            AddRemodelRecipe(ItemID.Frog, ItemID.FrogLeg, CalculateMagikeCost(Icicle, 12, 240), mainStack: 3);

            ////魔力磁铁
            //AddRemodelRecipe(ItemID.TreasureMagnet, ItemID.CelestialMagnet, 100);

            ////金戒指
            //AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldRing, 3500, mainStack: 50);
        }
    }
}
