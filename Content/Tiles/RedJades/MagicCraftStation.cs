using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.RedJades
{
    public class MagicCraftStation : ModTile
    {
        public override string Texture => AssetDirectory.RedJadeTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AnimationFrameHeight = 54;
            DustType = DustID.GemRuby;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("魔导台");
            AddMapEntry(Coralite.Instance.RedJadeRed, name);
        }

        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];

            Texture2D glowTexture = ModContent.Request<Texture2D>("Coralite/Assets/Tiles/RedJades/MagicCraftStation_Glow").Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            int frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;

            spriteBatch.Draw(glowTexture,
                    new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                    new Rectangle(tile.TileFrameX, tile.TileFrameY + frameYOffset, 16, 16),
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;

            if (frameCounter > 5)
            {
                frame++;
                if (frame > 8)
                    frame = 0;
                frameCounter = 0;
            }
        }
    }
}
