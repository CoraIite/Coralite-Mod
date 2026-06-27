using Coralite.Core;
using Coralite.Core.Prefabs.Particles;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class BarrierShineParticle() : BaseFrameParticle(3, 6, 2)
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override Color GetColor()
        {
            return Color;
        }
    }

    public class BarrierDust : ModDust
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(10), 1, 10);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            dust.position += dust.velocity;

            if (dust.fadeIn < 6)
            {
                dust.color = Color.Lerp(Color.Transparent, Color.White, dust.fadeIn / 6);
            }
            else if (dust.fadeIn < 8)
            {
            }
            else if (dust.fadeIn < 8 + 8)
            {
                dust.color = Color.Lerp(Color.White, Color.Transparent, (dust.fadeIn - 8) / 8);
            }
            else
                dust.active = false;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch,dust.frame, dust.position - Main.screenPosition,dust.color, 0, dust.scale);

            return false;
        }
    }
}
