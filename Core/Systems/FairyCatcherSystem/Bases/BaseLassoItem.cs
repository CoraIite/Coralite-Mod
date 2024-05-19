using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseLassoItem : BaseFairyCatcher
    {
        public abstract int LassoProjType { get; }

        public override void ModifyShootFairyStyle(Player player)
        {
            Item.useStyle = ItemUseStyleID.Rapier;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                ShootCatcherProjectile(player, source, position, velocity, type);
                return false;
            }

            ModifyFairyStats(player, ref position, ref velocity);

            int itemType = 0;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp) && fcp.FairyShoot_GetFairyBottle(out IFairyBottle bottle))
            {
                float damage2 = damage;
                fcp.TotalCatchPowerBonus(ref damage2, Item);
                Item[] fairies = bottle.Fairies;

                for (int i = 0; i < fairies.Length; i++)
                {
                    currentFairyIndex++;
                    if (currentFairyIndex > fairies.Length - 1)
                        currentFairyIndex = 0;

                    if (bottle.CanShootFairy(currentFairyIndex, out _))
                    {
                        itemType = fairies[currentFairyIndex].type;
                        break;
                    }
                }
            }

            ShootLassoProj(player, source, damage, knockback, itemType);

            return false;
        }

        public sealed override void ShootFairy(IFairyBottle bottle, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int damage, float knockback) { }

        public virtual void ShootLassoProj(Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback, int itemType)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, LassoProjType,
                damage, knockback, player.whoAmI, itemType);
        }
    }
}
