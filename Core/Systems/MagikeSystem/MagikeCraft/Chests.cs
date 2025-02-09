using Coralite.Content.Items.FlyingShields.Accessories;
using Coralite.Content.Items.ThyphionSeries;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    internal class Chests : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            //木箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.Chest, ItemID.Spear, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.Blowpipe, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.WoodenBoomerang, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.Umbrella, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.WandofSparking, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNewCraft<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))
                .Register();

            //金箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.GoldChest, ItemID.BandofRegeneration, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.MagicMirror, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.CloudinaBottle, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.HermesBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.Mace, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.EnchantedBoomerang, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.ShoeSpikes, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.FlareGun, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.Extractinator, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft<FlyingShieldMaintenanceGuide>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft<FlyingShieldBattleGuide>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft<HeavyWedges>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.LavaCharm, CalculateMagikeCost(RedJade, 8, 180))
                .SetMainStack(8)
                .Register();

            //地牢金箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.GoldChest, ItemID.Muramasa, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.CobaltShield, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.AquaScepter, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.BubbleGun, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.RemixWorld)
                .RegisterNewCraft(ItemID.BlueMoon, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.MagicMissile, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.Valor, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.Handgun, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.BoneWelder, CalculateMagikeCost(Bone, 3, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .Register();

            //沙漠箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.DesertChest, ItemID.ThunderSpear, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.ThunderStaff, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.MagicConch, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.MysticCoilSnake, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.AncientChisel, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.SandBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.CatBast, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.DesertMinecart, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.EncumberingStone, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft<TremblingBow>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.PharaohsMask, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.PharaohsRobe, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.FlyingCarpet, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.SandstorminaBottle, CalculateMagikeCost(RedJade, 8, 180))
                .Register();

            //冰冻箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.IceChest, ItemID.Fish, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.IceBlade, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.IceBoomerang, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.IceSkates, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.SnowballCannon, CalculateMagikeCost(RedJade, 8, 180))
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.BlizzardinaBottle, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.FlurryBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNewCraft(ItemID.IceMachine, CalculateMagikeCost(RedJade, 8, 180))
                .Register();

            //天空箱子
            MagikeRecipe.CreateCraftRecipe(ItemID.SkywareChest, ItemID.ShinyRedBalloon, CalculateMagikeCost(Glistent, 8, 60 * 3))
                .RegisterNewCraft(ItemID.Starfury, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.LuckyHorseshoe, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.SkyMill, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.CreativeWings, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.HighPitch, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.BlessingfromTheHeavens, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.Constellation, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.SeeTheWorldForWhatItIs, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.LoveisintheTrashSlot, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.SunOrnament, CalculateMagikeCost(Glistent, 8, 180))
                .Register();

            //常春藤箱
            MagikeRecipe.CreateCraftRecipe(ItemID.IvyChest, ItemID.FeralClaws, CalculateMagikeCost(Glistent, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.AnkletoftheWind, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.StaffofRegrowth, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.Boomstick, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.FlowerBoots, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.FiberglassFishingPole, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.Seaweed, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.BeeMinecart, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.HoneyDispenser, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.LivingMahoganyWand, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNewCraft(ItemID.LivingMahoganyLeafWand, CalculateMagikeCost(Glistent, 8, 180))
                .Register();

            //暗影箱
            MagikeRecipe.CreateCraftRecipe(ItemID.ShadowChest, ItemID.Sunfury, CalculateMagikeCost(Shadow, 8, 60 * 3), 3)
                .RegisterNewCraft(ItemID.FlowerofFire, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.UnholyTrident, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .AddCondition(Condition.RemixWorld)
                .RegisterNewCraft(ItemID.Flamelash, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNewCraft(ItemID.DarkLance, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNewCraft(ItemID.HellwingBow, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNewCraft(ItemID.TreasureMagnet, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNewCraft(ItemID.HellMinecart, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .RegisterNewCraft(ItemID.OrnateShadowKey, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .RegisterNewCraft(ItemID.HellCake, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .Register();
        }
    }
}
