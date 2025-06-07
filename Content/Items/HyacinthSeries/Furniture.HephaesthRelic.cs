using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class HyacinthRelic : BaseRelicItem
    {
        public HyacinthRelic() : base(ModContent.TileType<HyacinthRelicTile>(), AssetDirectory.HyacinthSeriesItems) { }
    }

    public class HyacinthRelicDust : ModDust
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override bool Update(Dust dust)
        {
            //旋转每帧增加0.2f
            //dust.rotation += 0.05f;
            //fadeIn这个东西可以把它当成计时器来用，当然也不只是能当成计时器，有需要的可以自行修改。
            dust.fadeIn++;
            if (dust.fadeIn > 60 * 3)//设置3秒钟后消失。原版的粒子基本上都是scale小于一定值就自动消失的
                dust.active = false;//直接让粒子消失
                                    //如果粒子不是无视重力的就给它加一点Y方向速度
            if (!dust.noGravity)
                dust.velocity.Y += 0.05f;

            dust.rotation += dust.velocity.X / 5;

            //检测碰撞
            if (Collision.SolidCollision(dust.position - Vector2.One * 5f, 10, 10, true))
            {
                dust.scale *= 0.96f;
                dust.velocity *= 0.25f;
            }

            //由于我们直接把原版的粒子AI给阻挡掉了所以需要自己更新粒子位置，千万别忘了这一点！！！！
            dust.position += dust.velocity;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Color c = Lighting.GetColor(dust.position.ToTileCoordinates());
            if (dust.fadeIn < 10)
            {
                c *= dust.fadeIn / 10;
            }

            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, dust.position - Main.screenPosition, c
                , dust.rotation, dust.scale);
            return false;
        }
    }

    public class HyacinthRelicTile : BaseBossRelicTile
    {
        public override string Texture => AssetDirectory.MiscTiles + "HyacinthRelicPedestal";
        public override string RelicTextureName => AssetDirectory.MiscTiles + Name;

        public override bool CanDrop(int i, int j) => true;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CorruptBiome[Type] = -300;
            TileID.Sets.CrimsonBiome[Type] = -300;
            TileID.Sets.HallowBiome[Type] = -300;
            base.SetStaticDefaults();
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            return
            [
                new(ModContent.ItemType<HyacinthRelic>())
            ];
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer && Main.rand.NextBool(6))
            {
                Point p = Helper.FindTopLeftPoint(i, j);
                if (i == p.X && j == p.Y)
                {
                    Vector2 pos = p.ToWorldCoordinates(8 + 16, 8 + 20);

                    int count = Main.rand.Next(1, 3);

                    for (int k = 0; k < count; k++)
                    {
                        Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(20, 24)
                             , ModContent.DustType<HyacinthRelicDust>(), Vector2.UnitY.RotateByRandom(-0.8f, 0.8f) * Main.rand.NextFloat(0.25f, 0.75f));
                        d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                }
            }
        }

        public override void DrawRelicTop(SpriteBatch spriteBatch, Texture2D texture, Vector2 offScreen, Point p, Tile tile)
        {
            //绘制底部
            Rectangle frame = texture.Frame(2, 1, 1, 0);

            Vector2 origin = frame.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(24f, 70f);

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
            drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -44f) + new Vector2(0f, offset * 4f);

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
