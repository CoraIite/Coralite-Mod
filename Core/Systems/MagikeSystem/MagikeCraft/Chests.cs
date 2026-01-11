using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    internal class Chests : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木箱子
            int woddenChestCost = CalculateMagikeCost<CrystalLevel>(4, 60 * 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.Chest, ItemID.Spear, woddenChestCost, 3)
                .RegisterNewCraft(ItemID.Blowpipe, woddenChestCost)
                .RegisterNewCraft(ItemID.WoodenBoomerang, woddenChestCost)
                .RegisterNewCraft(ItemID.Aglet, woddenChestCost)
                .RegisterNewCraft(ItemID.ClimbingClaws, woddenChestCost)
                .RegisterNewCraft(ItemID.Umbrella, woddenChestCost)
                .RegisterNewCraft(ItemID.CordageGuide, woddenChestCost)
                .RegisterNewCraft(ItemID.WandofSparking, woddenChestCost)
                .RegisterNewCraft(ItemID.Radar, woddenChestCost)
                .RegisterNewCraft(ItemID.PortableStool, woddenChestCost)
                .RegisterNewCraft<FlyingShieldVarnish>(woddenChestCost)
                .Register();

            //金箱子
            int goldChectCost = CalculateMagikeCost<RedJadeLevel>(4, 60 * 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.GoldChest, ItemID.BandofRegeneration, goldChectCost, 3)
                .RegisterNewCraft(ItemID.MagicMirror, goldChectCost)
                .RegisterNewCraft(ItemID.CloudinaBottle, goldChectCost)
                .RegisterNewCraft(ItemID.HermesBoots, goldChectCost)
                .RegisterNewCraft(ItemID.Mace, goldChectCost)
                .RegisterNewCraft(ItemID.EnchantedBoomerang, goldChectCost)
                .RegisterNewCraft(ItemID.ShoeSpikes, goldChectCost)
                .RegisterNewCraft(ItemID.FlareGun, goldChectCost)
                .RegisterNewCraft(ItemID.Extractinator, goldChectCost)
                .RegisterNewCraft<FlyingShieldMaintenanceGuide>(goldChectCost)
                .RegisterNewCraft<FlyingShieldBattleGuide>(goldChectCost)
                .RegisterNewCraft<HeavyWedges>(goldChectCost)
                .RegisterNewCraft(ItemID.LavaCharm, goldChectCost)
                .SetMainStack(8)
                .Register();

            //地牢金箱子
            int dungeonChestCost = CalculateMagikeCost<BoneLevel>(4, 60 * 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.GoldChest, ItemID.Muramasa, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.CobaltShield, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.AquaScepter, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.BubbleGun, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.RemixWorld)
                .RegisterNewCraft(ItemID.BlueMoon, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.MagicMissile, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.Valor, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.Handgun, dungeonChestCost)
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.BoneWelder, CalculateMagikeCost<BoneLevel>(3, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .Register();

            //沙漠箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.DesertChest, ItemID.ThunderSpear, goldChectCost, 3)
                .RegisterNewCraft(ItemID.ThunderStaff, goldChectCost)
                .RegisterNewCraft(ItemID.MagicConch, goldChectCost)
                .RegisterNewCraft(ItemID.MysticCoilSnake, goldChectCost)
                .RegisterNewCraft(ItemID.AncientChisel, goldChectCost)
                .RegisterNewCraft(ItemID.SandBoots, goldChectCost)
                .RegisterNewCraft(ItemID.CatBast, goldChectCost)
                .RegisterNewCraft(ItemID.DesertMinecart, goldChectCost)
                .RegisterNewCraft(ItemID.EncumberingStone, goldChectCost)
                .RegisterNewCraft<TremblingBow>(goldChectCost)
                .RegisterNewCraft(ItemID.PharaohsMask, goldChectCost)
                .RegisterNewCraft(ItemID.PharaohsRobe, goldChectCost)
                .RegisterNewCraft(ItemID.FlyingCarpet, goldChectCost)
                .RegisterNewCraft(ItemID.SandstorminaBottle, goldChectCost)
                .Register();

            //冰冻箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.IceChest, ItemID.Fish, goldChectCost, 3)
                .RegisterNewCraft(ItemID.IceBlade, goldChectCost)
                .RegisterNewCraft(ItemID.IceBoomerang, goldChectCost)
                .RegisterNewCraft(ItemID.IceSkates, goldChectCost)
                .RegisterNewCraft(ItemID.SnowballCannon, goldChectCost)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.BlizzardinaBottle, goldChectCost)
                .RegisterNewCraft(ItemID.FlurryBoots, goldChectCost)
                .RegisterNewCraft(ItemID.IceMachine, goldChectCost)
                .Register();

            //水中箱
            MagikeRecipe.CreateCraftRecipe(ItemID.WaterChest, ItemID.BreathingReed, goldChectCost, 3)
                .RegisterNewCraft(ItemID.Flipper, goldChectCost)
                .RegisterNewCraft(ItemID.Trident, goldChectCost)
                .RegisterNewCraft(ItemID.FloatingTube, goldChectCost)
                .RegisterNewCraft(ItemID.WaterWalkingBoots, goldChectCost)
                .RegisterNewCraft(ItemID.BeachBall, goldChectCost)
                .RegisterNewCraft(ItemID.SandcastleBucket, goldChectCost)
                .RegisterNewCraft(ItemID.SharkBait, goldChectCost)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.Chest, ItemID.WaterChest, CalculateMagikeCost<CrystalLevel>(1))
                .AddIngredient(ItemID.Coral)
                .AddIngredient(ItemID.WaterBucket)
                .Register();

            //天空箱子
            int skyChestCost = CalculateMagikeCost<GlistentLevel>(4, 180);
            MagikeRecipe.CreateCraftRecipe(ItemID.SkywareChest, ItemID.ShinyRedBalloon, skyChestCost)
                .RegisterNewCraft(ItemID.Starfury, skyChestCost)
                .RegisterNewCraft(ItemID.LuckyHorseshoe, skyChestCost)
                .RegisterNewCraft(ItemID.SkyMill, skyChestCost)
                .RegisterNewCraft(ItemID.CreativeWings, skyChestCost)
                .RegisterNewCraft(ItemID.HighPitch, skyChestCost)
                .RegisterNewCraft(ItemID.BlessingfromTheHeavens, skyChestCost)
                .RegisterNewCraft(ItemID.Constellation, skyChestCost)
                .RegisterNewCraft(ItemID.SeeTheWorldForWhatItIs, skyChestCost)
                .RegisterNewCraft(ItemID.LoveisintheTrashSlot, skyChestCost)
                .RegisterNewCraft(ItemID.SunOrnament, skyChestCost)
                .Register();

            //常春藤箱
            MagikeRecipe.CreateCraftRecipe(ItemID.IvyChest, ItemID.FeralClaws, skyChestCost, 3)
                .RegisterNewCraft(ItemID.AnkletoftheWind, skyChestCost)
                .RegisterNewCraft(ItemID.StaffofRegrowth, skyChestCost)
                .RegisterNewCraft(ItemID.Boomstick, skyChestCost)
                .RegisterNewCraft(ItemID.FlowerBoots, skyChestCost)
                .RegisterNewCraft(ItemID.FiberglassFishingPole, skyChestCost)
                .RegisterNewCraft(ItemID.Seaweed, skyChestCost)
                .RegisterNewCraft(ItemID.BeeMinecart, skyChestCost)
                .RegisterNewCraft(ItemID.HoneyDispenser, skyChestCost)
                .RegisterNewCraft(ItemID.LivingMahoganyWand, skyChestCost)
                .RegisterNewCraft(ItemID.LivingMahoganyLeafWand, skyChestCost)
                .Register();

            //暗影箱
            int shadowChestCost = CalculateMagikeCost<ShadowLevel>(4, 60 * 3);
            MagikeRecipe.CreateCraftRecipe(ItemID.ShadowChest, ItemID.Sunfury, shadowChestCost, 3)
                .RegisterNewCraft(ItemID.FlowerofFire, shadowChestCost)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.UnholyTrident, shadowChestCost)
                .AddCondition(Condition.RemixWorld)
                .RegisterNewCraft(ItemID.Flamelash, shadowChestCost)
                .RegisterNewCraft(ItemID.DarkLance, shadowChestCost)
                .RegisterNewCraft(ItemID.HellwingBow, shadowChestCost)
                .RegisterNewCraft(ItemID.TreasureMagnet, shadowChestCost)
                .RegisterNewCraft(ItemID.HellMinecart, CalculateMagikeCost<ShadowLevel>(4, 60 * 3))
                .RegisterNewCraft(ItemID.OrnateShadowKey, CalculateMagikeCost<ShadowLevel>(4, 60 * 3))
                .RegisterNewCraft(ItemID.HellCake, CalculateMagikeCost<ShadowLevel>(4, 60 * 3))
                .Register();
        }
    }
}
