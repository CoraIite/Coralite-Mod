using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Accessories
{
    public class CopperRing : BaseAccessory, IMagikeRemodelable
    {
        public CopperRing() : base(ItemRarityID.White, Item.sellPrice(0, 0, 0, 10)) { }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<CopperRing>(0f, ItemID.CopperBar, 150, selfStack: 100);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.defense = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Generic) -= 0.06f;
        }
    }
}
