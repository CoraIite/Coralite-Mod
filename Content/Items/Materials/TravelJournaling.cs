using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Materials
{
    public class TravelJournaling : BaseMaterial, IMagikeCraftable
    {
        public TravelJournaling() : base(Item.CommonMaxStack, Item.buyPrice(0, 0, 25, 0), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ModContent.ItemType<TravelJournaling>(), ItemID.YuumaTheBlueTiger, 50, MainItenStack: 2)
                .RegisterNewCraft(ItemID.SunshineofIsrapony, 50)
                .RegisterNewCraft(ItemID.DoNotEattheVileMushroom, 50)
                .RegisterNewCraft(ItemID.ParsecPals, 50)
                .RegisterNewCraft(ItemID.HoplitePizza, 50)
                .RegisterNewCraft(ItemID.Duality, 50)
                .RegisterNewCraft(ItemID.BennyWarhol, 50)
                .RegisterNewCraft(ItemID.KargohsSummon, 50)

                .RegisterNewCraft(ItemID.Stopwatch, 150)
                .SetMainStack(4 * 5)
                .RegisterNewCraft(ItemID.LifeformAnalyzer, 150)
                .RegisterNewCraft(ItemID.DPSMeter, 150)
                .RegisterNewCraft(ItemID.LawnFlamingo, 150)

                .RegisterNewCraft(ItemID.TeamBlockPink, 25, 25)
                .SetMainStack(1)
                .RegisterNewCraft(ItemID.TeamBlockPinkPlatform, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockRed, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockRedPlatform, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockYellow, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockYellowPlatform, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockGreen, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockGreenPlatform, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockBlue, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockBluePlatform, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockWhite, 25, 25)
                .RegisterNewCraft(ItemID.TeamBlockWhitePlatform, 25, 25)

                .RegisterNewCraft(ItemID.DynastyWood, 25, 50)
                .RegisterNewCraft(ItemID.RedDynastyShingles, 25, 50)
                .RegisterNewCraft(ItemID.BlueDynastyShingles, 25, 50)
                .RegisterNewCraft(ItemID.FancyDishes, 25)
                .RegisterNewCraft(ItemID.SteampunkCup, 25)
                .SetMainStack(2)
                .RegisterNewCraft(ItemID.ZebraSkin, 75)
                .SetMainStack(4)
                .RegisterNewCraft(ItemID.LeopardSkin, 75)
                .RegisterNewCraft(ItemID.TigerSkin, 75)
                .RegisterNewCraft(ItemID.Pho, 50)
                .SetMainStack(3)
                .RegisterNewCraft(ItemID.PadThai, 75)
                .SetMainStack(2)
                .RegisterNewCraft(ItemID.Sake, 50)
                .SetMainStack(1)

                .RegisterNewCraft(ItemID.PaintingWilson, 75)
                .SetMainStack(4)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNewCraft(ItemID.PaintingWillow, 75)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNewCraft(ItemID.PaintingWendy, 75)
                .AddCondition(Condition.DontStarveWorld)
                .RegisterNewCraft(ItemID.PaintingWolfgang, 75)
                .AddCondition(Condition.DontStarveWorld)

                .RegisterNewCraft(ItemID.UltrabrightTorch, 25, 10)
                .SetMainStack(1)
                .RegisterNewCraft(ItemID.Katana, 150)
                .SetMainStack(4 * 10)
                .AddCondition(Condition.NotRemixWorld)
                .RegisterNewCraft(ItemID.Keybrand, 150)
                .AddCondition(Condition.RemixWorld)
                .RegisterNewCraft(ItemID.ActuationAccessory, 150)
                .SetMainStack(4 * 10)
                .RegisterNewCraft(ItemID.PortableCementMixer, 150)
                .RegisterNewCraft(ItemID.PaintSprayer, 150)
                .RegisterNewCraft(ItemID.ExtendoGrip, 150)
                .RegisterNewCraft(ItemID.BrickLayer, 150)

                //冰雪女王后卖的的各种画
                .RegisterNewCraft(ItemID.PaintingTheSeason, 5000)
                .SetMainStack(2)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNewCraft(ItemID.PaintingSnowfellas, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNewCraft(ItemID.PaintingCursedSaint, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNewCraft(ItemID.PaintingColdSnap, 5000)
                .AddCondition(Condition.DownedFrostLegion)
                .RegisterNewCraft(ItemID.PaintingAcorns, 5000)
                .AddCondition(Condition.DownedFrostLegion)

                .RegisterNewCraft(ItemID.MoonmanandCompany, 2000)
                .SetMainStack(4 * 3)
                .AddCondition(Condition.DownedMoonLord)
                .RegisterNewCraft(ItemID.MoonLordPainting, 2000)
                .AddCondition(Condition.DownedMoonLord)

                .RegisterNewCraft(ItemID.PaintingTheTruthIsUpThere, 1000)
                .SetMainStack(4 * 2)
                .AddCondition(Condition.DownedMartians)
                .RegisterNewCraft(ItemID.PaintingMartiaLisa, 2000)
                .AddCondition(Condition.DownedMoonLord)
                .RegisterNewCraft(ItemID.PaintingCastleMarsberg, 2000)
                .AddCondition(Condition.DownedMoonLord)

                .RegisterNewCraft(ItemID.Code2, 600)
                .SetMainStack(4 * 25)
                .AddCondition(Condition.DownedMechBossAny)
                .RegisterNewCraft(ItemID.Code1, 150)
                .SetMainStack(4 * 5)
                .AddCondition(Condition.DownedEyeOfCthulhu)

                .RegisterNewCraft(ItemID.ZapinatorGray, 150)
                .SetMainStack(4 * 17)
                .AddCondition(Condition.DownedEarlygameBoss)
                .RegisterNewCraft(ItemID.ZapinatorOrange, 600)
                .SetMainStack(4 * 50)
                .AddCondition(Condition.Hardmode)

                .RegisterNewCraft(ItemID.PrettyPinkRibbon, 150)
                .SetMainStack(4 * 15)
                .RegisterNewCraft(ItemID.PrettyPinkDressSkirt, 150)
                .SetMainStack(4 * 20)
                .RegisterNewCraft(ItemID.PrettyPinkDressPants, 150)
                .RegisterNewCraft(ItemID.UnicornHornHat, 150)
                .SetMainStack(10)
                .RegisterNewCraft(ItemID.HeartHairpin, 150)
                .RegisterNewCraft(ItemID.Fedora, 150)
                .RegisterNewCraft(ItemID.GoblorcEar, 150)
                .RegisterNewCraft(ItemID.VulkelfEar, 150)
                .RegisterNewCraft(ItemID.PandaEars, 150)
                .RegisterNewCraft(ItemID.DevilHorns, 150)
                .SetMainStack(4 * 2)
                .RegisterNewCraft(ItemID.StarHairpin, 150)
                .RegisterNewCraft(ItemID.LincolnsHood, 150)
                .SetMainStack(4)
                .RegisterNewCraft(ItemID.LincolnsHoodie, 150)
                .RegisterNewCraft(ItemID.LincolnsPants, 150)

                .RegisterNewCraft(ItemID.StarPrincessCrown, 450)
                .SetMainStack(4 * 50)
                .RegisterNewCraft(ItemID.StarPrincessDress, 450)

                .RegisterNewCraft(ItemID.CelestialWand, 450)
                .SetMainStack(4 * 100)
                .RegisterNewCraft(ItemID.BambooLeaf, 450)
                .RegisterNewCraft(ItemID.BedazzledNectar, 450)
                .RegisterNewCraft(ItemID.ExoticEasternChewToy, 450)

                .RegisterNewCraft(ItemID.GameMasterShirt, 450)
                .SetMainStack(4 * 5)
                .RegisterNewCraft(ItemID.GameMasterPants, 450)
                .RegisterNewCraft(ItemID.ChefHat, 450)
                .SetMainStack(10)
                .RegisterNewCraft(ItemID.ChefShirt, 450)
                .RegisterNewCraft(ItemID.ChefPants, 450)
                .RegisterNewCraft(ItemID.Gi, 450)
                .SetMainStack(4 * 2)
                .RegisterNewCraft(ItemID.GypsyRobe, 150)
                .SetMainStack((4 * 3) + 2)
                .RegisterNewCraft(ItemID.Fez, 150)
                .RegisterNewCraft(ItemID.MagicHat, 150)
                .SetMainStack(4 * 3)
                .RegisterNewCraft(ItemID.AmmoBox, 500)
                .SetMainStack(4 * 10)
                .RegisterNewCraft(ItemID.Revolver, 150)
                .AddCondition(Condition.SmashedShadowOrb)

                .RegisterNewCraft(ItemID.BlueEgg, 150)
                .SetMainStack(4 * 25)
                .RegisterNewCraft(ItemID.AntiPortalBlock, 150, 25)
                .SetMainStack(1)
                .AddCondition(Condition.Hardmode)
                .RegisterNewCraft(ItemID.CompanionCube, 150)
                .SetMainStack(4 * 500)
                .RegisterNewCraft(ItemID.SittingDucksFishingRod, 450)
                .SetMainStack(4 * 35)
                .AddCondition(Condition.DownedSkeletron)
                .RegisterNewCraft(ItemID.HunterCloak, 150)
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
