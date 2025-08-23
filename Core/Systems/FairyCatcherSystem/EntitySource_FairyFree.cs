using Terraria.DataStructures;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class EntitySource_FairyFree(int fairyType) : IEntitySource
    {
        public string Context => "FairyFree";

        public int fairyType = fairyType;
    }
}
