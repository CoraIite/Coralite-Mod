using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.CoraliteNotes.Readfragment
{
    public class NamePage : KnowledgePage
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = CoraliteAssets.ReadFragmant.BookName.Value;
            spriteBatch.Draw(mainTex, Center/* + new Vector2(10, -20)*/, null, Color.White, 0, mainTex.Size() / 2, 1.18f, 0, 0);

            //测试用代码
            //Terraria.UI.CalculatedStyle calculatedStyle = GetDimensions();
            //Vector2 pos = calculatedStyle.Position();
            //var tex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "White32x32").Value;

            //spriteBatch.Draw(tex, pos, null, Color.White * 0.75f, 0, Vector2.Zero
            //   , new Vector2(calculatedStyle.Width / tex.Width, calculatedStyle.Height / tex.Height), 0, 0);
        }
    }
}
