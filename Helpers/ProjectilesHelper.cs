using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Coralite.Helpers
{
    public static class ProjectilesHelper
    {
        /// <summary>
        /// 自动追踪最近敌人的弹幕
        /// </summary>
        /// <param name="projectile">弹幕</param>
        /// <param name="offset">追踪力度</param>
        /// <param name="chasingSpeed">追踪速度</param>
        /// <param name="distanceMax">最大追踪距离</param>
        [DebuggerHidden]
        public static bool AutomaticTracking(Projectile projectile, float offset, float chasingSpeed = 0, float distanceMax = 1000f)
        {
            NPC target = FindClosestEnemy(projectile.Center, distanceMax, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(projectile.Center, 1, 1, n.Center, 1, 1);
            });

            //原本的弹幕速度加上一个弹幕位置指向NPC位置的向量
            if (target != null)
            {
                Vector2 plrToTheNearestNPC = Vector2.Normalize(target.Center - projectile.Center);
                float originSpeed = projectile.velocity.Length();
                projectile.velocity += plrToTheNearestNPC * offset;
                projectile.velocity = Vector2.Normalize(projectile.velocity) * (chasingSpeed == 0 ? originSpeed : chasingSpeed);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 发射朝向最近敌人的弹幕
        /// 建议只触发一次，不然会变成超强效果的追踪
        /// </summary>
        /// <param name="projectile">弹幕</param>
        /// <param name="speed">速度</param>
        /// <param name="distanceMax">最大瞄准距离</param>
        [DebuggerHidden]
        public static void AimingTheNearestNPC(Projectile projectile, float speed, float distanceMax = 1000f)
        {
            NPC target = FindClosestEnemy(projectile.Center, distanceMax, (n) =>
            {
                return n.CanBeChasedBy() &&
                !n.dontTakeDamage && Collision.CanHitLine(projectile.Center, 1, 1, n.Center, 1, 1);
            });
            if (target != null)
                projectile.velocity = Vector2.Normalize(target.Center - projectile.Center) * speed;
        }

        /// <summary>
        /// 找到距离弹幕最近的敌人
        /// </summary>
        /// <param name="position"></param>
        /// <param name="maxDistance"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static NPC FindClosestEnemy(Vector2 position, float maxDistance, Func<NPC, bool> predicate)
        {
            float maxDis = maxDistance;
            NPC target = null;
            foreach (var npc in Main.npc.Where(n => n.active && !n.friendly && predicate(n)))
            {
                float dis = Vector2.Distance(position, npc.Center);
                if (dis < maxDis)
                {
                    maxDis = dis;
                    target = npc;
                }
            }
            return target;
        }

        /// <summary>
        /// 找到同类弹幕并知道自己是第几个弹幕
        /// </summary>
        /// <param name="projType"></param>
        /// <param name="whoAmI"></param>
        /// <param name="owner"></param>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        [DebuggerHidden]
        public static void GetMyProjIndexWithSameType(int projType, int whoAmI, int owner, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == owner && projectile.type == projType)
                {
                    if (whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        [DebuggerHidden]
        public static void GetMyProjIndexWithModProj<T>(Projectile Projectile, out int index, out int totalIndexesInGroup) where T : ModProjectile
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.ModProjectile is T)
                {
                    if (Projectile.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }


        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float extraRot = 0, float scale = -1)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, mainTex.Size() / 2, scale == -1 ? projectile.scale : scale, 0, 0);
        }

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scaleStep, float extraRot = 0, float scale = -1)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, mainTex.Size() / 2, (scale == -1 ? projectile.scale : scale) * (1 - i * scaleStep), 0, 0);
        }

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, Vector2 scale, float extraRot = 0)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, mainTex.Size() / 2, scale, 0, 0);
        }

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, Vector2 scale, float scaleStep, float extraRot = 0)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, mainTex.Size() / 2, scale * (1 - i * scaleStep), 0, 0);
        }

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scale, Rectangle frameBox, float extraRot = 0)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, frameBox.Size() / 2, scale, 0, 0);
        }

        public static void DrawShadowTrailsSacleStep(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scaleStep, Rectangle frameBox, float extraRot = 0, float scale = -1)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, frameBox.Size() / 2, (scale == -1 ? projectile.scale : scale) * (1 - i * scaleStep), 0, 0);
        }


        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, Vector2 scale, Rectangle frameBox, float extraRot = 0)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, frameBox.Size() / 2, scale, 0, 0);
        }

        public static void DrawLine(List<Vector2> list, Color originColor)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), originColor);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
        {
            Texture2D value = TextureAssets.Extra[98].Value;
            Color color = shineColor * opacity * 0.5f;
            color.A = 0;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
            Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
            color *= num;
            color2 *= num;
            Main.EntitySpriteDraw(value, drawPos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir, 0);
            Main.EntitySpriteDraw(value, drawPos, null, color, 0f + rotation, origin, vector2, dir, 0);
            Main.EntitySpriteDraw(value, drawPos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir, 0);
            Main.EntitySpriteDraw(value, drawPos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir, 0);
        }

        public static void DrawPrettyLine(float opacity, SpriteEffects dir, Vector2 drawPos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, float scale, Vector2 fatness)
        {
            Texture2D value = TextureAssets.Extra[98].Value;
            Color color = shineColor * opacity * 0.5f;
            color.A = 0;
            Vector2 origin = value.Size() / 2f;
            Color color2 = drawColor * 0.5f;
            float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
            Vector2 vector = new Vector2(fatness.X * 0.5f, scale) * num;
            color *= num;
            color2 *= num;
            Main.EntitySpriteDraw(value, drawPos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir, 0);
            Main.EntitySpriteDraw(value, drawPos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir, 0);
        }

        /// <summary>
        /// 从<see cref="TextureAssets"/>中获取弹幕的贴图
        /// </summary>
        /// <param name="projectile"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(this Projectile projectile)
        {
            return TextureAssets.Projectile[projectile.type].Value;
        }
    }
}
