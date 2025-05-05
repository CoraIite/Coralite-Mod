using Coralite.Helpers;
using System.Collections.Generic;
using Terraria.ID;

namespace Coralite.Core
{
    public static class CoraliteSets
    {
        /// <summary>
        /// 爆破弹幕
        /// </summary>
        //public static bool[] ProjectileExplosible;
        [ReinitializeDuringResizeArrays]
        public static class Items
        {
            /// <summary>
            /// 标记物品是魔鸟
            /// </summary>
            public static bool[] IsMabird = ItemID.Sets.Factory.CreateCoraliteSet(nameof(IsMabird))
                .Description("标记物品是魔鸟")
                .RegisterBoolSet(false);

            /// <summary>
            /// 标记物品是仙灵
            /// </summary>
            public static bool[] IsFairy = ItemID.Sets.Factory.CreateCoraliteSet(nameof(IsFairy))
                .Description("标记物品是仙灵")
                .RegisterBoolSet(false);
        }

        [ReinitializeDuringResizeArrays]
        public static class Tiles
        {
            /// <summary>
            /// 粘性物块
            /// </summary>
            public static bool[] Sticky = TileID.Sets.Factory.CreateCoraliteSet(nameof(Sticky))
                .Description("让物块具有蜘蛛网的粘性")
                .RegisterBoolSet(false, TileID.Cobweb);

            /// <summary>
            /// 会被判定为影子物块，在这上面才能生成影之城的敌怪
            /// </summary>
            public static bool[] ShadowCastle = TileID.Sets.Factory.CreateCoraliteSet(nameof(ShadowCastle))
                .Description("使物块作为敌怪生成的环境判定：影之城")
                .RegisterBoolSet(false);

            /// <summary>
            /// 特殊绘制
            /// </summary>
            public static bool[] SpecialDraw = TileID.Sets.Factory.CreateCoraliteSet(nameof(SpecialDraw))
                .Description("让物块使用特殊的绘制方式")
                .RegisterBoolSet(false);

            /// <summary>
            /// 物块是否是特殊苔藓
            /// </summary>
            public static bool[] SpecialMoss = TileID.Sets.Factory.CreateCoraliteSet(nameof(SpecialMoss))
                .Description("特殊苔藓，挖掉后会放置苔藓下的物块")
                .RegisterBoolSet(false);
        }

        [ReinitializeDuringResizeArrays]
        public static class Walls
        {
            /// <summary>
            /// 会被判定为影子墙壁，在墙前才判定为影之城环境
            /// </summary>
            public static bool[] ShadowCastle = WallID.Sets.Factory.CreateCoraliteSet(nameof(ShadowCastle))
                .Description("使墙壁作为环境判定：影之城")
                .RegisterBoolSet(false);
        }
    }

    public class CoraliteSetsSystem : ModSystem
    {
        /// <summary>
        /// 魔能物块的摆放方式
        /// </summary>
        public static Dictionary<int, MagikeTileType> MagikeTileTypes = new Dictionary<int, MagikeTileType>();

        public enum MagikeTileType
        {
            /// <summary>
            /// 普通四向放置，横向放置时会将宽高交换
            /// </summary>
            FourWayNormal,
            /// <summary>
            /// 无反转的四向放置，所有方向宽高相同
            /// </summary>
            FourWayNoFilp,
            /// <summary>
            /// 不支持四向放置
            /// </summary>
            None
        }
    }
}
