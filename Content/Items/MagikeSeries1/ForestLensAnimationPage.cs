using Coralite.Content.CoraliteNotes;
using Coralite.Content.Items.Magike.Lens.BiomeLens;
using Coralite.Content.UI.Animations;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class ForestLensAnimationPage : AnimationPage
    {
        public static LocalizedText ForestLens { get; private set; }
        public static LocalizedText ForestLensDescription1 { get; private set; }
        public static LocalizedText ForestLensDescription2 { get; private set; }

        public override void InitOthers()
        {
            ForestLens = this.GetLocalization(nameof(ForestLens));
            ForestLensDescription1 = this.GetLocalization(nameof(ForestLensDescription1));
            ForestLensDescription2 = this.GetLocalization(nameof(ForestLensDescription2));
        }

        public override UIAnimation GetAnimation()
        {
            UIAnimation anmi = new UIAnimation();

            anmi.AddKeyFrame();//将第0帧设置为关键帧

            #region 阶段1：简单介绍森林透镜

          var LensItem = anmi.CreateItem<ForestLens>(new Vector2(0, 40));

            //描述1
            var d1 = anmi.CreateText(ForestLensDescription1, new Vector2(-120, -100), 300)
                .SetLineColor(Color.LightGreen)
                //.SetTextColor(Color.LightGreen)
                .SetPointerSmoother(Coralite.Instance.BezierEaseSmoother);
            d1.SetPosSmoother(Coralite.Instance.BezierEaseSmoother);

            anmi.LetTimePass(d1.FadeTime)
                .TextAddPointerMove(d1, new Vector2(-16, 20), 0)//设置文字的初始指向点

                .LetTimePass(60 * 4)
                .ComponentSetEnd(d1);

            //描述2
            var d2 = anmi.CreateText(ForestLensDescription2, new Vector2(120, -100), 300)
                .SetLineColor(Color.LightGreen)
                //.SetTextColor(Color.LightGreen)
                .SetPointerSmoother(Coralite.Instance.BezierEaseSmoother);
            d2.SetPosSmoother(Coralite.Instance.BezierEaseSmoother);

            anmi.LetTimePass(d2.FadeTime)
                .TextAddPointerMove(d2, new Vector2(16, 20), 0)//设置文字的初始指向点

                .LetTimePass(60 * 4)
                .ComponentSetEnd(d2)
                .ComponentSetEnd(LensItem);


            #endregion
            //Vector2 TileCenter = new Vector2(-8, 40);

            //var grassWalls = anmi.CreateWalls(TileCenter, 6, [
            //    (WallID.Grass,AnimationBlockFrame.DownLeftCorner,new Point(0,-1)),
            //    (WallID.Grass,AnimationBlockFrame.LeftSide,new Point(0,-2)),
            //    (WallID.Grass,AnimationBlockFrame.TopLeftCorner,new Point(0,-3)),

            //    (WallID.Grass,AnimationBlockFrame.DownRightCorner,new Point(1,-1)),
            //    (WallID.Grass,AnimationBlockFrame.RightSide,new Point(1,-2)),
            //    (WallID.Grass,AnimationBlockFrame.TopRightCorner,new Point(1,-3)),
            //    ]);

            //var tiles = anmi.CreateTiles(TileCenter, 6, [
            //    //(TileID.Grass,AnimationBlockFrame.LeftTip,new Point(-3,0)),
            //   (TileID.Grass,AnimationBlockFrame.LeftTip,new Point(-2,0)),
            //   (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(-1,0)),
            //   (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(0,0)),
            //   (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(1,0)),
            //   (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(2,0)),
            //   (TileID.Grass,AnimationBlockFrame.RightTip,new Point(3,0)),
            //   ]);

            //var forestLensTile = anmi.CreateTexture(AssetDirectory.NoteMagikeS1 + "ForestLensTile", TileCenter + new Vector2(8, -8 + 2))
            //    .SetOrigin(new Vector2(0.5f, 1))
            //    .SetHoverItemType<ForestLens>();

            //var forestLensTop = anmi.CreateTexture(AssetDirectory.MagikeLensTiles + "ForestLensTile_Glistent", TileCenter + new Vector2(8, -8 - 16 - 8))
            //    .SetPosOffsetEase(timer => new Vector2(0, MathF.Sin(timer * 0.05f) * 4))
            //    .SetHoverItemType<ForestLens>();


            //var text = anmi.CreateText(ForestLens, new Vector2(-200, -80), 400)
            //    //.SetPointerPos(new Vector2(-16, 10))
            //    .SetLineColor(Color.LightGreen)
            //    .SetPointerSmoother(Coralite.Instance.BezierEaseSmoother);
            //text.SetColor(Color.ForestGreen)
            //    .SetPosSmoother(Coralite.Instance.BezierEaseSmoother);

            //anmi.LetTimePass(text.FadeTime)//缓动时间
            //    .TextAddPointerMove(text, new Vector2(-16, 10), 0)//设置文字的初始指向点
            //    .AddKeyFrame()

            //    .LetTimePass(60 * 2)//过去2秒
            //    .AddKeyFrame()
            //    .TextAddPointerMove(text, new Vector2(0, -20), 20)
            //    .ComponentAddPosMove(text, new Vector2(200, -40),20)//文字本体和指向点的移动

            //    .LetTimePass(60 * 2)
            //    .AddKeyFrame()
            //    .TextAddPointerMove(text, new Vector2(16, 10), 20)
            //    .ComponentAddPosMove(text, new Vector2(400, 40), 20)//文字本体和指向点的移动

            //    .LetTimePass(60 * 2)
            //    .ComponentSetEnd(text)//让文字消失
            //    .ComponentSetEnd(forestLensTile)//让森林透镜消失
            //    .ComponentSetEnd(forestLensTop)//

            //    .TilesSetEnd(tiles)//让物块消失
            //    .WallsSetEnd(grassWalls)//让墙壁消失
            anmi.AddKeyFrame()
                .EndTime();//结束动画

            return anmi;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch,ForestLens,Color.LightGreen);
        }
    }
}
