using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedEyeOfCthulhu : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedEyeOfCthulhu> singleton = new Lazy<DownedEyeOfCthulhu>(() => new DownedEyeOfCthulhu());
        public static DownedEyeOfCthulhu Instance { get => singleton.Value; }

        public string Description => "击败克苏鲁之眼后可重塑";

        public bool CanRemodel(Item item) => NPC.downedBoss1;
    }

    public class DownedEvilBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedEvilBossCondition> singleton = new Lazy<DownedEvilBossCondition>(() => new DownedEvilBossCondition());
        public static DownedEvilBossCondition Instance { get => singleton.Value; }

        public string Description => "击败克苏鲁之脑或世界吞噬怪后可重塑";

        public bool CanRemodel(Item item) => NPC.downedBoss2;
    }

    public class DownedSkeletronCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedSkeletronCondition> singleton = new Lazy<DownedSkeletronCondition>(() => new DownedSkeletronCondition());
        public static DownedSkeletronCondition Instance { get => singleton.Value; }

        public string Description => "击败骷髅王后可重塑";

        public bool CanRemodel(Item item) => NPC.downedBoss3;
    }

    public class DownedAnyMachineBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedAnyMachineBossCondition> singleton = new Lazy<DownedAnyMachineBossCondition>(() => new DownedAnyMachineBossCondition());
        public static DownedAnyMachineBossCondition Instance { get => singleton.Value; }

        public string Description => "击败任意机械BOSS后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMechBossAny;
    }

    public class DownedAllMachineBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedAllMachineBossCondition> singleton = new Lazy<DownedAllMachineBossCondition>(() => new DownedAllMachineBossCondition());
        public static DownedAllMachineBossCondition Instance { get => singleton.Value; }

        public string Description => "击败所有机械BOSS后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
    }

    public class DownedFrostLegionCondition: IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedFrostLegionCondition> singleton = new Lazy<DownedFrostLegionCondition>(() => new DownedFrostLegionCondition());
        public static DownedFrostLegionCondition Instance { get => singleton.Value; }

        public string Description => "击败雪人军团后可重塑";

        public bool CanRemodel(Item item) => NPC.downedFrost;
    }

    public class DownedPlanteraCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedPlanteraCondition> singleton = new Lazy<DownedPlanteraCondition>(() => new DownedPlanteraCondition());
        public static DownedPlanteraCondition Instance { get => singleton.Value; }

        public string Description => "击败世纪之花后可重塑";

        public bool CanRemodel(Item item) => NPC.downedPlantBoss;
    }

    public class DownedMartianMadnessCondition: IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedMartianMadnessCondition> singleton = new Lazy<DownedMartianMadnessCondition>(() => new DownedMartianMadnessCondition());
        public static DownedMartianMadnessCondition Instance { get => singleton.Value; }

        public string Description => "击败火星人暴乱后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMartians;
    }

    public class DownedMoonlordCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedMoonlordCondition> singleton = new Lazy<DownedMoonlordCondition>(() => new DownedMoonlordCondition());
        public static DownedMoonlordCondition Instance { get => singleton.Value; }

        public string Description => "击败克月球领主后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMoonlord;

    }

}
