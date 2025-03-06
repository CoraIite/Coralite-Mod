using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public class SpellConnector(int connectLengthBase, byte maxConnectBase, Color drawColor) : CheckOnlyLinerSender
    {
        public override void Initialize()
        {
            ConnectLengthBase = connectLengthBase;
            MaxConnectBase = maxConnectBase;
        }

        public override void Update() { }

        /// <summary>
        /// 绘制连线
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D laserTex = CoraliteAssets.Sparkle.ShotLineSPA.Value;

            Vector2 selfPos = Helper.GetMagikeTileCenter(Entity.Position) - Main.screenPosition;

            foreach (var targetP in Receivers)
            {
                Vector2 targetPos = Helper.GetMagikeTileCenter(targetP) - Main.screenPosition;
                int width = (int)(selfPos - targetPos).Length();   //这个就是激光长度
                var origin = new Vector2(0, laserTex.Height / 2);

                var laserTarget = new Rectangle((int)selfPos.X, (int)selfPos.Y, width, 16);
                var laserSource = new Rectangle(0, 0, laserTex.Width, laserTex.Height);

                float rotation = (targetPos - selfPos).ToRotation();
                spriteBatch.Draw(laserTex, laserTarget, laserSource, drawColor, rotation, origin, 0, 0);
                spriteBatch.Draw(laserTex, laserTarget, laserSource, new Color(40, 40, 40, 0), rotation, origin, 0, 0);
            }
        }
    }
}
