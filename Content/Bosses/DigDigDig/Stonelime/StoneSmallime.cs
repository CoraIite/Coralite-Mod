using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Bosses.DigDigDig.Stonelime
{
    public class StoneSmallime : ModNPC
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "White32x32";

        public int frame = 1;

        public override void SetStaticDefaults()
        {
            NPC.SetHideInBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = NPCAIStyleID.Slime;
            NPC.damage = 7;
            NPC.defense = 2;
            NPC.lifeMax = 25;
            NPC.value = 25f;
            NPC.defense += 4;
            NPC.width = NPC.height = 32;
            AIType = NPCID.BlueSlime;

            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.MeteorImpact_Item89;
            frame = Main.rand.Next(1, 3);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!CoraliteWorld.DigDigDigWorld)
                return 0;

            if (spawnInfo.Player.ZoneUnderworldHeight
                && !spawnInfo.Player.ZoneOverworldHeight
                && !spawnInfo.Player.ZoneBeach)
            {
                return 0.3f;
            }

            return base.SpawnChance(spawnInfo);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Tile[TileID.Stone].Value;

            //检测宽度
            float scale = NPC.scale;
            Vector2 bottom = NPC.Bottom - Main.screenPosition;

            int fullWidth = 16 * 2;

            //绘制四个角
            spriteBatch.Draw(mainTex, bottom + new Vector2(-fullWidth / 2, -fullWidth) * scale, new Rectangle(18 * 2 * frame, 18 * 3, 16, 16)
                , drawColor, 0, Vector2.Zero, scale, 0, 0);
            spriteBatch.Draw(mainTex, bottom + new Vector2(fullWidth / 2 - 16, -fullWidth) * scale, new Rectangle(18 * (2 * frame + 1), 18 * 3, 16, 16)
                , drawColor, 0, Vector2.Zero, scale, 0, 0);
            spriteBatch.Draw(mainTex, bottom + new Vector2(-fullWidth / 2, -16) * scale, new Rectangle(18 * 2 * frame, 18 * 4, 16, 16)
                , drawColor, 0, Vector2.Zero, scale, 0, 0);
            spriteBatch.Draw(mainTex, bottom + new Vector2(fullWidth / 2 - 16, -16) * scale, new Rectangle(18 * (2 * frame + 1), 18 * 4, 16, 16)
                , drawColor, 0, Vector2.Zero, scale, 0, 0);

            return false;
        }
    }
}
