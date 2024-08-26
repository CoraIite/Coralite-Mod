using Coralite.Content.Items.FlyingShields.Accessories;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    internal class Chests : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木箱子
            AddRemodelRecipe(ItemID.Chest, ItemID.Spear, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.Blowpipe, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.WoodenBoomerang, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.Aglet, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.ClimbingClaws, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.Umbrella, 100, mainStack: 3);
            AddRemodelRecipe(ItemID.Chest, ItemID.CordageGuide, 50, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, ItemID.WandofSparking, 150, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, ItemID.Radar, 150, mainStack: 2);
            AddRemodelRecipe(ItemID.Chest, ItemID.PortableStool, 10);
            AddRemodelRecipe(ItemID.Chest, ItemType<FlyingShieldVarnish>(), 100, 3);

            //金箱子
            AddRemodelRecipe(ItemID.GoldChest, ItemID.BandofRegeneration, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.MagicMirror, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.CloudinaBottle, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.HermesBoots, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.Mace, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.EnchantedBoomerang, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.ShoeSpikes, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.FlareGun, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.Extractinator, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemID.LavaCharm, 300, mainStack: 10);
            AddRemodelRecipe(ItemID.GoldChest, ItemType<FlyingShieldMaintenanceGuide>(), 250, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemType<FlyingShieldBattleGuide>(), 250, mainStack: 3);
            AddRemodelRecipe(ItemID.GoldChest, ItemType<HeavyWedges>(), 100, mainStack: 3);

            //沙漠箱子
            AddRemodelRecipe(ItemID.DesertChest, ItemID.ThunderSpear, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.ThunderStaff, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.MagicConch, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.MysticCoilSnake, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.AncientChisel, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.SandBoots, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.CatBast, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.DesertMinecart, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.DesertChest, ItemID.EncumberingStone, 150, mainStack: 3);

            //冰冻箱子
            AddRemodelRecipe(ItemID.IceChest, ItemID.Fish, 150, mainStack: 25);
            AddRemodelRecipe(ItemID.IceChest, ItemID.IceBlade, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.IceBoomerang, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.IceSkates, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.SnowballCannon, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.BlizzardinaBottle, 150, mainStack: 5);
            AddRemodelRecipe(ItemID.IceChest, ItemID.FlurryBoots, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.IceMirror, 50, mainStack: 3);
            AddRemodelRecipe(ItemID.IceChest, ItemID.IceMachine, 50, mainStack: 3);

            //天空箱子
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.ShinyRedBalloon, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.Starfury, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.LuckyHorseshoe, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.SkyMill, 50, mainStack: 3);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.CreativeWings, 150, mainStack: 12);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.HighPitch, 25, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.BlessingfromTheHeavens, 25, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.Constellation, 25, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.SeeTheWorldForWhatItIs, 25, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.LoveisintheTrashSlot, 25, mainStack: 2);
            AddRemodelRecipe(ItemID.SkywareChest, ItemID.SunOrnament, 25, mainStack: 2);

            //常春藤箱
            AddRemodelRecipe(ItemID.IvyChest, ItemID.FeralClaws, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.AnkletoftheWind, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.StaffofRegrowth, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.Boomstick, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.FlowerBoots, 150, mainStack: 5);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.FiberglassFishingPole, 150, mainStack: 5);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.Seaweed, 200, mainStack: 10);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.BeeMinecart, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.HoneyDispenser, 150, mainStack: 3);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.LivingMahoganyWand, 50, mainStack: 1);
            AddRemodelRecipe(ItemID.IvyChest, ItemID.LivingMahoganyLeafWand, 50, mainStack: 1);

        }
    }
}
