using Coralite.Content.Dusts;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.RedJadeBigBoom(Projectile.Center);
        }

        public override bool PreAI() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool? CanHitNPC(NPC target)
        {
            return Collision.CanHitLine(Projectile.Center,1,1,target.Center,1,1)&&Vector2.Distance(Projectile.Center,target.Center)<200;
        }
    }
}
