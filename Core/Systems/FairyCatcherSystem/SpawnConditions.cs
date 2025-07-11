using System;
using Terraria.Localization;
using static Coralite.Core.Systems.FairyCatcherSystem.FairySystem;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public sealed record FairySpawnCondition(Func<LocalizedText> Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        /// <summary> 森林环境 </summary>
        public static FairySpawnCondition ZoneForest =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.Player.ZoneForest);

        /// <summary> 洞穴环境 </summary>
        public static FairySpawnCondition ZoneRockLayerHeight =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.Player.ZoneRockLayerHeight);

        /// <summary> 海洋环境 </summary>
        public static FairySpawnCondition ZoneBeach =
            new(() => GetSpawnCondition(DescriptionID.ZoneBeach)
                , attempt => attempt.Player.ZoneBeach);

        /// <summary> 沙漠环境 </summary>
        public static FairySpawnCondition ZoneDesert =
            new(() => GetSpawnCondition(DescriptionID.ZoneDesert)
                , attempt => attempt.Player.ZoneDesert);

        /// <summary> 猩红环境 </summary>
        public static FairySpawnCondition ZoneCrimson =
            new(() => GetSpawnCondition(DescriptionID.ZoneCrimson)
                , attempt => attempt.Player.ZoneCrimson);

        /// <summary> 腐化环境 </summary>
        public static FairySpawnCondition ZoneCorrupt =
            new(() => GetSpawnCondition(DescriptionID.ZoneCorrupt)
                , attempt => attempt.Player.ZoneCorrupt);

        /// <summary> 地狱环境 </summary>
        public static FairySpawnCondition ZoneHell =
            new(() => GetSpawnCondition(DescriptionID.ZoneHell)
                , attempt => attempt.Player.ZoneUnderworldHeight);

        /// <summary> 仙灵捕捉环半径大于9 </summary>
        public static FairySpawnCondition CircleR_9 =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest)
                , attempt => attempt.CircleRadius >= 9 * 16);

    }
}
