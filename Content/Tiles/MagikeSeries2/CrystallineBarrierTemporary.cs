using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineBarrierTemporary : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        public override void SetStaticDefaults()
        {
            Main.tileBlockLight[Type] = false;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            DustType = ModContent.DustType<CrystallineDust>();
            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;

            AddMapEntry(Coralite.CrystallinePurple);
        }

        public override bool Slope(int i, int j)
        {
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offscreenVector = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
            {
                offscreenVector = Vector2.Zero;
            }

            Texture2D tex = TextureAssets.Tile[Type].Value;

            Tile t = Main.tile[i, j];
            short frameX = t.TileFrameX;
            short frameY = t.TileFrameY;

            Rectangle box = new Rectangle(frameX, frameY, 16, 16);
            Vector2 pos = new Vector2(i, j) * 16 + offscreenVector - Main.screenPosition;

            Color selfC = Color.Lerp(Color.White * 0.7f, Color.White * 0.2f, MathF.Cos((2 * i * j) * MathHelper.PiOver4 + Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);

            Color c2 = selfC * 0.2f;
            c2.A = 0;

            for (int k = 0; k < 4; k++)
            {
                Vector2 off = (Main.GlobalTimeWrappedHourly + k * MathHelper.PiOver4).ToRotationVector2() * 2;
                spriteBatch.Draw(tex, pos + off, box, c2, 0, Vector2.Zero, 1, 0, 0);
            }

            spriteBatch.Draw(tex, pos, box, selfC, 0, Vector2.Zero, 1, 0, 0);

            box.Y += 90;

            spriteBatch.Draw(tex, pos, box, Color.White * 0.8f, 0, Vector2.Zero, 1, 0, 0);

            return false;
        }
    }
}
