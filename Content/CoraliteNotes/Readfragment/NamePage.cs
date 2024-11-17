﻿using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class NamePage : KnowledgePage
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = CoraliteAssets.ReadFragmant.BookName.Value;
            spriteBatch.Draw(mainTex, Center, null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);
        }
    }
}
