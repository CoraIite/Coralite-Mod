using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Projectiles
{
    public abstract class BaseGunHeldProj : ModProjectile
    {
        private readonly string TexturePath;
        private readonly bool PathHasName;

        internal ref float Time => ref Projectile.ai[0];

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        protected BaseGunHeldProj(string texturePath, bool pathHasName = false)
        {
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = (int)Time;
        }

        public override void AI()
        { 
            
        }
    }
}
