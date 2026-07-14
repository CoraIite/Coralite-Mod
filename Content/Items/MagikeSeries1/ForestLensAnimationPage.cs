using Coralite.Content.CoraliteNotes;
using Coralite.Content.UI.Animations;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class ForestLensAnimationPage : AnimationPage
    {
        public override UIAnimation GetAnimation()
        {
            UIAnimation anmi = new UIAnimation();

            anmi.AddKeyFrame();//将第0帧设置为关键帧

            var tiles = anmi.CreateTilesCurrent(new Vector2(0, 40), 8, [
                (TileID.Grass,AnimationBlockFrame.LeftTip,new Point(-3,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(-2,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(-1,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(0,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(1,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(2,0)),
               (TileID.Grass,AnimationBlockFrame.RightTip,new Point(3,0)),
               ]);

            anmi.Init_LetTimePass(60 * 2)//过去6秒
                .TilesSetEnd(tiles)//让物块消失
                .Init_EndTime();//结束动画

            return anmi;
        }
    }
}
