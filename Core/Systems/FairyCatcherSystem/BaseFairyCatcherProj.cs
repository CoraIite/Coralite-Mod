using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public abstract class BaseFairyCatcherProj : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public abstract string CursorTexture { get; }

        public ref float SpawnTimer => ref Projectile.localAI[0];

        private Rectangle cursorRect;
        public Rectangle CursorBox => cursorRect;

        #region 字段

        public List<Fairy> fairys;
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

        /// <summary>
        /// 指针的位置
        /// </summary>
        public Vector2 curserCenter;

        #endregion

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.hide = true;

            SetOtherDefaults();
        }

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
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;

            if (init)
            {
                init = false;

                //初始指定鼠标位置为目标点
                Vector2 targetPos = Main.MouseWorld;
                Vector2 selfPos = Projectile.Center;
                Vector2 dir = (targetPos - selfPos).SafeNormalize(Vector2.Zero);
                float checkCount = Vector2.Distance(selfPos, targetPos) / 8;

                for (int i = 0; i < checkCount; i++)
                {
                    Vector2 currentPos = selfPos + dir * i * 8;
                    if (!WorldGen.InWorld((int)currentPos.X / 16, (int)currentPos.Y / 16))
                    {
                        TrunToBacking();
                        break;
                    }
                    Tile t = Framing.GetTileSafely(currentPos);
                    if (t.HasUnactuatedTile)
                    {
                        webCenter = currentPos.ToTileCoordinates();
                        break;
                    }
                }

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
                        Projectile.velocity = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) *speed;

                        curserCenter = Projectile.Center;

                        if (Vector2.Distance(Projectile.Center,aimPos)<speed*2)
                            TurnToCatching();

                        //玩家距离过远进入回收阶段
                        if (Vector2.Distance(Owner.Center, Projectile.Center) > 1000)
                            TrunToBacking();
                    }
                    break;
                case (int)AIStates.Catching:
                    {
                        fairys ??= new List<Fairy>();

                        //更新圆环的透明度和大小
                        UpdateWebVisualEffect_Catching();
                        //更新指针
                        UpdateCurser();

                        //更新仙灵的活动
                        for (int i = fairys.Count - 1; i >= 0; i--)
                        {
                            Fairy fairy = fairys[i];
                            fairy.UpdateInCatcher(this);
                            if (!fairy.active)
                                fairys.Remove(fairy);
                        }

                        //随机刷新仙灵
                        UpdateFairySpawn();

                        //右键一下就结束捕捉
                        if (Main.mouseRight && Main.mouseRightRelease)
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
                        Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.1f);
                        curserCenter = Vector2.Lerp(curserCenter, Projectile.Center, 0.1f);

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
        }

        public void TrunToBacking()
        {
            state = (int)AIStates.Catching;

            Projectile.timeLeft = 60 * 10;
        }

        public virtual void UpdateFairySpawn()
        {
            if (fairys.Count > webRadius / 16)
                return;

            SpawnTimer += Main.rand.Next(1, 3);

            if (Main.rand.NextBool(60))
                SpawnTimer += 60;

            if (SpawnTimer > 660)
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
                attempt.wallType = spawnTile.TileType;
                attempt.X = (int)spawnPos.X / 16;
                attempt.Y = (int)spawnPos.Y / 16;
                attempt.Player = Owner;

                attempt.rarity = SetFairyAttemptRarity();

                fcp.FairyCatch_GetBait(out Item bait);
                attempt.baitItem = bait;
                if (bait.ModItem is IFairyBait fairybait)
                    fairybait.EditFiashingAttempt(attempt);

                if (FairySystem.SpawnFairy(attempt, out Fairy fairy))
                {
                    fairys.Add(fairy);
                }
            }
        }

        public void UpdateCurser()
        {
            CurserAI();

            cursorRect = GetCursorBox();//避免一大堆仙灵然后疯狂调用贴图
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
            int cursorWidth = 8;
            int cursorHeight = 8;

            Asset<Texture2D> cursorTex = ModContent.Request<Texture2D>(CursorTexture);
            if (cursorTex.IsLoaded)
            {
                cursorWidth = cursorTex.Width();
                cursorHeight = cursorTex.Height();
            }

            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                cursorWidth = (int)fcp.cursorSizeBonus.ApplyTo(cursorWidth);
                cursorHeight = (int)fcp.cursorSizeBonus.ApplyTo(cursorHeight);
            }

            return new Rectangle((int)curserCenter.X - cursorWidth / 2, (int)curserCenter.Y - cursorHeight / 2, cursorWidth, cursorHeight);
        }

        public void UpdateWebVisualEffect_Catching()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 1, 0.05f);
            if (Owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                webRadius = MathHelper.Lerp(webRadius, fcp.fairyCatcherRadius, 0.1f);
        }

        public void UpdateWebVisualEffect_Backing()
        {
            WebAlpha = MathHelper.Lerp(WebAlpha, 0, 0.05f);
                webRadius = MathHelper.Lerp(webRadius, 0, 0.05f);
        }

        #region 子类可用方法

        public virtual void OnInitialize() { }

        public virtual void CurserAI()
        {
            cursorMovement?.HandleMovement(this);
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

        public virtual Vector2 GetHandlePos()
        {
            return Vector2.Zero;
        }

        /// <summary>
        /// 绘制指针与手持物品间的连线
        /// </summary>
        public virtual void DrawLine(Vector2 handlePos)
        {

        }

        public virtual void DrawCursor()
        {

        }

        public virtual void DrawHandle()
        {

        }

        /// <summary>
        /// 获取线的点的列表
        /// </summary>
        /// <param name="handlePos"></param>
        /// <returns></returns>
        public virtual List<Vector2> GetLinePointList(Vector2 handlePos)
        {
            return null;
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制圆圈

            //绘制背景

            //绘制标红的物块

            Vector2 handlePos = GetHandlePos();

            //绘制连线
            DrawLine(handlePos);
            //绘制指针
            DrawCursor();
            //绘制手持
            DrawHandle();

            return base.PreDraw(ref lightColor);
        }

        #endregion
    }
}
