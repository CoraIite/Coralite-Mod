using Terraria.Graphics.CameraModifiers;

namespace Coralite.Core.Systems.CameraSystem
{
    public class FocusModifyer(Vector2 focusPos, string identity, int moveTime, int focusTime) : ICameraModifier
    {
        public string UniqueIdentity => Identity;

        public bool Finished => timer > MoveTime * 2 + FocusTime;

        public int timer;

        public Vector2 focusPos = focusPos;
        public string Identity = identity;
        public int MoveTime = moveTime;
        public int FocusTime = focusTime;

        public void Update(ref CameraInfo cameraPosition)
        {
            timer++;

            if (timer <= MoveTime)
            {
                cameraPosition.CameraPosition = Vector2.SmoothStep(cameraPosition.OriginalCameraCenter, focusPos, (float)timer / MoveTime);

            }
            else if (timer < MoveTime + FocusTime)
            {
                cameraPosition.CameraPosition = focusPos;
            }
            else if (timer < MoveTime * 2 + FocusTime)
            {
                cameraPosition.CameraPosition = Vector2.SmoothStep(focusPos, cameraPosition.OriginalCameraCenter, (float)(timer - MoveTime + FocusTime) / MoveTime);
            }
        }
    }
}
