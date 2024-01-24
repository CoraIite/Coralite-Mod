using Coralite.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Transactions;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class StarsBack : ModProjectile
    {
        public override string Texture => AssetDirectory.Particles + "LightBall";

        public override void SetDefaults()
        {
            Projectile.hide = true;
        }

        public override void AI()
        {
            base.AI();
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

    }
}
