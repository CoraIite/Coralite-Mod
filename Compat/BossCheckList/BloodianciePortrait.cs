using Coralite.Content.Bosses.ModReinforce.Bloodiancie;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Coralite.Compat.BossCheckList
{
    public class BloodianciePortrait
    {
        private static readonly BloodiancieFollower[] followers = new BloodiancieFollower[12]
        {
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
            new(new Vector2( Main.screenWidth/2,Main.screenHeight/2)),
        };//这也太蠢了... 

        public static void UpdateFollower_Idle(Vector2 center, float centerLerpSpeed = 0.6f)
        {
            float velLength = 24;
            float baseRot = (Main.GlobalTimeWrappedHourly / 2) + (velLength * 0.15f);
            float length = 68 + (velLength / 2);
            ///额...总之是非常复杂的立体解析几何，用于计算当前这个圆以X轴为轴的旋转角度，根据玩家位置来的
            float CircleRot = 1.57f - (Math.Clamp(-150f / 200, -1f, 1f) * 0.4f);
            for (int i = 0; i < followers.Length; i++)
            {
                FollowersAI_Idle(followers[i], center, i, baseRot, length, CircleRot, centerLerpSpeed);
            }
        }

        public static void FollowersAI_Idle(BloodiancieFollower follower, Vector2 center, int whoamI, float baseRot, float length, float CircleRot, float centerLerpSpeed)
        {
            float rot = baseRot + (whoamI / (float)followers.Length * MathHelper.TwoPi);

            Vector2 vector2D = rot.ToRotationVector2();
            Vector3 vector3D = Vector3.Transform(vector2D.Vec3(), Matrix.CreateRotationX(CircleRot));///将二维的向量转为3维的并绕着X轴旋转一下
            //vector3D = Vector3.Transform(vector3D, Matrix.CreateRotationZ(0f));///以Z为轴旋转，用来配合赤玉灵自身的旋转

            //将3维向量投影到二维
            float k1 = -1000 / (vector3D.Z - 1000);
            Vector2 targetDir = k1 * new Vector2(vector3D.X, vector3D.Y);
            Vector2 targetCenter = center + (targetDir * length * follower.lengthOffset) + new Vector2(0, MathF.Sin(whoamI * 1.2f) * 6);
            follower.center = Vector2.Lerp(follower.center, targetCenter, centerLerpSpeed);
            follower.rotation = 0;
            follower.drawBehind = vector3D.Z > 0;
            follower.scale = 0.9f - (vector3D.Z * 0.2f);
        }

        public static void DrawPortrait(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Vector2 center = rect.Center();
            UpdateFollower_Idle(center);

            if (Main.zenithWorld)
                color = Main.DiscoColor;

            foreach (var follower in followers)
            {
                if (follower.drawBehind)
                    follower.DrawInBCL(spriteBatch, color);
            }

            Texture2D mainTex = ModContent.Request<Texture2D>(AssetDirectory.Bloodiancie + "Bloodiancie").Value;

            Vector2 origin = mainTex.Size() / 2;

            spriteBatch.Draw(mainTex, center, null, color, 0, origin, 1, SpriteEffects.None, 0f);

            foreach (var follower in followers)
            {
                if (!follower.drawBehind)
                    follower.DrawInBCL(spriteBatch, color);
            }
        }
    }
}
