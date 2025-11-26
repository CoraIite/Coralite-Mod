using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Icicle;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.RedJades;
using Coralite.Content.Items.Thunder;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Terraria;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.MagikeCraft
{
    public class BossBag : IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            #region 赤玉灵
            MagikeRecipe.CreateCraftRecipe<RediancieBossBag, RedJade>(CalculateMagikeCost<RedJadeLevel>( 2, 120)
                , resultItemStack: 34)
                .RegisterNewCraft<RediancieMask>(CalculateMagikeCost<RedJadeLevel>())
                .RegisterNewCraft<RediancieTrophy>(CalculateMagikeCost<RedJadeLevel>( 2, 120))
                .RegisterNewCraft<RedianciePet>(CalculateMagikeCost<RedJadeLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆王
            MagikeRecipe.CreateCraftRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, CalculateMagikeCost<RedJadeLevel>( 5, 120))
                .RegisterNewCraft(ItemID.NinjaHood, CalculateMagikeCost<RedJadeLevel>( 4, 120))
                .RegisterNewCraft(ItemID.NinjaShirt, CalculateMagikeCost<RedJadeLevel>( 4, 120))
                .RegisterNewCraft(ItemID.NinjaPants, CalculateMagikeCost<RedJadeLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SlimeHook, CalculateMagikeCost<RedJadeLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SlimeGun, CalculateMagikeCost<RedJadeLevel>())
                .RegisterNewCraft(ItemID.KingSlimeMask, CalculateMagikeCost<RedJadeLevel>())
                .RegisterNewCraft(ItemID.KingSlimeTrophy, CalculateMagikeCost<RedJadeLevel>())
                .RegisterNewCraft(ItemID.KingSlimePetItem, CalculateMagikeCost<RedJadeLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之眼
            MagikeRecipe.CreateCraftRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost<GlistentLevel>( 4, 120)
                , resultItemStack: 85)
                .RegisterNewCraft(ItemID.DemoniteOre, CalculateMagikeCost<GlistentLevel>( 4, 120), 85)
                .RegisterNewCraft(ItemID.Binoculars, CalculateMagikeCost<GlistentLevel>( 5, 180))
                .RegisterNewCraft(ItemID.EyeMask, CalculateMagikeCost<GlistentLevel>())
                .RegisterNewCraft(ItemID.EyeofCthulhuTrophy, CalculateMagikeCost<GlistentLevel>())
                .RegisterNewCraft(ItemID.EyeOfCthulhuPetItem, CalculateMagikeCost<GlistentLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 世界吞噬怪
            MagikeRecipe.CreateCraftRecipe(ItemID.EaterOfWorldsBossBag, ItemID.DemoniteOre, CalculateMagikeCost<CorruptionLevel>( 4, 120), resultItemStack: 70)
                .RegisterNewCraft(ItemID.ShadowScale, CalculateMagikeCost<CorruptionLevel>( 4, 120), 25)
                .RegisterNewCraft(ItemID.EatersBone, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.EaterMask, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.EaterofWorldsTrophy, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.EaterOfWorldsPetItem, CalculateMagikeCost<CorruptionLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 克苏鲁之脑
            MagikeRecipe.CreateCraftRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.CrimtaneOre, CalculateMagikeCost<CorruptionLevel>( 4, 120), resultItemStack: 70)
                .RegisterNewCraft(ItemID.TissueSample, CalculateMagikeCost<CorruptionLevel>( 4, 120), 25)
                .RegisterNewCraft(ItemID.BoneRattle, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.BrainMask, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.BrainofCthulhuTrophy, CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft(ItemID.BrainOfCthulhuPetItem, CalculateMagikeCost<CorruptionLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 冰龙宝宝
            MagikeRecipe.CreateCraftRecipe<BabyIceDragonBossBag, IcicleCrystal>(CalculateMagikeCost<CorruptionLevel>( 4, 120), resultItemStack: 8)
                .RegisterNewCraft<IcicleScale>(CalculateMagikeCost<CorruptionLevel>( 4, 120), 7)
                .RegisterNewCraft<IcicleBreath>(CalculateMagikeCost<CorruptionLevel>( 4, 120), 10)
                .RegisterNewCraft<BabyIceDragonMask>(CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft<BabyIceDragonTrophy>(CalculateMagikeCost<CorruptionLevel>())
                .RegisterNewCraft<IcicleSoulStone>(CalculateMagikeCost<CorruptionLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 蜂后
            MagikeRecipe.CreateCraftRecipe(ItemID.QueenBeeBossBag, ItemID.BeeGun, CalculateMagikeCost<BeeswaxLevel>( 4, 120))
                .RegisterNewCraft(ItemID.BeeKeeper, CalculateMagikeCost<BeeswaxLevel>( 4, 120))//养蜂人
                .RegisterNewCraft(ItemID.BeesKnees, CalculateMagikeCost<BeeswaxLevel>( 4, 120))
                .RegisterNewCraft(ItemID.BeeHat, CalculateMagikeCost<BeeswaxLevel>( 3, 180))
                .RegisterNewCraft(ItemID.BeeShirt, CalculateMagikeCost<BeeswaxLevel>( 3, 180))
                .RegisterNewCraft(ItemID.BeePants, CalculateMagikeCost<BeeswaxLevel>( 3, 180))
                .RegisterNewCraft(ItemID.HoneyComb, CalculateMagikeCost<BeeswaxLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Nectar, CalculateMagikeCost<BeeswaxLevel>())//蜜蜂宠物
                .RegisterNewCraft(ItemID.HoneyedGoggles, CalculateMagikeCost<BeeswaxLevel>( 4, 120))//蜜蜂坐骑
                .RegisterNewCraft(ItemID.Beenade, CalculateMagikeCost<BeeswaxLevel>( 4, 120), 80)//蜜蜂手雷
                .RegisterNewCraft(ItemID.BeeMask, CalculateMagikeCost<BeeswaxLevel>())
                .RegisterNewCraft(ItemID.QueenBeeTrophy, CalculateMagikeCost<BeeswaxLevel>())
                .RegisterNewCraft(ItemID.QueenBeePetItem, CalculateMagikeCost<BeeswaxLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 巨鹿
            MagikeRecipe.CreateCraftRecipe(ItemID.DeerclopsBossBag, ItemID.ChesterPetItem, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.Eyebrella, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.DontStarveShaderItem, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.DizzyHat, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.PewMaticHorn, CalculateMagikeCost<HellstoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.WeatherPain, CalculateMagikeCost<HellstoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.HoundiusShootius, CalculateMagikeCost<HellstoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LucyTheAxe, CalculateMagikeCost<HellstoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.DeerclopsMask, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.DeerclopsTrophy, CalculateMagikeCost<HellstoneLevel>())
                .RegisterNewCraft(ItemID.DeerclopsPetItem, CalculateMagikeCost<HellstoneLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 骷髅王
            MagikeRecipe.CreateCraftRecipe(ItemID.SkeletronBossBag, ItemID.SkeletronHand, CalculateMagikeCost<BoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.BookofSkulls, CalculateMagikeCost<BoneLevel>( 4, 120))
                .RegisterNewCraft(ItemID.ChippysCouch, CalculateMagikeCost<BoneLevel>())
                .RegisterNewCraft(ItemID.SkeletronMask, CalculateMagikeCost<BoneLevel>())
                .RegisterNewCraft(ItemID.SkeletronTrophy, CalculateMagikeCost<BoneLevel>())
                .RegisterNewCraft(ItemID.SkeletronPetItem, CalculateMagikeCost<BoneLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 肉山
            MagikeRecipe.CreateCraftRecipe(ItemID.WallOfFleshBossBag, ItemID.WarriorEmblem, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RangerEmblem, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SorcererEmblem, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SummonerEmblem, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.BreakerBlade, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.ClockworkAssaultRifle, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LaserRifle, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.FireWhip, CalculateMagikeCost<QuicksandLevel>( 4, 120))
                .RegisterNewCraft(ItemID.FleshMask, CalculateMagikeCost<QuicksandLevel>())
                .RegisterNewCraft(ItemID.WallofFleshTrophy, CalculateMagikeCost<QuicksandLevel>())
                .RegisterNewCraft(ItemID.WallOfFleshGoatMountItem, CalculateMagikeCost<QuicksandLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 史莱姆皇后
            MagikeRecipe.CreateCraftRecipe(ItemID.QueenSlimeBossBag, ItemID.GelBalloon, CalculateMagikeCost<FlightLevel>( 4, 120), resultItemStack: 100)
                .RegisterNewCraft(ItemID.Smolstar, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeHook, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeMountSaddle, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaHelmet, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaChestplate, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.CrystalNinjaLeggings, CalculateMagikeCost<FlightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.QueenSlimeMask, CalculateMagikeCost<FlightLevel>())
                .RegisterNewCraft(ItemID.QueenSlimeTrophy, CalculateMagikeCost<FlightLevel>())
                .RegisterNewCraft(ItemID.QueenSlimePetItem, CalculateMagikeCost<FlightLevel>())
                .AddCondition(Condition.InMasterMode)
                .Register();
            #endregion

            #region 双子魔眼
            MagikeRecipe.CreateCraftRecipe(ItemID.TwinsBossBag, ItemID.HallowedBar, CalculateMagikeCost<HallowLevel>( 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofSight, CalculateMagikeCost<HallowLevel>( 4, 120), 40)
                .RegisterNewCraft(ItemID.TwinMask, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.RetinazerTrophy, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.SpazmatismTrophy, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.TwinsPetItem, CalculateMagikeCost<HallowLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HallowLevel>(), 3)
                .Register();
            #endregion

            #region 铁长直
            MagikeRecipe.CreateCraftRecipe(ItemID.DestroyerBossBag, ItemID.HallowedBar, CalculateMagikeCost<HallowLevel>( 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofMight, CalculateMagikeCost<HallowLevel>( 4, 120), 40)
                .RegisterNewCraft(ItemID.DestroyerMask, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.DestroyerTrophy, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.DestroyerPetItem, CalculateMagikeCost<HallowLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HallowLevel>(), 3)
                .Register();
            #endregion

            #region 铁骷髅王
            MagikeRecipe.CreateCraftRecipe(ItemID.SkeletronPrimeBossBag, ItemID.HallowedBar, CalculateMagikeCost<HallowLevel>( 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.SoulofFright, CalculateMagikeCost<HallowLevel>( 4, 120), 40)
                .RegisterNewCraft(ItemID.SkeletronPrimeMask, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.SkeletronPrimeTrophy, CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft(ItemID.SkeletronPrimePetItem, CalculateMagikeCost<HallowLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HallowLevel>(), 3)
                .Register();
            #endregion

            #region 荒雷龙
            MagikeRecipe.CreateCraftRecipe<ThunderveinDragonBossBag, ZapCrystal>(CalculateMagikeCost<HallowLevel>( 4, 120), resultItemStack: 16)
                .RegisterNewCraft<ElectrificationWing>(CalculateMagikeCost<HallowLevel>( 4, 120), 6)
                .RegisterNewCraft<InsulationCortex>(CalculateMagikeCost<HallowLevel>( 4, 120), 20)
                .RegisterNewCraft<ThunderveinDragonMask>(CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft<ThunderveinDragonTrophy>(CalculateMagikeCost<HallowLevel>())
                .RegisterNewCraft<ThunderveinSoulStone>(CalculateMagikeCost<HallowLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HallowLevel>(), 3)
                .Register();
            #endregion

            #region 世纪之花宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.PlanteraBossBag, ItemID.GrenadeLauncher, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.VenusMagnum, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.NettleBurst, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.FlowerPow, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.WaspGun, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Seedler, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Seedling, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.TheAxe, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.PygmyStaff, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.ThornHook, CalculateMagikeCost<SoulLevel>( 4, 120))
                .RegisterNewCraft(ItemID.PlanteraMask, CalculateMagikeCost<SoulLevel>())
                .RegisterNewCraft(ItemID.PlanteraTrophy, CalculateMagikeCost<SoulLevel>())
                .RegisterNewCraft(ItemID.PlanteraPetItem, CalculateMagikeCost<SoulLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<SoulLevel>(), 3)
                .Register();
            #endregion

            #region 石巨人宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.GolemBossBag, ItemID.BeetleHusk, CalculateMagikeCost<HolyLightLevel>( 4, 120), resultItemStack: 35)
                .RegisterNewCraft(ItemID.Picksaw, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Stynger, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.PossessedHatchet, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SunStone, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.HeatRay, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.EyeoftheGolem, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.StaffofEarth, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.GolemFist, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.GolemMask, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.GolemTrophy, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.GolemPetItem, CalculateMagikeCost<HolyLightLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HolyLightLevel>(), 3)
                .Register();
            #endregion

            #region 猪鲨宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.FishronBossBag, ItemID.BubbleGun, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Flairon, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RazorbladeTyphoon, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.TempestStaff, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Tsunami, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.FishronWings, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.DukeFishronMask, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.DukeFishronTrophy, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.DukeFishronPetItem, CalculateMagikeCost<HolyLightLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HolyLightLevel>(), 3)
                .Register();
            #endregion

            #region 光女宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.FairyQueenBossBag, ItemID.FairyQueenMagicItem, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.PiercingStarlight, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RainbowWhip, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.FairyQueenRangedItem, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RainbowWings, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SparkleGuitar, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RainbowCursor, CalculateMagikeCost<HolyLightLevel>( 4, 120))
                .RegisterNewCraft(ItemID.HallowBossDye, CalculateMagikeCost<HolyLightLevel>( 4, 120), 3)
                .RegisterNewCraft(ItemID.FairyQueenMask, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.FairyQueenTrophy, CalculateMagikeCost<HolyLightLevel>())
                .RegisterNewCraft(ItemID.FairyQueenPetItem, CalculateMagikeCost<HolyLightLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<HolyLightLevel>(), 3)
                .Register();
            #endregion

            #region 月总宝藏袋
            MagikeRecipe.CreateCraftRecipe(ItemID.MoonLordBossBag, ItemID.Meowmere, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Terrarian, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.StarWrath, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.SDMG, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LastPrism, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LunarFlareBook, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.RainbowCrystalStaff, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.MoonlordTurretStaff, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft<ConquerorOfTheSeas>(CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft<Aurora>(CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.Celeb2, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.LunarOre, CalculateMagikeCost<SplendorLevel>( 4, 120), 170)
                .RegisterNewCraft(ItemID.MeowmereMinecart, CalculateMagikeCost<SplendorLevel>( 4, 120))
                .RegisterNewCraft(ItemID.BossMaskMoonlord, CalculateMagikeCost<SplendorLevel>())
                .RegisterNewCraft(ItemID.MoonLordTrophy, CalculateMagikeCost<SplendorLevel>())
                .RegisterNewCraft(ItemID.MoonLordPetItem, CalculateMagikeCost<SplendorLevel>())
                .AddCondition(Condition.InMasterMode)
                .RegisterNewCraft<SoulOfDeveloper>(CalculateMagikeCost<SplendorLevel>(), 3)
                .Register();
            #endregion
        }
    }
}
