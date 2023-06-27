using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedAnyMachineBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedAnyMachineBossCondition> singleton = new Lazy<DownedAnyMachineBossCondition>(() => new DownedAnyMachineBossCondition());
        public static DownedAnyMachineBossCondition Instance { get => singleton.Value; }

        public string Description => "击败任意机械BOSS后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMechBossAny;
    }
}
