using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Magike
{
    public class MagikeWorldBall : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WorldGlobe);
            Item.shoot = ModContent.ProjectileType<MagikeWorldBallProj>();
        }
    }

    public class MagikeWorldBallProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeItems + "MagikeWorldBall";

        public override void SetDefaults()
        {
            AIType = ProjectileID.WorldGlobe;
            Projectile.CloneDefaults(ProjectileID.WorldGlobe);
        }

        public override void OnKill(int timeLeft)
        {
            for (int num405 = 0; num405 < 15; num405++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1.5f);
            }
            SoundEngine.PlaySound(CoraliteSoundID.BottleExplosion_Item107, Projectile.Center);
            MagikeSystem.CurrentConnectLineType++;
            if ((int)MagikeSystem.CurrentConnectLineType > (int)MagikeSystem.ConnectLineType.Wave)
                MagikeSystem.CurrentConnectLineType = MagikeSystem.ConnectLineType.Basic;
        }
    }
}
