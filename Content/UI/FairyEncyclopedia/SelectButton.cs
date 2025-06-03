using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    public class SelectButton(Asset<Texture2D> texture, FairyRarity? rarity) : UIImageButton(texture)
    {
        private readonly FairyRarity? _rarity = rarity;

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            UILoader.GetUIState<FairyEncyclopedia>().Select(_rarity);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            UILoader.GetUIState<FairyEncyclopedia>().Select(null);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (_rarity.HasValue)
            {
                Color c = FairySystem.GetRarityColor(_rarity.Value);
                string text = Enum.IsDefined(_rarity.Value) ? Enum.GetName(_rarity.Value) : "SP";
                if (FairyEncyclopedia.selectType.HasValue && FairyEncyclopedia.selectType.Value != _rarity)
                    c *= 0.5f;

                Utils.DrawBorderString(spriteBatch, text, GetDimensions().Center() + new Vector2(0, 4), c,
                    anchorx: 0.5f, anchory: 0.5f);
            }
            else
            {
                Utils.DrawBorderString(spriteBatch, "All", GetDimensions().Center() + new Vector2(0, 4), Color.White,
                    anchorx: 0.5f, anchory: 0.5f);
            }

            if (IsMouseHovering)
            {
                Main.instance.MouseText(FairySystem.SelectButtonMouseText.Value);
            }
        }
    }
}
