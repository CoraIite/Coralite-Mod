using Microsoft.Xna.Framework;
using System;

namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void SpikeGelBall()
        {
            switch ((int)SonState)
            {
                case 0: //确保自己在地上
                    Jump(2f, 8f, onLanding: () => SonState++);
                    break;
                case 1: //变扁
                    Scale = Vector2.Lerp(Scale, new Vector2(1.2f, 0.9f), 0.2f);
                    if (Scale.X > 1.15f)
                    {
                        SonState++;
                        //射刺球弹幕

                    }
                    break;
                case 2:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.8f, 1.25f), 0.2f);
                    if (Scale.Y>1.2f)
                    {
                        SonState++;
                    }
                    break;
                case 3:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.2f);
                    if (Math.Abs(Scale.X-1)<0.05f)
                    {
                        Scale = Vector2.One;
                        SonState++;
                    }
                    break;
                default:
                    ResetStates();
                    break;
            }
        }
    }
}
