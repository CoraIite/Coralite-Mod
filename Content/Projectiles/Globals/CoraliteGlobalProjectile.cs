using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Content.Projectiles.Globals
{
    public class CoraliteGlobalProjectile : GlobalProjectile
    {
        public bool isBossProjectile;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent)
            {
                if (parent.Entity is NPC npc && npc.boss)
                    isBossProjectile = true;
            }
        }
    }
}
