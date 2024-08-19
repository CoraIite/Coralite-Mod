using Coralite.Content.Items.FlyingShields.Accessories;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class VanillaRareItems : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            //火焰花
            AddRemodelRecipe(ItemID.Fireblossom, ItemID.LivingFireBlock, 25,  5);

            //凝胶
            AddRemodelRecipe(ItemID.Gel, 5, ItemID.PinkGel);
            AddRemodelRecipe(ItemID.Gel, 150, ItemID.SlimeStaff, mainStack: 999);

            //土块
            AddRemodelRecipe(ItemID.DirtBlock, 450, ItemID.DirtiestBlock, mainStack: 9999);

            //骨头
            AddRemodelRecipe(ItemID.Bone, 5000, ItemID.BoneFeather, mainStack: 99, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 1000, ItemID.BoneKey, mainStack: 999, condition: HardModeCondition.Instance);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BonePickaxe, mainStack: 20);
            AddRemodelRecipe(ItemID.Bone, 150, ItemID.BoneSword, mainStack: 20);

            //琥珀
            AddRemodelRecipe(ItemID.Amber, 300, ItemID.AmberMosquito, mainStack: 99);

            //黑曜石
            AddRemodelRecipe(ItemID.Obsidian, 400, ItemID.ShadowKey, mainStack: 99, condition: DownedSkeletronCondition.Instance);

            //青蛙
            AddRemodelRecipe(ItemID.Frog, 400, ItemID.FrogLeg, mainStack: 10);

            //木箱子
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Spear, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Blowpipe, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.WoodenBoomerang, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Aglet, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.ClimbingClaws, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 100, ItemID.Umbrella, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, 50, ItemID.CordageGuide, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.WandofSparking, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, 150, ItemID.Radar, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, 10, ItemID.PortableStool);
            AddRemodelRecipe<FlyingShieldVarnish>(0f, ItemID.Chest, 100, mainStack: 3);

            //金箱子
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.BandofRegeneration, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.MagicMirror, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.CloudinaBottle, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.HermesBoots, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Mace, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.EnchantedBoomerang, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.ShoeSpikes, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.FlareGun, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 150, ItemID.Extractinator, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, 300, ItemID.LavaCharm, mainStack: 10);
            AddRemodelRecipe<FlyingShieldMaintenanceGuide>(0f, ItemID.GoldChest, 250, mainStack: 3);
            AddRemodelRecipe<FlyingShieldBattleGuide>(0f, ItemID.GoldChest, 250, mainStack: 3);
            AddRemodelRecipe<HeavyWedges>(0f, ItemID.GoldChest, 100, mainStack: 3);

            //沙漠箱子
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderSpear, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.ThunderStaff, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MagicConch, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.MysticCoilSnake, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.AncientChisel, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.SandBoots, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.CatBast, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.DesertMinecart, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, 150, ItemID.EncumberingStone, mainStack: 3);

            //冰冻箱子
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.Fish, mainStack: 25);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBlade, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceBoomerang, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.IceSkates, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.SnowballCannon, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.BlizzardinaBottle, mainStack: 5);
            AddRemodelRecipe(ItemID.IceChest, 150, ItemID.FlurryBoots, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMirror, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, 50, ItemID.IceMachine, mainStack: 3);

            //天空箱子
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.ShinyRedBalloon, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.Starfury, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.LuckyHorseshoe, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 50, ItemID.SkyMill, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, 150, ItemID.CreativeWings, mainStack: 12);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.HighPitch, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.BlessingfromTheHeavens, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.Constellation, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SeeTheWorldForWhatItIs, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.LoveisintheTrashSlot, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, 25, ItemID.SunOrnament, mainStack: 2);

            //常春藤箱
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.FeralClaws, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.AnkletoftheWind, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.StaffofRegrowth, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.Boomstick, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.FlowerBoots, mainStack: 5);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.FiberglassFishingPole, mainStack: 5);
            AddRemodelRecipe(ItemID.IvyChest, 200, ItemID.Seaweed, mainStack: 10);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.BeeMinecart, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 150, ItemID.HoneyDispenser, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, 50, ItemID.LivingMahoganyWand, mainStack: 1);
            AddRemodelRecipe(ItemID.IvyChest, 50, ItemID.LivingMahoganyLeafWand, mainStack: 1);

            //魔力磁铁
            AddRemodelRecipe(ItemID.TreasureMagnet, 100, ItemID.CelestialMagnet);

            //金戒指
            AddRemodelRecipe(ItemID.GoldBar, 3500, ItemID.GoldRing, mainStack: 50);
        }
    }
}
