using Coralite.Content.ModPlayers;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    [PlayerEffect]
    public class EnergyDrink() : BaseFairyAccessory(ItemRarityID.Blue, Item.sellPrice(0, 0, 20)), IFairyAccessory
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.FairyAccessories.Add(this);

            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(EnergyDrink));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.JojaCola)
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.Shiverthorn)
                .AddTile(TileID.Bottles)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.BattlePotion)
                .AddIngredient(ItemID.Blinkroot)
                .AddIngredient(ItemID.Shiverthorn)
                .AddTile(TileID.Bottles)
                .Register();
        }

        public void ModifyJarInit(BaseJarProj proj)
        {
            proj.MaxChannelTime = (int)(proj.MaxChannelTime * 0.8f);

            if (proj.MaxChannelTime < 2)
                proj.MaxChannelTime = 2;

            proj.MaxChannelDamageBonus += 0.5f;
        }
    }
}
