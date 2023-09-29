﻿using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Helper.RedJadeExplosion(Projectile.Center);
        }

        public override bool PreAI() => false;
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool? CanHitNPC(NPC target)
        {
            return Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) && Vector2.Distance(Projectile.Center, target.Center) < 64;
        }
    }
}