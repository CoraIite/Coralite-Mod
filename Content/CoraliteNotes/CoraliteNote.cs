using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.CoraliteNotes
{
    [PlayerEffect]
    public class CoraliteNote : ModItem
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 30;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return true;

            var ui = UILoader.GetUIState<CoraliteNoteUIState>();
            if (!ui.visible)
            {
                ui.OpenBook();
                ui.Recalculate();
            }

            Main.playerInventory = false;
            return true;
        }

        public override void UpdateInventory(Player player)
        {
            if (GamePlaySystem.CoraliteNoteConsultAbility && player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(CoraliteNote));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.Coral, 2)
                .AddTile(TileID.Bookcases)
                .DisableDecraft()
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.MiscItems)]
    public class CoraliteNoteTile : ModTile
    {
        public override string Texture => AssetDirectory.MiscItems + Name;

        public static ATex CoraliteNoteTileTop { get; set; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.Direction = TileObjectDirection.None;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            DustType = DustID.Coralstone;

            AddMapEntry(Color.Pink, CreateMapEntryName());
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return [new Item(ModContent.ItemType<CoraliteNote>()), new Item(ModContent.ItemType<CoralBoneWorkbenchItem>())];
        }


        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            Point p = new(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            Texture2D texture = CoraliteNoteTileTop.Value;

            Vector2 origin = texture.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(16+8, 8);

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, offset * 4f);

            Vector2 offset2 = Vector2.Zero;

            drawPos += offset2;

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, origin, 1f, 0, 0f);
        }

        public override bool RightClick(int i, int j)
        {
            WorldGen.KillTile(i, j);
            return true;
        }
    }

    public class CoralBoneWorkbenchItem() : BaseWorkBenchItem(Item.sellPrice(0, 1), ItemRarityID.Pink, ModContent.TileType<CoralBoneWorkbench>(), AssetDirectory.MiscItems)
    {
    }

    public class CoralBoneWorkbench() : BaseWorkBenchTile(DustID.Coralstone, new Color(150, 120, 100), AssetDirectory.MiscItems)
    {

    }
}
