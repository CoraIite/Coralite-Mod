using Coralite.Content.Evevts.ShadowCastle;
using Coralite.Content.Items.ShadowCastle;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.ShadowCastle
{
    public class BlackHoleChestTile : ModTile
    {
        public override string Texture => AssetDirectory.ShadowCastleTiles + Name;

        public override void SetStaticDefaults()
        {
            //Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            //TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad);

            //Main.tileGlowMask[Type] = TextureAssets.GlowMask.Length;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            Main.tileSpelunker[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileContainer[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileOreFinderPriority[Type] = 500;
            Main.tileShine[Type] = 1000;
            Main.tileShine2[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.BasicChest[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            
            DustType = DustID.Granite;
            AdjTiles = new int[] { TileID.Containers };
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(245, 115, 31), name, MapChestName);
            
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;//new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
        }

        
        //public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool IsLockedChest(int i, int j) => false;

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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.15f;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Chest.DestroyChest(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            //BlackHoleTrials.DownedBlackHoleTrails = false;
            if (!BlackHoleTrials.DownedBlackHoleTrails && !Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<BlackHoleMainProj>()))
            {
                //开启试炼
                Vector2 pos = new Vector2(i - tile.TileFrameX / 18 + 1, j - tile.TileFrameY / 18 + 1) * 16;
                Projectile.NewProjectile(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), pos,
                    Vector2.Zero, ModContent.ProjectileType<BlackHoleMainProj>(), 100, 0, Main.myPlayer);
                return false;
            }

            Player player = Main.LocalPlayer;
            Main.mouseRightRelease = false;
            int left = i;
            int top = j;
            if (tile.TileFrameX % 36 != 0)
                left--;

            if (tile.TileFrameY != 0)
                top--;

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
                player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "黑洞箱子";
                if (player.cursorItemIconText == "黑洞箱子")
                {
                    player.cursorItemIconID = ModContent.ItemType<BlackHoleChest>();
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
                player.cursorItemIconID = 0;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            //Vector2 offScreen = new Vector2(Main.offScreenRange);
            //if (Main.drawToScreen)
            //    offScreen = Vector2.Zero;

            //spriteBatch.Draw(drawData.drawTexture, new Vector2(i, j) * 16 + offScreen - Main.screenPosition
            //    , new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, 18, 18), Color.White);
            //drawData.finalColor = Color.White;
            //drawData.colorTint = Color.White;
            //drawData.glowColor = Color.White;
            drawData.tileLight=Color.White;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //Tile tile = Main.tile[i, j];
            //Texture2D tex = TextureAssets.Tile[tile.TileType].Value;

            //spriteBatch.Draw(tex, new Vector2(i, j) * 16+offScreen-Main.screenPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 18, 18), Color.White);
            return true;
        }
    }
}
