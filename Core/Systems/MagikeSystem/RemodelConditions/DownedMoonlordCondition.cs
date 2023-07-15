using System;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedMoonlordCondition : IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedMoonlordCondition> singleton = new Lazy<DownedMoonlordCondition>(() => new DownedMoonlordCondition());
        public static DownedMoonlordCondition Instance { get => singleton.Value; }

        public string Description => "击败克月球领主后可重塑";

        public bool CanRemodel(Item item) => NPC.downedMoonlord;

    }
}
