using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public class EntitySource_FairyCatch : IEntitySource
    {
        public string Context => "FairyCatch";

        public Player player;

        public Fairy fairy;
    }
}
