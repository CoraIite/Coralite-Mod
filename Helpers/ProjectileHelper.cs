using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.Thunder;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Helpers
{
    public static partial class Helper
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

        [DebuggerHidden]
        public static bool TryFindClosestEnemy(Vector2 position, float maxDistance, Func<NPC, bool> predicate, out NPC target)
        {
            float maxDis = maxDistance;
            target = null;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.active && !n.friendly && predicate(n))
                {
                    float dis = Vector2.Distance(position, n.Center);
                    if (dis < maxDis)
                    {
                        maxDis = dis;
                        target = n;
                    }

                }
            }

            if (target == null)
                return false;

            return true;
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

        /// <summary>
        /// 获取自身是第几个召唤物弹幕
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        [DebuggerHidden]
        public static void GetMyGroupIndexAndFillBlackList(this Projectile Projectile, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == Projectile.type)
                {
                    if (Projectile.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        /// <summary>
        /// 获取目标NPC的索引
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="skipBodyCheck"></param>
        /// <returns></returns>
        public static int MinionFindTarget(this Projectile Projectile, bool skipBodyCheck = false)
        {
            Vector2 ownerCenter = Main.player[Projectile.owner].Center;
            int result = -1;
            float num = -1f;
            //如果有锁定的NPC那么就用锁定的，没有或不符合条件在从所有NPC里寻找
            NPC ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(Projectile))
            {
                bool flag = true;
                if (!ownerMinionAttackTargetNPC.boss)
                    flag = false;

                if (ownerMinionAttackTargetNPC.Distance(ownerCenter) > 1000f)
                    flag = false;

                if (!skipBodyCheck && !Projectile.CanHitWithOwnBody(ownerMinionAttackTargetNPC))
                    flag = false;

                if (flag)
                    return ownerMinionAttackTargetNPC.whoAmI;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile))
                {
                    float npcDistance2Owner = nPC.Distance(ownerCenter);
                    if (npcDistance2Owner <= 1000f && (npcDistance2Owner <= num || num == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(nPC)))
                    {
                        num = npcDistance2Owner;
                        result = i;
                    }
                }
            }

            return result;
        }


        public static bool GetProjectile(int projType, int index, out Projectile p)
        {
            if (Main.projectile.IndexInRange(index))
            {
                Projectile proj = Main.projectile[index];
                if (proj.active && proj.type == projType)
                {
                    p = proj;
                    return true;
                }
            }

            p = null;
            return false;
        }

        public static bool GetProjectile<T>(int index, out Projectile p) where T : ModProjectile
        {
            if (Main.projectile.IndexInRange(index))
            {
                Projectile proj = Main.projectile[index];
                if (proj.active && proj.type == ModContent.ProjectileType<T>())
                {
                    p = proj;
                    return true;
                }
            }

            p = null;
            return false;
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

        /// <summary>
        /// 非常正常的更新弹幕的帧，从0到最大之间循环
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="frameCountMax"></param>
        /// <param name="frameMax"></param>
        public static void UpdateFrameNormally(this Projectile proj, int frameCountMax, int frameMax)
        {
            if (++proj.frameCounter > frameCountMax)
            {
                proj.frameCounter = 0;
                if (++proj.frame > frameMax)
                    proj.frame = 0;
            }
        }


        /// <summary>
        /// 使用一个变量来获取对应npc数组中的NPC
        /// </summary>
        /// <param name="index"></param>
        /// <param name="npcType"></param>
        /// <param name="owner"></param>
        /// <param name="notExistAction"></param>
        /// <returns></returns>
        public static bool GetNPCOwner(this float index, int npcType, out NPC owner, Action notExistAction = null)
        {
            if (!Main.npc.IndexInRange((int)index))
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)index];
            if (!npc.active || npc.type != npcType)
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public static bool GetNPCOwner(this float index, out NPC owner, Action notExistAction = null)
        {
            if (!Main.npc.IndexInRange((int)index))
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)index];
            if (!npc.active)
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public static bool GetNPCOwner<T>(this float index, out NPC owner, Action notExistAction = null) where T : ModNPC
        {
            if (!Main.npc.IndexInRange((int)index))
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            NPC npc = Main.npc[(int)index];
            if (!npc.active || npc.type != ModContent.NPCType<T>())
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public static bool GetNPCOwner<T>(this int index, out NPC owner, Action notExistAction = null) where T : ModNPC
        {
            return GetNPCOwner<T>((float)index, out owner, notExistAction);
        }


        public static bool GetProjectileOwner(this float index, out Projectile owner, Action notExistAction = null)
        {
            if (!Main.projectile.IndexInRange((int)index))
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            Projectile npc = Main.projectile[(int)index];
            if (!npc.active)
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            owner = npc;
            return true;
        }

        public static bool GetProjectileOwner<T>(this float index, out Projectile owner, Action notExistAction = null) where T : ModProjectile
        {
            if (!Main.projectile.IndexInRange((int)index))
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            Projectile proj = Main.projectile[(int)index];
            if (!proj.active || proj.type != ModContent.ProjectileType<T>())
            {
                notExistAction?.Invoke();
                owner = null;
                return false;
            }

            owner = proj;
            return true;
        }

        public static bool GetProjectileOwner<T>(this int index, out Projectile owner, Action notExistAction = null) where T : ModProjectile
        {
            return GetProjectileOwner<T>((float)index, out owner, notExistAction);
        }


        /// <summary>
        /// 说是开始攻击，然鹅实际上是清空自身所有的本地NPC无敌帧
        /// </summary>
        /// <param name="Projectile"></param>
        public static void StartAttack(this Projectile Projectile)
        {
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 0;
            }
        }


        public static bool IsActiveAndHostile(this Projectile projectile) => projectile.active && projectile.hostile;

        public static int NewProjectileFromThis(this Projectile projectile, Vector2 position, Vector2 velocity
            , int type, int damage, float knockback, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return Projectile.NewProjectile(projectile.GetSource_FromAI(), position, velocity, type, damage, knockback, projectile.owner, ai0, ai1, ai2);
        }
        public static int NewProjectileFromThis<T>(this Projectile projectile, Vector2 position, Vector2 velocity
            , int damage, float knockback, float ai0 = 0, float ai1 = 0, float ai2 = 0) where T : ModProjectile
        {
            return Projectile.NewProjectile(projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<T>(), damage, knockback, projectile.owner, ai0, ai1, ai2);
        }

        /// <summary>
        /// 检测玩家是否存活，存活的话就给buff并且让弹幕持续存在
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="buffType"></param>
        /// <returns></returns>
        public static bool CheckMinionOwnerActive(this Projectile projectile,int buffType)
        {
            Player owner = Main.player[projectile.owner];
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(buffType);
                return false;
            }

            if (owner.HasBuff(buffType))
                projectile.timeLeft = 2;

            return true;
        }

        /// <summary>
        /// 检测玩家是否存活，存活的话就给buff并且让弹幕持续存在
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="buffType"></param>
        /// <returns></returns>
        public static bool CheckMinionOwnerActive<T>(this Projectile projectile) where T : ModBuff
        {
            Player owner = Main.player[projectile.owner];
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<T>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<T>()))
                projectile.timeLeft = 2;

            return true;
        }




        //--------------------------------------------------------------------------------------------
        //                                    以下是绘制相关部分
        //--------------------------------------------------------------------------------------------


        public enum TrailingMode
        {
            OnlyPosition = 0,
            RecordAll = 2,
            RecordAllAndSmooth = 3,
            RecordAllAndFollowPlayer = 4,
        }

        /// <summary>
        /// 快速设置拖尾相关数据
        /// </summary>
        /// <param name="trailingMode"></param>
        /// <param name="trailCacheLength"></param>
        public static void QuickTrailSets(int type, TrailingMode trailingMode, int trailCacheLength)
        {
            ProjectileID.Sets.TrailingMode[type] = (int)trailingMode;
            ProjectileID.Sets.TrailCacheLength[type] = trailCacheLength;
        }

        /// <summary>
        /// 快速设置拖尾相关数据
        /// </summary>
        /// <param name="trailingMode"></param>
        /// <param name="trailCacheLength"></param>
        public static void QuickTrailSets(this Projectile proj, TrailingMode trailingMode, int trailCacheLength)
            => QuickTrailSets(proj.type, trailingMode, trailCacheLength);

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

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scaleStep, Rectangle frameBox, float extraRot = 0, float scale = -1)
        {
            Texture2D mainTex = TextureAssets.Projectile[projectile.type].Value;
            Vector2 toCenter = new Vector2(projectile.width / 2, projectile.height / 2);
            var origin = frameBox.Size() / 2;

            for (int i = start; i < howMany; i += step)
                Main.spriteBatch.Draw(mainTex, projectile.oldPos[i] + toCenter - Main.screenPosition, frameBox,
                    drawColor * (maxAlpha - i * alphaStep), projectile.oldRot[i] + extraRot, origin, (scale == -1 ? projectile.scale : scale) * (1 - i * scaleStep), 0, 0);
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

        public static void DrawShadowTrails(this Projectile projectile, Color drawColor, float maxAlpha, float alphaStep, int start, int howMany, int step, float scale, Rectangle frameBox, float extraRot)
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

        public static void QuickDraw(this Projectile projectile, Color lightColor, float exRot)
        {
            Texture2D mainTex = projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + exRot,
                mainTex.Size() / 2, projectile.scale, 0, 0);
        }

        public static void QuickDraw(this Projectile projectile, Rectangle frameBox, Color lightColor, float exRot)
        {
            Texture2D mainTex = projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, projectile.Center - Main.screenPosition, frameBox, lightColor, projectile.rotation + exRot,
                frameBox.Size() / 2, projectile.scale, 0, 0);
        }

        public static void QuickDraw(this Projectile projectile, Vector2 overrideCenter, Color lightColor, float exRot)
        {
            Texture2D mainTex = projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, overrideCenter - Main.screenPosition, null, lightColor, projectile.rotation + exRot,
                mainTex.Size() / 2, projectile.scale, 0, 0);
        }

        public static void QuickDraw(this Projectile projectile, Color lightColor, float overrideScale, float exRot)
        {
            Texture2D mainTex = projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + exRot,
                mainTex.Size() / 2, overrideScale, 0, 0);
        }

        public static void QuickDraw(this Projectile projectile, Color lightColor, Vector2 overrideScale, float exRot)
        {
            Texture2D mainTex = projectile.GetTexture();

            Main.spriteBatch.Draw(mainTex, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + exRot,
                mainTex.Size() / 2, overrideScale, 0, 0);
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

        public static void InitOldPosCache(this Projectile projectile, int trailCount, bool useCenter = true)
        {
            projectile.oldPos = new Vector2[trailCount];

            for (int i = 0; i < trailCount; i++)
            {
                if (useCenter)
                    projectile.oldPos[i] = projectile.Center;
                else
                    projectile.oldPos[i] = projectile.position;
            }
        }

        public static void InitOldRotCache(this Projectile projectile, int trailCount)
        {
            projectile.oldRot = new float[trailCount];

            for (int i = 0; i < trailCount; i++)
            {
                projectile.oldRot[i] = projectile.rotation;
            }
        }

        public static void UpdateOldPosCache(this Projectile projectile, bool useCenter = true, bool addVelocity = true)
        {
            for (int i = 0; i < projectile.oldPos.Length - 1; i++)
                projectile.oldPos[i] = projectile.oldPos[i + 1];
            projectile.oldPos[^1] = (useCenter ? projectile.Center : projectile.position) + (addVelocity ? projectile.velocity : Vector2.Zero);
        }

        public static void UpdateOldRotCache(this Projectile projectile)
        {
            for (int i = 0; i < projectile.oldRot.Length - 1; i++)
                projectile.oldRot[i] = projectile.oldRot[i + 1];
            projectile.oldRot[^1] = projectile.rotation;
        }

        public static void DrawCrystal(SpriteBatch spriteBatch, int noiseFrame, Vector2 noiseBasePos, Vector2 noiseScale, float uTime
            , Color highlightC, Color brightC, Color darkC, Action doDraw, Action<SpriteBatch> endSpriteBatch
            , float lightRange = 0.2f, float lightLimit = 0.35f, float addC = 0.75f)
        {
            Effect effect = Filters.Scene["Crystal"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            Texture2D noiseTex = GemTextures.CrystalNoises[noiseFrame].Value;

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["basePos"].SetValue((noiseBasePos - Main.screenPosition) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(noiseScale / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue(uTime);
            effect.Parameters["lightRange"].SetValue(lightRange);
            effect.Parameters["lightLimit"].SetValue(lightLimit);
            effect.Parameters["addC"].SetValue(addC);
            effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(darkC.ToVector4());

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, effect, Main.GameViewMatrix.ZoomMatrix);
            
            Main.graphics.GraphicsDevice.Textures[1] = noiseTex;
            doDraw();
            endSpriteBatch(spriteBatch);
        }
    }
}
