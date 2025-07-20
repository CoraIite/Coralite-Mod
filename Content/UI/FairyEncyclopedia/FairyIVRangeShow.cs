using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    /// <summary>
    /// 显示仙灵IV范围
    /// </summary>
    public class FairyIVRangeShow : UIElement
    {
        private int _cornerSize = 12;
        private int _barSize = 4;

        private ATex tex;

        public FairyIVRangeShow()
        {
            tex = ModContent.Request<Texture2D>(AssetDirectory.UI + "MagikePanelBackground2");
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            FairyIV.GetIVForUI(FairyLoader.GetFairy(FairyEncyclopedia.ShowFairyID)
                , Main.LocalPlayer.GetModPlayer<FairyCatcherPlayer>(), out FairyIV min, out FairyIV max);

            var d = GetDimensions();
            //this.DrawDebugFrame(spriteBatch);

            const float padding = 70;

            Vector2 topLeft = d.Position() + new Vector2(0, d.Height / 2 - padding * 3/*+(d.Height-padding*7)/2*/);

            DrawPanel(spriteBatch, tex.Value, topLeft + new Vector2(0, -padding / 2)
                , new Vector2(d.Width, padding * 7), Color.MidnightBlue * 0.3f);


            DrawLine(spriteBatch, topLeft, 0, FairySystem.FairyLifeMax
                , min.LifeMaxLevel, max.LifeMaxLevel, min.LifeMax, max.LifeMax);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 1, FairySystem.FairyDamage
                , min.DamageLevel, max.DamageLevel, min.Damage, max.Damage);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 2, FairySystem.FairyDefence
                , min.DefenceLevel, max.DefenceLevel, min.Defence, max.Defence);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 3, FairySystem.FairySpeed
                , min.SpeedLevel, max.SpeedLevel, min.Speed, max.Speed);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 4, FairySystem.FairySkillLevel
                , min.SkillLevelLevel, max.SkillLevelLevel, min.SkillLevel, max.SkillLevel);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 5, FairySystem.FairyStamina
                , min.StaminaLevel, max.StaminaLevel, min.Stamina, max.Stamina);
            topLeft.Y += padding;

            DrawLine(spriteBatch, topLeft, 6, FairySystem.FairyScale
                , min.ScaleLevel, max.ScaleLevel, min.Scale, max.Scale);
            topLeft.Y += padding;
        }

        public void DrawLine(SpriteBatch spriteBatch, Vector2 center, int iconFrame, LocalizedText mainText, float minIV, float maxIV,float minValue,float maxValue)
        {
            float maxWidth = GetDimensions().Width;

            //绘制图标
            FairySystem.FairyIVIcon.Value.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, iconFrame, 1, 8),
                center + new Vector2(25, -4), scale:1.2f);

            //绘制名称
            string pre = mainText.Format("", "");

            //center.X += maxWidth / 6;

            Utils.DrawBorderString(spriteBatch, pre, center + new Vector2(30, 0), Color.White, 0.9f, 0, 0.5f);
            center.X += maxWidth / 2;


            //绘制等级
            (Color c1, LocalizedText t1) = FairyIV.GetFairyIVColorAndText(minIV);
            (Color c2, LocalizedText t2) = FairyIV.GetFairyIVColorAndText(maxIV);


            Utils.DrawBorderString(spriteBatch, "~", center, Color.White, 1, 0.5f, 0.5f);
            Vector2 size = Helper.GetStringSize("~", Vector2.One, maxWidth);
            Vector2 size2 = Helper.GetStringSize(t1.Value, Vector2.One, maxWidth);
            Vector2 size3 = Helper.GetStringSize(t2.Value, Vector2.One, maxWidth);

            Utils.DrawBorderString(spriteBatch, t1.Value
                , center + new Vector2(-size.X / 2 - 7 - size2.X / 2, 0), c1, 1, 0.5f, 0.5f);
            Utils.DrawBorderString(spriteBatch, t2.Value
                , center + new Vector2(size.X / 2 + 7 + size3.X / 2, 0), c2, 1, 0.5f, 0.5f);

            center.X += maxWidth / 3;

            //绘制数字~数字
            Utils.DrawBorderString(spriteBatch, "~", center, Color.White, 1, 0.5f, 0.5f);
            size2 = Helper.GetStringSize(minValue.ToString(), Vector2.One, maxWidth);
            size3 = Helper.GetStringSize(maxValue.ToString(), Vector2.One, maxWidth);

            Utils.DrawBorderString(spriteBatch, minValue.ToString()
                , center + new Vector2(-size.X / 2 - 7 - size2.X / 2, 0), c1, 1, 0.5f, 0.5f);
            Utils.DrawBorderString(spriteBatch, maxValue.ToString()
                , center + new Vector2(size.X / 2 + 7 + size3.X / 2, 0), c2, 1, 0.5f, 0.5f);

        }

        private void DrawPanel(SpriteBatch spriteBatch, Texture2D texture,Vector2 pos,Vector2 size, Color color)
        {
            Point point = new Point((int)pos.X, (int)pos.Y);
            Point point2 = new Point(point.X + (int)size.X - _cornerSize, point.Y + (int)size.Y - _cornerSize);
            int width = point2.X - point.X - _cornerSize;
            int height = point2.Y - point.Y - _cornerSize;
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, _cornerSize, _cornerSize), new Rectangle(0, 0, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, 0, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(0, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, _cornerSize, _cornerSize), new Rectangle(_cornerSize + _barSize, _cornerSize + _barSize, _cornerSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y, width, _cornerSize), new Rectangle(_cornerSize, 0, _barSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point2.Y, width, _cornerSize), new Rectangle(_cornerSize, _cornerSize + _barSize, _barSize, _cornerSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(0, _cornerSize, _cornerSize, _barSize), color);
            spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + _cornerSize, _cornerSize, height), new Rectangle(_cornerSize + _barSize, _cornerSize, _cornerSize, _barSize), color);
            spriteBatch.Draw(texture, new Rectangle(point.X + _cornerSize, point.Y + _cornerSize, width, height), new Rectangle(_cornerSize, _cornerSize, _barSize, _barSize), color);
        }

    }
}
