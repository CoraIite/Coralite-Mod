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
            MagikeRecipe.CreateRecipe(ItemID.Chest, ItemID.Spear, CalculateMagikeCost(MagicCrystal, 8, 60 * 3), 3)
                .RegisterNew(ItemID.Blowpipe, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.WoodenBoomerang, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.Aglet, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.ClimbingClaws, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.Umbrella, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.CordageGuide, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.WandofSparking, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.Radar, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew(ItemID.PortableStool, CalculateMagikeCost(MagicCrystal, 8, 180))
                .RegisterNew<FlyingShieldVarnish>(CalculateMagikeCost(MagicCrystal, 8, 180))
                .Register();

            //金箱子
            MagikeRecipe.CreateRecipe(ItemID.GoldChest, ItemID.BandofRegeneration, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNew(ItemID.MagicMirror, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.CloudinaBottle, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.HermesBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.Mace, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.EnchantedBoomerang, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.ShoeSpikes, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.FlareGun, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.Extractinator, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew<FlyingShieldMaintenanceGuide>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew<FlyingShieldBattleGuide>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew<HeavyWedges>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.LavaCharm, CalculateMagikeCost(RedJade, 8, 180))
                .SetMainStack(8)
                .Register();

            //地牢金箱子
            MagikeRecipe.CreateRecipe(ItemID.GoldChest, ItemID.Muramasa, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.CobaltShield, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.AquaScepter, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNew(ItemID.BubbleGun, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .AddCondition(Condition.RemixWorld)
                .RegisterNew(ItemID.BlueMoon, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.MagicMissile, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.Valor, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.Handgun, CalculateMagikeCost(Bone, 8, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.BoneWelder, CalculateMagikeCost(Bone, 3, 60 * 3))
                .AddIngredient(ItemID.GoldenKey)
                .AddCondition(Condition.DownedSkeletron)
                .Register();

            //沙漠箱子
            MagikeRecipe.CreateRecipe(ItemID.DesertChest, ItemID.ThunderSpear, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNew(ItemID.ThunderStaff, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.MagicConch, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.MysticCoilSnake, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.AncientChisel, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.SandBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.CatBast, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.DesertMinecart, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.EncumberingStone, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew<TremblingBow>(CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.PharaohsMask,CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.PharaohsRobe,CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.FlyingCarpet,CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.SandstorminaBottle,CalculateMagikeCost(RedJade, 8, 180))
                .Register();

            //冰冻箱子
            MagikeRecipe.CreateRecipe(ItemID.IceChest, ItemID.Fish, CalculateMagikeCost(RedJade, 8, 60 * 3), 3)
                .RegisterNew(ItemID.IceBlade, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.IceBoomerang, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.IceSkates, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.SnowballCannon, CalculateMagikeCost(RedJade, 8, 180))
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNew(ItemID.BlizzardinaBottle, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.FlurryBoots, CalculateMagikeCost(RedJade, 8, 180))
                .RegisterNew(ItemID.IceMachine, CalculateMagikeCost(RedJade, 8, 180))
                .Register();

            //天空箱子
            MagikeRecipe.CreateRecipe(ItemID.SkywareChest, ItemID.ShinyRedBalloon, CalculateMagikeCost(Glistent, 8, 60 * 3))
                .RegisterNew(ItemID.Starfury, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.LuckyHorseshoe, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.SkyMill, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.CreativeWings, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.HighPitch, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.BlessingfromTheHeavens, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.Constellation, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.SeeTheWorldForWhatItIs, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.LoveisintheTrashSlot, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.SunOrnament, CalculateMagikeCost(Glistent, 8, 180))
                .Register();

            //常春藤箱
            MagikeRecipe.CreateRecipe(ItemID.IvyChest, ItemID.FeralClaws, CalculateMagikeCost(Glistent, 8, 60 * 3), 3)
                .RegisterNew(ItemID.AnkletoftheWind, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.StaffofRegrowth, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.Boomstick, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.FlowerBoots, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.FiberglassFishingPole, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.Seaweed, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.BeeMinecart, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.HoneyDispenser, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.LivingMahoganyWand, CalculateMagikeCost(Glistent, 8, 180))
                .RegisterNew(ItemID.LivingMahoganyLeafWand, CalculateMagikeCost(Glistent, 8, 180))
                .Register();

            //暗影箱
            MagikeRecipe.CreateRecipe(ItemID.ShadowChest, ItemID.Sunfury, CalculateMagikeCost(Shadow, 8, 60 * 3), 3)
                .RegisterNew(ItemID.FlowerofFire, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNew(ItemID.UnholyTrident, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .AddCondition(Condition.RemixWorld)
                .RegisterNew(ItemID.Flamelash, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNew(ItemID.DarkLance, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNew(ItemID.HellwingBow, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNew(ItemID.TreasureMagnet, CalculateMagikeCost(Shadow, 8, 60 * 3))
                .RegisterNew(ItemID.HellMinecart, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .RegisterNew(ItemID.OrnateShadowKey, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .RegisterNew(ItemID.HellCake, CalculateMagikeCost(Shadow, 4, 60 * 3))
                .Register();
        }
    }
}
