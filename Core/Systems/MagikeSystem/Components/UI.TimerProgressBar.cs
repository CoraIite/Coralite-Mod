using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    [VaultLoaden(AssetDirectory.MagikeUI)]
    public class TimerProgressBar : UIElement
    {
        public static ATex ProgressBar { get; private set; }

        protected ITimerTriggerComponent timer;

        private const int LeftPaddling = 10;

        public TimerProgressBar(ITimerTriggerComponent producer)
        {
            this.timer = producer;

            ResetSize();
        }

        public void ResetSize()
        {
            Vector2 timerSize = GetStringSize(timer.Timer);
            Vector2 delaySize = GetStringSize(timer.Delay);

            float width = timerSize.X + 10;
            if (delaySize.X + 10 > width)
                width = delaySize.X + 10;
            if (ProgressBar.Width() + 10 > width)
                width = ProgressBar.Width() + 10;

            Width.Set(width + LeftPaddling, 0);
            Height.Set(timerSize.Y * 3f + ProgressBar.Height() / 2, 0);
        }

        private static Vector2 GetStringSize(int value)
        {
            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(value.ToString(), Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

            return ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, Vector2.One * 1.1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle size = GetDimensions().ToRectangle();

            float per = (size.Height - ProgressBar.Height() / 2) / 3f;
            int width = size.Width - LeftPaddling;
            Vector2 topLeft = size.TopLeft();
            Vector2 pos = topLeft + new Vector2(30 + width / 2, per / 2);

            //绘制时间
            int Delay = timer.Delay;
            Utils.DrawBorderString(spriteBatch, (Delay < 0 ? 0 : MathF.Round((1 - timer.Timer / (float)Delay) * 100)).ToString() + " %", pos + new Vector2(0, 4), Color.White
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);

            //绘制中间的进度条
            Texture2D barTex = ProgressBar.Value;

            Rectangle box = barTex.Frame(1, 2, 0, 1);

            pos += new Vector2(0, per / 2 + box.Height / 2);

            Vector2 barPos = pos - new Vector2(width / 2 - 4, 0);
            Vector2 origin = new Vector2(0, box.Height / 2);
            spriteBatch.Draw(barTex, barPos, box, Color.White, 0, origin
                , 1, 0, 0);

            int delay = Delay;
            if (Delay <= 0)
            {
                delay = 0;
            }
            else
            {
                box = barTex.Frame(1, 2);
                box.Width = (int)((1 - timer.Timer / (float)Delay) * box.Width);
                spriteBatch.Draw(barTex, barPos, box, Color.White, 0, origin
                    , 1, 0, 0);
            }

            pos += new Vector2(0, per / 2 + box.Height / 2);

            //绘制倒计时
            Color color = MagikeHelper.GetBonusColor(timer.DelayBonus, true);
            Utils.DrawBorderString(spriteBatch, MathF.Round(delay / 60f, 1).ToString() + " " + MagikeSystem.GetUIText(MagikeSystem.UITextID.Second), pos + new Vector2(0, 4), color
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);

            pos += new Vector2(0, per);

            //绘制倒计时加成
            Utils.DrawBorderString(spriteBatch, $"< × {timer.DelayBonus} >", pos + new Vector2(0, 4), color
                , 1, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
