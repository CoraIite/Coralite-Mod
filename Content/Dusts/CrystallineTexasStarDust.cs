using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Dusts
{
    public class CrystallineTexasStarDust : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFromList(MathHelper.TwoPi);
            dust.frame = new Rectangle(Main.rand.Next(6), 0, 6, 1);
            UpdateType = DustID.Firefly;
        }

        public override bool MidUpdate(Dust dust)
        {

            return base.MidUpdate(dust);
        }

        public override bool Update(Dust dust)
        {

            return base.Update(dust);
        }

        public override bool PreDraw(Dust dust)
        {
            Lighting.AddLight(dust.position, new Vector3(0.3f, 0.25f, 0.15f));

            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.frame, dust.position - Main.screenPosition, Lighting.GetColor(dust.position.ToTileCoordinates())*(1-dust.alpha/255f), dust.rotation, dust.scale);
            return false;
        }
    }
}
