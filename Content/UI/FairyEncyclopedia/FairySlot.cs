using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class FairySlot : UIElement
    {
        private int _cornerSize = 12;
        private int _barSize = 4;
        private Asset<Texture2D> _borderTexture;
        private Asset<Texture2D> _borderHoverTexture;
        private Asset<Texture2D> _backgroundTexture;
        public Color BorderColor = Color.Black;
        public Color BackgroundColor = new Color(63, 82, 151) * 0.85f;

        public const int XCount = 12;
        public const int YCount = 6;

        private Fairy _fairy;
        private Item _fairyItem;
        /// <summary>
        /// 自身在UIGrid里的索引
        /// </summary>
        public readonly int index;
        private float alpha;
        private float offset;

        public FairySlot(int fairyType, int index)
        {
            this.index = index;
            offset = 60;
            _borderTexture ??= FairySystem.FairySlotBorder;  //Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
            _borderHoverTexture ??= FairySystem.FairySlotHoverBorder;
            _backgroundTexture ??= FairySystem.FairySlotBackground;//Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

            _fairy = FairyLoader.GetFairy(fairyType).NewInstance();
            _fairyItem = new Item(_fairy.ItemType);
        }

        public void SetSize(UIElement parent)
        {
            Width.Set(parent.Width.Pixels / XCount - 6, 0);
            Height.Set(parent.Height.Pixels / YCount - 6, 0);
        }

        public override void Update(GameTime gameTime)
        {
            if (alpha < 1)//滑动效果
            {
                if (FairyEncyclopedia.Timer >= index)
                {
                    alpha = FairyEncyclopedia.Timer - index;
                    offset = 60 - alpha * 60;
                }

                if (alpha > 1)
                {
                    alpha = 1;
                    offset = 0;
                    Recalculate();
                }
            }

            UpdateFairy();

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            bool hovering = IsMouseHovering;
            if (_backgroundTexture != null)
                DrawBorder(spriteBatch, offset, _backgroundTexture.Value, Color.White * alpha);

            if (_borderTexture != null)
            {
                if (hovering)
                    DrawBorder(spriteBatch, offset, _borderHoverTexture.Value, Color.White * alpha);
                else
                    DrawBorder(spriteBatch, offset, _borderTexture.Value, Color.White * alpha);
            }

            //绘制仙灵本体
            Color c = FairySystem.FairyCaught[_fairy.Type] ? Color.White : Color.Black;
            c *= alpha;

            if (hovering)
            {
                if (FairySystem.FairyCaught[_fairy.Type])
                {
                    Main.HoverItem = _fairyItem.Clone();
                    Main.hoverItemName = "CoraliteFairyEncyclopedia";
                }
                else
                    Main.instance.MouseText(FairySystem.UncaughtMouseText.Value);
            }

            _fairy.QuickDraw(c, 0);
        }

        private void DrawBorder(SpriteBatch spriteBatch, float offset, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new((int)dimensions.X, (int)dimensions.Y + (int)offset);
            Point point2 = new(point.X + (int)dimensions.Width - _cornerSize, point.Y + (int)dimensions.Height - _cornerSize);
            int width = point2.X - point.X - _cornerSize;
            int height = point2.Y - point.Y - _cornerSize;
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, _cornerSize, _cornerSize), new Rectangle(0, 0, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, 0, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(0, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y, width, _cornerSize), new Rectangle(_cornerSize, 0, _barSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point2.Y, width, _cornerSize), new Rectangle(_cornerSize, _cornerSize + _barSize, _barSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(0, _cornerSize, _cornerSize, _barSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(_cornerSize + _barSize, _cornerSize, _cornerSize, _barSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y + _cornerSize, width, height), new Rectangle(_cornerSize, _cornerSize, _barSize, _barSize), color);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            //切换到个体面板

            base.LeftClick(evt);
        }



        public void UpdateFairy()
        {
            if (_fairy == null)
                return;
            if (++_fairy.frameCounter > 7)
            {
                _fairy.frameCounter = 0;
                if (++_fairy.frame.Y >= _fairy.VerticalFrames)
                    _fairy.frame.Y = 0;
            }

            var style = GetDimensions();
            _fairy.Center = style.Center() + Main.screenPosition;
            if (IsMouseHovering)
            {
                _fairy.scale = 1.4f;
                _fairy.Center += new Vector2(0, style.Height / 8 * MathF.Sin(Main.GlobalTimeWrappedHourly * 3));
            }
            else
            {
                _fairy.scale = 1f;
            }
        }
    }
}
