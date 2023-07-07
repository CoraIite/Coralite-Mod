using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.NPCs.Magike
{
    public class CrystalGolem : ModNPC
    {
        public override string Texture => AssetDirectory.MagikeNPCs + Name;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 84;
            NPC.height = 102;
            NPC.damage = 12;
            NPC.defense = 4;

            NPC.lifeMax = 40;
            NPC.aiStyle = -1;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.GlassBroken_Shatter;
            NPC.knockBackResist = 0.8f;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override void Load()
        {
            base.Load();
        }

        public override void AI()
        {
            //朝向玩家移动
            //可以打到玩家且距离小于一定值时停止并蓄力射激光


        }

        public override void OnKill()
        {
            base.OnKill();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}
