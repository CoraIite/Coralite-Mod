using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class PutItemIn : KnowledgePage
    {
        public static LocalizedText OpenUI { get; private set; }

        public override void OnInitialize()
        {
            OpenUI = this.GetLocalization(nameof(OpenUI));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

        }
    }
}
