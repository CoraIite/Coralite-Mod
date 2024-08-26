using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories
{
    public class TinRing : BaseAccessory, IMagikeCraftable
    {
        public TinRing() : base(ItemRarityID.White, Item.sellPrice(0, 0, 0, 10)) { }

        public void AddMagikeCraftRecipe()
        {
            //MagikeSystem.AddRemodelRecipe<TinRing>(0f, ItemID.TinBar, 150, selfStack: 100);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) -= 0.06f;
        }
    }
}
