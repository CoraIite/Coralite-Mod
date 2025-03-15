using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Dusts
{
    public class CrystallineDust:ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void SetStaticDefaults()
        {
            UpdateType = DustID.Iron;
        }

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(1, 3,0, Main.rand.Next(3));
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.color = Color.White;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position-Main.screenPosition, dust.frame, Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color)
                , dust.rotation, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
