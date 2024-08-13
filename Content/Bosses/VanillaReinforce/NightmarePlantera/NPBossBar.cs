using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    public class NPBossBar : ModBossBar
    {
        public override string Texture => AssetDirectory.Blank;

        public static NightmareTentacle tentacle;
        public static Asset<Texture2D> nameExtra;
        public Color mainColor;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            nameExtra = ModContent.Request<Texture2D>(AssetDirectory.NightmarePlantera + "NameExtra");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            nameExtra = null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            mainColor = ((NightmareSky)SkyManager.Instance["NightmareSky"]).color;
            //float percentage = Life / LifeMax;
            tentacle ??= new NightmareTentacle(180, factor =>
            {
                Color c = Life / LifeMax > factor ? mainColor : Color.DarkGray;
                if (factor < 0.1f)
                    return Color.Lerp(Color.Transparent, c, factor / 0.1f);
                else if (factor < 0.9f)
                    return c;
                else
                    return Color.Lerp(c, Color.Transparent, (factor - 0.9f) / 0.1f);
            }, factor => 18, NightmarePlantera.tentacleTex, NightmareSpike.FlowTex);
            Vector2 barCenter = drawParams.BarCenter;

            tentacle.pos = barCenter - new Vector2(460, 0);
            tentacle.rotation = 0f;
            tentacle.UpdateTentacle(920 / 180f, (i) => 2 * MathF.Sin(i / 6f * (0.75f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly))));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            tentacle?.DrawTentacle_NoEndBegin_UI();
            //还原原版的绘制参数
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

            Vector2 trueCenter = barCenter + new Vector2(0, 30);
            spriteBatch.Draw(nameExtra.Value, trueCenter, null, mainColor, 0, nameExtra.Size() / 2, new Vector2(0.75f, 0.6f), 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            bool showText = drawParams.ShowText;
            float life = drawParams.Life;
            float lifeMax = drawParams.LifeMax;

            Point barSize = new(920, 20); //条条尺寸

            Rectangle barPosition = Utils.CenteredRectangle(trueCenter, (barSize + new Point(4, 0)).ToVector2());

            if (BigProgressBarSystem.ShowText && showText)
            {
                BigProgressBarHelper.DrawHealthText(spriteBatch, barPosition, drawParams.TextOffset, life, lifeMax);
            }

            return false;
        }

    }
}
