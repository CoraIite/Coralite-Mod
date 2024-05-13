using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria.ID;
using Terraria;
using Coralite.Core.Prefabs.Items;

namespace Coralite.Content.Items.TheHyacinthSeries
{
    public class RoseGunpowder:BaseAccessory
    {
        public override string Texture => AssetDirectory.TheHyacinthSeriesItems + Name;

        public RoseGunpowder() : base(ItemRarityID.Blue, Item.sellPrice(0, 0, 2))
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetKnockback(DamageClass.Ranged) += 0.1f;
            player.bulletDamage += 0.1f;

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.PollenGunpowderEffect > 0)
                    cp.PollenGunpowderEffect--;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PollenGunpowder>()
                .AddIngredient(ItemID.JungleRose)
                .AddIngredient(ItemID.ExplosivePowder, 12)
                .AddTile(TileID.CrystalBall)
                .Register();
        }
    }
}
