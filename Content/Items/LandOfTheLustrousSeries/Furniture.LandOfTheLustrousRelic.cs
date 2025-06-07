using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.TileProcessors;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class LandOfTheLustrousRelic : BaseRelicItem
    {
        public LandOfTheLustrousRelic() : base(ModContent.TileType<LandOfTheLustrousRelicTile>(), AssetDirectory.LandOfTheLustrousSeriesItems) { }
    }

    public class LandOfTheLustrousRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.MiscTiles + "LandOfTheLustrousRelicPedestal";
        public override string RelicTextureName => AssetDirectory.MiscTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<LandOfTheLustrousRelic>())
            ];
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
            {
                Point p = Helper.FindTopLeftPoint(i, j);
                if (i == p.X && j == p.Y && (int)Main.timeForVisualEffects % 30 < 5 && Main.rand.NextBool(5))
                {
                    const float length = 16 * 2;
                    Color c = Main.rand.NextFromList(Color.White, Main.hslToRgb(Main.GlobalTimeWrappedHourly % 1, 1, 0.7f));
                    var p2 = PRTLoader.NewParticle<HexagramParticle>(p.ToWorldCoordinates(16 + 8, 16)
                        + Main.rand.NextVector2Circular(length, length),
                         Vector2.UnitX, c, Scale: Main.rand.NextFloat(0.075f, 0.125f));
                    p2.Rotation = -1.57f;
                }
            }
        }

        public override void DrawRelicTop(SpriteBatch spriteBatch, Texture2D texture, Vector2 offScreen, Point p, Tile tile)
        {
            //绘制底部
            Rectangle frame = texture.Frame(2, 1, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 69f);

            Color color = Lighting.GetColor(p.X, p.Y);

            bool direction = tile.TileFrameY / FrameHeight != 0;
            SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0, -40);

            // 绘制底座
            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);


            //绘制悬浮
            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);

            frame = texture.Frame(2, 1, 0, 0);
            drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -38f) + new Vector2(0f, offset * 4f);

            spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

            // 绘制周期性发光效果
            float scale = ((float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f) + 0.7f;
            Color effectColor = color;
            effectColor.A = 0;
            effectColor = effectColor * 0.1f * scale;
            for (float m = 0f; m < 1f; m += 355f / (678f * (float)Math.PI))
                spriteBatch.Draw(texture, drawPos + ((TwoPi * m).ToRotationVector2() * (6f + (offset * 2f))), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }

    public class LandOfTheLustrousRelicTP : TileProcessor
    {
        public override int TargetTileID => ModContent.TileType<LandOfTheLustrousRelicTile>();

        private int time;

        public override void Update()
        {
            //每1分半生成一只宝石小动物
            if (++time > 60 * 90)
            {
                time = 0;

                //我是超级打表王
                short type = Main.rand.NextFromList(
                    NPCID.GemBunnyAmber,
                    NPCID.GemBunnyAmethyst,
                    NPCID.GemBunnyDiamond,
                    NPCID.GemBunnyEmerald,
                    NPCID.GemBunnyRuby,
                    NPCID.GemBunnySapphire,
                    NPCID.GemBunnyTopaz,

                    NPCID.GemSquirrelAmber,
                    NPCID.GemSquirrelAmethyst,
                    NPCID.GemSquirrelDiamond,
                    NPCID.GemSquirrelEmerald,
                    NPCID.GemSquirrelRuby,
                    NPCID.GemSquirrelSapphire,
                    NPCID.GemSquirrelTopaz
                    );

                NPC.NewNPC(new EntitySource_TileUpdate(Position.X, Position.Y)
                    , (int)(PosInWorld.X + 16), (int)(PosInWorld.Y + 3 * 16), type);
            }
        }
    }
}