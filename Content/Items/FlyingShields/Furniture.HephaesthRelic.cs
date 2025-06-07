using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class HephaesthRelic : BaseRelicItem
    {
        public HephaesthRelic() : base(ModContent.TileType<HephaesthRelicTile>(), AssetDirectory.FlyingShieldItems) { }
    }

    public class HephaesthRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.MiscTiles + "HephaesthRelicPedestal";
        public override string RelicTextureName => AssetDirectory.MiscTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            AdjTiles =
            [
                TileID.Furnaces,
                TileID.Hellforge,
                TileID.GlassKiln,
                TileID.AdamantiteForge,
                TileID.LunarCraftingStation,
                ModContent.TileType<AncientFurnaceTile>()
            ];
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<HephaesthRelic>())
            ];
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main.rand.NextBool(2))
            {
                Point p = Helper.FindTopLeftPoint(i, j);
                if (i == p.X && j == p.Y)
                {
                    Vector2 pos = p.ToWorldCoordinates(8 + 16, 8 + 40);

                    const float angle = 0.4f;
                    const float randAngle = 0.2f;

                    int count = Main.rand.Next(1, 4);
                    for (int k = 0; k < count; k++)
                    {
                        Vector2 direction = (Main.rand.NextFromList(-angle, MathHelper.Pi + angle) + Main.rand.NextFloat(-randAngle, randAngle)).ToRotationVector2();
                        Dust d = Dust.NewDustPerfect(pos + direction * Main.rand.NextFloat(0, 16) + new Vector2(0, Main.rand.NextFloat(-4, 4)), DustID.Torch
                            , direction.RotateByRandom(-angle / 2, angle / 2) * Main.rand.NextFromList(0.1f, 1.5f), Scale: Main.rand.NextFloat(1, 2f));
                        d.noGravity = true;
                    }
                }
            }
        }

        public override void DrawRelicTop(SpriteBatch spriteBatch, Texture2D texture, Vector2 offScreen, Point p, Tile tile)
        {
            //绘制底部
            Rectangle frame = texture.Frame(2, 1, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 74f);

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
            drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

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
}
