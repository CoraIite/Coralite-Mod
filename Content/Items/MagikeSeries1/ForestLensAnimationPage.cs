using Coralite.Content.CoraliteNotes;
using Coralite.Content.Items.Magike.Lens.BiomeLens;
using Coralite.Content.Items.Magike.Refractors;
using Coralite.Content.UI.Animations;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class ForestLensAnimationPage : AnimationPage
    {
        public static LocalizedText ForestLens { get; private set; }
        public static LocalizedText ForestLensDescription1 { get; private set; }
        public static LocalizedText ForestLensDescription2 { get; private set; }
        public static LocalizedText ForestLensDescription3 { get; private set; }
        public static LocalizedText ForestLensDescription4 { get; private set; }
        public static LocalizedText ForestLensDescription5 { get; private set; }
        public static LocalizedText ForestLensDescription6 { get; private set; }
        public static LocalizedText ForestLensDescription7 { get; private set; }

        public override void InitOthers()
        {
            ForestLens = this.GetLocalization(nameof(ForestLens));
            ForestLensDescription1 = this.GetLocalization(nameof(ForestLensDescription1));
            ForestLensDescription2 = this.GetLocalization(nameof(ForestLensDescription2));
            ForestLensDescription3 = this.GetLocalization(nameof(ForestLensDescription3));
            ForestLensDescription4 = this.GetLocalization(nameof(ForestLensDescription4));
            ForestLensDescription5 = this.GetLocalization(nameof(ForestLensDescription5));
            ForestLensDescription6 = this.GetLocalization(nameof(ForestLensDescription6));
            ForestLensDescription7 = this.GetLocalization(nameof(ForestLensDescription7));
        }

        public override UIAnimation GetAnimation()
        {
            UIAnimation anmi = new UIAnimation();

            anmi.AddKeyFrame(2);//将第0帧设置为关键帧

            #region 阶段1：简单介绍森林透镜

            var LensItem = anmi.CreateItem<ForestLens>(new Vector2(0, 40));

            //描述1
            var d1 = anmi.CreateText(ForestLensDescription1, new Vector2(-120, -100), 300)
                .SetLineColor(Color.LightGreen)
                .SetPointerSmoother(Coralite.Instance.BezierEaseSmoother);
            d1.SetPosSmoother(Coralite.Instance.BezierEaseSmoother);

            anmi.LetTimePass(d1.FadeTime)
                .TextAddPointerMove(d1, new Vector2(-16, 20), 0)//设置文字的初始指向点

                .LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(d1);

            //描述2
            var d2 = anmi.CreateText(ForestLensDescription2, new Vector2(120, -100), 300)
                .SetLineColor(Color.LightGreen)
                .SetPointerSmoother(Coralite.Instance.BezierEaseSmoother);
            d2.SetPosSmoother(Coralite.Instance.BezierEaseSmoother);

            anmi.LetTimePass(d2.FadeTime)
                .TextAddPointerMove(d2, new Vector2(16, 20), 0)//设置文字的初始指向点

                .LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(d2)
                .ComponentSetEnd(LensItem);

            #endregion

            #region 阶段2：森林透镜环境要求

            anmi.AddKeyFrame()
                .LetTimePass(20);

            var forestIcon = anmi.CreateTexture(AssetDirectory.Vanilla + "UI/Bestiary/Icon_Tags_Shadow", new Vector2(100, 40))
                .SetFrameBox(new Rectangle(0, 0, 16, 5));

            anmi.ComponentAddPosMove(LensItem, new Vector2(-100, 0), 20);

            var d3 = anmi.CreateText(ForestLensDescription3, new Vector2(0, -100), 400)
                .SetLineColor(Color.LightGreen);

            anmi.LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(forestIcon)
                .ComponentSetEnd(LensItem)
                .ComponentSetEnd(d3);


            Vector2 TileCenter = new Vector2(-8, 40);


            var grassWalls = anmi.CreateWalls(TileCenter, 6, [
                (WallID.Grass,AnimationBlockFrame.DownLeftCorner,new Point(0,-1)),
                (WallID.Grass,AnimationBlockFrame.LeftSide,new Point(0,-2)),
                (WallID.Grass,AnimationBlockFrame.TopLeftCorner,new Point(0,-3)),

                (WallID.Grass,AnimationBlockFrame.DownRightCorner,new Point(1,-1)),
                (WallID.Grass,AnimationBlockFrame.RightSide,new Point(1,-2)),
                (WallID.Grass,AnimationBlockFrame.TopRightCorner,new Point(1,-3)),
                ]);

            var tiles = anmi.CreateTiles(TileCenter, 6, [
                //(TileID.Grass,AnimationBlockFrame.LeftTip,new Point(-3,0)),
               (TileID.Grass,AnimationBlockFrame.LeftTip,new Point(-2,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(-1,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(0,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(1,0)),
               (TileID.Grass,AnimationBlockFrame.HorizontalLine,new Point(2,0)),
               (TileID.Grass,AnimationBlockFrame.RightTip,new Point(3,0)),
               ]);

            var forestLensTile = anmi.CreateTexture(AssetDirectory.NoteMagikeS1 + "ForestLensTile", TileCenter + new Vector2(8, -8 + 2))
                .SetOrigin(new Vector2(0.5f, 1))
                .SetHoverItemType<ForestLens>();

            var forestLensTop = anmi.CreateTexture(AssetDirectory.MagikeLensTiles + "ForestLensTile_Glistent", TileCenter + new Vector2(8, -8 - 16 - 8))
                .SetPosOffsetEase(timer => new Vector2(0, MathF.Sin(timer * 0.05f) * 4))
                .SetHoverItemType<ForestLens>();


            var d4 = anmi.CreateText(ForestLensDescription4, new Vector2(-100, -80), 400)
                .SetLineColor(Color.LightGreen);

            anmi.LetTimePass(d4.FadeTime)//缓动时间
                .TextAddPointerMove(d4, new Vector2(-16, 10), 0)//设置文字的初始指向点
                .LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(d4);

            var d5 = anmi.CreateText(ForestLensDescription5, new Vector2(100, -80), 300)
                .SetLineColor(Color.LightGreen);

            anmi.LetTimePass(d5.FadeTime)//缓动时间
                .TextAddPointerMove(d5, new Vector2(16, 10), 0)//设置文字的初始指向点
                .LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(d5);

            var d6 = anmi.CreateText(ForestLensDescription6, new Vector2(100, -80), 320)
                .SetLineColor(Color.LightGreen);

            anmi.LetTimePass(d6.FadeTime)//缓动时间
                .TextAddPointerMove(d6, new Vector2(16, 10), 0)//设置文字的初始指向点
                .LetTimePassWithKeyInside(60 * 4)
                .ComponentSetEnd(d6)

                .ComponentSetEnd(forestLensTile)
                .ComponentSetEnd(forestLensTop)

                .TilesSetEnd(tiles)//让物块消失
                .WallsSetEnd(grassWalls)//让墙壁消失
                .AddKeyFrame(1);


            #endregion

            #region  阶段3：一大堆森林透镜

            TileCenter = new Vector2(0, 80);

            var tiles2 = anmi.CreateTilesArea(TileCenter, 6, TileID.Grass, new Point(27, 1), fadeTimePercent: 0.2f);
            var tiles3 = anmi.CreateTilesArea(TileCenter + new Vector2(0, -16 * 7), 6, TileID.Grass, new Point(27, 1), fadeTimePercent: 0.2f);

            var walls2 = anmi.CreateWallsArea(TileCenter + new Vector2(0, -16 * 3), 6, WallID.Grass, new Point(27, 6), fadeTimePercent: 0);

            UIAnimationComponent[] ForestLenstile =new UIAnimationComponent[24];
            UIAnimationComponent[] ForestLenstop =new UIAnimationComponent[24];

            UIAnimationComponent[] CrystalRefactor =new UIAnimationComponent[6];

            Vector2 topLeft = TileCenter + new Vector2(-8 - 13 * 16, -8 - 6 * 16);

            for (int k = 0; k < 2; k++)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 RCenter = topLeft + new Vector2(8 + 16 * 4, 0);
                    int iWidth = i * 16 * (2 * 4 + 1);//i是一个小单元的数量，宽度是4个森林透镜，2个广角镜
                    int kHeight= k * 16 * 6;//用k区分上下
                    int kHeight2=16+ k * 16 * 4;//用k区分上下

                    RCenter.X += iWidth+ (k == 0 ? -2 : 2);
                    RCenter.Y += kHeight2 + (k == 0 ? -6 : 6);

                    CrystalRefactor[i + k * 3] = anmi.CreateItem<BasicRefractor>(RCenter)
                        .SetRotation(k == 0 ? MathHelper.Pi : 0);

                    for (int j = 0; j < 4; j++)//神秘计算
                    {
                        Vector2 center = topLeft + new Vector2(16, 0);

                        if (j > 1)//2个森林透镜，中间一个广角镜，把中间的这个空出来
                            center.X += 16;

                        center.X += j * 16 * 2;

                        center.X += iWidth;

                        center.Y += kHeight;

                        int whoamI = j + i * 4 + k * 12;

                        ForestLenstile[whoamI] = anmi.CreateTexture(AssetDirectory.NoteMagikeS1 + "ForestLensTile", center + new Vector2((k == 0 ? -2 : 2)))
                            .SetOrigin(new Vector2(0.5f, k == 0 ? 0 : 1))
                            .SetHoverItemType<ForestLens>()
                            .SetRotation(k == 0 ? MathHelper.Pi : 0)
                            .SetDrawLayer(1.5f);

                        ForestLenstop[whoamI] = anmi.CreateTexture(AssetDirectory.MagikeLensTiles + "ForestLensTile_Glistent", center + new Vector2(0, (k == 0 ? 1 : -1) * (16 + 8)))
                            .SetPosOffsetEase(timer => new Vector2(0, MathF.Sin(timer * 0.05f + whoamI * MathHelper.Pi * 0.6f) * 4))
                            .SetHoverItemType<ForestLens>();

                        anmi.LetTimePass(2);
                    }
                }


            var d7 = anmi.CreateText(ForestLensDescription7, new Vector2(0, -90), 400)
                .SetLineColor(Color.LightGreen);

            anmi.LetTimePassWithKeyInside(60 * 4)
                .ComponentsSetEnd(ForestLenstile)
                .ComponentsSetEnd(ForestLenstop)
                .ComponentsSetEnd(CrystalRefactor)
                .ComponentSetEnd(d7)
                .TilesSetEnd(tiles2,0.2f)
                .TilesSetEnd(tiles3, 0.2f)
                .WallsSetEnd(walls2, 0);

            #endregion

            anmi.EndTime();//结束动画

            return anmi;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH2(spriteBatch, ForestLens, Color.LightGreen);
        }
    }
}
