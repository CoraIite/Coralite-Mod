using Coralite.Helpers;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Bosses.ShadowBalls
{
    public class OldManChatSystem : ModSystem
    {
        private Hook chatButtonsHook;

        private delegate void OrigSetChatButtons(ref string button, ref string button2);

        private delegate void HookSetChatButtons(OrigSetChatButtons orig, ref string button, ref string button2);

        public override void Load()
        {
            if (Main.dedServ)
                return;

            MethodInfo method = typeof(NPCLoader).GetMethod(
                nameof(NPCLoader.SetChatButtons),
                BindingFlags.Public | BindingFlags.Static);

            chatButtonsHook = new Hook(
                method,
                (HookSetChatButtons)ModifyChatButtons);
        }

        public override void Unload()
        {
            chatButtonsHook?.Dispose();
            chatButtonsHook = null;
        }

        private static void ModifyChatButtons(
            OrigSetChatButtons orig,
            ref string button,
            ref string button2)
        {
            orig(ref button, ref button2);

            int index = Main.LocalPlayer.talkNPC;

            if (index < 0 || index >= Main.maxNPCs)
                return;

            NPC npc = Main.npc[index];

            if (npc.active
                && npc.type == NPCID.OldMan
                && Main.IsItDay())
            {
                // 原版“诅咒”的本地化文字。
                // 老人的诅咒使用第一个按钮。
                button = Lang.inter[50].Value;
            }
        }
    }
    public class DungeonManChat:GlobalNPC,ILocalizedModType
    {
        public override bool InstancePerEntity => false;

        public static LocalizedText AlreadyHaveBoss { get; set; }
        public static LocalizedText ShadowSpawn { get; set; }
        public static LocalizedText[] SpawnShadowBallTips { get; set; }

        public string LocalizationCategory => "NPCs.ShadowballSpawn";

        public override void Load()
        {
            AlreadyHaveBoss = this.GetLocalization(nameof(AlreadyHaveBoss));
            ShadowSpawn = this.GetLocalization(nameof(ShadowSpawn));
            SpawnShadowBallTips = new LocalizedText[3];
            for (int i = 0; i < 3; i++)
            {
                SpawnShadowBallTips[i] = this.GetLocalization(nameof(SpawnShadowBallTips) + i.ToString());
            }
        }

        public override void Unload()
        {
            AlreadyHaveBoss = null;
            ShadowSpawn = null;
            SpawnShadowBallTips = null;
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type != NPCID.OldMan)
                return;

            if (!Main.dayTime)
                return;

            chat = SpawnShadowBallTips[Main.rand.Next(SpawnShadowBallTips.Length)].Value;
        }

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            // 只接管老人白天的第一个按钮。
            if (npc.type != NPCID.OldMan
                || !firstButton
                || !Main.dayTime)
            {
                return true;
            }

            int bossType = ModContent.NPCType<ShadowBall>();

            if (NPC.AnyNPCs(bossType))
            {
                Main.npcChatText = AlreadyHaveBoss.Value;
                return false;
            }

            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + new Vector2(0, -SpawnProj.SplitPointDistance * ((13f)/2-1)), Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer);

            Main.npcChatText = ShadowSpawn.Value;
            npc.Kill();

            return false;
        }
    }
}
