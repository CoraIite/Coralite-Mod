using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Thunder;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;
using static Coralite.Core.Systems.MagikeSystem.MagikeApparatusLevel;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossBag : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            #region 赤玉灵
            MagikeCraftRecipe.CreateRecipe<RediancieBossBag, RedJade>(CalculateMagikeCost(MagikeApparatusLevel.RedJade,5,120)
                , resultItemStack: 34)
                .RegisterNew<RediancieMask>(CalculateMagikeCost(MagikeApparatusLevel.RedJade))
                .RegisterNew<RediancieTrophy>(CalculateMagikeCost(MagikeApparatusLevel.RedJade,5,120))
                .RegisterNew<RedianciePet>(CalculateMagikeCost(MagikeApparatusLevel.RedJade))
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆王
            MagikeCraftRecipe.CreateRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, CalculateMagikeCost(MagikeApparatusLevel.RedJade,5,120))
                .RegisterNew(ItemID.NinjaHood, CalculateMagikeCost(MagikeApparatusLevel.RedJade,8,180))
                .RegisterNew(ItemID.NinjaShirt, CalculateMagikeCost(MagikeApparatusLevel.RedJade, 8, 180))
                .RegisterNew(ItemID.NinjaPants, CalculateMagikeCost(MagikeApparatusLevel.RedJade, 8, 180))
                .RegisterNew(ItemID.SlimeHook, CalculateMagikeCost(MagikeApparatusLevel.RedJade, 10, 180))
                .RegisterNew(ItemID.SlimeGun, CalculateMagikeCost(MagikeApparatusLevel.RedJade))
                .RegisterNew(ItemID.KingSlimeMask, CalculateMagikeCost(MagikeApparatusLevel.RedJade))
                .RegisterNew(ItemID.KingSlimeTrophy, CalculateMagikeCost(MagikeApparatusLevel.RedJade))
                .RegisterNew(ItemID.KingSlimePetItem, CalculateMagikeCost(MagikeApparatusLevel.RedJade))
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
            MagikeCraftRecipe.CreateRecipe(ItemID.WallOfFleshBossBag, ItemID.WarriorEmblem, 350)
                .RegisterNew(ItemID.RangerEmblem, 350)
                .RegisterNew(ItemID.SorcererEmblem, 350)
                .RegisterNew(ItemID.SummonerEmblem, 350)
                .RegisterNew(ItemID.BreakerBlade, 300)
                .RegisterNew(ItemID.ClockworkAssaultRifle, 300)
                .RegisterNew(ItemID.LaserRifle, 300)
                .RegisterNew(ItemID.FireWhip, 300)
                .RegisterNew(ItemID.FleshMask, 300)
                .RegisterNew(ItemID.WallofFleshTrophy, 400)
                .RegisterNew(ItemID.WallOfFleshGoatMountItem, 200)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆皇后
            MagikeCraftRecipe.CreateRecipe(ItemID.QueenSlimeBossBag, ItemID.GelBalloon, 1500, resultItemStack: 100)
                .RegisterNew(ItemID.Smolstar, 3000)
                .RegisterNew(ItemID.QueenSlimeHook, 3000)
                .RegisterNew(ItemID.QueenSlimeMountSaddle, 3500)
                .RegisterNew(ItemID.CrystalNinjaHelmet, 1500)
                .RegisterNew(ItemID.CrystalNinjaChestplate, 1500)
                .RegisterNew(ItemID.CrystalNinjaLeggings, 1500)
                .RegisterNew(ItemID.QueenSlimeMask, 1500)
                .RegisterNew(ItemID.QueenSlimeTrophy, 2500)
                .RegisterNew(ItemID.QueenSlimePetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 双子魔眼
            MagikeCraftRecipe.CreateRecipe(ItemID.TwinsBossBag, ItemID.HallowedBar, 500, resultItemStack: 35)
                .RegisterNew(ItemID.SoulofSight, 500, 40)
                .RegisterNew(ItemID.TwinMask, 1500)
                .RegisterNew(ItemID.RetinazerTrophy, 2500)
                .RegisterNew(ItemID.SpazmatismTrophy, 2500)
                .RegisterNew(ItemID.TwinsPetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 铁长直
            MagikeCraftRecipe.CreateRecipe(ItemID.DestroyerBossBag, ItemID.HallowedBar, 500, resultItemStack: 35)
                .RegisterNew(ItemID.SoulofMight, 500, 40)
                .RegisterNew(ItemID.DestroyerMask, 1500)
                .RegisterNew(ItemID.DestroyerTrophy, 2500)
                .RegisterNew(ItemID.DestroyerPetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 铁骷髅王
            MagikeCraftRecipe.CreateRecipe(ItemID.SkeletronPrimeBossBag, ItemID.HallowedBar, 500, resultItemStack: 35)
                .RegisterNew(ItemID.SoulofFright, 500, 40)
                .RegisterNew(ItemID.SkeletronPrimeMask, 1500)
                .RegisterNew(ItemID.SkeletronPrimeTrophy, 2500)
                .RegisterNew(ItemID.SkeletronPrimePetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 荒雷龙
            MagikeCraftRecipe.CreateRecipe<ThunderveinDragonBossBag, ZapCrystal>(3000, resultItemStack: 16)
                .RegisterNew<ElectrificationWing>(3000, 6)
                .RegisterNew<InsulationCortex>(3000, 20)
                .RegisterNew<ThunderveinDragonMask>(1000)
                .RegisterNew<ThunderveinDragonTrophy>(1000)
                .RegisterNew<ThunderveinSoulStone>(1000)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 世纪之花宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.PlanteraBossBag, ItemID.GrenadeLauncher, 3000)
                .RegisterNew(ItemID.VenusMagnum, 3000)
                .RegisterNew(ItemID.NettleBurst, 2500)
                .RegisterNew(ItemID.FlowerPow, 3000)
                .RegisterNew(ItemID.WaspGun, 3000)
                .RegisterNew(ItemID.Seedler, 3500)
                .RegisterNew(ItemID.Seedling, 6000)
                .RegisterNew(ItemID.TheAxe, 7000)
                .RegisterNew(ItemID.PygmyStaff, 3000)
                .RegisterNew(ItemID.ThornHook, 2000)
                .RegisterNew(ItemID.PlanteraMask, 2000)
                .RegisterNew(ItemID.PlanteraTrophy, 2000)
                .RegisterNew(ItemID.PlanteraPetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 石巨人宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.GolemBossBag, ItemID.BeetleHusk, 3000, resultItemStack: 35)
                .RegisterNew(ItemID.Picksaw, 5000)
                .RegisterNew(ItemID.Stynger, 4500)
                .RegisterNew(ItemID.PossessedHatchet, 4500)
                .RegisterNew(ItemID.SunStone, 4500)
                .RegisterNew(ItemID.HeatRay, 4500)
                .RegisterNew(ItemID.EyeoftheGolem, 4500)
                .RegisterNew(ItemID.StaffofEarth, 4500)
                .RegisterNew(ItemID.GolemFist, 4500)
                .RegisterNew(ItemID.GolemMask, 3000)
                .RegisterNew(ItemID.GolemTrophy, 4000)
                .RegisterNew(ItemID.GolemPetItem, 500)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 猪鲨宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.FishronBossBag, ItemID.BubbleGun, 5000)
                .RegisterNew(ItemID.Flairon, 5000)
                .RegisterNew(ItemID.RazorbladeTyphoon, 5000)
                .RegisterNew(ItemID.TempestStaff, 5000)
                .RegisterNew(ItemID.Tsunami, 5000)
                .RegisterNew(ItemID.FishronWings, 5000)
                .RegisterNew(ItemID.DukeFishronMask, 5000)
                .RegisterNew(ItemID.DukeFishronTrophy, 5000)
                .RegisterNew(ItemID.DukeFishronPetItem, 1000)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 光女宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.FairyQueenBossBag, ItemID.FairyQueenMagicItem, 5000)
                .RegisterNew(ItemID.PiercingStarlight, 5000)
                .RegisterNew(ItemID.RainbowWhip, 5000)
                .RegisterNew(ItemID.FairyQueenRangedItem, 5000)
                .RegisterNew(ItemID.RainbowWings, 6000)
                .RegisterNew(ItemID.SparkleGuitar, 7000)
                .RegisterNew(ItemID.RainbowCursor, 7000)
                .RegisterNew(ItemID.HallowBossDye, 6000, 3)
                .RegisterNew(ItemID.FairyQueenMask, 3000)
                .RegisterNew(ItemID.FairyQueenTrophy, 4000)
                .RegisterNew(ItemID.FairyQueenPetItem, 1000)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion

            #region 月总宝藏袋
            MagikeCraftRecipe.CreateRecipe(ItemID.MoonLordBossBag, ItemID.Meowmere, 7000)
                .RegisterNew(ItemID.Terrarian, 7000)
                .RegisterNew(ItemID.StarWrath, 7000)
                .RegisterNew(ItemID.SDMG, 7000)
                .RegisterNew(ItemID.LastPrism, 7000)
                .RegisterNew(ItemID.LunarFlareBook, 7000)
                .RegisterNew(ItemID.RainbowCrystalStaff, 7000)
                .RegisterNew(ItemID.MoonlordTurretStaff, 7000)
                .RegisterNew<ConquerorOfTheSeas>(7000)
                .RegisterNew(ItemID.Celeb2, 7000)
                .RegisterNew(ItemID.LunarOre, 7000, 170)
                .RegisterNew(ItemID.MeowmereMinecart, 7000)
                .RegisterNew(ItemID.BossMaskMoonlord, 4000)
                .RegisterNew(ItemID.MoonLordTrophy, 6000)
                .RegisterNew(ItemID.MoonLordPetItem, 2000)
                .AddCondition(Condition.InMasterMode)
                .RegisterNew<SoulOfDeveloper>(1500, 3)
                .Register();
            #endregion
        }
    }
}
