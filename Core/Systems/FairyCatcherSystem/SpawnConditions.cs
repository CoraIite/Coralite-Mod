using System;
using Terraria.Localization;
using static Coralite.Core.Systems.FairyCatcherSystem.FairySystem;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public sealed record FairySpawnCondition(Func<LocalizedText> Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        public static FairySpawnCondition ZoneForest =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.Player.ZoneForest);

        public static FairySpawnCondition ZoneRockLayerHeight =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.Player.ZoneRockLayerHeight);

        public static FairySpawnCondition ZoneBeach =
            new(() => GetSpawnCondition(DescriptionID.ZoneBeach)
                , attempt => attempt.Player.ZoneBeach);

        /// <summary>
        /// 仙灵捕捉环半径大于9
        /// </summary>
        public static FairySpawnCondition CircleR_9 =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.CircleRadius >= 9 * 16);

    }
}
