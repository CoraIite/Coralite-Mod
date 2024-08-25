using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Core.Systems.MagikeSystem.Remodels
{
    public class BossBag : IMagikeRemodelable
    {
        public void AddMagikeRemodelRecipe()
        {
            #region 史莱姆王
            MagikeCraftRecipe.CreateRecipe(ItemID.KingSlimeBossBag, ItemID.SlimySaddle, 150)
                .RegisterNew(ItemID.NinjaHood, 100)
                .RegisterNew(ItemID.NinjaShirt, 100)
                .RegisterNew(ItemID.NinjaPants, 100)
                .RegisterNew(ItemID.SlimeHook, 150)
                .RegisterNew(ItemID.SlimeGun, 50)
                .RegisterNew(ItemID.KingSlimeMask, 100)
                .RegisterNew(ItemID.KingSlimeTrophy, 150)
                .Register();
            AddRemodelRecipe(ItemID.KingSlimeBossBag, ItemID.KingSlimePetItem, 50, conditions: Condition.InMasterMode);
            #endregion

            //克苏鲁之眼宝藏袋
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.CrimtaneOre, 150, 85);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.DemoniteOre, 150, 85);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.Binoculars, 150);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.EyeMask, 100);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.EyeofCthulhuTrophy, 150);
            AddRemodelRecipe(ItemID.EyeOfCthulhuBossBag, ItemID.EyeOfCthulhuPetItem, 50, conditions: Condition.InMasterMode);

            //世界吞噬怪宝藏袋
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.DemoniteOre, 225, 70);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.ShadowScale, 250, 25);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.EatersBone, 450);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.EaterMask, 200);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.EaterofWorldsTrophy, 300);
            AddRemodelRecipe(ItemID.EaterOfWorldsBossBag, ItemID.EaterOfWorldsPetItem, 100, conditions: Condition.InMasterMode);

            //克苏鲁之脑宝藏袋
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.CrimtaneOre, 225, 70);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.TissueSample, 250, 25);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.BoneRattle, 450);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.BrainMask, 200);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.BrainofCthulhuTrophy, 300);
            AddRemodelRecipe(ItemID.BrainOfCthulhuBossBag, ItemID.BrainOfCthulhuPetItem, 100, conditions: Condition.InMasterMode);

            //蜂后宝藏袋
            AddRemodelRecipe(ItemID.QueenBeeBossBag, ItemID.BeeGun, 200);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, ItemID.BeeKeeper, 200);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 200, ItemID.BeesKnees);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeeHat);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeeShirt);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.BeePants);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 200, ItemID.HoneyComb);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 450, ItemID.Nectar);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 450, ItemID.HoneyedGoggles);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 300, ItemID.Beenade, 80);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 200, ItemID.BeeMask);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 300, ItemID.QueenBeeTrophy);
            AddRemodelRecipe(ItemID.QueenBeeBossBag, 100, ItemID.QueenBeePetItem, conditions: Condition.InMasterMode);

            //巨鹿宝藏袋
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 450, ItemID.ChesterPetItem);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 200, ItemID.Eyebrella);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 200, ItemID.DontStarveShaderItem);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 450, ItemID.DizzyHat);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 250, ItemID.PewMaticHorn);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 250, ItemID.WeatherPain);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 250, ItemID.HoundiusShootius);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 250, ItemID.LucyTheAxe);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 200, ItemID.DeerclopsMask);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 300, ItemID.DeerclopsTrophy);
            AddRemodelRecipe(ItemID.DeerclopsBossBag, 100, ItemID.DeerclopsPetItem, conditions: Condition.InMasterMode);

            //骷髅王宝藏袋
            AddRemodelRecipe(ItemID.SkeletronBossBag, 200, ItemID.SkeletronHand);
            AddRemodelRecipe(ItemID.SkeletronBossBag, 200, ItemID.BookofSkulls);
            AddRemodelRecipe(ItemID.SkeletronBossBag, 450, ItemID.ChippysCouch);
            AddRemodelRecipe(ItemID.SkeletronBossBag, 200, ItemID.SkeletronMask);
            AddRemodelRecipe(ItemID.SkeletronBossBag, 300, ItemID.SkeletronTrophy);
            AddRemodelRecipe(ItemID.SkeletronBossBag, 100, ItemID.SkeletronPetItem, conditions: Condition.InMasterMode);

            //肉山宝藏袋
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 350, ItemID.WarriorEmblem);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 350, ItemID.RangerEmblem);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 350, ItemID.SorcererEmblem);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 350, ItemID.SummonerEmblem);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 300, ItemID.BreakerBlade);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 300, ItemID.ClockworkAssaultRifle);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 300, ItemID.LaserRifle);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 300, ItemID.FireWhip);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 300, ItemID.FleshMask);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 400, ItemID.WallofFleshTrophy);
            AddRemodelRecipe(ItemID.WallOfFleshBossBag, 200, ItemID.WallOfFleshGoatMountItem, conditions: Condition.InMasterMode);

            //史莱姆皇后宝藏袋
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 1500, ItemID.GelBalloon, 100);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 3000, ItemID.Smolstar);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 3000, ItemID.QueenSlimeHook);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 3500, ItemID.QueenSlimeMountSaddle);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 1500, ItemID.CrystalNinjaHelmet);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 1500, ItemID.CrystalNinjaChestplate);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 1500, ItemID.CrystalNinjaLeggings);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 1500, ItemID.QueenSlimeMask);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 2500, ItemID.QueenSlimeTrophy);
            AddRemodelRecipe(ItemID.QueenSlimeBossBag, 500, ItemID.QueenSlimePetItem, conditions: Condition.InMasterMode);
            //AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.QueenSlimeBossBag, 1500, 3);

            //双子魔眼宝藏袋
            AddRemodelRecipe(ItemID.TwinsBossBag, 500, ItemID.HallowedBar, 35);
            AddRemodelRecipe(ItemID.TwinsBossBag, 500, ItemID.SoulofSight, 40);
            AddRemodelRecipe(ItemID.TwinsBossBag, 1500, ItemID.TwinMask);
            AddRemodelRecipe(ItemID.TwinsBossBag, 2500, ItemID.RetinazerTrophy);
            AddRemodelRecipe(ItemID.TwinsBossBag, 2500, ItemID.SpazmatismTrophy);
            AddRemodelRecipe(ItemID.TwinsBossBag, 500, ItemID.TwinsPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.TwinsBossBag, 1500, 3);

            //铁长直宝藏袋
            AddRemodelRecipe(ItemID.DestroyerBossBag, 500, ItemID.HallowedBar, 35);
            AddRemodelRecipe(ItemID.DestroyerBossBag, 500, ItemID.SoulofMight, 40);
            AddRemodelRecipe(ItemID.DestroyerBossBag, 1500, ItemID.DestroyerMask);
            AddRemodelRecipe(ItemID.DestroyerBossBag, 2500, ItemID.DestroyerTrophy);
            AddRemodelRecipe(ItemID.DestroyerBossBag, 500, ItemID.DestroyerPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.DestroyerBossBag, 1500, 3);

            //铁骷髅王宝藏袋
            AddRemodelRecipe(ItemID.SkeletronPrimeBossBag, 500, ItemID.HallowedBar, 35);
            AddRemodelRecipe(ItemID.SkeletronPrimeBossBag, 500, ItemID.SoulofFright, 40);
            AddRemodelRecipe(ItemID.SkeletronPrimeBossBag, 1500, ItemID.SkeletronPrimeMask);
            AddRemodelRecipe(ItemID.SkeletronPrimeBossBag, 2500, ItemID.SkeletronPrimeTrophy);
            AddRemodelRecipe(ItemID.SkeletronPrimeBossBag, 500, ItemID.SkeletronPrimePetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.SkeletronPrimeBossBag, 1500, 3);

            //世纪之花宝藏袋
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3000, ItemID.GrenadeLauncher);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3000, ItemID.VenusMagnum);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 2500, ItemID.NettleBurst);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3000, ItemID.FlowerPow);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3000, ItemID.WaspGun);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3500, ItemID.Seedler);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 6000, ItemID.Seedling);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 7000, ItemID.TheAxe);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 3000, ItemID.PygmyStaff);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 2000, ItemID.ThornHook);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 1500, ItemID.PlanteraMask);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 2500, ItemID.PlanteraTrophy);
            AddRemodelRecipe(ItemID.PlanteraBossBag, 500, ItemID.PlanteraPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.PlanteraBossBag, 1500, 3);

            //石巨人宝藏袋
            AddRemodelRecipe(ItemID.GolemBossBag, 3000, ItemID.BeetleHusk, 35);
            AddRemodelRecipe(ItemID.GolemBossBag, 5000, ItemID.Picksaw);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.Stynger);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.PossessedHatchet);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.SunStone);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.EyeoftheGolem);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.HeatRay);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.StaffofEarth);
            AddRemodelRecipe(ItemID.GolemBossBag, 4500, ItemID.GolemFist);
            AddRemodelRecipe(ItemID.GolemBossBag, 3000, ItemID.GolemMask);
            AddRemodelRecipe(ItemID.GolemBossBag, 4000, ItemID.GolemTrophy);
            AddRemodelRecipe(ItemID.GolemBossBag, 1000, ItemID.GolemPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.GolemBossBag, 1500, 3);

            //猪鲨宝藏袋
            AddRemodelRecipe(ItemID.FishronBossBag, 5000, ItemID.BubbleGun);
            AddRemodelRecipe(ItemID.FishronBossBag, 5000, ItemID.Flairon);
            AddRemodelRecipe(ItemID.FishronBossBag, 5000, ItemID.RazorbladeTyphoon);
            AddRemodelRecipe(ItemID.FishronBossBag, 5000, ItemID.TempestStaff);
            AddRemodelRecipe(ItemID.FishronBossBag, 5000, ItemID.Tsunami);
            AddRemodelRecipe(ItemID.FishronBossBag, 7000, ItemID.FishronWings);
            AddRemodelRecipe(ItemID.FishronBossBag, 3000, ItemID.DukeFishronMask);
            AddRemodelRecipe(ItemID.FishronBossBag, 4000, ItemID.DukeFishronTrophy);
            AddRemodelRecipe(ItemID.FishronBossBag, 1000, ItemID.DukeFishronPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.FishronBossBag, 1500, 3);

            //光女宝藏袋
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 5000, ItemID.FairyQueenMagicItem);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 5000, ItemID.PiercingStarlight);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 5000, ItemID.RainbowWhip);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 5000, ItemID.FairyQueenRangedItem);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 6000, ItemID.RainbowWings);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 7000, ItemID.SparkleGuitar);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 7000, ItemID.RainbowCursor);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 6000, ItemID.HallowBossDye, 3);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 3000, ItemID.FairyQueenMask);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 4000, ItemID.FairyQueenTrophy);
            AddRemodelRecipe(ItemID.FairyQueenBossBag, 1000, ItemID.FairyQueenPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.FairyQueenBossBag, 1500, 3);

            //月总宝藏袋
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.Meowmere);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.Terrarian);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.StarWrath);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.SDMG);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.LastPrism);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.LunarFlareBook);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.RainbowCrystalStaff);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.MoonlordTurretStaff);
            AddRemodelRecipe<ConquerorOfTheSeas>(0f, ItemID.MoonLordBossBag, 7000);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.Celeb2);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.LunarOre, 170);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 7000, ItemID.MeowmereMinecart);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 4000, ItemID.BossMaskMoonlord);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 6000, ItemID.MoonLordTrophy);
            AddRemodelRecipe(ItemID.MoonLordBossBag, 2000, ItemID.MoonLordPetItem, conditions: Condition.InMasterMode);
            AddRemodelRecipe<SoulOfDeveloper>(0f, ItemID.MoonLordBossBag, 1500, 3);
        }
    }
}
