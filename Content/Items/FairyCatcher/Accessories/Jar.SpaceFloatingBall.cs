using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class SpaceFloatingBall() : BaseFairyAccessory(ItemRarityID.Green, Item.sellPrice(0, 1)), IFairyAccessory
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.FairyAccessories.Add(this);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<SpaceFloatingBall>(equippedItem, incomingItem
                , ModContent.ItemType<ThorwChampion>()
                );
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SunplateBlock, 10)
                .AddIngredient(ItemID.MeteoriteBar, 5)
                .AddTile(TileID.WorkBenches)
                .DisableDecraft()
                .Register();
        }

        public void ModifyJarInit(BaseJarProj proj)
        {
            proj.MaxFlyTime = (int)(proj.MaxFlyTime * 1.2f);
        }
    }
}
