using Coralite.Content.Items.Magike.SpecialLens;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem
{
    public class MagikeGlobalProjectile : GlobalProjectile
    {
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (CoraliteSets.ProjectileExplosible[projectile.type])    //爆炸类弹幕炸到赤玉透镜时
            {
                Point16 position = projectile.Center.ToTileCoordinates16() + Point16.NegativeOne;

                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                    {
                        if (MagikeHelper.TryGetEntity(position.X + i, position.Y + j, out RedJadeLensEntity redJadeGen))
                        {
                            redJadeGen.Charge(redJadeGen.HowManyToGenerate);
                            goto redJadeGenCharged;
                        }
                    }
            }
        redJadeGenCharged:;
        }
    }
}
