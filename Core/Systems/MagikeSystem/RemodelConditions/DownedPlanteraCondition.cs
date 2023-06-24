using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    internal class DownedPlanteraCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedPlanteraCondition> singleton = new Lazy<DownedPlanteraCondition>(() => new DownedPlanteraCondition());
        public static DownedPlanteraCondition Instance { get => singleton.Value; }

        public string Description => "击败世纪之花后可重塑";

        public bool CanRemodel(Item item) => NPC.downedPlantBoss;
    }
}
