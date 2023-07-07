using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.RemodelConditions
{
    public class DownedSkeletronCondition: IMagikeRemodelCondition
    {
        private static readonly Lazy<DownedSkeletronCondition> singleton = new Lazy<DownedSkeletronCondition>(() => new DownedSkeletronCondition());
        public static DownedSkeletronCondition Instance { get => singleton.Value; }

        public string Description => "击败骷髅王后可重塑";

        public bool CanRemodel(Item item) => NPC.downedBoss3;

    }
}
