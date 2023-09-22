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
            AddRemodelRecipe<TravelJournaling>(50, ItemID.YuumaTheBlueTiger, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.SunshineofIsrapony, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.DoNotEattheVileMushroom, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.ParsecPals, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.HoplitePizza, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Duality, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.BennyWarhol, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.KargohsSummon, selfStack: 2);

            AddRemodelRecipe<TravelJournaling>(150, ItemID.Stopwatch, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LifeformAnalyzer, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DPSMeter, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LawnFlamingo, selfStack: 4 * 5);

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
            AddRemodelRecipe<TravelJournaling>(25, ItemID.SteampunkCup, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.ZebraSkin, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.LeopardSkin, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.TigerSkin, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Pho, selfStack: 3);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PadThai, selfStack: 2);
            AddRemodelRecipe<TravelJournaling>(50, ItemID.Sake, 5);

            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWilson, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWillow, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWendy, selfStack: 4, condition: DontStarveWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(75, ItemID.PaintingWolfgang, selfStack: 4, condition: DontStarveWorldCondition.Instance);

            AddRemodelRecipe<TravelJournaling>(25, ItemID.UltrabrightTorch, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Katana, selfStack: 4 * 10, condition: NotDontDigUpCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Keybrand, selfStack: 4 * 10, condition: DontDigUpWorldCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ActuationAccessory, selfStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PortableCementMixer, selfStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PaintSprayer, selfStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ExtendoGrip, selfStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BrickLayer, selfStack: 4 * 10);

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

            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkRibbon, selfStack: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressSkirt, selfStack: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PrettyPinkDressPants, selfStack: 4 * 20);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.UnicornHornHat, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HeartHairpin, selfStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.StarHairpin, selfStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fedora, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GoblorcEar, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.VulkelfEar, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.PandaEars, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DevilHorns, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHood, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsHoodie, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.LincolnsPants, selfStack: 4);

            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessCrown, selfStack: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.StarPrincessDress, selfStack: 4 * 50);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CelestialWand, selfStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterShirt, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GameMasterPants, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefHat, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefShirt, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.ChefPants, selfStack: 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Gi, selfStack: 4 * 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.GypsyRobe, selfStack: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MagicHat, selfStack: 4 * 3);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Fez, selfStack: 4 * 3 + 2);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AmmoBox, selfStack: 4 * 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Revolver, selfStack: 4 * 10, condition: new RemodelCondition(
                 (i) => WorldGen.shadowOrbSmashed, "敲碎一个暗影球或猩红心脏后可重塑"));

            AddRemodelRecipe<TravelJournaling>(450, ItemID.BambooLeaf, selfStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BedazzledNectar, selfStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.BlueEgg, selfStack: 4 * 25);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.ExoticEasternChewToy, selfStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BirdieRattle, selfStack: 4 * 100);
            AddRemodelRecipe<TravelJournaling>(300, ItemID.AntiPortalBlock, 25, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.CompanionCube, selfStack: 4 * 500);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.SittingDucksFishingRod, selfStack: 4 * 35, condition: DownedSkeletronCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.HunterCloak, selfStack: 4 * 15);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WinterCape, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.RedCape, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.MysteriousCape, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.CrimsonCloak, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.DiamondRing, selfStack: 4 * 200);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.WaterGun, selfStack: 4 + 2);
            AddRemodelRecipe<TravelJournaling>(1500, ItemID.PulseBow, selfStack: 4 * 45, condition: DownedPlanteraCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.YellowCounterweight, selfStack: 4 * 5);

            AddRemodelRecipe<TravelJournaling>(50, ItemID.ArcaneRuneWall, 10);
            AddRemodelRecipe<TravelJournaling>(150, ItemID.Kimono, selfStack: 4);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.BouncingShield, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(600, ItemID.Gatligator, selfStack: 4 * 35, condition: HardModeCondition.Instance);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.BlackCounterweight, selfStack: 4 * 5);
            AddRemodelRecipe<TravelJournaling>(450, ItemID.AngelHalo, selfStack: 4 * 40);




        }
    }
}
