using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Attributes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallinePylon : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item+Name;
        public override void SetDefaults()
        {
            // Basically, this a just a shorthand method that will set all default values necessary to place
            // the passed in tile type; in this case, the Example Pylon tile.
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystallinePylonTile>());

            // Another shorthand method that will set the rarity and how much the item is worth.
            Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 10));
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.MagikeSeries2Tile)]
    public class CrystallinePylonTile : ModPylon
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public const int CrystalVerticalFrameCount = 8;

        [AutoLoadTexture(Name = "CrystallinePylonTile_Crystal")]
        public static ATex CrystalTexture { get; private set; }
        [AutoLoadTexture(Name = "CrystallinePylonTile_Highlight")]
        public static ATex CrystalHighlightTexture { get; private set; }
        [AutoLoadTexture(Name = "CrystallinePylonTile_MapIcon")]
        public static ATex MapIcon { get; private set; }

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            // These definitions allow for vanilla's pylon TileEntities to be placed.
            // tModLoader has a built in Tile Entity specifically for modded pylons, which we must extend (see SimplePylonTileEntity)
            TEModdedPylon moddedPylon = ModContent.GetInstance<CrystallinePylonTileEntity>();
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);

            TileObjectData.addTile(Type);

            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            // Adds functionality for proximity of pylons; if this is true, then being near this tile will count as being near a pylon for the teleportation process.
            AddToArray(ref TileID.Sets.CountsAsPylon);

            LocalizedText pylonName = CreateMapEntryName(); //Name is in the localization file
            AddMapEntry(Color.White, pylonName);
        }

        public override NPCShop.Entry GetNPCShopEntry()
        {
            return new NPCShop.Entry(ModContent.ItemType<CrystallinePylon>(), Condition.AnotherTownNPCNearby
                , CoraliteConditions.InCrystallineSkyIsland, Condition.HappyEnoughToSellPylons
                , Condition.NotInEvilBiome);
        }

        public override void MouseOver(int i, int j)
        {
            // Show a little pylon icon on the mouse indicating we are hovering over it.
            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<CrystallinePylon>();
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<CrystallinePylonTileEntity>().Kill(i, j);
        }

        public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData)
        {
            return ModContent.GetInstance<CoraliteTileCount>().InCrystallineSkyIsland;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.5f;
            b = 0.9f;
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // We want to draw the pylon crystal the exact same way vanilla does, so we can use this built in method in ModPylon for default crystal drawing:
            // For the sake of example, lets make our pylon create a bit more dust by decreasing the dustConsequent value down to 1. If you want your dust spawning to be identical to vanilla, set dustConsequent to 4.
            // We also multiply the pylonShadowColor in order to decrease its opacity, so it actually looks like a "shadow"
            DefaultDrawPylonCrystal(spriteBatch, i, j, CrystalTexture, CrystalHighlightTexture, new Vector2(0f, -12f)
                , (Color.White * 0.1f) with { A = 5 }, Coralite.CrystallinePurple, 8, CrystalVerticalFrameCount);
        }

        public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale)
        {
            // Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
            bool mouseOver = DefaultDrawMapIcon(ref context, MapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
            DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<CrystallinePylon>().DisplayName.Key, ref mouseOverText);
        }
    }

    public sealed class CrystallinePylonTileEntity : TEModdedPylon { }
}
