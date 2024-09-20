using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    /// <summary>
    /// 绘制本体的不断收缩圆圈
    /// 绘制旋转的物品
    /// </summary>
    public class CraftParticle : Particle
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Circle3";

        private Point16 _pos;
        private int _totalTime;
        private float alpha;

        public int[] otherItems;

        public override void Update()
        {
            if (!TryGetEntity(_pos, out MagikeTileEntity entity))
            {
                active = false;
                return;
            }

            if (alpha < 1)
            {
                alpha += 0.2f;
            }

            float factor = Coralite.Instance.BezierEaseSmoother.Smoother(fadeIn / _totalTime);
            Rotation = factor * MathHelper.TwoPi * 30;
            Rotation = factor * MathHelper.TwoPi * 30;

            fadeIn--;
            if (fadeIn < 0)
                active = false;
        }

        public static CraftParticle Spawn(Vector2 center, int craftTime, MagikeCraftRecipe chosenRecipe)
        {
            CraftParticle p = NewParticle<CraftParticle>(center, Vector2.Zero, Coralite.MagicCrystalPink);

            if (chosenRecipe.RequiredItems != null)
            {
                p.otherItems = new int[chosenRecipe.RequiredItems.Count];
                int i = 0;
                foreach (var item in chosenRecipe.RequiredItems)
                {
                    p.otherItems[i] = item.type;
                    i++;
                }
            }

            p._totalTime = craftTime;
            p.fadeIn = craftTime;

            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

        }

        public override void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {

        }
    }
}
