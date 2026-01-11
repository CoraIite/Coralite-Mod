using Coralite.Content.Items.Magike.Columns;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.Particles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class GiantLens : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeRefractors + Name;

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.maxStack = 5;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.consumable = true;

            Item.value = Item.sellPrice(0, 0, 10);
        }

        public override bool CanUseItem(Player player)
        {
            Point p = Main.MouseWorld.ToTileCoordinates();
            Tile t = Framing.GetTileSafely(p);
            if (t.HasTile && t.TileType == ModContent.TileType<BasicColumnTile>())
            {
                Point16? p2 = ToTopLeft(p);

                if (p2.HasValue)
                {
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            Framing.GetTileSafely(p2.Value + new Point16(i, j)).ClearTile();

                    if (TryGetEntityWithTopLeft(p2.Value, out MagikeTP tp))
                    {
                        tp.Kill();
                    }

                    //WorldGen.KillTile(p2.Value.X, p2.Value.Y, false, false, true);
                    WorldGen.PlaceTile(p2.Value.X + 1, p2.Value.Y + 2, ModContent.TileType<FresnelMirrorTile>());

                    TileLoader.PlaceInWorld(p2.Value.X + 1, p2.Value.Y + 2, ContentSamples.ItemsByType[ModContent.ItemType<FresnelMirror>()]);
                    TileRenewalController.Spawn(p2.Value, Coralite.MagicCrystalPink);

                    Item.stack--;
                    if (Item.stack < 1)
                        Item.TurnToAir();

                    return true;
                }
            }

            return false;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<MagicCrystal, GiantLens>(CalculateMagikeCost<CrystalLevel>(6, 60), 15)
                .AddIngredient(ItemID.Glass, 5)
                .Register();
        }
    }
}
