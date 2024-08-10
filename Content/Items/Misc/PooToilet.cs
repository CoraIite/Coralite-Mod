using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Misc
{
    public class PooToilet : BaseToiletItem
    {
        public PooToilet() : base(0, ItemRarityID.White, ModContent.TileType<PooToiletTile>(), AssetDirectory.MiscItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PoopBlock)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class PooToiletTile : BaseToiletTile<PooToilet>
    {
        public PooToiletTile() : base(DustID.Poop, Color.Brown, AssetDirectory.MiscItems)
        {
        }

        public override void HitWire(int i, int j)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust d = Dust.NewDustPerfect(new Vector2(i, j) * 16 + new Vector2(Main.rand.NextFloat(0, 16), Main.rand.NextFloat(0, 16))
                    , DustID.Poop, -Vector2.UnitY * Main.rand.NextFloat(0, 3));
            }
        }
    }

}
