using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace Coralite.Core.Systems.CameraSystem
{
    public class MoveModifyer : ICameraModifier
    {
        public Func<Vector2, Vector2, float, Vector2> EaseFunction = Vector2.SmoothStep;
        public Func<Vector2> TargetCenter = () => Main.LocalPlayer.Center;
        public int Timer;

        /// <summary>
        /// 刚生成，将屏幕位置固定住
        /// </summary>
        public int FreezeTime;

        public int MovementDuration = 0;

        public string UniqueIdentity => "Coralite Move";

        public bool Finished => Timer == 0;

        public Vector2 originPos;
        public Vector2 originCenter;

        public MoveModifyer(int FreezeTime, int MaxMoveTime)
        {
            this.FreezeTime = FreezeTime;
            Timer = MovementDuration = MaxMoveTime;
            originPos = Main.screenPosition;
            var offset = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
            originCenter = originPos + offset;
        }

        public void Update(ref CameraInfo cameraPosition)
        {
            if (FreezeTime > 0)
            {
                FreezeTime--;
                cameraPosition.CameraPosition = originPos;
                return;
            }

            if (Timer > 0)
            {
                var offset = new Vector2(-Main.screenWidth / 2f, -Main.screenHeight / 2f);

                Timer--;
                cameraPosition.CameraPosition = EaseFunction(originCenter + offset, TargetCenter() + offset, 1 - Timer / (float)MovementDuration);
            }
        }
    }
}
