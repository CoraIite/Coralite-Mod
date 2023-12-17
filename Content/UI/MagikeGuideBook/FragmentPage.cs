using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.UI.MagikeGuideBook
{
    public abstract class FragmentPage : UIPage
    {
        protected sealed override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.MagikeGuideBook + "Fragment").Value;
            spriteBatch.Draw(mainTex, Center, null, Color.White, 0, mainTex.Size() / 2, 1, 0, 0);

            DrawOthers(spriteBatch);
        }

        protected virtual void DrawOthers(SpriteBatch spriteBatch)
        {

        }
    }
}
