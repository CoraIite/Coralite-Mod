using System;
using Terraria.ID;
using Terraria.Localization;
using static Coralite.Core.Systems.FairyCatcherSystem.FairySystem;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public record FairySpawnCondition(Func<string> Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        /// <summary> 森林环境 </summary>
        public static FairySpawnCondition ZoneForest =
            new(() => GetSpawnCondition(DescriptionID.ZoneForest).Value
                , attempt => attempt.Player.ZoneForest);

        /// <summary> 洞穴环境 </summary>
        public static FairySpawnCondition ZoneRockLayerHeight =
            new(() => GetSpawnCondition(DescriptionID.ZoneRockLayer).Value
                , attempt => attempt.Player.ZoneRockLayerHeight);

        /// <summary> 海洋环境 </summary>
        public static FairySpawnCondition ZoneBeach =
            new(() => GetSpawnCondition(DescriptionID.ZoneBeach).Value
                , attempt => attempt.Player.ZoneBeach);

        /// <summary> 沙漠环境 </summary>
        public static FairySpawnCondition ZoneDesert =
            new(() => GetSpawnCondition(DescriptionID.ZoneDesert).Value
                , attempt => attempt.Player.ZoneDesert);

        /// <summary> 猩红环境 </summary>
        public static FairySpawnCondition ZoneCrimson =
            new(() => GetSpawnCondition(DescriptionID.ZoneCrimson).Value
                , attempt => attempt.Player.ZoneCrimson);

        /// <summary> 腐化环境 </summary>
        public static FairySpawnCondition ZoneCorrupt =
            new(() => GetSpawnCondition(DescriptionID.ZoneCorrupt).Value
                , attempt => attempt.Player.ZoneCorrupt);

        /// <summary> 地狱环境 </summary>
        public static FairySpawnCondition ZoneHell =
            new(() => GetSpawnCondition(DescriptionID.ZoneHell).Value
                , attempt => attempt.Player.ZoneUnderworldHeight);

        /// <summary> 仙灵捕捉环半径大于9 </summary>
        public static FairySpawnCondition CircleR_9 =
            new(() => GetSpawnCondition(DescriptionID.CircleR_9).Value
                , attempt => attempt.CircleRadius >= 9 * 16);

        /// <summary> 处于宝石墙前 </summary>
        public static FairySpawnCondition GemWall =
            new(() => GetSpawnCondition(DescriptionID.GemWall).Value
                , attempt => attempt.CircleRadius >= 9 * 16);

    }

    public record FairySpawn_Wall(int ItemType)
        : FairySpawnCondition(() => FairySpawnCondition_Wall.Format(ContentSamples.ItemsByType[ItemType])
        , fairyDatas => true)
    {

    }
}
