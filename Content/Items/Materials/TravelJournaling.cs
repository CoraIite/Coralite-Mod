using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.CraftConditions;
using Terraria;
using Terraria.ID;
using static Coralite.Core.Systems.MagikeSystem.MagikeSystem;

namespace Coralite.Content.Items.Materials
{
    public class TravelJournaling : BaseMaterial, IMagikeRemodelable
    {
        public TravelJournaling() : base(Item.CommonMaxStack, Item.buyPrice(0, 0, 25, 0), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public void AddMagikeRemodelRecipe()
        {
            AddRemodelRecipe<TravelJournaling>(50, ItemID.YuumaTheBlueTiger, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.SunshineofIsrapony, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.DoNotEattheVileMushroom, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.ParsecPals, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.HoplitePizza, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Duality, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.BennyWarhol, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.KargohsSummon, mainStack: 2);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.Stopwatch, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LifeformAnalyzer, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DPSMeter, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LawnFlamingo, mainStack: 4 * 5);

            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockPink, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockPinkPlatform, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockRed, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockRedPlatform, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockYellow, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockYellowPlatform, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockGreen, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockGreenPlatform, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockBlue, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockBluePlatform, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockWhite, 25);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.TeamBlockWhitePlatform, 25);

            AddRemodelRecipe<TravelJournaling>(25, ItemID.DynastyWood, 50);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.RedDynastyShingles, 50);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.BlueDynastyShingles, 50);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.FancyDishes);
            AddRemodelRecipe<TravelJournaling>(25, ItemID.SteampunkCup, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.ZebraSkin, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.LeopardSkin, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.TigerSkin, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Pho, mainStack: 3);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PadThai, mainStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Sake, 5);

            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWilson, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWillow, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWendy, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWolfgang, selfStack: 4, condition: DontStarveWorldCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(25, ItemID.UltrabrightTorch, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Katana, selfStack: 4 * 10, condition: NotDontDigUpCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Keybrand, selfStack: 4 * 10, condition: DontDigUpWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ActuationAccessory, mainStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PortableCementMixer, mainStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PaintSprayer, mainStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ExtendoGrip, mainStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BrickLayer, mainStack: 4 * 10);

            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingTheSeason, selfStack: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingSnowfellas, selfStack: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingCursedSaint, selfStack: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingColdSnap, selfStack: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingAcorns, selfStack: 2, condition: DownedFrostLegionCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(2000, ItemID.MoonmanandCompany, selfStack: 4 * 3, condition: DownedMoonlordCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(2000, ItemID.MoonLordPainting, selfStack: 4 * 3, condition: DownedMoonlordCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingTheTruthIsUpThere, selfStack: 4 * 2, condition: DownedMartianMadnessCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingMartiaLisa, selfStack: 4 * 2, condition: DownedMartianMadnessCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingCastleMarsberg, selfStack: 4 * 2, condition: DownedMartianMadnessCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(600, ItemID.Code2, selfStack: 4 * 25, condition: DownedAnyMachineBossCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Code1, selfStack: 4 * 5, condition: DownedEyeOfCthulhu.Instance);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.ZapinatorGray, selfStack: 4 * 17, condition: new RemodelCondition(
                (i) => NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee || Main.hardMode, "击败前期BOSS后可重塑"));
            AddRemodelRecipe<TravelJournaling>(600, ItemID.ZapinatorOrange, selfStack: 4 * 50, condition: HardModeCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkRibbon, mainStack: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressSkirt, mainStack: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressPants, mainStack: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.UnicornHornHat, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HeartHairpin, mainStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.StarHairpin, mainStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fedora, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GoblorcEar, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.VulkelfEar, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PandaEars, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DevilHorns, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHood, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHoodie, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsPants, mainStack: 4);

            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessCrown, mainStack: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessDress, mainStack: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CelestialWand, mainStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterShirt, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterPants, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefHat, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefShirt, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefPants, mainStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Gi, mainStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GypsyRobe, mainStack: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MagicHat, mainStack: 4 * 3);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fez, mainStack: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AmmoBox, mainStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Revolver, selfStack: 4 * 10, condition: new RemodelCondition(
                 (i) => WorldGen.shadowOrbSmashed, "敲碎一个暗影球或猩红心脏后可重塑"));

            AddRemodelRecipe<TravelJournaling>(450, ItemID.BambooLeaf, mainStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BedazzledNectar, mainStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BlueEgg, mainStack: 4 * 25);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.ExoticEasternChewToy, mainStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BirdieRattle, mainStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AntiPortalBlock, 25, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CompanionCube, mainStack: 4 * 500);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.SittingDucksFishingRod, selfStack: 4 * 35, condition: DownedSkeletronCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HunterCloak, mainStack: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WinterCape, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.RedCape, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MysteriousCape, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.CrimsonCloak, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DiamondRing, mainStack: 4 * 200);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WaterGun, mainStack: 4 + 2);
            AddRemodelRecipe<TravelJournaling>(1500, ItemID.PulseBow, selfStack: 4 * 45, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.YellowCounterweight, mainStack: 4 * 5);

            AddRemodelRecipe<TravelJournaling>(50, ItemID.ArcaneRuneWall, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Kimono, mainStack: 4);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.BouncingShield, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.Gatligator, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BlackCounterweight, mainStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.AngelHalo, mainStack: 4 * 40);




        }
    }
}
