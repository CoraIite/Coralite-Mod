using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Tiles.MagikeSeries2
{
    public class SoulOfLightAltarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(105, 97, 90), CreateMapEntryName());
        }

        public override bool RightClick(int i, int j)
        {
            if (!CoraliteWorld.PlaceLightSoul && Main.LocalPlayer.ConsumeItem(ItemID.SoulofLight, includeVoidBag: true))
            {
                CoraliteWorld.PlaceLightSoul = true;
            }

            return true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (CoraliteWorld.PlaceLightSoul)
            {
                //绘制光明之魂
            }
        }
    }

    public class SoulOfNightAltarTile : ModTile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Tile;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(105, 97, 90), CreateMapEntryName());
        }

        public override bool RightClick(int i, int j)
        {
            if (!CoraliteWorld.PlaceNightSoul && Main.LocalPlayer.ConsumeItem(ItemID.SoulofNight, includeVoidBag: true))
            {
                CoraliteWorld.PlaceNightSoul = true;
            }

            return true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX == 0 && drawData.tileFrameY == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (CoraliteWorld.PlaceNightSoul)
            {
                //绘制光明之魂
            }
        }
    }
}
