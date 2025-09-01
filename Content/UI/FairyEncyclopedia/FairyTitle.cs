using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    [VaultLoaden(AssetDirectory.FairyUI)]
    public class FairyTitle : UIElement
    {
        public static ATex ConditionTitle { get; set; }
        public static ATex SkillTitle { get; set; }


        private int type;

        public FairyTitle(int type)
        {
            this.type = type;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float width = GetDimensions().Width;
            float height = GetDimensions().Height;

            switch (type)
            {
                default:
                case 0:
                    {
                        Texture2D tex = ConditionTitle.Value;

                        spriteBatch.Draw(tex, GetDimensions().Position() + new Vector2(width / 2, height)
                            , null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), 0.3f, 0, 0);

                        Utils.DrawBorderString(spriteBatch, FairySystem.UIConditionTitle.Value
                            , GetDimensions().Center()+new Vector2(0,4), Color.White, 1.2f, 0.5f, 0.5f);

                    }
                    break;
                case 1:
                    {
                        Texture2D tex = SkillTitle.Value;

                        spriteBatch.Draw(tex, GetDimensions().Position() + new Vector2(width / 2, height)
                            , null, Color.White, 0, new Vector2(tex.Width / 2, tex.Height), 0.3f, 0, 0);
                     
                        Utils.DrawBorderString(spriteBatch, FairySystem.UISkillTitle.Value
                            , GetDimensions().Center() + new Vector2(0, 4), Color.White, 1.2f, 0.5f, 0.5f);
                    }
                    break;
            }
        }
    }
}
