using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    [AutoLoadTexture(Path = AssetDirectory.FairyUI)]
    public class FairySlot : UIElement
    {
        public static ATex CommonCorner { get; set; }
        public static ATex UncommonCorner { get; set; }
        public static ATex RareCorner { get; set; }
        public static ATex DoubleRareCorner { get; set; }

        public static ATex FairySlotBorder { get; set; }
        public static ATex FairySlotHoverBorder { get; set; }
        public static ATex FairySlotBackground { get; set; }

        private int _cornerSize = 16;
        private int _barSize = 10;
        public Color BorderColor = Color.White;
        public Color BackgroundColor = Color.White;//new Color(63, 82, 151) * 0.85f;

        public const int XCount = 10;
        public const int YCount = 5;

        private Fairy _fairy;
        private Item _fairyItem;
        /// <summary>
        /// 自身在UIGrid里的索引
        /// </summary>
        public readonly int index;

        public FairySlot(int fairyType, int index)
        {
            this.index = index;
            //offset = 60;

            _fairy = FairyLoader.GetFairy(fairyType).NewInstance();
            _fairyItem = new Item(_fairy.ItemType);
        }

        public void SetSize(UIElement parent)
        {
            //Width.Set((FairyEncyclopedia.PanelWidth / XCount) - 6, 0);
            //Height.Set((FairyEncyclopedia.PanelHeight / YCount) - 6, 0);

            Width.Set(-5, 1f / XCount);
            Height.Set(-5, 1f / YCount);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateFairy();

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            bool hovering = IsMouseHovering;
            Color borderColor = FairySystem.FairyCaught[_fairy.Type] ? Color.White : Color.LightGray;
            DrawBorder(spriteBatch, FairySlotBackground.Value, borderColor);

            if (hovering)
                DrawBorder(spriteBatch, FairySlotHoverBorder.Value, borderColor);
            else
                DrawBorder(spriteBatch, FairySlotBorder.Value, borderColor);

            //绘制仙灵本体
            Color c = FairySystem.FairyCaught[_fairy.Type] ? Color.White : Color.Black;

            var style = GetDimensions();
            _fairy.Center = style.Center();
            if (IsMouseHovering)
            {
                _fairy.scale = 1.4f;
                _fairy.Center += new Vector2(0, style.Height / 8 * MathF.Sin(Main.GlobalTimeWrappedHourly * 3));
            }
            else
                _fairy.scale = 1f;

            if (hovering)
            {
                if (FairySystem.FairyCaught[_fairy.Type])
                {
                    Main.HoverItem = _fairyItem.Clone();
                    (Main.HoverItem.ModItem as BaseFairyItem).DontShowIV = true;
                    Main.hoverItemName = "CoraliteFairyEncyclopedia";
                }
                else
                    Main.instance.MouseText(FairySystem.UncaughtMouseText.Value);
            }

            Color c2 = FairySystem.GetRarityColor(_fairy.Rarity);
            string text = Enum.IsDefined(_fairy.Rarity) ? Enum.GetName(_fairy.Rarity) : "SP";

            Utils.DrawBorderString(spriteBatch, text, GetDimensions().Position() + new Vector2(16, 27), c2,
                anchorx: 0f, anchory: 0.5f);


            CalculatedStyle dimensions = GetDimensions();

            DrawCorner(spriteBatch, dimensions);

            _fairy.QuickDraw(Vector2.Zero, c, 0);
        }

        private void DrawCorner(SpriteBatch spriteBatch, CalculatedStyle dimensions)
        {
            int yFrame = IsMouseHovering ? 1 : 0;
            const int Length = 22;

            Texture2D tex = _fairy.Rarity switch
            {
                FairyRarity.U => UncommonCorner.Value,
                FairyRarity.R => RareCorner.Value,
                FairyRarity.RR or FairyRarity.RRR => DoubleRareCorner.Value,
                _ => CommonCorner.Value
            };

            tex.QuickCenteredDraw(spriteBatch, new Rectangle(0, yFrame, 2, 2)
                , dimensions.Position() + new Vector2(Length, Length), Color.White * 0.6f);
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(1, yFrame, 2, 2)
                , dimensions.Position() + new Vector2(dimensions.Width - Length, dimensions.Height - Length), Color.White * 0.6f);
        }

        private void DrawBorder(SpriteBatch spriteBatch, Texture2D texture, Color color)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new((int)dimensions.X, (int)dimensions.Y);
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
            FairyEncyclopedia e = UILoader.GetUIState<FairyEncyclopedia>();

            e.ShowFairy = true;
            FairyEncyclopedia.ShowFairyID = _fairy.Type;
            e.Recalculate();
            e.CircleShow.Reset();

            base.LeftClick(evt);
        }

        public void UpdateFairy()
        {
            if (_fairy == null)
                return;

            _fairy.UpdateFrameY(IsMouseHovering ? 7 : 10);
        }
    }
}
