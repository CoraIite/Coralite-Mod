using Coralite.Content.Biomes;
using Coralite.Content.WorldGeneration;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.Localization;

namespace Coralite.Core
{
    public class CoraliteConditions : ModSystem, ILocalizedModType
    {
        public static Condition LearnedMagikeBase { get; private set; }
        public static Condition LearnedMagikeAdvance { get; private set; }
        public static Condition CoralCat { get; private set; }
        public static Condition MagikeCraft { get; private set; }
        public static Condition SpellCraft { get; private set; }

        public static Condition InDigDigDig { get; private set; }
        public static Condition NotInDigDigDig { get; private set; }
        public static Condition UseMultiBlockStructure { get; private set; }
        public static Condition UseShlimmerTranslation { get; private set; }

        public static Condition InMagicCrystalCave { get; private set; }
        public static Condition InCrystallineSkyIsland { get; private set; }

        public static Condition UseRuneParchment { get; private set; }

        public static Condition DownedRediancie { get; private set; }
        public static Condition DownedBabyIceDragon { get; private set; }
        public static Condition DownedThunderveinDragon { get; private set; }
        public static Condition DownedNightmarePlantera { get; private set; }

        /// <summary>
        /// 解锁染料商
        /// </summary>
        public static Condition UnlockDyeTrder { get; private set; }
        /// <summary>
        /// 解锁发型师
        /// </summary>
        public static Condition UnlockStylist { get; private set; }
        /// <summary>
        /// 解锁电工妹
        /// </summary>
        public static Condition UnlockMechanic { get; private set; }
        /// <summary>
        /// 解锁派对女孩
        /// </summary>
        public static Condition UnlockPartyGirl { get; private set; }
        /// <summary>
        /// 解锁税收官
        /// </summary>
        public static Condition UnlockTaxCollector { get; private set; }
        /// <summary>
        /// 解锁公主
        /// </summary>
        public static Condition UnlockPrincess { get; private set; }

        public string LocalizationCategory => "Conditions";

        public static LocalizedText DownedGolemCondition { get; private set; }

        public override void Load()
        {
            LearnedMagikeBase = new(this.GetLocalization(nameof(LearnedMagikeBase))
                , () => MagikeSystem.learnedMagikeBase);
            LearnedMagikeAdvance = new(this.GetLocalization(nameof(LearnedMagikeAdvance))
                , () => MagikeSystem.learnedMagikeAdvanced);
            CoralCat = new(this.GetLocalization(nameof(CoralCat))
                , () => CoraliteWorld.CoralCatWorld);
            InDigDigDig = new(this.GetLocalization(nameof(InDigDigDig))
                , () => CoraliteWorld.DigDigDigWorld);
            NotInDigDigDig = new(this.GetLocalization(nameof(NotInDigDigDig))
                , () => !CoraliteWorld.DigDigDigWorld);


            MagikeCraft = new(this.GetLocalization(nameof(MagikeCraft))
                , () => false);
            SpellCraft = new(this.GetLocalization(nameof(SpellCraft))
                , () => false);
            UseRuneParchment = new Condition(this.GetLocalization(nameof(UseRuneParchment))
                , () => false);


            DownedRediancie = new Condition(this.GetLocalization(nameof(DownedRediancie))
                , () => DownedBossSystem.downedRediancie);
            DownedBabyIceDragon = new Condition(this.GetLocalization(nameof(DownedBabyIceDragon))
                , () => DownedBossSystem.downedBabyIceDragon);
            DownedThunderveinDragon = new Condition(this.GetLocalization(nameof(DownedThunderveinDragon))
                , () => DownedBossSystem.downedThunderveinDragon);
            DownedNightmarePlantera = new Condition(this.GetLocalization(nameof(DownedNightmarePlantera))
                , () => DownedBossSystem.downedNightmarePlantera);


            InMagicCrystalCave = new(this.GetLocalization(nameof(InMagicCrystalCave))
                , () => Main.LocalPlayer.InModBiome<MagicCrystalCave>());
            InCrystallineSkyIsland = new Condition(this.GetLocalization(nameof(InCrystallineSkyIsland))
                , () => Main.LocalPlayer.InModBiome<CrystallineSkyIsland>());



            UseMultiBlockStructure = new(this.GetLocalization(nameof(UseMultiBlockStructure))
                , () => false);
            UseShlimmerTranslation = new(this.GetLocalization(nameof(UseShlimmerTranslation))
                , () => false);

            UnlockDyeTrder = new(this.GetLocalization(nameof(UnlockDyeTrder))
                , () => NPC.unlockedDyeTraderSpawn);
            UnlockStylist = new(this.GetLocalization(nameof(UnlockStylist))
                , () => NPC.savedStylist);
            UnlockMechanic = new(this.GetLocalization(nameof(UnlockMechanic))
                , () => NPC.savedMech);
            UnlockPartyGirl = new(this.GetLocalization(nameof(UnlockPartyGirl))
                , () => NPC.unlockedPartyGirlSpawn);
            UnlockTaxCollector = new(this.GetLocalization(nameof(UnlockTaxCollector))
                , () => NPC.savedTaxCollector);
            UnlockPrincess = new(this.GetLocalization(nameof(UnlockPrincess))
                , () => NPC.unlockedPrincessSpawn);

            DownedGolemCondition = this.GetLocalization(nameof(DownedGolemCondition));

        }

        public override void Unload()
        {
        }
    }
}
