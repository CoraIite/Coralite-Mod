using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Coralite.Content.WorldGeneration
{
    public partial class CoraliteWorld
    {
        public static LocalizedText DigTheShimmer { get; set; }

        public static void GenDigShimmer(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = DigTheShimmer.Value;

            GenVars.shimmerPosition = new ReLogic.Utilities.Vector2D(Main.maxTilesX / 2, 150);

            Point origin = new Point(Main.maxTilesX / 2, 100);

            Modifiers.Flip actions = new Modifiers.Flip(false, true);
            Actions.Clear clear = new Actions.Clear();

            WorldUtils.Gen(
                origin,  //中心点
                new Shapes.Slime(24, 1, 0.8f),   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    actions,//与矩形相交，裁成半圆环
                    clear,    //清除形状内所有物块
                    new Actions.SetTile(TileID.Stone)));   //放微光

            WorldUtils.Gen(
                origin,  //中心点
                new Shapes.Slime(20, 1, 0.8f),   //形状：圆
                Actions.Chain(  //如果要添加多个效果得使用这个chain
                    actions,//与矩形相交，裁成半圆环
                    clear,    //清除形状内所有物块
                    new Actions.SetLiquid(LiquidID.Shimmer)));   //放微光
        }
    }
}
