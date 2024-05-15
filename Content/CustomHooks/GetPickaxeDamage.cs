using Terraria;

namespace Coralite.Content.CustomHooks
{
    public class GetPickaxeDamage : HookGroup
    {
        public override void Load()
        {
            On_Player.GetPickaxeDamage += On_Player_GetPickaxeDamage;
        }

        public override void Unload()
        {
            On_Player.GetPickaxeDamage -= On_Player_GetPickaxeDamage;
        }

        private int On_Player_GetPickaxeDamage(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            if (self.HeldItem.ModItem is ISpecialPickaxe pickaxe)
            {
                int damage = 0;
                if (pickaxe.ModifyPickaxeDamage(ref damage, self, x, y, pickPower, hitBufferIndex, tileTarget))
                    return damage;
            }

            return orig.Invoke(self, x, y, pickPower, hitBufferIndex, tileTarget);
        }
    }

    public interface ISpecialPickaxe
    {
        bool ModifyPickaxeDamage(ref int damage, Player player, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget);
    }
}
