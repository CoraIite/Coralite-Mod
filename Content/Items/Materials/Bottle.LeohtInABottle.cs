using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Materials
{
    public class LeohtInABottle : BaseMaterial, IMagikeCraftable
    {
        public LeohtInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Orange, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<LeohtInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            //PolymerizeRecipe.CreateRecipe<HeatanInABottle>(60)
            //    .SetMainItem(ItemID.Bottle)
            //    .AddIngredient(ItemID.LivingFireBlock, 20)
            //    .AddIngredient<EmpyrosPowder>(7)
            //    .Register();
        }
    }

    public class LeohtInABottleTile : ModTile
    {
        public override string Texture => AssetDirectory.Materials + Name;

        public const int FrameWidth = 18;
        public const int FrameHeight = 18 * 2;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;

            DustType = DustID.OrangeTorch;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            AddMapEntry(new Microsoft.Xna.Framework.Color(208, 86, 29));
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Helper.DrawMultWine(i, j, 1, 2);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile selfTile = Main.tile[i, j];
            if (Main.LightingEveryFrame && selfTile.TileFrameX % FrameWidth == 0 && selfTile.TileFrameY % FrameHeight == 0)
                ModContent.GetInstance<CoraliteTileDrawing>().AddSpecialPoint(i, j, CoraliteTileDrawing.TileCounterType.MultiTileVine);
            return false;
        }
    }

}
