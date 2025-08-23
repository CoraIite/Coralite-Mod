using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Coralite.Helpers
{
    public partial class Helper
    {
        /// <summary>
        /// 在弹幕身上生成粒子，默认为与弹幕相反的速度，如果需要调整速度请将<paramref name="velocityMult"/>设置为负数
        /// </summary>
        /// <param name="fairy">弹幕自身</param>
        /// <param name="type">弹幕种类</param>
        /// <param name="velocityMult">速度系数</param>
        /// <param name="Alpha"></param>
        /// <param name="newColor"></param>
        /// <param name="Scale"></param>
        /// <param name="noGravity">粒子重力</param>
        public static void SpawnTrailDust(this Fairy fairy, int type, float velocityMult, int Alpha = 0, Color newColor = default, float Scale = 1f, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(fairy.position, fairy.width, fairy.height, type, Alpha: Alpha, newColor: newColor, Scale: Scale);
            dust.noGravity = noGravity;
            dust.velocity = -fairy.velocity * velocityMult;
        }

        /// <summary>
        /// 从<see cref="FairySystem.FairyAssets"/>中获取仙灵贴图
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(this Fairy fairy)
        {
            return FairySystem.FairyAssets[fairy.Type].Value;
        }

        public static void QuickDraw(this Fairy fairy, Vector2 screenPos,Color lightColor, float exRot)
        {
            Texture2D mainTex = fairy.GetTexture();
            var frame = mainTex.Frame(fairy.HorizontalFrames, fairy.VerticalFrames, fairy.frame.X, fairy.frame.Y);
            Main.spriteBatch.Draw(mainTex, fairy.Center - screenPos,
                frame, lightColor, fairy.rotation + exRot,
                frame.Size() / 2, fairy.scale,
                fairy.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public static void QuickDraw(this Fairy fairy, Vector2 overrideCenter, Vector2 screenPos, Color lightColor, float exRot)
        {
            Texture2D mainTex = fairy.GetTexture();
            var frame = mainTex.Frame(fairy.HorizontalFrames, fairy.VerticalFrames, fairy.frame.X, fairy.frame.Y);

            Main.spriteBatch.Draw(mainTex, overrideCenter - screenPos, frame, lightColor, fairy.rotation + exRot,
                frame.Size() / 2, fairy.scale, fairy.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public static void QuickDraw(this Fairy fairy, Vector2 overrideCenter, Vector2 screenPos,Rectangle frame2, Color lightColor, float exRot,SpriteEffects effect)
        {
            Texture2D mainTex = fairy.GetTexture();
            var frame = mainTex.Frame(fairy.HorizontalFrames, fairy.VerticalFrames, frame2.X, frame2.Y);

            Main.spriteBatch.Draw(mainTex, overrideCenter - screenPos, frame, lightColor, fairy.rotation + exRot,
                frame.Size() / 2, fairy.scale, effect, 0);
        }

        public static void QuickDraw(this Fairy fairy, Color lightColor, float overrideScale, float exRot)
        {
            Texture2D mainTex = fairy.GetTexture();
            var frame = mainTex.Frame(fairy.HorizontalFrames, fairy.VerticalFrames, fairy.frame.X, fairy.frame.Y);

            Main.spriteBatch.Draw(mainTex, fairy.Center - Main.screenPosition, frame, lightColor, fairy.rotation + exRot,
                frame.Size() / 2, overrideScale, fairy.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        /// <summary>
        /// 检测传入的矩形与仙灵捕捉环的碰撞，发生碰撞后自动增加捕捉进度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="selfRect"></param>
        public static bool CheckCollideWithFairyCircle(Player player, Rectangle selfRect,ref HashSet<int> IDs)
        {
            if (!player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return false;

            if (!Main.projectile.IndexInRange(fcp.FairyCircleProj))
                return false;

            if (Main.projectile[fcp.FairyCircleProj].ModProjectile is not FairyCatcherProj fcproj)
                return false;

            int catchPower = fcp.GetCatchPowerByHeldItem();
            bool caught = false;

            foreach (var fairyRect in fcproj.GetFairyCollides())
            {
                if (selfRect.Intersects(fairyRect.Item1))
                {
                    IDs ??= [];
                    if (!IDs.Contains(fairyRect.Item2.IDInCatcher))
                        if(fairyRect.Item2.Catch(player, catchPower))
                        {
                            IDs.Add(fairyRect.Item2.IDInCatcher);
                            //Main.NewText(fairyRect.Item2.IDInCatcher);
                            caught = true;
                        }
                }
            }

            return caught;
        }

        /// <summary>
        /// 尝试后去玩家所属的仙灵捕捉环
        /// </summary>
        /// <param name="player"></param>
        /// <param name="catcherProj"></param>
        /// <returns></returns>
        public static bool TryGetFairyCircle(Player player,out FairyCatcherProj catcherProj)
        {
            catcherProj = null;
            if (!player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return false;

            if (!Main.projectile.IndexInRange(fcp.FairyCircleProj))
                return false;

            if (Main.projectile[fcp.FairyCircleProj].ModProjectile is not FairyCatcherProj fcproj)
                return false;

            catcherProj = fcproj;
            return true;
        }

        /// <summary>
        /// 检测传入的矩形与仙灵捕捉环的碰撞，发生碰撞后自动增加捕捉进度，只会对第一个命中的仙灵生效
        /// </summary>
        /// <param name="player"></param>
        /// <param name="selfRect"></param>
        /// <param name="overrideCatchPower"></param>
        /// <returns></returns>
        public static bool CheckCollideWithFairyCircleSingle(Player player, Rectangle selfRect, int? overrideCatchPower)
        {
            if (!player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return false;

            if (!Main.projectile.IndexInRange(fcp.FairyCircleProj))
                return false;

            if (Main.projectile[fcp.FairyCircleProj].ModProjectile is not FairyCatcherProj fcproj)
                return false;

            int catchPower = overrideCatchPower ?? fcp.GetCatchPowerByHeldItem();

            foreach (var fairyRect in fcproj.GetFairyCollides())
                if (selfRect.Intersects(fairyRect.Item1))
                    if (fairyRect.Item2.Catch(player, catchPower))
                        return true;

            return false;
        }
    }
}
