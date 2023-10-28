using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Tiles.Trees
{
    public class SlimeTree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
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
            GrowsOnTileId = new int[3] { TileID.SlimeBlock, TileID.FrozenSlimeBlock, TileID.PinkSlimeBlock };
        }

        public override int DropWood()
        {
            if (Main.rand.NextBool(100))
                return ItemID.SlimeStaff;

            if (Main.rand.NextBool(25))
                return ItemID.PinkGel;

            if (Main.rand.NextBool(8))
                return ModContent.ItemType<Items.Placeable.SlimeSapling>();

            int woodType = Main.rand.Next(2) switch
            {
                0 => ItemID.Gel,
                _ => ModContent.ItemType<Items.Gels.GelFiber>()
            };
            return woodType;
        }

        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/SlimeTree");
        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/SlimeTree_Tops");
        public override Asset<Texture2D> GetBranchTextures() => ModContent.Request<Texture2D>("Coralite/Assets/Tiles/Trees/SlimeTree_Branches");

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {

        }

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<SlimeSapling>();
        }

        public override bool CanDropAcorn()
        {
            return false;
        }

        public override int TreeLeaf()
        {
            return GoreID.TreeLeaf_Hallow;
        }

        public override int CreateDust()
        {
            return DustID.t_Slime;
        }

        public override bool Shake(int x, int y, ref bool createLeaves)
        {
            if (Main.rand.NextBool(10))
            {
                NPC.NewNPC(new EntitySource_ShakeTree(x, y), x, y, NPCID.BlueSlime);
                return false;
            }

            if (Main.rand.NextBool(5))
            {
                int howMany = Main.rand.Next(1, 4);
                Item.NewItem(new EntitySource_ShakeTree(x, y), new Microsoft.Xna.Framework.Vector2(x, y).ToWorldCoordinates(), ItemID.Gel, howMany);
            }

            return false;
        }
    }
}
