using Coralite.Content.Dusts;
using Terraria;

namespace Coralite.Content.Items.Gels
{
    public class EmperorSabreStrikeDust : BaseStrikeDust
    {
        public EmperorSabreStrikeDust() : base(new Color(247, 245, 176), new Color(83, 46, 85), 20) { }

        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0;
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, 1f, 1f, 0.3f);
            float factor = dust.fadeIn / 20f;
            if (factor < 0.5f)
            {
                dust.scale += 0.4f;
            }
            else
                dust.scale -= 0.15f;

            dust.fadeIn++;
            if (dust.fadeIn > 20)
                dust.active = false;

            return false;
        }
    }
}
