using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PhantomCrystalBall() : BasePlaceableItem(Item.sellPrice(0, 1), ItemRarityID.Red, ModContent.TileType<PhantomCrystalBallTile>(), AssetDirectory.LandOfTheLustrousSeriesItems)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalBall)
                .AddIngredient(ItemID.AlchemyTable)
                .AddIngredient<RedJades.MagicCraftStation>()
                .AddTile<AncientFurnaceTile>()
                .Register();
        }
    }

    public class PhantomCrystalBallTile : ModTile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.Origin = new Terraria.DataStructures.Point16(0, 2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AdjTiles = new int[]
            {
                TileID.CrystalBall,
                TileID.AlchemyTable,
                ModContent.TileType<MagicCraftStation>()
            };

            DustType = DustID.WitherLightning;
            MinPick = 160;

            AddMapEntry(Color.Purple, CreateMapEntryName());
        }


        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Main.LocalPlayer.Center);
            Main.LocalPlayer.AddBuff(BuffID.Clairvoyance, 18000);
            return true;
        }
    }
}
