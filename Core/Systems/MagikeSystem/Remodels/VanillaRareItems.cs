using Coralite.Core.Systems.MagikeSystem.CraftConditions;
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
            AddRemodelRecipe(ItemID.Gel, 150, ItemID.SlimeStaff,selfStack:999);

            //土块
            AddRemodelRecipe(ItemID.DirtBlock, 450, ItemID.DirtiestBlock, selfStack: 9999);

            //骨头
            AddRemodelRecipe(ItemID.Bone, 5000, ItemID.BoneFeather, selfStack: 99, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 1000, ItemID.BoneKey, selfStack: 999, condition: HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BonePickaxe, selfStack: 20);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BoneSword, selfStack: 20);

            //琥珀
            AddRemodelRecipe(ItemID.Amber, 300, ItemID.AmberMosquito, selfStack: 99);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, 400, ItemID.ShadowKey, selfStack: 99,condition:DownedSkeletronCondition.Instance);

            //青蛙
            AddRemodelRecipe(ItemID.Frog, 400, ItemID.FrogLeg, selfStack: 10);

            //木箱子
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Spear, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Blowpipe, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.WoodenBoomerang, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Aglet, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.ClimbingClaws, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Umbrella, selfStack: 3);
            AddRemodelRecipe(ItemID.Chest, 50, ItemID.CordageGuide, selfStack: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.WandofSparking, selfStack: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.Radar, selfStack: 2);
            AddRemodelRecipe(ItemID.Chest, 10, ItemID.PortableStool);

            //金箱子
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.BandofRegeneration, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.MagicMirror, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.CloudinaBottle, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.HermesBoots, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Mace, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.EnchantedBoomerang, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.ShoeSpikes, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.FlareGun, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Extractinator, selfStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 300, ItemID.LavaCharm, selfStack: 10);

            //沙漠箱子
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderSpear, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderStaff, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MagicConch, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MysticCoilSnake, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.AncientChisel, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.SandBoots, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.CatBast, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.DesertMinecart, selfStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.EncumberingStone, selfStack: 3);

            //冰冻箱子
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.Fish, selfStack: 25);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBlade, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBoomerang, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceSkates, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.SnowballCannon, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.BlizzardinaBottle, selfStack: 5);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.FlurryBoots, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMirror, selfStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMachine, selfStack: 3);

            //天空箱子
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.ShinyRedBalloon, selfStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.Starfury, selfStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.LuckyHorseshoe, selfStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 50, ItemID.SkyMill, selfStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.CreativeWings, selfStack: 12);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.HighPitch, selfStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.BlessingfromTheHeavens, selfStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.Constellation, selfStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SeeTheWorldForWhatItIs, selfStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.LoveisintheTrashSlot, selfStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SunOrnament, selfStack: 2);

            //魔力磁铁
            AddRemodelRecipe(ItemID.TreasureMagnet, 100, ItemID.CelestialMagnet);

            //金戒指
            AddRemodelRecipe(ItemID.GoldBar, 3500, ItemID.GoldRing, selfStack: 50);
        }
    }
}
