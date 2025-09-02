using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    //[VaultLoaden(AssetDirectory.UI)]
    public class FairySpawnBar : UIPanel
    {
        //public static ATex AlphaBar { get; set; }

        private FairySpawnCondition condition;

        public FairySpawnBar(FairySpawnCondition condition, float maxWidth)
            : base(FairyEncyclopedia.FairyPanelBackGround
            , FairyEncyclopedia.FairyPanelBorder, 12, 20)
        {
            this.condition = condition;
            Vector2 size = Helper.GetStringSize(condition.Descripetion()
                , Vector2.One, maxWidth - 10);
            this.SetSize(maxWidth, size.Y + 16);
            BackgroundColor = new Color(63, 107, 151) * 0.85f;
            BorderColor = Color.White;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 pos = calculatedStyle.Position();

            Rectangle targetRect = new Rectangle((int)pos.X, (int)pos.Y, (int)calculatedStyle.Width, (int)calculatedStyle.Height);

            //spriteBatch.Draw(AlphaBar.Value, targetRect, Color.MidnightBlue * 0.5f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value
                , condition.Descripetion(), pos + new Vector2(10, 9+3), Color.White, 0, Vector2.Zero, Vector2.One, calculatedStyle.Width - 10);
            //Helper.DrawTextParagraph(spriteBatch, condition.Descripetion()
            //    , calculatedStyle.Width - 10, pos + new Vector2(5, 9), out _,shadowColor:new Color(20,20,20,180));
        }
    }

    //[VaultLoaden(AssetDirectory.UI)]
    public class FairySkillBar : UIPanel
    {
        //public static ATex AlphaBar { get; set; }

        private FairySkill skill;

        public FairySkillBar(int skillType, float maxWidth)
            : base(FairyEncyclopedia.FairyPanelBackGround
            , FairyEncyclopedia.FairyPanelBorder, 12, 20)
        {
            skill = FairyLoader.GetFairySkill(skillType);
            Vector2 size = skill.GetSkillTipSizeForUI();
            this.SetSize(maxWidth, size.Y + 20);
            BackgroundColor = new Color(63, 107, 151) * 0.85f;
            BorderColor = Color.White;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle calculatedStyle = GetDimensions();
            Vector2 pos = calculatedStyle.Position();

            //Rectangle targetRect = new Rectangle((int)pos.X, (int)pos.Y, (int)calculatedStyle.Width, (int)calculatedStyle.Height);

            //spriteBatch.Draw(AlphaBar.Value, targetRect, Color.MidnightBlue * 0.5f);

            skill.DrawSkillTipInUI(pos + new Vector2(10, 0), new Vector2(calculatedStyle.Width - 10, calculatedStyle.Height));
        }
    }
}
