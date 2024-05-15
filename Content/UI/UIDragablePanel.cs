﻿using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI
{
    public class UIDragablePanel : UIPanel
    {
        private static Asset<Texture2D> dragTexture;
        private Vector2 offset;
        private bool dragable;
        private bool dragging;
        private bool resizeableX;
        private bool resizeableY;
        private bool Resizeable => resizeableX || resizeableY;
        private bool resizeing;

        public event Action OnResizing;

        //private int minX, minY, maxX, maxY;
        private List<UIElement> additionalDragTargets;

        // TODO, move panel back in if offscreen? prevent drag off screen?
        public UIDragablePanel(bool dragable = true, bool resizeableX = false, bool resizeableY = false)
        {
            this.dragable = dragable;
            this.resizeableX = resizeableX;
            this.resizeableY = resizeableY;
            dragTexture ??= ModContent.Request<Texture2D>("Terraria/Images/UI/PanelBorder");
            additionalDragTargets = new List<UIElement>();
        }

        public void AddDragTarget(UIElement element)
        {
            additionalDragTargets.Add(element);
        }

        //public void SetMinMaxWidth(int min, int max)
        //{
        //	this.minX = min;
        //	this.maxX = max;
        //}

        //public void SetMinMaxHeight(int min, int max)
        //{
        //	this.minY = min;
        //	this.maxY = max;
        //}

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            DragStart(evt);
            base.LeftMouseDown(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            DragEnd(evt);
            base.LeftMouseUp(evt);
        }

        private void DragStart(UIMouseEvent evt)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (evt.Target == this || additionalDragTargets.Contains(evt.Target))
            {
                //直接判断位置决定是否可以缩放大小
                if (Resizeable && new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 12), (int)(innerDimensions.Y + innerDimensions.Height - 12), 12 + 6, 12 + 6).Contains(evt.MousePosition.ToPoint()))
                {
                    offset = new Vector2(evt.MousePosition.X - innerDimensions.X - innerDimensions.Width - 6, evt.MousePosition.Y - innerDimensions.Y - innerDimensions.Height - 6);
                    resizeing = true;
                }
                else if (dragable)
                {
                    offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
                    dragging = true;
                }
            }
        }

        private void DragEnd(UIMouseEvent evt)
        {
            if (evt.Target == this || additionalDragTargets.Contains(evt.Target))
            {
                dragging = false;
                resizeing = false;
            }
            //if (this == RecipeBrowserUI.instance.favoritePanel) {
            //	RecipeBrowserClientConfig config = ModContent.GetInstance<RecipeBrowserClientConfig>();
            //	config.FavoritedRecipePanelPosition = new Vector2(Left.Pixels, Top.Pixels);
            //	RecipeBrowserClientConfig.SaveConfig();
            //}
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetOuterDimensions();
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.LocalPlayer.cursorItemIconEnabled = false;
                Main.ItemIconCacheUpdate(0);
            }

            if (dragging)
            {
                Left.Set(Main.MouseScreen.X - offset.X, 0f);
                Top.Set(Main.MouseScreen.Y - offset.Y, 0f);
                Recalculate();
            }
            else
            {
                if (Parent != null && !dimensions.ToRectangle().Intersects(Parent.GetDimensions().ToRectangle()))
                {
                    var parentSpace = Parent.GetDimensions().ToRectangle();
                    Left.Pixels = Utils.Clamp(Left.Pixels, Width.Pixels - parentSpace.Right, 0); // TODO: Adjust automatically for Left.Percent (measure from left or right edge)
                    Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                    Recalculate();
                }
            }
            if (resizeing)
            {
                if (resizeableX)
                {
                    //Width.Pixels = Utils.Clamp(Main.MouseScreen.X - dimensions.X - offset.X, minX, maxX);
                    Width.Pixels = Main.MouseScreen.X - dimensions.X - offset.X;
                }

                if (resizeableY)
                {
                    //Height.Pixels = Utils.Clamp(Main.MouseScreen.Y - dimensions.Y - offset.Y, minY, maxY);
                    Height.Pixels = Main.MouseScreen.Y - dimensions.Y - offset.Y;
                }

                Recalculate();
                OnResizing?.Invoke();
            }

            base.DrawSelf(spriteBatch);

            if (Resizeable)
                DrawDragAnchor(spriteBatch, TextureAssets.Cursors[16].Value /*dragTexture.Value*/, BorderColor);
        }

        private void DrawDragAnchor(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = GetDimensions();

            //	Rectangle hitbox = GetInnerDimensions().ToRectangle();
            //	Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);

            Point point = new Point((int)(dimensions.X + dimensions.Width - 12), (int)(dimensions.Y + dimensions.Height - 12));
            var source = texture.Frame();//new Rectangle(12 + 4, 12 + 4, 12, 12);

            //spriteBatch.Draw(texture, new Rectangle(point.X - 2, point.Y - 2, 12 - 2, 12 - 2), source, color);
            //spriteBatch.Draw(texture, new Rectangle(point.X - 4, point.Y - 4, 12 - 4, 12 - 4), source, color);
            spriteBatch.Draw(texture, new Rectangle(point.X - 6, point.Y - 6, 12, 12), source, color);
            texture = TextureAssets.Cursors[15].Value;
            source = texture.Frame();
            spriteBatch.Draw(texture, new Rectangle(point.X - 6, point.Y - 6, 12, 12), source, Color.White);
        }
    }
}