using Coralite.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class RareItems : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //火焰花
            AddRemodelRecipe(ItemID.Fireblossom, ItemID.LivingFireBlock, CalculateMagikeCost(CrystallineMagike, 1, 15)
                , 5, conditions: Condition.Hardmode);

            //凝胶
            AddRemodelRecipe(ItemID.Gel, ItemID.PinkGel, CalculateMagikeCost(MagicCrystal, 2, 20));
            AddRemodelRecipe(ItemID.Gel, ItemID.SlimeStaff, CalculateMagikeCost(MagicCrystal, 6, 120), 99 * 3);

            //土块
            AddRemodelRecipe(ItemID.DirtBlock, ItemID.DirtiestBlock, CalculateMagikeCost(Glistent, 6, 120), mainStack: 9999);

            //骨头
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneFeather, CalculateMagikeCost(Soul, 8, 120), mainStack: 99, conditions: Condition.DownedPlantera);
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneKey, CalculateMagikeCost(Bone, 6, 120), mainStack: 666, conditions: Condition.Hardmode);
            AddRemodelRecipe(ItemID.Bone, ItemID.BonePickaxe, CalculateMagikeCost(Bone, 3, 120), mainStack: 20, conditions: CoraliteConditions.NotInDigDigDig);
            AddRemodelRecipe(ItemID.Bone, ItemID.BoneSword, CalculateMagikeCost(Bone, 3, 60), mainStack: 20);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, ItemID.ShadowKey, CalculateMagikeCost(Bone, 6, 60), mainStack: 49, conditions: Condition.DownedSkeletron);
            AddRemodelRecipe(ItemID.Obsidian, ItemID.ObsidianRose, CalculateMagikeCost(Bone, 6, 120), mainStack: 99, conditions: Condition.DownedSkeletron);

            //青蛙
            AddRemodelRecipe(ItemID.Frog, ItemID.FrogLeg, CalculateMagikeCost(Icicle, 8, 240), mainStack: 3);

            ////魔力磁铁
            //AddRemodelRecipe(ItemID.TreasureMagnet, ItemID.CelestialMagnet, 100);

            ////金戒指
            //AddRemodelRecipe(ItemID.GoldBar, ItemID.GoldRing, 3500, mainStack: 50);

            //魔法箭袋
            MagikeRecipe.CreateCraftRecipe(ItemID.EndlessQuiver, ItemID.MagicQuiver, CalculateMagikeCost(CrystallineMagike, 12, 60 * 3))
                .AddIngredient<MutatusInABottle>()
                .AddIngredient<DeorcInABottle>()
                .Register();

            //深度计
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<MagicalPowder>(), ItemID.DepthMeter, CalculateMagikeCost(RedJade, 12, 60 * 3), 4)
                .AddIngredient(ItemID.Wire)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .Register();

            //指南针
            MagikeRecipe.CreateCraftRecipe(ItemID.CelestialMagnet, ItemID.Compass, CalculateMagikeCost(RedJade, 12, 60 * 3))
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .Register();

            //杀怪计数器
            MagikeRecipe.CreateCraftRecipe(ItemID.Bone, ItemID.TallyCounter, CalculateMagikeCost(Bone, 12, 60 * 3), 25)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .AddIngredient(ItemID.GoldBar, 4)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.Bone, ItemID.TallyCounter, CalculateMagikeCost(Bone, 12, 60 * 3), 25)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .AddIngredient(ItemID.PlatinumBar, 4)
                .Register();

            //金属探测仪
            MagikeRecipe.CreateCraftRecipe(ItemID.Diamond, ItemID.MetalDetector, CalculateMagikeCost(RedJade, 12, 60 * 3), 5)
                .AddIngredientGroup(RecipeGroupID.IronBar, 12)
                .Register();

            //珍珠
            AddRemodelRecipe(ItemID.Oyster, ItemID.WhitePearl, CalculateMagikeCost(Glistent, 3, 60), mainStack: 5);
            AddRemodelRecipe(ItemID.Oyster, ItemID.BlackPearl, CalculateMagikeCost(Glistent, 6, 60), mainStack: 10);
            AddRemodelRecipe(ItemID.Oyster, ItemID.PinkPearl, CalculateMagikeCost(Glistent, 12, 60), mainStack: 15);

            AddRemodelRecipe(ItemID.ShuckedOyster, ItemID.WhitePearl, CalculateMagikeCost(Glistent, 3, 120), mainStack: 10);
            AddRemodelRecipe(ItemID.ShuckedOyster, ItemID.BlackPearl, CalculateMagikeCost(Glistent, 6, 120), mainStack: 20);
            AddRemodelRecipe(ItemID.ShuckedOyster, ItemID.PinkPearl, CalculateMagikeCost(Glistent, 12, 120), mainStack: 30);
        }
    }
}
