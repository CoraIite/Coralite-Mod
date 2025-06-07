using Coralite.Content.Dusts;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Tiles.MagikeSeries2;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
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
            return ModContent.GoreType<ChalcedonyTreeLeaf>();
        }

        public override int CreateDust()
        {
            return ModContent.DustType<ChalcedonyDust>();
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

    public class ChalcedonyTreeLeaf : ModGore
    {
        public override string Texture => "Coralite/Assets/Tiles/Trees/ChalcedonyTreeLeaf";

        public override void SetStaticDefaults()
        {
            ChildSafety.SafeGore[Type] = true; // Leaf gore should appear regardless of the "Blood and Gore" setting
            GoreID.Sets.SpecialAI[Type] = 3; // Falling leaf behavior
            GoreID.Sets.PaintedFallingLeaf[Type] = true; // This is used for all vanilla tree leaves, related to the bigger spritesheet for tile paints
        }

        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.Frame = new SpriteFrame(1, 5, 0, (byte)Main.rand.Next(5));
            gore.Frame.PaddingX = 0;
            gore.Frame.PaddingY = 0;
        }

        public override bool Update(Gore gore)
        {
            UpdateLeaf(gore);
            return false;
        }

        /// <summary>
        /// 源码里复制黏贴出来的，我也看不懂写的什么东西
        /// </summary>
        /// <param name="gore"></param>
        private void UpdateLeaf(Gore gore)
        {
            Vector2 vector = gore.position + new Vector2(12f) / 2f - new Vector2(4f) / 2f;
            vector.Y -= 4f;
            Vector2 vector2 = gore.position - vector;
            if (gore.velocity.Y < 0f)
            {
                Vector2 vector3 = new Vector2(gore.velocity.X, -0.2f);
                int num = 4;
                num = (int)(num * 0.9f);
                Point point = (new Vector2(num, num) / 2f + vector).ToTileCoordinates();
                if (!WorldGen.InWorld(point.X, point.Y))
                {
                    gore.active = false;
                    return;
                }

                Tile tile = Main.tile[point.X, point.Y];
                if (tile == null)
                {
                    gore.active = false;
                    return;
                }

                int num2 = 6;
                Rectangle rectangle = new Rectangle(point.X * 16, point.Y * 16 + tile.LiquidAmount / 16, 16, 16 - tile.LiquidAmount / 16);
                Rectangle value = new Rectangle((int)vector.X, (int)vector.Y + num2, num, num);
                bool flag = tile != null && tile.LiquidAmount > 0 && rectangle.Intersects(value);
                if (flag)
                {
                    if (tile.LiquidType == LiquidID.Honey)
                    {
                        vector3.X = 0f;
                    }
                    else if (tile.LiquidType == LiquidID.Lava)
                    {
                        gore.active = false;
                        for (int i = 0; i < 5; i++)
                            Dust.NewDust(gore.position, num, num, DustID.Smoke, 0f, -0.2f);
                    }
                    else
                    {
                        vector3.X = Main.WindForVisuals;
                    }

                    if (gore.position.Y > Main.worldSurface * 16f)
                        vector3.X = 0f;
                }

                if (!WorldGen.SolidTile(point.X, point.Y + 1) && !flag)
                {
                    gore.velocity.Y = 0.1f;
                    gore.timeLeft = 0;
                    gore.alpha += 20;
                }

                vector3 = Collision.TileCollision(vector, vector3, num, num);
                if (flag)
                    gore.rotation = vector3.ToRotation() + (float)Math.PI / 2f;

                vector3.X *= 0.94f;
                if (!flag || (vector3.X > -0.01f && vector3.X < 0.01f))
                    vector3.X = 0f;

                if (gore.timeLeft > 0)
                    gore.timeLeft -= GoreID.Sets.DisappearSpeed[gore.type];
                else
                    gore.alpha += GoreID.Sets.DisappearSpeedAlpha[gore.type];

                gore.velocity.X = vector3.X;
                gore.position.X += gore.velocity.X;
                return;
            }

            gore.velocity.Y += (float)Math.PI / 180f;
            Vector2 vector4 = new Vector2(Vector2.UnitY.RotatedBy(gore.velocity.Y).X * 1f, Math.Abs(Vector2.UnitY.RotatedBy(gore.velocity.Y).Y) * 1f);
            int num3 = 4;
            if (gore.position.Y < Main.worldSurface * 16f)
                vector4.X += Main.WindForVisuals * 4f;

            Vector2 vector5 = vector4;
            vector4 = Collision.TileCollision(vector, vector4, num3, num3);
            Vector4 vector6 = Collision.SlopeCollision(vector, vector4, num3, num3, 1f);
            gore.position.X = vector6.X;
            gore.position.Y = vector6.Y;
            vector4.X = vector6.Z;
            vector4.Y = vector6.W;
            gore.position += vector2;
            if (vector4 != vector5)
                gore.velocity.Y = -1f;

            Point point2 = (new Vector2(gore.Width, gore.Height) * 0.5f + gore.position).ToTileCoordinates();
            if (!WorldGen.InWorld(point2.X, point2.Y))
            {
                gore.active = false;
                return;
            }

            Tile tile2 = Main.tile[point2.X, point2.Y];
            if (tile2 == null)
            {
                gore.active = false;
                return;
            }

            int num4 = 6;
            Rectangle rectangle2 = new Rectangle(point2.X * 16, point2.Y * 16 + tile2.LiquidAmount / 16, 16, 16 - tile2.LiquidAmount / 16);
            Rectangle value2 = new Rectangle((int)vector.X, (int)vector.Y + num4, num3, num3);
            if (tile2 != null && tile2.LiquidAmount > 0 && rectangle2.Intersects(value2))
                gore.velocity.Y = -1f;

            gore.position += vector4;
            gore.rotation = vector4.ToRotation() + (float)Math.PI / 2f;
            if (gore.timeLeft > 0)
                gore.timeLeft -= GoreID.Sets.DisappearSpeed[gore.type];
            else
                gore.alpha += GoreID.Sets.DisappearSpeedAlpha[gore.type];
        }
    }
}
