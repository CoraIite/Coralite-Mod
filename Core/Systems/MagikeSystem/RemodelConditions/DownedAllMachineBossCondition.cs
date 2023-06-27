using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedAllMachineBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedAllMachineBossCondition> singleton = new Lazy<DownedAllMachineBossCondition>(() => new DownedAllMachineBossCondition());
        public static DownedAllMachineBossCondition Instance { get => singleton.Value; }

        public string Description => "击败所有机械BOSS后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
    }

}
