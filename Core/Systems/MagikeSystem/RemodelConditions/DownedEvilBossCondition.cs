using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedEvilBossCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedEvilBossCondition> singleton = new Lazy<DownedEvilBossCondition>(() => new DownedEvilBossCondition());
        public static DownedEvilBossCondition Instance { get => singleton.Value; }

        public string Description => "击败克苏鲁之脑或世界吞噬怪后可重塑";

        public bool CanRemodel(Item item) => NPC.downedBoss2;
    }
}
