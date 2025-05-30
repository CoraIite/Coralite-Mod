using Coralite.Core.Attributes;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    [AutoLoadTexture(Path = AssetDirectory.Misc)]
    public abstract class BaseFairyCatcherProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;
        public virtual string HandleTexture => Texture + "Handle";

        public ref float SpawnTimer => ref Projectile.localAI[0];

        private Rectangle cursorRect;
        public Rectangle CursorBox => cursorRect;
        public int CursorWidth = 8;
        public int CursorHeight = 8;

        public static ATex TwistTex { get; private set; }

        [AutoLoadTexture(Path = AssetDirectory.OtherProjectiles,Name = "White32x32")]
        public static ATex TileTexture { get; private set; }

        #region 字段

        public List<Fairy> Fairies;
        public FairyCursor cursorMovement;

        public bool init = true;

        /// <summary>
        /// 状态，使用<see cref="AIStates"/>来判断
        /// </summary>
        public int state;

        /// <summary>
        /// 捕捉器的圆圈的中心点坐标
        /// </summary>
        public Point webCenter;

        /// <summary>
        /// 捕捉器的圆圈的半径大小
        /// </summary>
        public float webRadius;

        public float WebAlpha;
        protected float extraHandleRotation = 0.785f;

        /// <summary>
        /// 指针的位置
        /// </summary>
        public Vector2 cursorCenter;
        public Vector2 cursorVelocity;
        public float cursorRotation;
        /// <summary>
        /// 指针是否和仙灵重叠
        /// </summary>
        public bool cursorIntersects;
        public float cursorScale = 1f;

        #endregion

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hide = true;

            SetOtherDefaults();
        }

        /// <summary>
        /// 请务必设置<see cref="cursorMovement"/><br></br>
        /// </summary>
        public virtual void SetOtherDefaults() { }

        #region AI

        public enum AIStates
        {
            /// <summary>
            /// 射击时，到鼠标位置就会停止
            /// </summary>
            Shooting,
            /// <summary>
            /// 捕捉仙灵，绝大部分的逻辑在这里
            /// </summary>
            Catching,
            /// <summary>
            /// 返回玩家
            /// </summary>
            Backing
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active)
                Projectile.Kill();
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            SetOwnerItemLocation();

            if (init)
            {
                init = false;

                //初始指定鼠标位置为目标点
                Vector2 targetPos = Main.MouseWorld;
                Vector2 selfPos = Projectile.Center;
                Vector2 dir = (targetPos - selfPos).SafeNormalize(Vector2.Zero);
                float checkCount = Vector2.Distance(selfPos, targetPos) / 8;
                Vector2 currentPos = selfPos;

                for (int i = 0; i < checkCount; i++)
                {
                    currentPos = selfPos + (dir * i * 8);
                    if (!WorldGen.InWorld((int)currentPos.X / 16, (int)currentPos.Y / 16))
                    {
                        TrunToBacking();
                        break;
                    }

                    Tile t = Framing.GetTileSafely(currentPos);
                    if (t.HasUnactuatedTile)
                        break;
                }

                webCenter = currentPos.ToTileCoordinates();

                OnInitialize();
            }

            switch (state)
            {
                default: Projectile.Kill(); break;
                case (int)AIStates.Shooting:
                    {
                        Vector2 aimPos = webCenter.ToWorldCoordinates();
                        float speed = Projectile.velocity.Length();
                        //指针射向初始位置
                        Projectile.velocity = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;

                        cursorCenter = Projectile.Center;
                        cursorRotation = (cursorCenter - Owner.Center).ToRotation();

                        if (Vector2.Distance(Projectile.Center, aimPos) < speed * 2)
                            TurnToCatching();

                        //玩家距离过远进入回收阶段
                        if (Vector2.Distance(Owner.Center, Projectile.Center) > 1000)
                            TrunToBacking();
                    }
                    break;
                case (int)AIStates.Catching:
                    {
                        Fairies ??= new List<Fairy>();
                        cursorRotation = (cursorCenter - Owner.Center).ToRotation();

                        Projectile.timeLeft = 100;
                        //更新圆环的透明度和大小
                        UpdateWebVisualEffect_Catching();
                        //更新指针
                        UpdateCurser();

                        //更新仙灵的活动
                        for (int i = Fairies.Count - 1; i >= 0; i--)
                        {
                            Fairy fairy = Fairies[i];
                            fairy.UpdateInCatcher(this);
                            if (!fairy.active)
                                Fairies.Remove(fairy);
                        }

                        //随机刷新仙灵
                        UpdateFairySpawn();

                        //右键一下就结束捕捉
                        if (DownRight)
                            TrunToBacking();
                        //玩家距离过远进入回收阶段
                        if (Vector2.Distance(Owner.Center, Projectile.Center) > 1000)
                            TrunToBacking();
                    }
                    break;
                case (int)AIStates.Backing:
                    {
                        UpdateWebVisualEffect_Backing();

                        //直接向玩家lerp
                        Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.3f);
                        cursorCenter = Vector2.Lerp(cursorCenter, Projectile.Center, 0.8f);
                        cursorRotation = (cursorCenter - Owner.Center).ToRotation();

                        if (Vector2.Distance(Projectile.Center, Owner.Center) < 48)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 转换到捕捉状态
        /// </summary>
        public void TurnToCatching()
        {
            state = (int)AIStates.Catching;

            Projectile.tileCollide = false;
            Projectile.velocity *= 0;

            Helper.PlayPitched("Fairy/CursorExpand", 0.4f, 0);
        }

        public void TrunToBacking()
        {
            state = (int)AIStates.Backing;

            Projectile.timeLeft = 60 * 10;

            Helper.PlayPitched("Fairy/CursorBack", 0.4f, 0);
        }

        public virtual void UpdateFairySpawn()
        {
            if (Fairies.Count > webRadius / 16)
                return;

            SpawnTimer += Main.rand.Next(1, 3);

            if (Main.rand.NextBool(60))
                SpawnTimer += 60;

            if (SpawnTimer > 860)
            {
                SpawnTimer = 0;
                SpawnFairy();
            }
        }

        public void SpawnFairy()
        {
            if (!Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                return;

            int spawnCount = fcp.spawnFairyCount;

            for (int i = 0; i < spawnCount; i++)
            {
                //随机生成点
                Vector2 spawnPos = webCenter.ToWorldCoordinates() + Helper.NextVec2Dir(0, webRadius);
                //不在世界里就重新尝试
                if (!WorldGen.InWorld((int)spawnPos.X / 16, (int)spawnPos.Y / 16))
                    continue;

                Tile spawnTile = Framing.GetTileSafely(spawnPos);
                //不能有物块，虽然这个限制没啥意义
                if (spawnTile.HasUnactuatedTile)
                    continue;

                FairyAttempt attempt = new();
                attempt.wallType = spawnTile.WallType;
                attempt.X = (int)spawnPos.X / 16;
                attempt.Y = (int)spawnPos.Y / 16;
                attempt.Player = Owner;

                attempt.rarity = SetFairyAttemptRarity();

                fcp.FairyCatch_GetBait(out Item bait);
                if (bait != null)
                {
                    attempt.baitItem = bait;
                    if (bait.ModItem is IFairyBait fairybait)
                        fairybait.EditFiashingAttempt(attempt);
                }

                if (FairySystem.SpawnFairy(attempt, out Fairy fairy))
                {
                    Fairies.Add(fairy);
                }
            }
        }

        public void UpdateCurser()
        {
            CurserAI();
            cursorIntersects = false;

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                cursorScale = fcp.cursorSizeBonus.ApplyTo(cursorScale);

            cursorRect = GetCursorBox();//避免一大堆仙灵然后疯狂调用贴图

            //限制指针不能出圈
            Vector2 webCenter = this.webCenter.ToWorldCoordinates();
            if (Vector2.Distance(cursorCenter, webCenter) > webRadius)
                cursorCenter = webCenter + ((cursorCenter - webCenter).SafeNormalize(Vector2.Zero) * webRadius);
        }

        /// <summary>
        /// 设置玩家的手持位置
        /// </summary>
        public virtual void SetOwnerItemLocation() { }

        public void UpdateWebVisualEffect_Catching()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 1, 0.05f);
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                webRadius = MathHelper.Lerp(webRadius, fcp.fairyCatcherRadius, 0.1f);
        }

        public void UpdateWebVisualEffect_Backing()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 0, 0.2f);
            webRadius = MathHelper.Lerp(webRadius, 0, 0.2f);
        }

        #endregion

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (state)
            {
                default: return false;
                case (int)AIStates.Shooting://射击时撞墙直接返回
                    TurnToCatching();
                    return false;
                case (int)AIStates.Catching: return false;
                case (int)AIStates.Backing://返回时撞墙直接kill
                    return true;
            }
        }

        /// <summary>
        /// 获取指针的碰撞盒
        /// </summary>
        /// <returns></returns>
        public Rectangle GetCursorBox()
        {
            int cursorWidth = CursorWidth;
            int cursorHeight = CursorHeight;

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                cursorWidth = (int)fcp.cursorSizeBonus.ApplyTo(cursorWidth);
                cursorHeight = (int)fcp.cursorSizeBonus.ApplyTo(cursorHeight);
            }

            return new Rectangle((int)cursorCenter.X - (cursorWidth / 2), (int)cursorCenter.Y - (cursorHeight / 2), cursorWidth, cursorHeight);
        }

        #region 子类可用方法

        public virtual void OnInitialize() { }

        public virtual void CurserAI()
        {
            cursorMovement?.HandleMovement(this);

            cursorCenter += cursorVelocity;

            //限制不能出圈
            Vector2 webCenter = this.webCenter.ToWorldCoordinates();
            if (Vector2.Distance(cursorCenter, webCenter) > webRadius)
                cursorCenter = webCenter + ((cursorCenter - webCenter).SafeNormalize(Vector2.Zero) * webRadius);
        }

        public virtual FairyAttempt.Rarity SetFairyAttemptRarity()
        {
            FairyAttempt.Rarity rarity;
            int randomNumber = Owner.RollLuck(1000);

            if (randomNumber == 999)//0.1%概率为UR
                rarity = FairyAttempt.Rarity.UR;
            else if (randomNumber > 999 - 10)//1%概率为SR
                rarity = FairyAttempt.Rarity.SR;
            else if (randomNumber > 999 - 10 - 50)//5%概率为RR
                rarity = FairyAttempt.Rarity.RR;
            else if (randomNumber > 999 - 10 - 50 - 100)//10%概率为RR
                rarity = FairyAttempt.Rarity.R;
            else if (randomNumber > 999 - 10 - 50 - 100 - 150)//15%概率为RR
                rarity = FairyAttempt.Rarity.U;
            else//其他时候为C
                rarity = FairyAttempt.Rarity.C;

            return rarity;
        }

        #endregion

        #region 绘制

        #region 子类绘制

        /// <summary>
        /// 手柄的与线段连接的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetHandlePos(Texture2D handleTex)
        {
            return Owner.itemLocation + new Vector2(Owner.direction * Owner.gravDir * DrawOriginOffsetX, DrawOriginOffsetY);
        }

        /// <summary>
        /// 获取线的末端的位置
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetStringTipPos(Texture2D cursorTex)
        {
            return cursorCenter;
        }

        /// <summary>
        /// 绘制指针与手持物品间的连线
        /// </summary>
        public virtual void DrawLine(Vector2 handlePos, Vector2 stringTipPos)
        {
            bool flag = true;
            handlePos.Y += Owner.gfxOffY;

            float distanceX = stringTipPos.X - handlePos.X;
            float distanceY = stringTipPos.Y - handlePos.Y;
            bool flag2 = true;
            float rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;

            Texture2D stringTex = GetStringTex();

            float halfWidth = stringTex.Width / 2;
            float halfHeight = stringTex.Height / 2;
            Vector2 origin = new(halfWidth, 0f);

            if (distanceX == 0f && distanceY == 0f)
            {
                flag = false;
            }
            else
            {
                float distance = (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                distance = stringTex.Height / distance;
                distanceX *= distance;
                distanceY *= distance;
                handlePos.X -= distanceX * 0.1f;
                handlePos.Y -= distanceY * 0.1f;
                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
            }

            while (flag)
            {
                float sourceHeight = stringTex.Height;
                float distance1 = (float)Math.Sqrt((distanceX * distanceX) + (distanceY * distanceY));
                float distance2 = distance1;
                if (float.IsNaN(distance1) || float.IsNaN(distance2))
                {
                    flag = false;
                    continue;
                }

                if (distance1 < stringTex.Height + 8)
                {
                    sourceHeight = distance1 - 8f;
                    flag = false;
                }

                distance1 = stringTex.Height / distance1;
                distanceX *= distance1;
                distanceY *= distance1;
                if (flag2)
                {
                    flag2 = false;
                }
                else
                {
                    handlePos.X += distanceX;
                    handlePos.Y += distanceY;
                }

                distanceX = stringTipPos.X - handlePos.X;
                distanceY = stringTipPos.Y - handlePos.Y;
                if (distance2 > stringTex.Height)
                {
                    float num9 = 0.3f;
                    float num10 = Math.Abs(cursorVelocity.X) + Math.Abs(cursorVelocity.Y);
                    if (num10 > 16f)
                        num10 = 16f;

                    num10 = 1f - (num10 / 16f);
                    num9 *= num10;
                    num10 = distance2 / 80f;
                    if (num10 > 1f)
                        num10 = 1f;

                    num9 *= num10;
                    if (num9 < 0f)
                        num9 = 0f;

                    num9 *= num10;
                    num9 *= 0.5f;
                    if (distanceY > 0f)
                    {
                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                    else
                    {
                        num10 = Math.Abs(Projectile.velocity.X) / 3f;
                        if (num10 > 1f)
                            num10 = 1f;

                        num10 -= 0.5f;
                        num9 *= num10;
                        if (num9 > 0f)
                            num9 *= 2f;

                        distanceY *= 1f + num9;
                        distanceX *= 1f - num9;
                    }
                }

                rot = (float)Math.Atan2(distanceY, distanceX) - 1.57f;
                Color c = GetStringColor(handlePos);

                Main.EntitySpriteDraw(
                    color: c, texture: stringTex,
                    position: handlePos - Main.screenPosition + new Vector2(0, halfHeight), sourceRectangle: new Rectangle(0, 0, stringTex.Width, (int)sourceHeight), rotation: rot, origin: origin, scale: 1f, effects: SpriteEffects.None);
            }
        }

        public virtual void DrawCursor(Texture2D cursorTex)
        {
            var pos = cursorCenter - Main.screenPosition;
            var origin = cursorTex.Size() / 2;
            if (cursorIntersects && Main.mouseLeft)
                Main.spriteBatch.Draw(cursorTex, pos, null,
                    new Color(150, 150, 150, 0), cursorRotation, origin, cursorScale * 1.05f, 0, 0);
            Main.spriteBatch.Draw(cursorTex, pos, null,
                cursorIntersects ? Color.White * 0.5f : Color.White, cursorRotation, origin, cursorScale, 0, 0);
        }

        public virtual void DrawHandle(Texture2D HandleTex)
        {
            Main.spriteBatch.Draw(HandleTex, Owner.itemLocation - Main.screenPosition, null,
                Lighting.GetColor(Owner.Center.ToTileCoordinates()), 0, HandleTex.Size() / 2, 1f, Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            Color c = Color.White;
            c.A = (byte)(c.A * 0.4f);
            c = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), c);
            c *= 0.5f;
            return c;
        }

        public virtual Texture2D GetStringTex() => TextureAssets.FishingLine.Value;

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            if (state != (int)AIStates.Shooting)
            {
                Vector2 circlePos = webCenter.ToWorldCoordinates() - Main.screenPosition;

                Color circleColor = Color.White;
                Color backColor = Color.White;
                if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                {
                    circleColor = fcp.CatcherCircleColor;
                    backColor = fcp.CatcherBackColor;
                }

                //绘制背景
                DrawBack(circlePos, circleColor, backColor);
                //绘制标红的物块
                DrawBlockedTile(circlePos);

                //绘制仙灵
                if (Fairies != null)
                    foreach (var fairy in Fairies)
                        fairy.Draw_InCatcher();
            }

            Texture2D handleTex = ModContent.Request<Texture2D>(HandleTexture).Value;
            Texture2D cursorTex = Projectile.GetTexture();

            //绘制连线
            DrawLine(GetHandlePos(handleTex), GetStringTipPos(handleTex));
            //绘制指针
            DrawCursor(cursorTex);
            //绘制手持
            DrawHandle(handleTex);

            return false;
        }

        public virtual void DrawBack(Vector2 pos, Color circleColor, Color backColor)
        {
            Texture2D texture = TwistTex.Value;
            Effect shader = Filters.Scene["FairyCircle"].GetShader().Shader;

            float dia = webRadius;
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                dia = fcp.fairyCatcherRadius * 2+50;

            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 10);
            shader.Parameters["r"].SetValue(webRadius);
            shader.Parameters["dia"].SetValue(dia);
            shader.Parameters["edgeColor"].SetValue(Color.SkyBlue.ToVector4());
            shader.Parameters["innerColor"].SetValue(Color.DarkBlue.ToVector4() * 0.5f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

            float scale = dia / texture.Width;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition
                , null, Color.White, 0, texture.Size() / 2, scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0],
                            Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public virtual void DrawBlockedTile(Vector2 center)
        {
            int howMany = (int)(webRadius * 2 / 16);

            int baseX = webCenter.X - (howMany / 2);
            int baseY = webCenter.Y - (howMany / 2);

            Texture2D tex = TileTexture.Value;
            Color c = Color.IndianRed * 0.3f;

            for (int i = 0; i < howMany; i++)
                for (int j = 0; j < howMany; j++)
                {
                    Tile tile = Framing.GetTileSafely(baseX + i, baseY + j);

                    if (!tile.HasUnactuatedTile)
                        continue;
                    Vector2 worldPos = (new Vector2(baseX + i, baseY + j) * 16) - Main.screenPosition;
                    if (Vector2.Distance(worldPos, center) > webRadius)
                        continue;

                    Main.spriteBatch.Draw(tex, worldPos, null, c, 0, Vector2.Zero, 0.5f, 0, 0);
                }
        }

        #endregion
    }
}
