using Coralite.Core;
using InnoVault.PRT;
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
            Main.tileNoFail[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            DustType = ModContent.DustType<BarrierDust>();
            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact;

            AddMapEntry(new Color(169,248,247), CreateMapEntryName());
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Vector2 center = new Vector2(i, j) * 16 + new Vector2(8, 8);
            PRTLoader.NewParticle<BarrierShineParticle>(center, Vector2.Zero, Color.White);

            return false;
        }

        public override bool Slope(int i, int j)
        {
            return false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            Tile t = Main.tile[i, j];
            t.TileFrameY += 100;
            if (t.TileFrameY / 100 > 60 * 5)
            {
                t.TileFrameY = (short)(t.TileFrameY % 100);
                t.TileFrameX += 200;
                if (t.TileFrameX > 200 * 2)
                {
                    t.TileFrameX = (short)(t.TileFrameX % 200);

                    foreach (var player in Main.ActivePlayers)//不要封住玩家，虽然意义不大
                        if (player.getRect().Intersects(new Rectangle(i * 16, j * 16, 16, 16)))
                            return;

                    t.ResetToType((ushort)ModContent.TileType<CrystallineBarrier>());
                    for (int m = 0; m < 3; m++)
                        for (int n = 0; n < 3; n++)
                        {
                            WorldGen.TileFrame(i - 1 + m, j - 1 + n, true, true);
                        }

                    Vector2 center = new Vector2(i, j) * 16 + new Vector2(8, 8);
                    Helpers.Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "BarrierRecovery", 0.35f, 0, 1, 2, center, soundAdjust: st =>
                    {
                        st.MaxInstances = 1;
                        st.LimitsArePerVariant = true;
                        return st;
                    });

                    PRTLoader.NewParticle<BarrierShineParticle>(center, Vector2.Zero, Color.White);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offscreenVector = new Vector2(Main.offScreenRange);
            if (Main.drawToScreen)
                offscreenVector = Vector2.Zero;

            Texture2D tex = TextureAssets.Tile[Type].Value;

            Tile t = Main.tile[i, j];
            short frameX = (short)(t.TileFrameX % 200);
            short frameY = (short)(t.TileFrameY % 100);

            Rectangle box = new Rectangle(frameX, frameY, 16, 16);
            Vector2 pos = new Vector2(i, j) * 16 + offscreenVector - Main.screenPosition;

            float time = (t.TileFrameX / 200) * 60 * 5 + (t.TileFrameY / 100);
            float f = Math.Clamp(time / (60 * 10), 0, 1);
            float f2 = Main.GlobalTimeWrappedHourly * 1f+f*f*MathHelper.TwoPi*10;

            Color selfC = Color.Lerp(Color.White * 0.2f, Color.White * 0.7f, MathF.Cos((2 * i * j) * MathHelper.PiOver4 + f2) / 2 + 0.5f) * (0.3f + f * 0.7f);

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

        private static int x;
        private static int y;

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile t = Main.tile[i, j];
            x = t.TileFrameX / 200;
            y = t.TileFrameY / 100;
            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile t = Main.tile[i, j];
            t.TileFrameX += (short)(x * 200);
            t.TileFrameY += (short)(y * 100);
        }
    }
}
