using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Coralite.Content.Items.Crimson
{
    public class VertebraeChest : BaseChestItem
    {
        public VertebraeChest() : base(Item.sellPrice(0, 0, 0, 10), ItemRarityID.Blue, ModContent.TileType<VertebraeChestTile>(), AssetDirectory.CrimsonItems)
        { }
    }

    public class VertebraeChestTile : ModTile
    {
        public override string Texture => AssetDirectory.CrimsonItems + Name;

        public LocalizedText TryUnlock => this.GetLocalization("TryUnlock");

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringGeneration[TileID.CrimtaneBrick] = false;
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            Main.tileSpelunker[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 1200;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;
            TileID.Sets.FriendlyFairyCanLureTo[Type] = true;

            DustType = DustID.Crimson;
            AdjTiles = new int[] { TileID.Containers };

            // Other tiles with just one map entry use CreateMapEntryName() to use the default translationkey, "MapEntry"
            // Since ExampleChest needs multiple, we register our own MapEntry keys
            AddMapEntry(Color.Red, this.GetLocalization("MapEntry0"), MapChestName);
            AddMapEntry(Color.Red, this.GetLocalization("MapEntry1"), MapChestName);

            // Style 1 is ExampleChest when locked. We want that tile style to drop the ExampleChest item as well. Use the Chest Lock item to lock this chest.
            // No item places ExampleChest in the locked style, so the automatically determined item drop is unknown, this is why RegisterItemDrop is necessary in this situation. 
            RegisterItemDrop(ModContent.ItemType<VertebraeChest>(), 1);
            // Sometimes mods remove content, such as tile styles, or tiles accidentally get corrupted. We can, if desired, register a fallback item for any tile style that doesn't have an automatically determined item drop. This is done by omitting the tileStyles parameter.
            RegisterItemDrop(ModContent.ItemType<VertebraeChest>());

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] {
                TileID.MagicalIceBlock,
                TileID.Boulder,
                TileID.BouncyBoulder,
                TileID.LifeCrystalBoulder,
                TileID.RollingCactus
            };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
        }

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool IsLockedChest(int i, int j)
        {
            return Main.tile[i, j].TileFrameX / 36 == 1;
        }

        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
        {
            return true;
        }

        public override bool LockChest(int i, int j, ref short frameXAdjustment, ref bool manual)
        {
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            // We need to return true only if the tile style is the unlocked variant of a chest that supports locking. 
            if (style == 0)
            {
                // We can check other conditions as well, such as how biome chests can't be locked until Plantera is defeated
                return true;
            }
            return false;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public static string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
                return Language.GetTextValue("LegacyChestType.0");

            if (Main.chest[chest].name == "")
                return name;

            return name + ": " + Main.chest[chest].name;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            if (Main.tile[left, top].TileFrameX > 18)
            {
                if (player.ConsumeItem(ModContent.ItemType<VertebraeKey>()))
                {
                    Chest.Unlock(left, top);
                    return true;
                }

                Main.NewText(TryUnlock.Value, Color.Red);
                return false;
            }

            if (player.sign >= 0)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.sign = -1;
                Main.editSign = false;
                Main.npcChatText = "";
            }

            if (Main.editChest)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                Main.editChest = false;
                Main.npcChatText = "";
            }

            if (player.editedChestName)
            {
                NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
                player.editedChestName = false;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (left == player.chestX && top == player.chestY && player.chest >= 0)
                {
                    player.chest = -1;
                    Recipe.FindRecipes();
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
                else
                {
                    NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
                    Main.stackSplit = 600;
                }
            }
            else
            {
                int chest = Chest.FindChest(left, top);
                if (chest >= 0)
                {
                    Main.stackSplit = 600;
                    if (chest == player.chest)
                    {
                        player.chest = -1;
                        SoundEngine.PlaySound(SoundID.MenuClose);
                    }
                    else
                    {
                        SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
                        player.chest = chest;
                        Main.playerInventory = true;
                        Main.recBigList = false;
                        player.chestX = left;
                        player.chestY = top;
                    }

                    Recipe.FindRecipes();
                }
            }
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            Tile tile = Main.tile[i, j];
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
                player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
            else
            {
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "椎骨箱";
                if (player.cursorItemIconText == "椎骨箱")
                {
                    if (tile.TileFrameX / 36 != 0)
                        player.cursorItemIconID = ModContent.ItemType<VertebraeKey>();
                    else
                        player.cursorItemIconID = ModContent.ItemType<VertebraeChest>();

                    player.cursorItemIconText = "";
                }
            }

            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override void MouseOverFar(int i, int j)
        {
            MouseOver(i, j);
            Player player = Main.LocalPlayer;
            if (player.cursorItemIconText == "")
            {
                player.cursorItemIconEnabled = false;
                player.cursorItemIconID = ItemID.None;
            }
        }
    }
}
