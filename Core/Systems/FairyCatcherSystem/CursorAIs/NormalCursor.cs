using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;

namespace Coralite.Core.Systems.FairyCatcherSystem.CursorAIs
{
    /// <summary>
    /// XY方向分别匀速移动的指针
    /// </summary>
    public class NormalCursor : FairyCursor
    {
        private readonly float maxSpeedX;
        private readonly float maxSpeedY;
        private readonly float accX;
        private readonly float accY;
        private readonly float turnAccX;
        private readonly float turnAccY;
        private readonly float slowDownFactorX;
        private readonly float slowDownFactorY;
        private readonly float slowDownLeengthX;
        private readonly float slowDownLeengthY;

        private int directionX;
        private int directionY;

        public NormalCursor(float maxSpeedX, float maxSpeedY, float accX, float accY
            ,float turnAccX, float turnAccY, float slowDownFactorX, float slowDownFactorY, float slowDownLeengthX, float slowDownLeengthY)
        {
            this.maxSpeedX = maxSpeedX;
            this.maxSpeedY = maxSpeedY;
            this.accX = accX;
            this.accY = accY;
            this.turnAccX = turnAccX;
            this.turnAccY = turnAccY;
            this.slowDownFactorX = slowDownFactorX;
            this.slowDownFactorY = slowDownFactorY;
            this.slowDownLeengthX = slowDownLeengthX;
            this.slowDownLeengthY = slowDownLeengthY;
        }

        public NormalCursor(float maxSpeed, float acc, float turnAcc, float slowDownFactor,float slowDownLeength)
        {
            maxSpeedX = maxSpeed;
            maxSpeedY = maxSpeed;
            accX = acc;
            accY = acc;
            turnAccX = turnAcc;
            turnAccY = turnAcc;
            slowDownFactorX = slowDownFactor;
            slowDownFactorY = slowDownFactor;
            slowDownLeengthX = slowDownLeength;
            slowDownLeengthY = slowDownLeength;
        }

        public override void HandleMovement(BaseFairyCatcherProj catcher)
        {
            SetDirection(catcher.cursorCenter);

            //追踪玩家
            GetLengthToTargetPos(Main.MouseWorld,catcher.cursorCenter, out float xLength, out float yLength);

            if (xLength > slowDownLeengthX)
                Helper.Movement_SimpleOneLine(ref catcher.cursorVelocity.X, directionX, maxSpeedX, accX,turnAccX, 0.95f);
            else
                catcher.cursorVelocity.X *= slowDownFactorX;

            if (yLength > slowDownLeengthY)
                Helper.Movement_SimpleOneLine(ref catcher.cursorVelocity.Y, directionY, maxSpeedY, accY, turnAccY, 0.95f);
            else
                catcher.cursorVelocity.Y *= slowDownFactorY;
        }

        public void SetDirection(Vector2 cursorCenter)
        {
            directionX  = Main.MouseWorld.X > cursorCenter.X ? 1 : -1;
            directionY = Main.MouseWorld.Y > cursorCenter.Y ? 1 : -1;
        }

        public void GetLengthToTargetPos(Vector2 targetPos, Vector2 cursorCenter, out float xLength, out float yLength)
        {
            xLength = cursorCenter.X - targetPos.X;
            yLength = cursorCenter.Y - targetPos.Y;

            xLength = Math.Abs(xLength);
            yLength = Math.Abs(yLength);
        }
    }
}
