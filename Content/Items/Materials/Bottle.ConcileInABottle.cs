﻿using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Materials
{
    /// <summary>
    /// 瓶中调和
    /// </summary>
    public class ConcileInABottle : BaseMaterial, IMagikeCraftable
    {
        public ConcileInABottle() : base(9999, Item.sellPrice(0, 0, 1, 50), ItemRarityID.Pink, AssetDirectory.Materials) { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToPlaceableTile(ModContent.TileType<ConcileInABottleTile>());
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeCraftRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<ConcileInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 6))
                .AddIngredient(ItemID.IronAnvil)
                .AddIngredient(ItemID.TinkerersWorkshop)
                .Register();

            MagikeCraftRecipe.CreateRecipe(ItemID.Bottle, ModContent.ItemType<ConcileInABottle>(), MagikeHelper.CalculateMagikeCost(MALevel.Glistent, 6))
                .AddIngredient(ItemID.LeadAnvil)
                .AddIngredient(ItemID.TinkerersWorkshop)
                .Register();
        }
    }

    public class ConcileInABottleTile : ModTile
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

            DustType = DustID.PinkFairy;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.MagicCrystalPink);
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
