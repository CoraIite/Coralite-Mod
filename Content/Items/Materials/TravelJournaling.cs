using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem.RemodelConditions;
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
            AddRemodelRecipe<TravelJournaling>(50, ItemID.YuumaTheBlueTiger, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.SunshineofIsrapony, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.DoNotEattheVileMushroom, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.ParsecPals, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.HoplitePizza, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Duality, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.BennyWarhol, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.KargohsSummon, selfRequiredNumber: 2);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.Stopwatch, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LifeformAnalyzer, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DPSMeter, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LawnFlamingo, selfRequiredNumber: 4 * 5);

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
            AddRemodelRecipe<TravelJournaling>(25, ItemID.SteampunkCup, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.ZebraSkin, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.LeopardSkin, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.TigerSkin, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Pho, selfRequiredNumber: 3);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PadThai, selfRequiredNumber: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Sake, 5);

            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWilson, selfRequiredNumber: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWillow, selfRequiredNumber: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWendy, selfRequiredNumber: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWolfgang, selfRequiredNumber: 4, condition: DontStarveWorldCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(25, ItemID.UltrabrightTorch, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Katana, selfRequiredNumber: 4 * 10, condition: NotDontDigUpCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Keybrand, selfRequiredNumber: 4 * 10, condition: DontDigUpWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ActuationAccessory, selfRequiredNumber: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PortableCementMixer, selfRequiredNumber: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PaintSprayer, selfRequiredNumber: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ExtendoGrip, selfRequiredNumber: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BrickLayer, selfRequiredNumber: 4 * 10);

            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingTheSeason, selfRequiredNumber: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingSnowfellas, selfRequiredNumber: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingCursedSaint, selfRequiredNumber: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingColdSnap, selfRequiredNumber: 2, condition: DownedFrostLegionCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.PaintingAcorns, selfRequiredNumber: 2, condition: DownedFrostLegionCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(2000, ItemID.MoonmanandCompany, selfRequiredNumber: 4 * 3, condition: DownedMoonlordCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(2000, ItemID.MoonLordPainting, selfRequiredNumber: 4 * 3, condition: DownedMoonlordCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingTheTruthIsUpThere, selfRequiredNumber: 4 * 2, condition: DownedMartianMadnessCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingMartiaLisa, selfRequiredNumber: 4 * 2, condition: DownedMartianMadnessCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(1000, ItemID.PaintingCastleMarsberg, selfRequiredNumber: 4 * 2, condition: DownedMartianMadnessCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(600, ItemID.Code2, selfRequiredNumber: 4 * 25, condition: DownedAnyMachineBossCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Code1, selfRequiredNumber: 4 * 5, condition: DownedEyeOfCthulhu.Instance);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.ZapinatorGray, selfRequiredNumber: 4 * 17, condition: new RemodelCondition(
                (i) => NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee || Main.hardMode, "击败前期BOSS后可重塑"));
            AddRemodelRecipe<TravelJournaling>(600, ItemID.ZapinatorOrange, selfRequiredNumber: 4 * 50, condition: HardModeCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkRibbon, selfRequiredNumber: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressSkirt, selfRequiredNumber: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressPants, selfRequiredNumber: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.UnicornHornHat, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HeartHairpin, selfRequiredNumber: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.StarHairpin, selfRequiredNumber: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fedora, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GoblorcEar, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.VulkelfEar, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PandaEars, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DevilHorns, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHood, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHoodie, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsPants, selfRequiredNumber: 4);

            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessCrown, selfRequiredNumber: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessDress, selfRequiredNumber: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CelestialWand, selfRequiredNumber: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterShirt, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterPants, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefHat, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefShirt, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefPants, selfRequiredNumber: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Gi, selfRequiredNumber: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GypsyRobe, selfRequiredNumber: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MagicHat, selfRequiredNumber: 4 * 3);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fez, selfRequiredNumber: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AmmoBox, selfRequiredNumber: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Revolver, selfRequiredNumber: 4 * 10, condition: new RemodelCondition(
                 (i) => WorldGen.shadowOrbSmashed, "敲碎一个暗影球或猩红心脏后可重塑"));

            AddRemodelRecipe<TravelJournaling>(450, ItemID.BambooLeaf, selfRequiredNumber: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BedazzledNectar, selfRequiredNumber: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BlueEgg, selfRequiredNumber: 4 * 25);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.ExoticEasternChewToy, selfRequiredNumber: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BirdieRattle, selfRequiredNumber: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AntiPortalBlock, 25, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CompanionCube, selfRequiredNumber: 4 * 500);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.SittingDucksFishingRod, selfRequiredNumber: 4 * 35, condition: DownedSkeletronCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HunterCloak, selfRequiredNumber: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WinterCape, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.RedCape, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MysteriousCape, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.CrimsonCloak, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DiamondRing, selfRequiredNumber: 4 * 200);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WaterGun, selfRequiredNumber: 4 + 2);
            AddRemodelRecipe<TravelJournaling>(1500, ItemID.PulseBow, selfRequiredNumber: 4 * 45, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.YellowCounterweight, selfRequiredNumber: 4 * 5);

            AddRemodelRecipe<TravelJournaling>(50, ItemID.ArcaneRuneWall, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Kimono, selfRequiredNumber: 4);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.BouncingShield, selfRequiredNumber: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.Gatligator, selfRequiredNumber: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BlackCounterweight, selfRequiredNumber: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.AngelHalo, selfRequiredNumber: 4 * 40);




        }
    }
}
