using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Thunder;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;
using static Coralite.Core.Systems.MagikeSystem.MALevel;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossBag : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            #region 赤玉灵
            MagikeCraftRecipe.CreateRecipe<RediancieBossBag, RedJade>(CalculateMagikeCost(MALevel.RedJade,5,120)
                , resultItemStack: 34)
                .RegisterNew<RediancieMask>(CalculateMagikeCost(MALevel.RedJade))
                .RegisterNew<RediancieTrophy>(CalculateMagikeCost(MALevel.RedJade,5,120))
                .RegisterNew<RedianciePet>(CalculateMagikeCost(MALevel.RedJade))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆王
            MagikeCraftRecipe.CreateRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, CalculateMagikeCost(MALevel.RedJade,5,120))
                .RegisterNew(ItemID.NinjaHood, CalculateMagikeCost(MALevel.RedJade,8,180))
                .RegisterNew(ItemID.NinjaShirt, CalculateMagikeCost(MALevel.RedJade, 8, 180))
                .RegisterNew(ItemID.NinjaPants, CalculateMagikeCost(MALevel.RedJade, 8, 180))
                .RegisterNew(ItemID.SlimeHook, CalculateMagikeCost(MALevel.RedJade, 10, 180))
                .RegisterNew(ItemID.SlimeGun, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNew(ItemID.KingSlimeMask, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNew(ItemID.KingSlimeTrophy, CalculateMagikeCost(MALevel.RedJade))
                .RegisterNew(ItemID.KingSlimePetItem, CalculateMagikeCost(MALevel.RedJade))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之眼
            MagikeCraftRecipe.CreateRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost(Glistent,10,180)
                , resultItemStack: 85)
                .RegisterNew(ItemID.DemoniteOre, CalculateMagikeCost(Glistent, 10, 180), 85)
                .RegisterNew(ItemID.Binoculars, CalculateMagikeCost(Glistent, 5, 180))
                .RegisterNew(ItemID.EyeMask, CalculateMagikeCost(Glistent))
                .RegisterNew(ItemID.EyeofCthulhuTrophy, CalculateMagikeCost(Glistent))
                .RegisterNew(ItemID.EyeOfCthulhuPetItem, CalculateMagikeCost(Glistent))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 世界吞噬怪
            MagikeCraftRecipe.CreateRecipe(ItemID.EaterOfWorldsBossBag, ItemID.DemoniteOre, CalculateMagikeCost(Corruption,10,180), resultItemStack: 70)
                .RegisterNew(ItemID.ShadowScale, CalculateMagikeCost(Corruption, 10, 180), 25)
                .RegisterNew(ItemID.EatersBone, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.EaterMask, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.EaterofWorldsTrophy, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.EaterOfWorldsPetItem, CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之脑
            MagikeCraftRecipe.CreateRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost(Corruption, 10, 180), resultItemStack: 70)
                .RegisterNew(ItemID.TissueSample, CalculateMagikeCost(Corruption, 10, 180), 25)
                .RegisterNew(ItemID.BoneRattle, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.BrainMask, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.BrainofCthulhuTrophy, CalculateMagikeCost(Corruption))
                .RegisterNew(ItemID.BrainOfCthulhuPetItem, CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 冰龙宝宝
            MagikeCraftRecipe.CreateRecipe<BabyIceDragonBossBag, IcicleCrystal>(CalculateMagikeCost(Corruption,10,180), resultItemStack: 8)
                .RegisterNew<IcicleScale>(CalculateMagikeCost(Corruption, 10, 180), 7)
                .RegisterNew<IcicleBreath>(CalculateMagikeCost(Corruption, 10, 180), 10)
                .RegisterNew<BabyIceDragonMask>(CalculateMagikeCost(Corruption))
                .RegisterNew<BabyIceDragonTrophy>(CalculateMagikeCost(Corruption))
                .RegisterNew<IcicleSoulStone>(CalculateMagikeCost(Corruption))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 蜂后
            MagikeCraftRecipe.CreateRecipe(ItemID.QueenBeeBossBag, ItemID.BeeGun, CalculateMagikeCost(Beeswax, 10, 180))
                .RegisterNew(ItemID.BeeKeeper, CalculateMagikeCost(Beeswax, 10, 180))//养蜂人
                .RegisterNew(ItemID.BeesKnees, CalculateMagikeCost(Beeswax, 10, 180))
                .RegisterNew(ItemID.BeeHat, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNew(ItemID.BeeShirt, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNew(ItemID.BeePants, CalculateMagikeCost(Beeswax, 3, 180))
                .RegisterNew(ItemID.HoneyComb, CalculateMagikeCost(Beeswax, 10, 180))
                .RegisterNew(ItemID.Nectar, CalculateMagikeCost(Beeswax))//蜜蜂宠物
                .RegisterNew(ItemID.HoneyedGoggles, CalculateMagikeCost(Beeswax, 10, 180))//蜜蜂坐骑
                .RegisterNew(ItemID.Beenade, CalculateMagikeCost(Beeswax, 10, 180), 80)//蜜蜂手雷
                .RegisterNew(ItemID.BeeMask, CalculateMagikeCost(Beeswax))
                .RegisterNew(ItemID.QueenBeeTrophy, CalculateMagikeCost(Beeswax))
                .RegisterNew(ItemID.QueenBeePetItem, CalculateMagikeCost(Beeswax))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 巨鹿
            MagikeCraftRecipe.CreateRecipe(ItemID.DeerclopsBossBag, ItemID.ChesterPetItem, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.Eyebrella, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.DontStarveShaderItem, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.DizzyHat, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.PewMaticHorn, CalculateMagikeCost(Hellstone,10,180))
                .RegisterNew(ItemID.WeatherPain, CalculateMagikeCost(Hellstone, 10, 180))
                .RegisterNew(ItemID.HoundiusShootius, CalculateMagikeCost(Hellstone, 10, 180))
                .RegisterNew(ItemID.LucyTheAxe, CalculateMagikeCost(Hellstone, 10, 180))
                .RegisterNew(ItemID.DeerclopsMask, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.DeerclopsTrophy, CalculateMagikeCost(Hellstone))
                .RegisterNew(ItemID.DeerclopsPetItem, CalculateMagikeCost(Hellstone))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 骷髅王
            MagikeCraftRecipe.CreateRecipe(ItemID.SkeletronBossBag, ItemID.SkeletronHand, CalculateMagikeCost(Bone, 10, 180))
                .RegisterNew(ItemID.BookofSkulls, CalculateMagikeCost(Bone,10,180))
                .RegisterNew(ItemID.ChippysCouch, CalculateMagikeCost(Bone))
                .RegisterNew(ItemID.SkeletronMask, CalculateMagikeCost(Bone))
                .RegisterNew(ItemID.SkeletronTrophy, CalculateMagikeCost(Bone))
                .RegisterNew(ItemID.SkeletronPetItem, CalculateMagikeCost(Bone))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 肉山
            MagikeCraftRecipe.CreateRecipe(ItemID.WallOfFleshBossBag, ItemID.WarriorEmblem, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.RangerEmblem, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.SorcererEmblem, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.SummonerEmblem, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.BreakerBlade, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.ClockworkAssaultRifle, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.LaserRifle, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.FireWhip, CalculateMagikeCost(Quicksand, 15, 180))
                .RegisterNew(ItemID.FleshMask, CalculateMagikeCost(Quicksand))
                .RegisterNew(ItemID.WallofFleshTrophy, CalculateMagikeCost(Quicksand))
                .RegisterNew(ItemID.WallOfFleshGoatMountItem, CalculateMagikeCost(Quicksand))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆皇后
            MagikeCraftRecipe.CreateRecipe(ItemID.QueenSlimeBossBag, ItemID.GelBalloon, CalculateMagikeCost(Flight, 10, 180), resultItemStack: 100)
                .RegisterNew(ItemID.Smolstar, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.QueenSlimeHook, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.QueenSlimeMountSaddle, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.CrystalNinjaHelmet, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.CrystalNinjaChestplate, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.CrystalNinjaLeggings, CalculateMagikeCost(Flight, 10, 180))
                .RegisterNew(ItemID.QueenSlimeMask, CalculateMagikeCost(Flight))
                .RegisterNew(ItemID.QueenSlimeTrophy, CalculateMagikeCost(Flight))
                .RegisterNew(ItemID.QueenSlimePetItem, CalculateMagikeCost(Flight))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 双子魔眼
            MagikeCraftRecipe.CreateRecipe(ItemID.TwinsBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 10, 180), resultItemStack: 35)
                .RegisterNew(ItemID.SoulofSight, CalculateMagikeCost(Hallow, 10, 180), 40)
                .RegisterNew(ItemID.TwinMask, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.RetinazerTrophy, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.SpazmatismTrophy, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.TwinsPetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 铁长直
            MagikeCraftRecipe.CreateRecipe(ItemID.DestroyerBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 10, 180), resultItemStack: 35)
                .RegisterNew(ItemID.SoulofMight, CalculateMagikeCost(Hallow, 10, 180), 40)
                .RegisterNew(ItemID.DestroyerMask, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.DestroyerTrophy, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.DestroyerPetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 铁骷髅王
            MagikeCraftRecipe.CreateRecipe(ItemID.SkeletronPrimeBossBag, ItemID.HallowedBar, CalculateMagikeCost(Hallow, 10, 180), resultItemStack: 35)
                .RegisterNew(ItemID.SoulofFright, CalculateMagikeCost(Hallow, 10, 180), 40)
                .RegisterNew(ItemID.SkeletronPrimeMask, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.SkeletronPrimeTrophy, CalculateMagikeCost(Hallow))
                .RegisterNew(ItemID.SkeletronPrimePetItem, CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 荒雷龙
            MagikeCraftRecipe.CreateRecipe<ThunderveinDragonBossBag, ZapCrystal>(CalculateMagikeCost(Hallow, 10, 180), resultItemStack: 16)
                .RegisterNew<ElectrificationWing>(CalculateMagikeCost(Hallow, 10, 180), 6)
                .RegisterNew<InsulationCortex>(CalculateMagikeCost(Hallow, 10, 180), 20)
                .RegisterNew<ThunderveinDragonMask>(CalculateMagikeCost(Hallow))
                .RegisterNew<ThunderveinDragonTrophy>(CalculateMagikeCost(Hallow))
                .RegisterNew<ThunderveinSoulStone>(CalculateMagikeCost(Hallow))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(Hallow), 3)
                .Register();
            #endregion

            #region 世纪之花宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.PlanteraBossBag, ItemID.GrenadeLauncher, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.VenusMagnum, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.NettleBurst, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.FlowerPow, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.WaspGun, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.Seedler, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.Seedling, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.TheAxe, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.PygmyStaff, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.ThornHook, CalculateMagikeCost(Soul, 10, 180))
                .RegisterNew(ItemID.PlanteraMask, CalculateMagikeCost(Soul ))
                .RegisterNew(ItemID.PlanteraTrophy, CalculateMagikeCost(Soul))
                .RegisterNew(ItemID.PlanteraPetItem, CalculateMagikeCost(Soul))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(Soul), 3)
                .Register();
            #endregion

            #region 石巨人宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.GolemBossBag, ItemID.BeetleHusk, CalculateMagikeCost(HolyLight, 10, 180), resultItemStack: 35)
                .RegisterNew(ItemID.Picksaw, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.Stynger, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.PossessedHatchet, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.SunStone, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.HeatRay, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.EyeoftheGolem, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.StaffofEarth, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.GolemFist, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.GolemMask, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.GolemTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.GolemPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 猪鲨宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.FishronBossBag, ItemID.BubbleGun, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.Flairon, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.RazorbladeTyphoon, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.TempestStaff, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.Tsunami, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.FishronWings, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.DukeFishronMask, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.DukeFishronTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.DukeFishronPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 光女宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.FairyQueenBossBag, ItemID.FairyQueenMagicItem, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.PiercingStarlight, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.RainbowWhip, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.FairyQueenRangedItem, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.RainbowWings, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.SparkleGuitar, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.RainbowCursor, CalculateMagikeCost(HolyLight, 10, 180))
                .RegisterNew(ItemID.HallowBossDye, CalculateMagikeCost(HolyLight, 10, 180), 3)
                .RegisterNew(ItemID.FairyQueenMask, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.FairyQueenTrophy, CalculateMagikeCost(HolyLight))
                .RegisterNew(ItemID.FairyQueenPetItem, CalculateMagikeCost(HolyLight))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(HolyLight), 3)
                .Register();
            #endregion

            #region 月总宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.MoonLordBossBag, ItemID.Meowmere, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.Terrarian, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.StarWrath, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.SDMG, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.LastPrism, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.LunarFlareBook, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.RainbowCrystalStaff, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.MoonlordTurretStaff, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew<ConquerorOfTheSeas>(CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.Celeb2, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.LunarOre, CalculateMagikeCost(SplendorMagicore, 10, 180), 170)
                .RegisterNew(ItemID.MeowmereMinecart, CalculateMagikeCost(SplendorMagicore, 10, 180))
                .RegisterNew(ItemID.BossMaskMoonlord, CalculateMagikeCost(SplendorMagicore))
                .RegisterNew(ItemID.MoonLordTrophy, CalculateMagikeCost(SplendorMagicore))
                .RegisterNew(ItemID.MoonLordPetItem, CalculateMagikeCost(SplendorMagicore))
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(CalculateMagikeCost(SplendorMagicore), 3)
                .Register();
            #endregion
        }
    }
}
