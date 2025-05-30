﻿using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Items.ThyphionSeries;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MALevel;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class BossBag : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            #region 赤玉灵
            MagikeRecipe.CreateCraftRecipe<RediancieBossBag, RedJade>(CalculateMagikeCost(MALevel.RedJade, 2, 120)
                , resultItemStack: 34)
                .RegisterNewCraft<RediancieMask>(CalculateMagikeCost(MALevel.RedJade))
                .RegisterNewCraft<RediancieTrophy>(CalculateMagikeCost(MALevel.RedJade, 2, 120))
                .RegisterNewCraft<RedianciePet>(CalculateMagikeCost(MALevel.RedJade))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆王
            MagikeRecipe.CreateCraftRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, CalculateMagikeCost(MALevel.RedJade, 5, 120))
                .RegisterNewCraft(ItemID.NinjaHood, CalculateMagikeCost(MALevel.RedJade, 4, 120))
                .RegisterNewCraft(ItemID.NinjaShirt, CalculateMagikeCost(MALevel.RedJade, 4, 120))
                .RegisterNewCraft(ItemID.NinjaPants, CalculateMagikeCost(MALevel.RedJade, 4, 120))
                .RegisterNewCraft(ItemID.SlimeHook, CalculateMagikeCost(MALevel.RedJade, 4, 120))
                .RegisterNewCraft(ItemID.SlimeGun, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNewCraft(ItemID.KingSlimeMask, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNewCraft(ItemID.KingSlimeTrophy, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNewCraft(ItemID.KingSlimePetItem, CalculateMagikeCost(MALevel.RedJade))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之眼
            MagikeRecipe.CreateCraftRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost(Glistent, 4, 120)
                , resultItemStack: 85)
                .RegisterNewCraft(ItemID.DemoniteOre, CalculateMagikeCost(Glistent, 4, 120), 85)
                .RegisterNewCraft(ItemID.Binoculars, CalculateMagikeCost(Glistent, 5, 180))
                .RegisterNewCraft(ItemID.EyeMask, CalculateMagikeCost(Glistent))
                .RegisterNewCraft(ItemID.EyeofCthulhuTrophy, CalculateMagikeCost(Glistent))
                .RegisterNewCraft(ItemID.EyeOfCthulhuPetItem, CalculateMagikeCost(Glistent))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 世界吞噬怪
            MagikeRecipe.CreateCraftRecipe(ItemID.EaterOfWorldsBossBag, ItemID.DemoniteOre, CalculateMagikeCost(Corruption, 4, 120), resultItemStack: 70)
                .RegisterNewCraft(ItemID.ShadowScale, CalculateMagikeCost(Corruption, 4, 120), 25)
                .RegisterNewCraft(ItemID.EatersBone, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.EaterMask, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.EaterofWorldsTrophy, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.EaterOfWorldsPetItem, CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之脑
            MagikeRecipe.CreateCraftRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost(Corruption, 4, 120), resultItemStack: 70)
                .RegisterNewCraft(ItemID.TissueSample, CalculateMagikeCost(Corruption, 4, 120), 25)
                .RegisterNewCraft(ItemID.BoneRattle, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.BrainMask, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.BrainofCthulhuTrophy, CalculateMagikeCost(Corruption))
                .RegisterNewCraft(ItemID.BrainOfCthulhuPetItem, CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 冰龙宝宝
            MagikeRecipe.CreateCraftRecipe<BabyIceDragonBossBag, IcicleCrystal>(CalculateMagikeCost(Corruption, 4, 120), resultItemStack: 8)
                .RegisterNewCraft<IcicleScale>(CalculateMagikeCost(Corruption, 4, 120), 7)
                .RegisterNewCraft<IcicleBreath>(CalculateMagikeCost(Corruption, 4, 120), 10)
                .RegisterNewCraft<BabyIceDragonMask>(CalculateMagikeCost(Corruption))
                .RegisterNewCraft<BabyIceDragonTrophy>(CalculateMagikeCost(Corruption))
                .RegisterNewCraft<IcicleSoulStone>(CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 蜂后
            MagikeRecipe.CreateCraftRecipe(ItemID.QueenBeeBossBag, ItemID.BeeGun, CalculateMagikeCost(Beeswax, 4, 120))
                .RegisterNewCraft(ItemID.BeeKeeper, CalculateMagikeCost(Beeswax, 4, 120))//养蜂人
                .RegisterNewCraft(ItemID.BeesKnees, CalculateMagikeCost(Beeswax, 4, 120))
                .RegisterNewCraft(ItemID.BeeHat, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNewCraft(ItemID.BeeShirt, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNewCraft(ItemID.BeePants, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNewCraft(ItemID.HoneyComb, CalculateMagikeCost(Beeswax, 4, 120))
                .RegisterNewCraft(ItemID.Nectar, CalculateMagikeCost(Beeswax))//蜜蜂宠物
                .RegisterNewCraft(ItemID.HoneyedGoggles, CalculateMagikeCost(Beeswax, 4, 120))//蜜蜂坐骑
                .RegisterNewCraft(ItemID.Beenade, CalculateMagikeCost(Beeswax, 4, 120), 80)//蜜蜂手雷
                .RegisterNewCraft(ItemID.BeeMask, CalculateMagikeCost(Beeswax))
                .RegisterNewCraft(ItemID.QueenBeeTrophy, CalculateMagikeCost(Beeswax))
                .RegisterNewCraft(ItemID.QueenBeePetItem, CalculateMagikeCost(Beeswax))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 巨鹿
            MagikeRecipe.CreateCraftRecipe(ItemID.DeerclopsBossBag, ItemID.ChesterPetItem, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.Eyebrella, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.DontStarveShaderItem, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.DizzyHat, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.PewMaticHorn, CalculateMagikeCost(Hellstone, 4, 120))
                .RegisterNewCraft(ItemID.WeatherPain, CalculateMagikeCost(Hellstone, 4, 120))
                .RegisterNewCraft(ItemID.HoundiusShootius, CalculateMagikeCost(Hellstone, 4, 120))
                .RegisterNewCraft(ItemID.LucyTheAxe, CalculateMagikeCost(Hellstone, 4, 120))
                .RegisterNewCraft(ItemID.DeerclopsMask, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.DeerclopsTrophy, CalculateMagikeCost(Hellstone))
                .RegisterNewCraft(ItemID.DeerclopsPetItem, CalculateMagikeCost(Hellstone))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 骷髅王
            MagikeRecipe.CreateCraftRecipe(ItemID.SkeletronBossBag, ItemID.SkeletronHand, CalculateMagikeCost(Bone, 4, 120))
                .RegisterNewCraft(ItemID.BookofSkulls, CalculateMagikeCost(Bone, 4, 120))
                .RegisterNewCraft(ItemID.ChippysCouch, CalculateMagikeCost(Bone))
                .RegisterNewCraft(ItemID.SkeletronMask, CalculateMagikeCost(Bone))
                .RegisterNewCraft(ItemID.SkeletronTrophy, CalculateMagikeCost(Bone))
                .RegisterNewCraft(ItemID.SkeletronPetItem, CalculateMagikeCost(Bone))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 肉山
            MagikeRecipe.CreateCraftRecipe(ItemID.WallOfFleshBossBag, ItemID.WarriorEmblem, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.RangerEmblem, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.SorcererEmblem, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.SummonerEmblem, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.BreakerBlade, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.ClockworkAssaultRifle, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.LaserRifle, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.FireWhip, CalculateMagikeCost(Quicksand, 4, 120))
                .RegisterNewCraft(ItemID.FleshMask, CalculateMagikeCost(Quicksand))
                .RegisterNewCraft(ItemID.WallofFleshTrophy, CalculateMagikeCost(Quicksand))
                .RegisterNewCraft(ItemID.WallOfFleshGoatMountItem, CalculateMagikeCost(Quicksand))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆皇后
            MagikeRecipe.CreateCraftRecipe(ItemID.QueenSlimeBossBag, ItemID.GelBalloon, CalculateMagikeCost(Flight, 4, 120), resultItemStack: 100)
                .RegisterNewCraft(ItemID.Smolstar, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeHook, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeMountSaddle, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaHelmet, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaChestplate, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaLeggings, CalculateMagikeCost(Flight, 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeMask, CalculateMagikeCost(Flight))
                .RegisterNewCraft(ItemID.QueenSlimeTrophy, CalculateMagikeCost(Flight))
                .RegisterNewCraft(ItemID.QueenSlimePetItem, CalculateMagikeCost(Flight))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 双子魔眼
            MagikeRecipe.CreateCraftRecipe(ItemID.TwinsBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofSight, CalculateMagikeCost(Hallow, 4, 120), 40)
                .RegisterNewCraft(ItemID.TwinMask, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.RetinazerTrophy, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.SpazmatismTrophy, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.TwinsPetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 铁长直
            MagikeRecipe.CreateCraftRecipe(ItemID.DestroyerBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofMight, CalculateMagikeCost(Hallow, 4, 120), 40)
                .RegisterNewCraft(ItemID.DestroyerMask, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.DestroyerTrophy, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.DestroyerPetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 铁骷髅王
            MagikeRecipe.CreateCraftRecipe(ItemID.SkeletronPrimeBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofFright, CalculateMagikeCost(Hallow, 4, 120), 40)
                .RegisterNewCraft(ItemID.SkeletronPrimeMask, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.SkeletronPrimeTrophy, CalculateMagikeCost(Hallow))
                .RegisterNewCraft(ItemID.SkeletronPrimePetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 荒雷龙
            MagikeRecipe.CreateCraftRecipe<ThunderveinDragonBossBag, ZapCrystal>(CalculateMagikeCost(Hallow, 4, 120), resultItemStack: 16)
                .RegisterNewCraft<ElectrificationWing>(CalculateMagikeCost(Hallow, 4, 120), 6)
                .RegisterNewCraft<InsulationCortex>(CalculateMagikeCost(Hallow, 4, 120), 20)
                .RegisterNewCraft<ThunderveinDragonMask>(CalculateMagikeCost(Hallow))
                .RegisterNewCraft<ThunderveinDragonTrophy>(CalculateMagikeCost(Hallow))
                .RegisterNewCraft<ThunderveinSoulStone>(CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 世纪之花宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.PlanteraBossBag, ItemID.GrenadeLauncher, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.VenusMagnum, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.NettleBurst, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.FlowerPow, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.WaspGun, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.Seedler, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.Seedling, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.TheAxe, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.PygmyStaff, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.ThornHook, CalculateMagikeCost(Soul, 4, 120))
                .RegisterNewCraft(ItemID.PlanteraMask, CalculateMagikeCost(Soul))
                .RegisterNewCraft(ItemID.PlanteraTrophy, CalculateMagikeCost(Soul))
                .RegisterNewCraft(ItemID.PlanteraPetItem, CalculateMagikeCost(Soul))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(Soul), 3)
                .Register();
            #endregion

            #region 石巨人宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.GolemBossBag, ItemID.BeetleHusk, CalculateMagikeCost(HolyLight, 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.Picksaw, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.Stynger, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.PossessedHatchet, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.SunStone, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.HeatRay, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.EyeoftheGolem, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.StaffofEarth, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.GolemFist, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.GolemMask, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.GolemTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.GolemPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 猪鲨宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.FishronBossBag, ItemID.BubbleGun, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.Flairon, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.RazorbladeTyphoon, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.TempestStaff, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.Tsunami, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.FishronWings, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.DukeFishronMask, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.DukeFishronTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.DukeFishronPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 光女宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.FairyQueenBossBag, ItemID.FairyQueenMagicItem, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.PiercingStarlight, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.RainbowWhip, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.FairyQueenRangedItem, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.RainbowWings, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.SparkleGuitar, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.RainbowCursor, CalculateMagikeCost(HolyLight, 4, 120))
                .RegisterNewCraft(ItemID.HallowBossDye, CalculateMagikeCost(HolyLight, 4, 120), 3)
                .RegisterNewCraft(ItemID.FairyQueenMask, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.FairyQueenTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNewCraft(ItemID.FairyQueenPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 月总宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.MoonLordBossBag, ItemID.Meowmere, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.Terrarian, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.StarWrath, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.SDMG, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.LastPrism, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.LunarFlareBook, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.RainbowCrystalStaff, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.MoonlordTurretStaff, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft<ConquerorOfTheSeas>(CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft<Aurora>(CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.Celeb2, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.LunarOre, CalculateMagikeCost(SplendorMagicore, 4, 120), 170)
                .RegisterNewCraft(ItemID.MeowmereMinecart, CalculateMagikeCost(SplendorMagicore, 4, 120))
                .RegisterNewCraft(ItemID.BossMaskMoonlord, CalculateMagikeCost(SplendorMagicore))
                .RegisterNewCraft(ItemID.MoonLordTrophy, CalculateMagikeCost(SplendorMagicore))
                .RegisterNewCraft(ItemID.MoonLordPetItem, CalculateMagikeCost(SplendorMagicore))
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost(SplendorMagicore), 3)
                .Register();
            #endregion
        }
    }
}
