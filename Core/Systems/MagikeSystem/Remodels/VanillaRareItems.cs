using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaRareItems : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //火焰花
            AddRemodelRecipe(ItemID.Fireblossom, 25, ItemID.LivingFireBlock, 5);

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
            AddRemodelRecipe(ItemID.Amber, 300, ItemID.AmberMosquito, selfRequiredNumber: 99);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, 400, ItemID.ShadowKey, selfRequiredNumber: 99,condition:DownedSkeletronCondition.Instance);

            //青蛙
            AddRemodelRecipe(ItemID.Frog, 400, ItemID.FrogLeg, selfRequiredNumber: 10);

            //木箱子
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Spear, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Blowpipe, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.WoodenBoomerang, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Aglet, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.ClimbingClaws, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Umbrella, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.Chest, 50, ItemID.CordageGuide, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.WandofSparking, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.Radar, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.Chest, 10, ItemID.PortableStool);

            //金箱子
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.BandofRegeneration, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.MagicMirror, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.CloudinaBottle, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.HermesBoots, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Mace, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.EnchantedBoomerang, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.ShoeSpikes, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.FlareGun, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Extractinator, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.GoldChest, 300, ItemID.LavaCharm, selfRequiredNumber: 10);

            //沙漠箱子
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderSpear, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderStaff, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MagicConch, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MysticCoilSnake, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.AncientChisel, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.SandBoots, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.CatBast, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.DesertMinecart, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.EncumberingStone, selfRequiredNumber: 3);

            //冰冻箱子
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.Fish, selfRequiredNumber: 25);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBlade, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBoomerang, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceSkates, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.SnowballCannon, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.BlizzardinaBottle, selfRequiredNumber: 5);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.FlurryBoots, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMirror, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMachine, selfRequiredNumber: 3);

            //天空箱子
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.ShinyRedBalloon, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.Starfury, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.LuckyHorseshoe, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 50, ItemID.SkyMill, selfRequiredNumber: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.CreativeWings, selfRequiredNumber: 12);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.HighPitch, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.BlessingfromTheHeavens, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.Constellation, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SeeTheWorldForWhatItIs, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.LoveisintheTrashSlot, selfRequiredNumber: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SunOrnament, selfRequiredNumber: 2);

            //魔力磁铁
            AddRemodelRecipe(ItemID.TreasureMagnet, 100, ItemID.CelestialMagnet);

            //金戒指
            AddRemodelRecipe(ItemID.GoldBar, 3500, ItemID.GoldRing, selfRequiredNumber: 50);
        }
    }
}
