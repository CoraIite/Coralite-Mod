using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Coralite.Core.Systems.FairyCatcherSystem.FairySystem;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public record FairySpawnCondition(Func<string> Descripetion, Func<FairyAttempt, bool> Predicate)
    {
        /// <summary> 森林环境 </summary>
        public static FairySpawnCondition ZoneForest =
            new(() => Condition.InShoppingZoneForest.Description.Value
                , attempt => attempt.Player.ZoneForest);

        /// <summary> 洞穴环境 </summary>
        public static FairySpawnCondition ZoneRockLayerHeight =
            new(() => Condition.InRockLayerHeight.Description.Value
                , attempt => attempt.Player.ZoneRockLayerHeight);

        /// <summary> 海洋环境 </summary>
        public static FairySpawnCondition ZoneBeach =
            new(() => Condition.InBeach.Description.Value
                , attempt => attempt.Player.ZoneBeach);

        /// <summary> 沙漠环境 </summary>
        public static FairySpawnCondition ZoneDesert =
            new(() => Condition.InDesert.Description.Value
                , attempt => attempt.Player.ZoneDesert);

        /// <summary> 雪原环境 </summary>
        public static FairySpawnCondition ZoneSnow =
            new(() => Condition.InSnow.Description.Value
                , attempt => attempt.Player.ZoneSnow);

        /// <summary> 猩红环境 </summary>
        public static FairySpawnCondition ZoneCrimson =
            new(() => Condition.InCrimson.Description.Value
                , attempt => attempt.Player.ZoneCrimson);

        /// <summary> 腐化环境 </summary>
        public static FairySpawnCondition ZoneCorrupt =
            new(() => Condition.InCorrupt.Description.Value
                , attempt => attempt.Player.ZoneCorrupt);

        /// <summary> 地狱环境 </summary>
        public static FairySpawnCondition ZoneHell =
            new(() => Condition.InUnderworldHeight.Description.Value
                , attempt => attempt.Player.ZoneUnderworldHeight);

        /// <summary> 地下丛林环境 </summary>
        public static FairySpawnCondition ZoneUndergroundJungle =
            new(() => GetSpawnCondition(DescriptionID.ZoneUndergroundJungle).Value
                , attempt => attempt.Player.ZoneRockLayerHeight && attempt.Player.ZoneJungle);


        /// <summary> 仙灵捕捉环半径大于9 </summary>
        public static FairySpawnCondition CircleR_9 =
            new(() => GetSpawnCondition(DescriptionID.CircleR_9).Value
                , attempt => attempt.CircleRadius >= 9 * 16);


        /// <summary> 处于宝石墙前 </summary>
        public static FairySpawnCondition GemWall =
            new(() => GetSpawnCondition(DescriptionID.GemWall).Value
                , attempt => true);
        /// <summary> 处于泥土墙前 </summary>
        public static FairySpawnCondition DirtWall =
            new(() => GetSpawnCondition(DescriptionID.DirtWall).Value
                , attempt => true);

        /// <summary> 击败史莱姆ang </summary>
        public static FairySpawnCondition DownedSlimeKing =
            new(() => Condition.DownedKingSlime.Description.Value
                , attempt => NPC.downedSlimeKing);

        /// <summary> 白天 </summary>
        public static FairySpawnCondition DayTime =
            new(() => Condition.TimeDay.Description.Value
                , attempt => Main.dayTime);

    }

    public record FairySpawn_Wall(int ItemType)
        : FairySpawnCondition(() => FairySpawnCondition_Wall.Format(ContentSamples.ItemsByType[ItemType].Name)
        , fairyDatas => true)
    {

    }

    public record FairySpawn_Circle(int ItemType)
        : FairySpawnCondition(() => FairySpawnCondition_Circle.Format(ItemType, ContentSamples.ItemsByType[ItemType].Name)
        , fairyDatas => false)
    {

    }
}
