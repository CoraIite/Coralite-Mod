using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class FlyingShieldVarnish : BaseAccessory
    {
        public FlyingShieldVarnish() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 10))
        { }

        //public void OnInitialize(BaseFlyingShield projectile)
        //{
        //    projectile.maxJump++;
        //}
    }
}
