using System;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceDragonEgg : ModNPC
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;

        public override void SetDefaults()
        {
            NPC.npcSlots = 0;
            NPC.width = 38;
            NPC.height = 36;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
        }

        public override void Load()
        {

        }

        public override void AI()
        {
            if (NPC.ai[0] > 0f)
            {
                float sinProgress = MathF.Sin(NPC.ai[0]);
                NPC.rotation = sinProgress * 0.2f;
                NPC.ai[0] -= 0.19634f;     //1/16 Pi
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            //受击时摇晃一下
            NPC.ai[0] += 6.282f;
        }

        public override void OnKill()
        {
            //生成冰龙宝宝
            NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BabyIceDragon>());
            //生成蛋壳gore

            if (Main.netMode!=NetmodeID.MultiplayerClient)
            {
                IceEggSpawner.EggDestroyed();
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = TextureAssets.Npc[NPC.type].Value;

            spriteBatch.Draw(mainTex, NPC.Bottom - Main.screenPosition, null, drawColor, NPC.rotation, new Vector2(19, 36), 1, SpriteEffects.None, 0f);
            return false;
        }
    }
}
