using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaBossBag : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //史莱姆王宝藏袋
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 150, ItemID.SlimySaddle);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 100, ItemID.NinjaHood);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 100, ItemID.NinjaShirt);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 100, ItemID.NinjaPants);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 150, ItemID.SlimeHook);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 50, ItemID.SlimeGun);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 100, ItemID.KingSlimeMask);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 150, ItemID.KingSlimeTrophy);
            AddRemodelRecipe(ItemID.KingSlimeBossBag, 50, ItemID.KingSlimePetItem, condition: MasterModeCondition.Instance);

            //克苏鲁之眼宝藏袋
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 150, ItemID.CrimtaneOre,85);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 150, ItemID.DemoniteOre, 85);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 150, ItemID.Binoculars );
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 100, ItemID.EyeMask);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 150, ItemID.EyeofCthulhuTrophy );
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, 50, ItemID.EyeOfCthulhuPetItem, condition: MasterModeCondition.Instance);

            //世界吞噬怪宝藏袋
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 125, ItemID.DemoniteOre, 70);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 150, ItemID.ShadowScale, 25);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 250, ItemID.EatersBone);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 100, ItemID.EaterMask);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 150, ItemID.EaterofWorldsTrophy);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, 50, ItemID.EaterOfWorldsPetItem, condition: MasterModeCondition.Instance);

            //克苏鲁之脑宝藏袋
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 125, ItemID.CrimtaneOre, 70);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 125, ItemID.TissueSample, 25);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 250, ItemID.BoneRattle);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 100, ItemID.BrainMask);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 150, ItemID.BrainofCthulhuTrophy);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, 50, ItemID.BrainOfCthulhuPetItem, condition: MasterModeCondition.Instance);

            //蜂后宝藏袋
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeeGun);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeeKeeper);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeesKnees);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 50, ItemID.BeeHat);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 50, ItemID.BeeShirt);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 50, ItemID.BeePants);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.HoneyComb);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 250, ItemID.Nectar);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 250, ItemID.HoneyedGoggles);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.Beenade,60);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeeMask);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 150, ItemID.QueenBeeTrophy);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 50, ItemID.QueenBeePetItem, condition: MasterModeCondition.Instance);

            //巨鹿宝藏袋
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 150, ItemID.ChesterPetItem);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 100, ItemID.Eyebrella);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 100, ItemID.DontStarveShaderItem);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 250, ItemID.DizzyHat);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 125, ItemID.PewMaticHorn);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 125, ItemID.WeatherPain);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 125, ItemID.HoundiusShootius);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 125, ItemID.LucyTheAxe);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 100, ItemID.DeerclopsMask);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 150, ItemID.DeerclopsTrophy);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 50, ItemID.DeerclopsPetItem, condition: MasterModeCondition.Instance);
        }
    }
}
