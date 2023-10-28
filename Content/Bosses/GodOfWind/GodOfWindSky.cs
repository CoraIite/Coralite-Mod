using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;

namespace Coralite.Content.Bosses.GodOfWind
{
    public class GodOfWindSky : CustomSky
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public static Vector2 CycloneCenter { get; private set; }
        private bool drawCyclone;
        private Vector2[] cyclone0;
        private float cycloneTimer;

        public List<int> AosSiIndex;

        private float timer;

        public override void Activate(Vector2 position, params object[] args)
        {

        }

        public override void Deactivate(params object[] args)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {

        }

        public override bool IsActive() => timer > 0;

        public override void Reset()
        {
            timer = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused)
            {

                return;
            }



            timer--;
        }



        public void StartCyclone(Vector2 pos)
        {
            drawCyclone = true;
            cycloneTimer = 0;
            //初始化龙卷风数组
        }
    }
}
