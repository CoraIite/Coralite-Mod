using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class BabyIceDragonBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        public override string Texture => AssetDirectory.BabyIceDragon + "BIDBossBar";

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            //显示以前分配的头部索引
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            bossHeadIndex = npc.GetBossHeadTextureIndex();
            Texture2D barTexture = drawParams.BarTexture;
            Texture2D iconTexture = drawParams.IconTexture;
            Vector2 barCenter = drawParams.BarCenter;
            Rectangle iconFrame = drawParams.IconFrame;
            Color iconColor = drawParams.IconColor;
            float iconScale = drawParams.IconScale;

            Point barSize = new Point(406, 20); //条条尺寸
            Point topLeftOffset = new Point(66, 20); //框框左上角的位置
            int frameCount = 4;

            int scale = barSize.X * npc.life / npc.lifeMax;
            scale -= scale % 2;

            Rectangle barPosition = Utils.CenteredRectangle(barCenter, (barSize + new Point(4, 0)).ToVector2());
            Vector2 barTopLeft = barPosition.TopLeft();
            Vector2 topLeft = barTopLeft - topLeftOffset.ToVector2();

            // 背景
            Rectangle bgFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 3);
            spriteBatch.Draw(barTexture, topLeft, bgFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            topLeftOffset = new Point(68, 20);

            barPosition = Utils.CenteredRectangle(barCenter, barSize.ToVector2());
            barTopLeft = barPosition.TopLeft();
            topLeft = barTopLeft - topLeftOffset.ToVector2();

            // 条条
            Rectangle barFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 1);   //这里和原版的不一样 抄的时候注意点
            barFrame.X += topLeftOffset.X;
            barFrame.Y += topLeftOffset.Y;
            barFrame.Width = 2;
            barFrame.Height = barSize.Y;
            Vector2 stretchScale = new Vector2(scale / barFrame.Width, 1f);
            Color barColor = Color.White;

            spriteBatch.Draw(barTexture, barTopLeft, barFrame, barColor, 0f, Vector2.Zero, stretchScale, SpriteEffects.None, 0f);

            // 顶端的绘制
            Rectangle tipFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 2);  //这里和原版的不一样 抄的时候注意点
            tipFrame.X += topLeftOffset.X;
            tipFrame.Y += topLeftOffset.Y;
            tipFrame.Width = 2;
            tipFrame.Height = barSize.Y;

            spriteBatch.Draw(barTexture, barTopLeft + new Vector2(scale - 2, 0f), tipFrame, barColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // 框框的绘制
            Rectangle frameFrame = barTexture.Frame(verticalFrames: frameCount, frameY: 0);
            spriteBatch.Draw(barTexture, topLeft, frameFrame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //图标的绘制
            Vector2 iconOffset = new Vector2(34f, 14f);     //应该在哪绘制图标
            Vector2 iconSize = new Vector2(26f, 28f);   //这个要跟着贴图变化
            Vector2 iconPos = iconOffset + iconSize / 2f;
            spriteBatch.Draw(iconTexture, topLeft + iconPos, iconFrame, iconColor, 0f, iconFrame.Size() / 2f, iconScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
