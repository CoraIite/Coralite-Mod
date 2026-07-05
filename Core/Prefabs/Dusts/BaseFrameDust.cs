using Coralite.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Prefabs.Dusts
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameXCount"></param>
    /// <param name="frameYCount"></param>
    /// <param name="frameCounterMax"></param>
    /// <param name="frameDirVertical">帧图排列方向是否为竖向</param>
    /// <param name="randRot"></param>
    public abstract class BaseFrameDust(int frameXCount, int frameYCount, int frameCounterMax, bool frameDirVertical = true, bool randRot = false) : ModDust
    {
        public virtual bool RotFollowVel { get => false; }

        public override void OnSpawn(Dust dust)
        {
            if (randRot)
                dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);

            int frameX = 0;
            int frameY = Main.rand.Next(frameYCount);
            if (frameDirVertical)
            {
                frameX = Main.rand.Next(frameXCount);
                frameY = 0;
            }

            dust.frame = new Rectangle(frameX, frameY, 0, 0);
        }

        public override bool Update(Dust dust)
        {
            if (++dust.fadeIn > frameCounterMax)
            {
                dust.fadeIn = 0;
                if (frameDirVertical)
                {
                    if (++dust.frame.Y >= frameYCount)
                        dust.active = false;
                }
                else
                {
                    if (++dust.frame.X >= frameXCount)
                        dust.active = false;
                }
            }

            dust.position += dust.velocity;
            if (RotFollowVel)
            {
                dust.rotation = dust.velocity.ToRotation();
            }

            return false;
        }

        public virtual Color GetColor(Dust d)
            => Lighting.GetColor(d.position.ToTileCoordinates(), d.color);

        public override bool PreDraw(Dust dust)
        {
            Texture2D tex = Texture2D.Value;

            var frameBox = tex.Frame(frameXCount, frameYCount, dust.frame.X, dust.frame.Y);

            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, frameBox
                , GetColor(dust), dust.rotation, frameBox.Size() / 2, dust.scale, 0, 0);

            return false;
        }
    }
}
