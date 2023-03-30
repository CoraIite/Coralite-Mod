using Coralite.Core;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.IcicleItems
{
    public class CrushedIceDust:ModDust
    {
        public override string Texture => AssetDirectory.IcicleProjectiles+ "Old_CrushedIceProj";

        public override void OnSpawn(Dust dust)
        {
            dust.rotation = Main.rand.NextFloat(6.282f);
            dust.frame = new Microsoft.Xna.Framework.Rectangle(0, 0, 10, 10);
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y += 0.2f;
            dust.position += dust.velocity;
            dust.rotation += 0.06f;
            if (dust.fadeIn > 30)
                dust.scale *= 0.98f;

            dust.fadeIn++;
            if (dust.fadeIn > 60)
                dust.active = false;
            return false;
        }
    }
}
