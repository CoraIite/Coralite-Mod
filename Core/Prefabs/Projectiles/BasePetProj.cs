using System;
using Terraria;
using static Coralite.Content.WorldGeneration.ShadowCastleRoom;

namespace Coralite.Core.Prefabs.Projectiles
{
    /// <summary>
    /// 装饰性宠物的共享生命周期和移动辅助基类
    /// </summary>
    public abstract class BasePetProj : ModProjectile
    {
        /// <summary>
        /// 宠物对应的Buff类型ID，用于检测宠物是否应该保持存活
        /// </summary>
        protected virtual int PetBuffType => -1;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            SetPetStaticDefaults();
        }

        /// <summary>
        /// 设置宠物的静态默认值，子类可重写此方法来自定义静态属性
        /// </summary>
        protected virtual void SetPetStaticDefaults() { }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 34;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 100;
            SetPetDefaults();
        }

        /// <summary>
        /// 设置宠物的默认值，子类可重写此方法来自定义属性
        /// </summary>
        protected virtual void SetPetDefaults() { }

        /// <summary>
        /// 检查宠物是否应该保持存活。如果玩家死亡则清除Buff，如果玩家有Buff则保持宠物存活
        /// </summary>
        /// <param name="player">宠物的主人</param>
        protected void CheckActive(Player player)
        {
            if (PetBuffType <= 0)
                return;

            if (player.dead)
                player.ClearBuff(PetBuffType);
            else if (player.HasBuff(PetBuffType))
                Projectile.timeLeft = 2;
        }

        /// <summary>
        /// 当宠物距离主人过远时，将其传送回主人身边
        /// </summary>
        /// <param name="player">宠物的主人</param>
        /// <param name="maxDistance">触发传送的最大距离，默认2000像素</param>
        /// <returns>如果进行了传送返回true，否则返回false</returns>
        protected bool TeleportToOwner(Player player, float maxDistance = 2000f)
        {
            if (Vector2.Distance(Projectile.Center, player.Center) <= maxDistance)
                return false;

            Projectile.Center = player.Center;
            return true;
        }

        protected bool TeleportToPos(Vector2 pos, float maxDistance = 2000f)
        {
            if (Vector2.Distance(Projectile.Center, pos) <= maxDistance)
                return false;

            Projectile.Center = pos;
            return true;
        }

        /// <summary>
        /// 飞行宠物的移动逻辑，使宠物在空中跟随玩家
        /// </summary>
        /// <param name="player">宠物的主人</param>
        /// <param name="acceleration">加速度，默认0.2</param>
        /// <param name="maxSpeed">最大速度，默认10</param>
        /// <param name="landingDistance">开始降落的距离，默认200像素</param>
        /// <param name="stopDistance">停止移动的距离，默认60像素</param>
        /// <param name="horizontalCorrection">水平方向的修正系数，默认1.5</param>
        /// <param name="verticalCorrection">垂直方向的修正系数，默认1.5</param>
        /// <returns>如果宠物正在降落返回true，否则返回false</returns>
        protected bool FlyMovement(Player player, float acceleration = 0.2f, float maxSpeed = 10f,
            float landingDistance = 200f, float stopDistance = 60f,
            float horizontalCorrection = 1.5f, float verticalCorrection = 1.5f)
        {
            Projectile.tileCollide = false;

            // 目标速度至少要跟上玩家的速度
            float targetSpeed = Math.Max(maxSpeed, Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y));
            Vector2 toPlayer = player.Center - Projectile.Center;
            float distance = toPlayer.Length();

            TeleportToOwner(player);

            // 如果距离足够近且玩家在地面上，尝试降落
            if (distance < landingDistance && player.velocity.Y == 0f
                && Projectile.Bottom.Y <= player.Bottom.Y
                && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (Projectile.velocity.Y < -6f)
                    Projectile.velocity.Y = -6f;
                return true;
            }

            // 如果距离很近，停止移动
            if (distance < stopDistance)
                return false;

            // 向玩家方向移动
            toPlayer.SafeNormalize(Vector2.Zero);
            toPlayer *= targetSpeed;
            AdjustVelocity(toPlayer.X, ref Projectile.velocity.X, acceleration, horizontalCorrection);
            AdjustVelocity(toPlayer.Y, ref Projectile.velocity.Y, acceleration, verticalCorrection);

            // 更新面向方向
            if (Projectile.velocity.X != 0f && MathF.Abs(player.Center.X - Projectile.Center.X) > Projectile.width / 2)
                Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

            return false;
        }

        /// <summary>
        /// 地面宠物的移动逻辑，使宠物在地面上跟随玩家行走
        /// </summary>
        /// <param name="player">宠物的主人</param>
        /// <param name="maxSpeed">最大移动速度，默认4</param>
        /// <param name="acceleration">加速度，默认0.5</param>
        /// <param name="braking">减速度，默认0.1</param>
        /// <param name="gravity">重力加速度，默认0.4</param>
        /// <param name="maxFallSpeed">最大下落速度，默认10</param>
        /// <param name="maxFollowDistance">最大跟随距离，超过此距离将传送回玩家，默认500像素</param>
        /// <param name="maxVerticalDistance">最大垂直距离，超过此距离将传送回玩家，默认300像素</param>
        /// <param name="followOffset">跟随偏移量，默认在玩家身后40像素</param>
        protected void GroundMovement(Vector2 basePos, float maxSpeed = 4f, float acceleration = 0.5f,
            float braking = 0.1f, float gravity = 0.4f, float maxFallSpeed = 10f,
            float maxFollowDistance = 500f, float maxVerticalDistance = 300f,
            Vector2 followOffset=default)
        {
            // 计算目标位置（玩家身后）
            Vector2 target = basePos +followOffset;
            TeleportToPos(basePos);

            // 如果宠物和目标在玩家的两侧，直接移动到玩家位置
            if (Projectile.Distance(basePos) > 60f && Projectile.Distance(target) > 60f
                && Math.Sign(target.X - basePos.X) != Math.Sign(Projectile.Center.X - basePos.X))
                target = basePos;

            // 寻找地面位置
            Rectangle targetRect = Utils.CenteredRectangle(target, Projectile.Size);
            for (int i = 0; i < 20; i++)
            {
                if (Collision.SolidCollision(targetRect.TopLeft(), targetRect.Width, targetRect.Height))
                    break;
                targetRect.Y += 16;
                target.Y += 16f;
            }

            // 检测物块碰撞
            Vector2 tileCollision = Collision.TileCollision(basePos - Projectile.Size / 2f,
                target - basePos, Projectile.width, Projectile.height);
            target = basePos - Projectile.Size / 2f + tileCollision;

            // 如果距离过远或垂直距离过大，传送到玩家位置
            Vector2 ownerToTarget = basePos - target;
            if (ownerToTarget.Length() > maxFollowDistance || Math.Abs(ownerToTarget.Y) > maxVerticalDistance)
                target.Y = basePos.Y;

            Projectile.tileCollide = true;
            Projectile.rotation = 0f;

            // 水平移动逻辑
            float horizontalDistance = target.X - Projectile.Center.X;
            int direction = 0;
            if (Math.Abs(horizontalDistance) > 5f)
            {
                direction = Math.Sign(horizontalDistance);
                if (direction < 0)
                    Projectile.velocity.X -= Projectile.velocity.X > -maxSpeed ? acceleration : braking;
                else
                    Projectile.velocity.X += Projectile.velocity.X < maxSpeed ? acceleration : braking;
            }
            else
            {
                // 距离很近时减速停止
                Projectile.velocity.X *= 0.9f;
                if (Math.Abs(Projectile.velocity.X) < acceleration * 2f)
                    Projectile.velocity.X = 0f;
            }

            // 检测前方障碍物
            bool obstacle = false;
            if (direction != 0 && (Math.Abs(target.X - Projectile.Center.X) >= 64f
                || (target.Y - Projectile.Center.Y <= -48f && Math.Abs(target.X - Projectile.Center.X) >= 8f)))
            {
                int tileX = (int)(Projectile.position.X + Projectile.width / 2f) / 16 + direction + (int)Projectile.velocity.X;
                int tileY = (int)Projectile.position.Y / 16;
                for (int y = tileY; y < tileY + Projectile.height / 16 + 1; y++)
                {
                    if (WorldGen.SolidTile(tileX, y))
                    {
                        obstacle = true;
                        break;
                    }
                }
            }

            // 自动爬台阶
            Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height,
                ref Projectile.stepSpeed, ref Projectile.gfxOffY);

            // 遇到障碍物时跳跃
            if (Projectile.velocity.Y == 0f && obstacle)
                Projectile.velocity.Y = -9.1f;

            // 限制速度范围
            Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -maxSpeed, maxSpeed);

            // 更新面向方向
            if (direction != 0)
                Projectile.direction = direction;
            else if (MathF.Abs(target.X - Projectile.Center.X) > Projectile.width / 2)
                Projectile.direction = target.X >= Projectile.Center.X ? 1 : -1;

            Projectile.spriteDirection = Projectile.direction;

            // 应用重力
            Projectile.velocity.Y += gravity;
            if (Projectile.velocity.Y > maxFallSpeed)
                Projectile.velocity.Y = maxFallSpeed;
        }

        /// <summary>
        /// 平滑调整速度，使其逐渐接近目标速度
        /// </summary>
        /// <param name="target">目标速度</param>
        /// <param name="velocity">当前速度（会被修改）</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="correction">方向切换时的修正系数，用于更快地改变方向</param>
        private static void AdjustVelocity(float target, ref float velocity, float acceleration, float correction)
        {
            if (velocity < target)
            {
                velocity += acceleration;
                // 如果当前向相反方向移动，加快修正速度
                if (velocity < 0f)
                    velocity += acceleration * correction;
            }
            else if (velocity > target)
            {
                velocity -= acceleration;
                // 如果当前向相反方向移动，加快修正速度
                if (velocity > 0f)
                    velocity -= acceleration * correction;
            }
        }
    }
}
