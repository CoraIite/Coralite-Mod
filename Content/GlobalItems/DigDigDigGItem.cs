using Coralite.Content.DamageClasses;
using Coralite.Content.WorldGeneration;
using Terraria;

namespace Coralite.Content.GlobalItems
{
    public class DigDigDigGItem : GlobalItem
    {
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (CoraliteWorld.DigDigDigWorld && !item.DamageType.CountsAsClass<CreateDamage>())
                damage = new StatModifier(0, 0,1,1);
        }
    }
}
