using Coralite.Content.ModPlayers;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Accessories
{
    public class ThorwChampion() : BaseFairyAccessory(ItemRarityID.Green, Item.sellPrice(0, 0, 20)), IFairyAccessory
    {
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.DrawJarAimLine = true;
                fcp.AutoShootJar = true;
                fcp.FairyAccessories.Add(this);
            }

            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(EnergyDrink));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SuperChargeWristband>()
                .AddIngredient<LeafScope>()
                .AddIngredient<SpaceFloatingBall>()
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }

        public void ModifyJarInit(BaseJarProj proj)
        {
            proj.MaxFlyTime = (int)(proj.MaxFlyTime * 1.3f) + 5;
            proj.MaxChannelTime = (int)(proj.MaxChannelTime * 0.7f);

            if (proj.MaxChannelTime < 2)
                proj.MaxChannelTime = 2;

            if (Main.rand.NextBool(3))
                proj.Timer = proj.MaxChannelTime / 2;

            proj.MaxChannelDamageBonus += 0.75f;
        }
    }
}
