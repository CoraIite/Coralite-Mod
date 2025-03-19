using Coralite.Core;
using Terraria;

namespace Coralite.Content.NPCs.Crystalline
{
    public class CrystallineImpact : ModDust
    {
        public override string Texture => AssetDirectory.CrystallineNPCs + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(2, 4, Main.rand.Next(2), 0);
            dust.color = Color.White;
        }

        public override bool Update(Dust dust)
        {
            if (++dust.fadeIn > 2)
            {
                dust.frame.Y += Texture2D.Height() / 4;
                if (dust.frame.Y > Texture2D.Height() / 4 * 3)
                    dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color)
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
