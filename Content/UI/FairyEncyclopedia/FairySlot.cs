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
        private Asset<Texture2D> _backgroundTexture;
        public Color BorderColor = Color.Black;
        public Color BackgroundColor = new Color(63, 82, 151) * 0.7f;

        public const int XCount = 8;
        public const int YCount = 5;

        private Fairy _fairy;

        public FairySlot(int fairyType)
        {
            _borderTexture ??= Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
            _backgroundTexture ??= Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

            _fairy=FairyLoader.GetFairy(fairyType).NewInstance();
        }

        public void SetSize()
        {
            Width.Set(Parent.Width.Pixels / XCount - 6, 0);
            Height.Set(Parent.Height.Pixels / YCount - 6, 0);
        }

        /// <summary>
        /// 自身在UIGrid里的索引
        /// </summary>
        public readonly int index;
        private float alpha;
        private float offset;

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHovering)//鼠标在上面，开始更新自身的样子
            {
                UpdateFairy();
            }
            else
            {
                
            }
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_backgroundTexture != null)
                DrawPanel(spriteBatch, offset, _backgroundTexture.Value, BackgroundColor * alpha);

            if (_borderTexture != null)
                DrawPanel(spriteBatch, offset, _borderTexture.Value, BorderColor * alpha);

            var style = GetDimensions();
            Color c = FairySystem.FairyCaught[_fairy.Type]?Color.White:Color.Black;

            _fairy.position = style.Center() + (IsMouseHovering ? new Vector2(0, style.Height / 10 * MathF.Sin(Main.GlobalTimeWrappedHourly * 3)) : Vector2.Zero);
            _fairy.QuickDraw(c, 0);

        }

        private void DrawPanel(SpriteBatch spriteBatch,float offset, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y + (int)offset);
            Point point2 = new Point(point.X + (int)dimensions.Width - _cornerSize, point.Y + (int)dimensions.Height - _cornerSize);
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
            if (++_fairy.frameCounter > 6)
            {
                _fairy.frameCounter = 0;
                if (++_fairy.frame.Y > _fairy.VerticalFrames)
                    _fairy.frame.Y = 0;
            }
        }
    }
}
