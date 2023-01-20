using Coralite.Content.Items.Machines;
using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.Machines
{
    public class CrossBreedMachine_G1 : ModTile
    {
        public override string Texture => AssetDirectory.MachineTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;//帧重要
            Main.tileSolid[Type] = false;//实心
            Main.tileSolidTop[Type] = false;//顶部实心，类似平台
            Main.tileNoAttach[Type] = false;//
            Main.tileTable[Type] = false;//是桌子
            Main.tileLavaDeath[Type] = false;//会被岩浆烫
            Main.tileCut[Type] = false;//被弹幕切掉
            Main.tileBlockLight[Type] = false;//阻挡光转递

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<CrossBreedMachineEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            MineResist = 1;
            MinPick = 20;
        }

        public override bool RightClick(int i, int j)
        {
            if (BotanicalHelper.TryGetTileEntityForMultTile(Type, i, j, out CrossBreedMachineEntity entity))
            {
                Main.playerInventory = true;
                CrossBreedUI.machineEntity = entity;
                CrossBreedUI.visible = !CrossBreedUI.visible;
                UILoader.GetUIState<CrossBreedUI>().Recalculate();
            }

            return true;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), ModContent.ItemType<CrossBreedMachine_G1Item>());
        }
    }
}
