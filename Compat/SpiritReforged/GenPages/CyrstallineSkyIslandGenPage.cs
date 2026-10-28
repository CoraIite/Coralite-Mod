using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using SpiritReforged.Common.WorldGeneration.GenConfiguration;

namespace Coralite.Compat.SpiritReforged.GenPages
{
    [ExtendsFromMod("SpiritReforged")]
    [JITWhenModsEnabled("SpiritReforged")]
    public class CyrstallineSkyIslandGenPage : IGenerationPage
    {
        PageInfo IGenerationPage.Info => new PageInfo("CrystallineSkyIsland", ModContent.Request<Texture2D>(AssetDirectory.Backgrounds+ "CrystallineSkyIslandMap"), ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "CrystallineSkyIslandMap"));
        Mod IGenerationPage.Mod => Coralite.Instance;
    }

    /// <summary>
    /// 蕴魔空岛的生成控制类
    /// </summary>
    public class CyrstallineSkyIslandConfiguration
    {
        /// <summary>
        /// 额外的岛屿数量
        /// </summary>
        [BindToGenConfig(0, 6, 1, true)]
        public static int ExtraIslands { get; set; } = 0;

        //主岛大小
        [BindToGenConfig(60, 120, 1, true)]
        public static int MainBaseX { get; set; } = 60;
        [BindToGenConfig(60, 80, 1, true)]
        public static int MainBaseY { get; set; } = 60;

        [BindToGenConfig(18, 18 * 2, 1, true)]
        public static int MainPlusX { get; set; } = 18;
        [BindToGenConfig(22, 32, 1, true)]
        public static int MainPlusY { get; set; } = 22;

        /// <summary>
        /// 主岛的额外隧道
        /// </summary>
        [BindToGenConfig(0, 3, 1, true)]
        public static int ExtraMainTunnels { get; set; } = 0;

        /// <summary>
        /// 主岛的额外矿物
        /// </summary>
        [BindToGenConfig(0, 8, 1, true)]
        public static int ExtraMainOres { get; set; } = 0;
    }
}
