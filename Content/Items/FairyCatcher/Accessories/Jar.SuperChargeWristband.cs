using Coralite.Content.Items.Fairies.FairyEVBonus;
using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class SuperChargeWristband() : BaseFairyAccessory(ItemRarityID.Green, Item.sellPrice(0, 0, 20)), IFairyAccessory
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.AutoShootJar = true;
                fcp.FairyAccessories.Add(this);
            }

            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(EnergyDrink));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyDrink>()
                .AddIngredient<ReboundButton>()
                .AddIngredient<AttackDroplet>()
                .AddIngredient(ItemID.DemoniteBar, 13)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<EnergyDrink>()
                .AddIngredient<ReboundButton>()
                .AddIngredient<AttackDroplet>()
                .AddIngredient(ItemID.CrimtaneBar, 13)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }

        public void ModifyJarInit(BaseJarProj proj)
        {
            proj.MaxChannelTime = (int)(proj.MaxChannelTime * 0.75f);

            if (proj.MaxChannelTime < 2)
                proj.MaxChannelTime = 2;

            if (Main.rand.NextBool(4))
                proj.Timer = proj.MaxChannelTime / 2;

            proj.MaxChannelDamageBonus += 0.65f;
        }
    }
}
