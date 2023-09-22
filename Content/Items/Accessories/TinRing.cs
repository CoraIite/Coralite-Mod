using Coralite.Core.Systems.MagikeSystem;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Coralite.Core.Prefabs.Items;
using Coralite.Core;

namespace Coralite.Content.Items.Accessories
{
    public class TinRing : BaseAccessory, IMagikeRemodelable
    {
        public TinRing() : base(ItemRarityID.White, Item.sellPrice(0, 0, 0, 10)) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<TinRing>(0f, ItemID.TinBar, 150, selfStack: 100);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 7;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) -= 0.06f;
        }
    }
}
