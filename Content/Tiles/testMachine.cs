using Coralite.Content.UI;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles
{
    public class BaseMachineEntity : ModTileEntity
    {
        public Item father;
        public Item mother;
        public Item son;
        public Item catalyst;//催化剂
        public int maglite = 0;//魔能
        public readonly int magliteMax = 2000;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y);
            return tile.HasTile && tile.TileType == ModContent.TileType<testMachine>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("father", father);
            tag.Add("mother", mother);
            tag.Add("son", son);
            tag.Add("catalyst", catalyst);
            tag.Add("maglite", maglite);
        }

        public override void LoadData(TagCompound tag)
        {
            father = tag.Get<Item>("father");
            mother = tag.Get<Item>("mother");
            son = tag.Get<Item>("son");
            catalyst = tag.Get<Item>("catalyst");
            maglite = tag.GetInt("maglite");
        }
    }

    public class testMachine:ModTile
    {
        public override string Texture => AssetDirectory.Tiles+Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;//帧重要
            //Main.tileObsidianKill[tile.Type] = true;
            Main.tileNoFail[Type] = true;//沙子等的特性，受重力影响
            //Main.tileLighted[Type] = tileLighted;//光照
            Main.tileSolid[Type] = false;//实心
            Main.tileSolidTop[Type] = false;//顶部实心，类似平台
            Main.tileNoAttach[Type] = false;//
            Main.tileTable[Type] = false;//是桌子
            Main.tileLavaDeath[Type] = true;//会被岩浆烫
            Main.tileCut[Type] = false;//被弹幕切掉
            Main.tileBlockLight[Type] = false;//阻挡光转递

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            //TileObjectData.newTile.Width = 1;
            //TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[2] {16,16};
            TileObjectData.newTile.CoordinateWidth = 16;
            //TileObjectData.newTile.DrawYOffset = 0;
            TileObjectData.newTile.CoordinatePadding = 0;
            //TileObjectData.newTile.AnchorValidTiles = AnchorValidTiles;
            //种在这上面时就可以使用快速再种植功能
            //TileObjectData.newTile.AnchorAlternateTiles = AnchorAlternateTiles;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BaseMachineEntity>().Hook_AfterPlacement, -1, 0, false);
            TileObjectData.addTile(Type);
            //HitSound = HitSound;
            //tile.DustType = DustType;
            MineResist = 1;
            MinPick = 1;
        }

        public override bool RightClick(int i, int j)
        {
            if(BotanicalHelper.TryGetTileEntityForMultTile(Type,i, j, out BaseMachineEntity entity))
            {
                Main.playerInventory = true;
                CrossBreedUI.machineEntity = entity;
                CrossBreedUI.visible = !CrossBreedUI.visible;
            }

            return true;
        }
    }
}
