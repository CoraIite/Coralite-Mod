using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Tiles.MagikeSeries2;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Tiles.Trees
{
    public class ChalcedonyTree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new()
        {
            UseSpecialGroups = true,    //使用特殊组
            SpecialGroupMinimalHueValue = 11f / 72f,    //特殊组最小色调值
            SpecialGroupMaximumHueValue = 0.25f,    //特殊组最大色调值
            SpecialGroupMinimumSaturationValue = 0.88f, //特殊组最小饱和度值
            SpecialGroupMaximumSaturationValue = 1f     //特殊组最大饱和度值
            //HueTestOffset 色调测试偏移
            //UseWallShaderHacks 使用墙壁着色器技巧
            //InvertSpecialGroupResult 反转特殊组结果
        };

        public override void SetStaticDefaults()
        {
            // Makes Example Tree grow on ExampleBlock
            GrowsOnTileId =
                [
                ModContent.TileType<SkarnTile>(),
                ModContent.TileType<SmoothSkarnTile>(),
                ModContent.TileType<ChalcedonySkarn>(),
                ModContent.TileType<ChalcedonySmoothSkarn>()
                ];
        }

        public override int DropWood()
        {
            if (Main.rand.NextBool(8))
                return ModContent.ItemType<Items.MagikeSeries2.ChalcedonySapling>();

            return ModContent.ItemType<Chalcedony>();
        }

        public override ATex GetTexture() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/ChalcedonyTree");
        public override ATex GetTopTextures() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/ChalcedonyTree_Tops");
        public override ATex GetBranchTextures() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/ChalcedonyTree_Branches");

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {

        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<ChalcedonySapling>();
        }

        public override bool CanDropAcorn()
        {
            return false;
        }

        public override int TreeLeaf()
        {
            return GoreID.TreeLeaf_Boreal;
        }

        public override int CreateDust()
        {
            return DustID.t_Slime;
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            //if (Main.rand.NextBool(10))
            //{
            //    NPC.NewNPC(new EntitySource_ShakeTree(x, y), x * 16, y * 16, NPCID.BlueSlime);
            //    return false;
            //}

            //if (Main.rand.NextBool(75))
            //{
            //    int itemType = Main.rand.NextFromList(
            //        ModContent.ItemType<Woodbine>(),
            //        ModContent.ItemType<Princesstrawberry>(),
            //        ItemID.SlimeStaff);

            //    Item.NewItem(new EntitySource_ShakeTree(x, y), new Vector2(x, y).ToWorldCoordinates(), itemType);
            //    return false;
            //}

            //if (Main.rand.NextBool(5))
            //{
            //    int howMany = Main.rand.Next(1, 4);
            //    Item.NewItem(new EntitySource_ShakeTree(x, y), new Vector2(x, y).ToWorldCoordinates(), ItemID.Gel, howMany);
            //}

            return false;
        }
    }
}
