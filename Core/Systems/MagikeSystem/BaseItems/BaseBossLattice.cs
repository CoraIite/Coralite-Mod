using Terraria;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class BaseBossLattice(int bossBagItemType) : ModItem
    {
        public override string Texture => AssetDirectory.MagikeLattices + Name;

        public override void SetDefaults()
        {
            Item.master = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTime = Item.useAnimation = 30;

            Item.UseSound = CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            player.QuickSpawnItem(player.GetSource_FromThis(), bossBagItemType);
        }
    }
}
