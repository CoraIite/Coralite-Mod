using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Coralite.Content.UI
{
    class NianliChargingBar : BetterUIState
    {
        public static bool visible = false;
        public static Vector2 basePos = new Vector2((Main.screenWidth / 2) + 40, (Main.screenHeight / 2) + 80);
        public static ChargingBar ChargingBar = new ChargingBar();

        public override int UILayer(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

        public override bool Visible => visible;

        public override void OnInitialize()
        {
            ChargingBar.Width.Set(68, 0f);
            ChargingBar.Height.Set(22, 0f);
            ChargingBar.Top.Set(basePos.Y, 0f);
            ChargingBar.Left.Set(basePos.X, 0f);
            Append(ChargingBar);
            base.OnInitialize();
        }

        //public override void Update(GameTime gameTime)
        //{
        //    CoralitePlayer coralitePlayer = Main.LocalPlayer.GetModPlayer<CoralitePlayer>();
        //    if (coralitePlayer.yujianUIAlpha <= 0f)
        //        visible = false;

        //    base.Update(gameTime);
        //}

        public override void Recalculate()
        {
            ChargingBar.Top.Set(basePos.Y, 0f);
            ChargingBar.Left.Set(basePos.X, 0f);
            base.Recalculate();
        }
    }

    public class ChargingBar : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 center = GetDimensions().Center();
            CoralitePlayer coralitePlayer = Main.LocalPlayer.GetModPlayer<CoralitePlayer>();
            Texture2D backTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "NianliBarBackground").Value;
            Texture2D barTex = ModContent.Request<Texture2D>(AssetDirectory.UI + "NianliBar").Value;

            //绘制背景
            spriteBatch.Draw(backTex, center, backTex.Frame(), Color.White * coralitePlayer.yujianUIAlpha, 0f, new Vector2(backTex.Width / 2, backTex.Height / 2), 1f, SpriteEffects.None, 0f);

            float chargePercent = coralitePlayer.nianli / coralitePlayer.nianliMax;
            Rectangle barSource = new Rectangle(0, 0, (int)(chargePercent * barTex.Width), barTex.Height);
            spriteBatch.Draw(barTex, center, barSource, Color.White * coralitePlayer.yujianUIAlpha, 0f, new Vector2(barTex.Width / 2, barTex.Height / 2), 1f, SpriteEffects.None, 0f);

            if (IsMouseHovering)
            {
                int nianli = (int)Math.Round(coralitePlayer.nianli);
                int nianliMax = (int)Math.Round(coralitePlayer.nianliMax);

                Main.instance.MouseText(string.Concat(new object[]
                {
                    "念力值: ",
                    nianli,
                    "/",
                    nianliMax
                 }) ?? "", 0, 0, -1, -1, -1, -1);

                coralitePlayer.yujianUIAlpha = MathHelper.Lerp(coralitePlayer.yujianUIAlpha, 0.25f, 0.35f);
            }
        }
    }
}
