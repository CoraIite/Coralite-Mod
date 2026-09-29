using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public float DarkValue = 0f;//黑暗值
        private float ToDarkValue = 0f;//至黑暗值
        public float TrueToDarkValue = 0f;//引用黑暗值
        public Vector2 DrakCen = Vector2.Zero;//黑暗中心

        public void ResetDarkCircle()
        {
            ToDarkValue = TrueToDarkValue;
            TrueToDarkValue = 0f;
        }

        public void UpdateDarkCircle()
        {
            if (DarkValue != ToDarkValue)
            {
                DarkValue = MathHelper.Lerp(DarkValue, ToDarkValue, 0.1f);
                if (ToDarkValue == 0f)
                {
                    if (DarkValue > 0f) DarkValue -= 0.01f;
                    if (DarkValue < 0f) DarkValue += 0.01f;
                    if (Math.Abs(ToDarkValue - DarkValue) < 0.01f) DarkValue = 0f;
                }
            }
            if (DarkValue == 0f) 
                DrakCen = Player.Center;
        }
    }
}
