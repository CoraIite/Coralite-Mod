using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.UI;

namespace Coralite.Content.NPCs.Crystalline
{
    public class FakeSentinel : ModNPC
    {
        public override string Texture => AssetDirectory.Blank;

        private ref float Timer => ref NPC.ai[0];

        public override void SetStaticDefaults()
        {
            NPC.SetHideInBestiary();
            NPCID.Sets.CannotDropSouls[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.chaseable = false;
            NPC.noGravity = true;
            NPC.npcSlots = 0;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 600;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.netAlways = true;
        }

        public override void AI()
        {
            Timer++;
            NPC.velocity = Vector2.Zero;
            if (Timer > 10)
            {
                Timer = 0;

                if (VaultUtils.isClient)
                    return;

                foreach (var player in Main.ActivePlayers)
                {
                    if (Vector2.DistanceSquared(player.MountedCenter, NPC.Center) < 1700 * 1700)
                    {
                        NPC.Kill();
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)(NPC.Center.Y - 50), ModContent.NPCType<CrystallineSentinel>(), Target: player.whoAmI);

                        return;
                    }
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class FakeSentinelDrawer : ModMapLayer
    {
        public static ATex CrystallineSentinel_Head_Boss { get; private set; }

        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            NPC fakeSentinel = null;

            foreach (var npc in Main.ActiveNPCs)
                if (npc.type == ModContent.NPCType<FakeSentinel>())
                {
                    fakeSentinel = npc;
                    break;
                }

            if (fakeSentinel == null)
                return;

            const float scaleIfNotSelected = 1f;
            const float scaleIfSelected = scaleIfNotSelected * 2f;

            if (context.Draw(CrystallineSentinel_Head_Boss.Value, fakeSentinel.Center / 16, Color.White
                , new SpriteFrame(1, 1, 0, 0)
                , scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
            {
                text = ContentSamples.NpcsByNetId[ModContent.NPCType<CrystallineSentinel>()].FullName;
            }
        }
    }
}
