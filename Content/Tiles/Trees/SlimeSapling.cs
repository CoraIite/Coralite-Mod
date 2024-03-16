using Coralite.Core;
using Coralite.Core.Prefabs.Tiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Tiles.Trees
{
    public class SlimeSapling : ModTile
    {
        public override string Texture => AssetDirectory.TreeTiles + Name;

        public override void SetStaticDefaults()
        {
            this.SaplingPrefab(new int[] { TileID.SlimeBlock, TileID.FrozenSlimeBlock, TileID.PinkSlimeBlock }, DustID.Water, Color.LightBlue);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            // 随机生长
            if (!WorldGen.genRand.NextBool(20))
                return;

            Tile tile = Framing.GetTileSafely(i, j); // 安全获取物块
            bool growSucess; // 是否成功生长了的变量

            // Style 0 is for the ExampleTree sapling, and style 1 is for ExamplePalmTree, so here we check frameX to call the correct method.
            // Any pixels before 54 on the tilesheet are for ExampleTree while any pixels above it are for ExamplePalmTree
            growSucess = WorldGen.GrowTree(i, j);

            // A flag to check if a player is near the sapling
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);

            //If growing the tree was a sucess and the player is near, show growing effects
            if (growSucess && isPlayerNear)
                WorldGen.TreeGrowFXCheck(i, j);
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
        {
            if (i % 2 == 1)
                effects = SpriteEffects.FlipHorizontally;
        }
    }
}
