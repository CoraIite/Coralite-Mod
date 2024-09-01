using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Thunder;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossBag : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            #region 赤玉灵
            MagikeCraftRecipe.CreateRecipe<RediancieBossBag, RedJade>(150, resultItemStack: 34)
                .RegisterNew<RediancieMask>(50)
                .RegisterNew<RediancieTrophy>(250)
                .RegisterNew<RedianciePet>(50)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆王
            MagikeCraftRecipe.CreateRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, 150)
                .RegisterNew(ItemID.NinjaHood, 100)
                .RegisterNew(ItemID.NinjaShirt, 100)
                .RegisterNew(ItemID.NinjaPants, 100)
                .RegisterNew(ItemID.SlimeHook, 150)
                .RegisterNew(ItemID.SlimeGun, 50)
                .RegisterNew(ItemID.KingSlimeMask, 100)
                .RegisterNew(ItemID.KingSlimeTrophy, 150)
                .RegisterNew(ItemID.KingSlimePetItem, 50)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之眼
            MagikeCraftRecipe.CreateRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.CrimtaneOre, 150, resultItemStack: 85)
                .RegisterNew(ItemID.DemoniteOre, 150, 85)
                .RegisterNew(ItemID.Binoculars, 150)
                .RegisterNew(ItemID.EyeMask, 100)
                .RegisterNew(ItemID.EyeofCthulhuTrophy, 150)
                .RegisterNew(ItemID.EyeOfCthulhuPetItem, 50)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 世界吞噬怪
            MagikeCraftRecipe.CreateRecipe(ItemID.EaterOfWorldsBossBag, ItemID.DemoniteOre, 225, resultItemStack: 70)
                .RegisterNew(ItemID.ShadowScale, 250, 25)
                .RegisterNew(ItemID.EatersBone, 450)
                .RegisterNew(ItemID.EaterMask, 200)
                .RegisterNew(ItemID.EaterofWorldsTrophy, 300)
                .RegisterNew(ItemID.EaterOfWorldsPetItem, 100)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之脑
            MagikeCraftRecipe.CreateRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.CrimtaneOre, 225, resultItemStack: 70)
                .RegisterNew(ItemID.TissueSample, 250, 25)
                .RegisterNew(ItemID.BoneRattle, 450)
                .RegisterNew(ItemID.BrainMask, 200)
                .RegisterNew(ItemID.BrainofCthulhuTrophy, 300)
                .RegisterNew(ItemID.BrainOfCthulhuPetItem, 100)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 冰龙宝宝
            MagikeCraftRecipe.CreateRecipe<BabyIceDragonBossBag, IcicleCrystal>(225, resultItemStack: 8)
                .RegisterNew<IcicleScale>(225, 7)
                .RegisterNew<IcicleBreath>(225, 10)
                .RegisterNew<BabyIceDragonMask>(150)
                .RegisterNew<BabyIceDragonTrophy>(150)
                .RegisterNew<IcicleSoulStone>(150)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 蜂后
            MagikeCraftRecipe.CreateRecipe(ItemID.QueenBeeBossBag, ItemID.BeeGun, 200)
                .RegisterNew(ItemID.BeeKeeper, 200)
                .RegisterNew(ItemID.BeesKnees, 200)
                .RegisterNew(ItemID.BeeHat, 100)
                .RegisterNew(ItemID.BeeShirt, 100)
                .RegisterNew(ItemID.BeePants, 100)
                .RegisterNew(ItemID.HoneyComb, 200)
                .RegisterNew(ItemID.Nectar, 450)
                .RegisterNew(ItemID.HoneyedGoggles, 450)
                .RegisterNew(ItemID.Beenade, 300, 80)
                .RegisterNew(ItemID.BeeMask, 200)
                .RegisterNew(ItemID.QueenBeeTrophy, 300)
                .RegisterNew(ItemID.QueenBeePetItem, 100)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 巨鹿
            MagikeCraftRecipe.CreateRecipe(ItemID.DeerclopsBossBag, ItemID.ChesterPetItem, 450)
                .RegisterNew(ItemID.Eyebrella, 200)
                .RegisterNew(ItemID.DontStarveShaderItem, 200)
                .RegisterNew(ItemID.DizzyHat, 450)
                .RegisterNew(ItemID.PewMaticHorn, 250)
                .RegisterNew(ItemID.WeatherPain, 250)
                .RegisterNew(ItemID.HoundiusShootius, 250)
                .RegisterNew(ItemID.LucyTheAxe, 250)
                .RegisterNew(ItemID.DeerclopsMask, 200)
                .RegisterNew(ItemID.DeerclopsTrophy, 300)
                .RegisterNew(ItemID.DeerclopsPetItem, 100)
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 骷髅王
            MagikeCraftRecipe.CreateRecipe(ItemID.SkeletronBossBag, ItemID.SkeletronHand, 200)
                .RegisterNew(ItemID.BookofSkulls, 200)
                .RegisterNew(ItemID.ChippysCouch, 450)
                .RegisterNew(ItemID.SkeletronMask, 200)
                .RegisterNew(ItemID.SkeletronTrophy, 300)
                .RegisterNew(ItemID.SkeletronPetItem, 100)
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
