﻿using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class DescriptionPage : KnowledgePage
    {
        public static LocalizedText Description { get; private set; }
        public static LocalizedText HowToUse { get; private set; }

        public override void OnInitialize()
        {
            Description = this.GetLocalization(nameof(Description));
            HowToUse = this.GetLocalization(nameof(HowToUse));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>("Coralite/icon").Value;

            spriteBatch.Draw(mainTex, Center + new Vector2(0, -180), null, Color.White, 0, mainTex.Size() / 2, 2, 0, 0);

            Helper.DrawText(spriteBatch, Description.Value, PageWidth, Center, Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Coralite.MagicCrystalPink, out Vector2 size);

            Helper.DrawText(spriteBatch, HowToUse.Value, PageWidth, Center + new Vector2(0, size.Y + 80), Vector2.One / 2, Vector2.One
                , Coralite.TextShadowColor, Coralite.MagicCrystalPink, out _);
        }
    }
}
