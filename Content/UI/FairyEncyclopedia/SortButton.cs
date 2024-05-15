using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class SortButton(FairyEncyclopedia.SortStyle sortStyle, Func<LocalizedText> description) : UIPanel
    {
        public readonly FairyEncyclopedia.SortStyle sortStyle = sortStyle;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();

            LocalizedText text = description();
            if (text != null)
                Utils.DrawBorderString(spriteBatch, text.Value, dimensions.Center() + new Vector2(0, 4),
                    FairyEncyclopedia.CurrentSortStyle == sortStyle ? Color.White : Color.White * 0.5f, anchorx: 0.5f, anchory: 0.5f);

            if (IsMouseHovering)
            {
                Main.instance.MouseText(FairySystem.SortButtonMouseText.Value);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            UILoader.GetUIState<FairyEncyclopedia>().Sort(sortStyle);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            UILoader.GetUIState<FairyEncyclopedia>().Sort(FairyEncyclopedia.SortStyle.ByType);
        }
    }
}
