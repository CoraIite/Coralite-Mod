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
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 10, 10, 10);
            dust.rotation = Main.rand.NextFloat(6.282f);
        }
    }
}
