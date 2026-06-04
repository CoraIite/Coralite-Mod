using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.ModPlayers
{
    public interface IHookPlayerShoot
    {
        public bool DisableShoot { get => false; }

        public void PlayerShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback);
    }
}
