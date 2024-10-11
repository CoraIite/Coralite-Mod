using Coralite.Content.DamageClasses;
using Coralite.Content.WorldGeneration;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.GlobalItems
{
    public class DigDigDigGItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (CoraliteWorld.DigDigDigWorld)
            {
                switch (item.type)
                {
                    default:
                        break;
                    case ItemID.ManaCrystal://禁用魔力星
                        return false;
                }
            }

            return base.CanUseItem(item, player);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (CoraliteWorld.DigDigDigWorld && !item.DamageType.CountsAsClass<CreateDamage>())
                damage = new StatModifier(0, 0,1,1);
        }
    }
}
