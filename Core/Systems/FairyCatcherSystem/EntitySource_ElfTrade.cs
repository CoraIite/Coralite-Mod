using Terraria.DataStructures;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class EntitySource_ElfTrade(int selfItemType,ElfPortalTrade trade) : IEntitySource
    {
        public string Context => "FairyCatch";

        public int  selfItemType= selfItemType;

        public ElfPortalTrade trade = trade;
    }
}
