using System;
using Terraria.Localization;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public sealed record FairySpawnCondition(Func<LocalizedText> Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        public static FairySpawnCondition ZoneForest =
            new(() => FairySystem.SpawnDescriptions[FairySystem.DescriptionID.ZoneForest]
                , attempt => attempt.Player.ZoneForest);

        public static FairySpawnCondition ZoneRockLayerHeight =
            new(() => FairySystem.SpawnDescriptions[FairySystem.DescriptionID.ZoneRockLayer]
                , attempt => attempt.Player.ZoneRockLayerHeight);

        public static FairySpawnCondition ZoneBeach =
            new(() => FairySystem.SpawnDescriptions[FairySystem.DescriptionID.ZoneBeach]
                , attempt => attempt.Player.ZoneBeach);

    }
}
