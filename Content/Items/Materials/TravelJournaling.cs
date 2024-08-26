using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Content.Items.Materials
{
    public class TravelJournaling : BaseMaterial, IMagikeCraftable
    {
        public TravelJournaling() : base(Item.CommonMaxStack, Item.buyPrice(0, 0, 25, 0), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ModContent.ItemType<TravelJournaling>(), ItemID.YuumaTheBlueTiger, 50, MainItenStack: 2)
                .RegisterNew(ItemID.SunshineofIsrapony, 50)
                .RegisterNew(ItemID.DoNotEattheVileMushroom, 50)
                .RegisterNew(ItemID.ParsecPals, 50)
                .RegisterNew(ItemID.HoplitePizza, 50)
                .RegisterNew(ItemID.Duality, 50)
                .RegisterNew(ItemID.BennyWarhol, 50)
                .RegisterNew(ItemID.KargohsSummon, 50)

                .RegisterNew(ItemID.Stopwatch, 150)
                .SetMainStack(4 * 5)
                .RegisterNew(ItemID.LifeformAnalyzer, 150)
                .RegisterNew(ItemID.DPSMeter, 150)
                .RegisterNew(ItemID.LawnFlamingo, 150)

                .RegisterNew(ItemID.TeamBlockPink, 25, 25)
                .SetMainStack(1)
                .RegisterNew(ItemID.TeamBlockPinkPlatform, 25, 25)
                .RegisterNew(ItemID.TeamBlockRed, 25, 25)
                .RegisterNew(ItemID.TeamBlockRedPlatform, 25, 25)
                .RegisterNew(ItemID.TeamBlockYellow, 25, 25)
                .RegisterNew(ItemID.TeamBlockYellowPlatform, 25, 25)
                .RegisterNew(ItemID.TeamBlockGreen, 25, 25)
                .RegisterNew(ItemID.TeamBlockGreenPlatform, 25, 25)
                .RegisterNew(ItemID.TeamBlockBlue, 25, 25)
                .RegisterNew(ItemID.TeamBlockBluePlatform, 25, 25)
                .RegisterNew(ItemID.TeamBlockWhite, 25, 25)
                .RegisterNew(ItemID.TeamBlockWhitePlatform, 25, 25)

                .RegisterNew(ItemID.DynastyWood, 25, 50)
                .RegisterNew(ItemID.RedDynastyShingles, 25, 50)
                .RegisterNew(ItemID.BlueDynastyShingles, 25, 50)
                .RegisterNew(ItemID.FancyDishes, 25)
                .RegisterNew(ItemID.SteampunkCup, 25)
                .SetMainStack(2)
                .RegisterNew(ItemID.ZebraSkin, 75)
                .SetMainStack(4)
                .RegisterNew(ItemID.LeopardSkin, 75)
                .RegisterNew(ItemID.TigerSkin, 75)
                .RegisterNew(ItemID.Pho, 50)
                .SetMainStack(3)
                .RegisterNew(ItemID.PadThai, 75)
                .SetMainStack(2)
                .RegisterNew(ItemID.Sake, 50)
                .SetMainStack(1)

                .RegisterNew(ItemID.PaintingWilson, 75)
                .SetMainStack(4)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNew(ItemID.PaintingWillow, 75)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNew(ItemID.PaintingWendy, 75)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNew(ItemID.PaintingWolfgang, 75)
                .AddCondition(Condition.DontStarveWorld)

                .RegisterNew(ItemID.UltrabrightTorch, 25, 10)
                .SetMainStack(1)
                .RegisterNew(ItemID.Katana, 150)
                .SetMainStack(4 * 10)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNew(ItemID.Keybrand, 150)
                .AddCondition(Condition.RemixWorld)
                .RegisterNew(ItemID.ActuationAccessory, 150)
                .SetMainStack(4 * 10)
                .RegisterNew(ItemID.PortableCementMixer, 150)
                .RegisterNew(ItemID.PaintSprayer, 150)
                .RegisterNew(ItemID.ExtendoGrip, 150)
                .RegisterNew(ItemID.BrickLayer, 150)

                //冰雪女王后卖的的各种画
                .RegisterNew(ItemID.PaintingTheSeason, 5000)
                .SetMainStack(2)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNew(ItemID.PaintingSnowfellas, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNew(ItemID.PaintingCursedSaint, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNew(ItemID.PaintingColdSnap, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNew(ItemID.PaintingAcorns, 5000)
                .AddCondition(Condition.DownedFrostLegion)

                .RegisterNew(ItemID.MoonmanandCompany, 2000)
                .SetMainStack(4 * 3)
                .AddCondition(Condition.DownedMoonLord)
                .RegisterNew(ItemID.MoonLordPainting, 2000)
                .AddCondition(Condition.DownedMoonLord)

                .RegisterNew(ItemID.PaintingTheTruthIsUpThere, 1000)
                .SetMainStack(4 * 2)
                .AddCondition(Condition.DownedMartians)
                .RegisterNew(ItemID.PaintingMartiaLisa, 2000)
                .AddCondition(Condition.DownedMoonLord)
                .RegisterNew(ItemID.PaintingCastleMarsberg, 2000)
                .AddCondition(Condition.DownedMoonLord)

                .RegisterNew(ItemID.Code2, 600)
                .SetMainStack(4 * 25)
                .AddCondition(Condition.DownedMechBossAny)
                .RegisterNew(ItemID.Code1, 150)
                .SetMainStack(4 * 5)
                .AddCondition(Condition.DownedEyeOfCthulhu)

                .RegisterNew(ItemID.ZapinatorGray, 150)
                .SetMainStack(4 * 17)
                .AddCondition(Condition.DownedEarlygameBoss)
                .RegisterNew(ItemID.ZapinatorOrange, 600)
                .SetMainStack(4 * 50)
                .AddCondition(Condition.Hardmode)

                .RegisterNew(ItemID.PrettyPinkRibbon, 150)
                .SetMainStack(4 * 15)
                .RegisterNew(ItemID.PrettyPinkDressSkirt, 150)
                .SetMainStack(4 * 20)
                .RegisterNew(ItemID.PrettyPinkDressPants, 150)
                .RegisterNew(ItemID.UnicornHornHat, 150)
                .SetMainStack(10)
                .RegisterNew(ItemID.HeartHairpin, 150)
                .RegisterNew(ItemID.Fedora, 150)
                .RegisterNew(ItemID.GoblorcEar, 150)
                .RegisterNew(ItemID.VulkelfEar, 150)
                .RegisterNew(ItemID.PandaEars, 150)
                .RegisterNew(ItemID.DevilHorns, 150)
                .SetMainStack(4 * 2)
                .RegisterNew(ItemID.StarHairpin, 150)
                .RegisterNew(ItemID.LincolnsHood, 150)
                .SetMainStack(4)
                .RegisterNew(ItemID.LincolnsHoodie, 150)
                .RegisterNew(ItemID.LincolnsPants, 150)

                .RegisterNew(ItemID.StarPrincessCrown, 450)
                .SetMainStack(4 * 50)
                .RegisterNew(ItemID.StarPrincessDress, 450)

                .RegisterNew(ItemID.CelestialWand, 450)
                .SetMainStack(4 * 100)
                .RegisterNew(ItemID.BambooLeaf, 450)
                .RegisterNew(ItemID.BedazzledNectar, 450)
                .RegisterNew(ItemID.ExoticEasternChewToy, 450)

                .RegisterNew(ItemID.GameMasterShirt, 450)
                .SetMainStack(4 * 5)
                .RegisterNew(ItemID.GameMasterPants, 450)
                .RegisterNew(ItemID.ChefHat, 450)
                .SetMainStack(10)
                .RegisterNew(ItemID.ChefShirt, 450)
                .RegisterNew(ItemID.ChefPants, 450)
                .RegisterNew(ItemID.Gi, 450)
                .SetMainStack(4 * 2)
                .RegisterNew(ItemID.GypsyRobe, 150)
                .SetMainStack(4 * 3 + 2)
                .RegisterNew(ItemID.Fez, 150)
                .RegisterNew(ItemID.MagicHat, 150)
                .SetMainStack(4 * 3)
                .RegisterNew(ItemID.AmmoBox, 500)
                .SetMainStack(4 * 10)
                .RegisterNew(ItemID.Revolver, 150)
                .AddCondition(Condition.SmashedShadowOrb)

                .RegisterNew(ItemID.BlueEgg, 150)
                .SetMainStack(4 * 25)
                .RegisterNew(ItemID.AntiPortalBlock, 150, 25)
                .SetMainStack(1)
                .AddCondition(Condition.Hardmode)
                .RegisterNew(ItemID.CompanionCube, 150)
                .SetMainStack(4 * 500)
                .RegisterNew(ItemID.SittingDucksFishingRod, 450)
                .SetMainStack(4 * 35)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNew(ItemID.HunterCloak, 150)
                .SetMainStack(4 * 15)
                .Register();
            //AddRemodelRecipe<TravelJournaling>(, ItemID., mainStack: 4 * 15);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.WinterCape, mainStack: 4 * 5);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.RedCape, mainStack: 4 * 5);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.MysteriousCape, mainStack: 4 * 5);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.CrimsonCloak, mainStack: 4 * 5);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.DiamondRing, mainStack: 4 * 200);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.WaterGun, mainStack: 4 + 2);
            //AddRemodelRecipe<TravelJournaling>(1500, ItemID.PulseBow, selfStack: 4 * 45, condition: DownedPlanteraCondition.Instance);
            //AddRemodelRecipe<TravelJournaling>(450, ItemID.YellowCounterweight, mainStack: 4 * 5);

            //AddRemodelRecipe<TravelJournaling>(50, ItemID.ArcaneRuneWall, 10);
            //AddRemodelRecipe<TravelJournaling>(150, ItemID.Kimono, mainStack: 4);
            //AddRemodelRecipe<TravelJournaling>(600, ItemID.BouncingShield, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            //AddRemodelRecipe<TravelJournaling>(600, ItemID.Gatligator, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            //AddRemodelRecipe<TravelJournaling>(450, ItemID.BlackCounterweight, mainStack: 4 * 5);
            //AddRemodelRecipe<TravelJournaling>(450, ItemID.AngelHalo, mainStack: 4 * 40);




        }
    }
}
