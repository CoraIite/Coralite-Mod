using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    [AutoLoadTexture(Path = AssetDirectory.UI)]
    public class FairySpawnBar : UIElement
    {
        public static ATex AlphaBar { get; set; }

        private FairySpawnCondition condition;

        public FairySpawnBar(FairySpawnCondition condition, float maxWidth)
        {
            this.condition = condition;
            Vector2 size = Helper.GetStringSize(condition.Descripetion()
                , Vector2.One, maxWidth - 10);
            this.SetSize(maxWidth, size.Y + 10);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 pos = calculatedStyle.Position();

            Rectangle targetRect = new Rectangle((int)pos.X, (int)pos.Y, (int)calculatedStyle.Width, (int)calculatedStyle.Height);

            spriteBatch.Draw(AlphaBar.Value, targetRect, Color.MidnightBlue * 0.5f);

            Helper.DrawTextParagraph(spriteBatch, condition.Descripetion()
                , calculatedStyle.Width - 10, pos + new Vector2(5, 9), out _,shadowColor:new Color(20,20,20,180));
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.UI)]
    public class FairySkillBar : UIElement
    {
        public static ATex AlphaBar { get; set; }

        private FairySkill skill;

        public FairySkillBar(int skillType, float maxWidth)
        {
            this.skill = FairyLoader.GetFairySkill(skillType);
            Vector2 size = skill.GetSkillTipSizeForUI();
            this.SetSize(maxWidth, size.Y + 10);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 pos = calculatedStyle.Position();

            Rectangle targetRect = new Rectangle((int)pos.X, (int)pos.Y, (int)calculatedStyle.Width, (int)calculatedStyle.Height);

            spriteBatch.Draw(AlphaBar.Value, targetRect, Color.MidnightBlue*0.5f);

            skill.DrawSkillTipInUI(pos + new Vector2(5, 5), new Vector2(calculatedStyle.Width - 10, calculatedStyle.Height - 10));
        }
    }
}
