using Coralite.Content.UI.UILib;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    public class CoraliteNotePanel : UI_BookPanel
    {
        public CoraliteNotePanel() : base(ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanel", AssetRequestMode.ImmediateLoad)
            , 28, 80, 40, 30)
        {
            PanelBackTex = ModContent.Request<Texture2D>(AssetDirectory.CoraliteNote + "BookPanelBack", AssetRequestMode.ImmediateLoad);
        }

        public override void InitPageGroups()
        {
            pageGroups =
                [
                    new Readfragment.GroupReadfragment(),

                    new RedJade.GroupRedJade(),
                    new IceDragonChapter1.GroupIceDragonChapter1(),
                    new ThunderChapter1.GroupThunderChapter1(),

                    new SlimeChapter1.GroupSlimeChapter1(),
                    new NightmareChapter.GroupNightmare(),

                    new MagikeChapter1.GroupMagikeChapter1(),
                    new MagikeInterstitial1.GroupMagikeInterstitial1(),
                    new MagikeChapter2.GroupMagikeChapter2(),
                ];
        }

        public static void DrawDebugFrame(UIElement element, SpriteBatch spriteBatch)
        {
            CalculatedStyle calculatedStyle = element.GetDimensions();
            Vector2 pos = calculatedStyle.Position();
            var tex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "White32x32").Value;

            spriteBatch.Draw(tex, pos, null, Color.White * 0.75f, 0, Vector2.Zero
               , new Vector2(calculatedStyle.Width / tex.Width, calculatedStyle.Height / tex.Height), 0, 0);

        }
    }
}
