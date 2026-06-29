using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class CrystallineBarrier: ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile + Name;

        private static byte tipCount;
        public static LocalizedText DontTakeDamageTip { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                DontTakeDamageTip = this.GetLocalization(nameof(DontTakeDamageTip));
            }
        }

        public override void Unload()
        {
            DontTakeDamageTip = null;
        }

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = false;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = false;
            TileID.Sets.AvoidedByMeteorLanding[Type] = true;

            DustType = ModContent.DustType<BarrierDust>();
            HitSound = CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact with { Volume=0.3f};
            MinPick = 200;

            AddMapEntry(Coralite.CrystallinePurple, CreateMapEntryName());
        }

        public override void PostSetDefaults()
        {
            Main.tileNoSunLight[Type] = false;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.1f;
            g = 0.05f;
            b = 0.17f;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            Vector2 center = new Vector2(i, j) * 16 + new Vector2(8, 8);

            //int count = Main.rand.Next(1, 3);
            //for (int k = 0; k < count; k++)
            Dust.NewDustPerfect(center, ModContent.DustType<BarrierDust>(), Helper.NextVec2Dir(1f, 2f));

            return false;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail)
            {
                Vector2 center = new Vector2(i, j) * 16 + new Vector2(8, 8);
                PRTLoader.NewParticle<BarrierShineParticle>(center, Vector2.Zero, Color.White);

                return;
            }

            if (tipCount == 0)//生成提示，只有被挖5次后才会有
            {
                Main.NewText(DontTakeDamageTip.Value,Coralite.CrystallinePurple);
            }

            tipCount++;
            if (tipCount > 8)
            {
                tipCount = 0;
            }

        }

        public override bool CanExplode(int i, int j)
        {
            return false;
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

            Color selfC = Color.Lerp(Color.White * 0.9f, Color.White * 0.2f, MathF.Cos((2 * i * j) * MathHelper.PiOver4 + Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);
            spriteBatch.Draw(tex, pos, box, Color.White * 0.5f, 0, Vector2.Zero, 1, 0, 0);

            Color c2 = selfC * 0.5f;
            c2.A = 0;

            for (int k = 0; k < 4; k++)
            {
                Vector2 off = (Main.GlobalTimeWrappedHourly + k * MathHelper.PiOver4).ToRotationVector2() * 2;
                spriteBatch.Draw(tex, pos + off, box, c2, 0, Vector2.Zero, 1, 0, 0);
            }

            spriteBatch.Draw(tex, pos, box, Color.White * 0.4f, 0, Vector2.Zero, 1, 0, 0);

            box.Y += 90;

            spriteBatch.Draw(tex, pos, box, Color.White, 0, Vector2.Zero, 1, 0, 0);
            //box.Y += 90;

            //spriteBatch.Draw(tex, pos, box, Color.White * 0.8f, 0, Vector2.Zero, 1, 0, 0);

            return false;
        }
    }
}
