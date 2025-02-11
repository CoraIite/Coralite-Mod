using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class OnGameEventClearedForTheFirstTime:HookGroup
    {
        public override void Load()
        {
            On_NPC.OnGameEventClearedForTheFirstTime += On_NPC_OnGameEventClearedForTheFirstTime;
        }

        public override void Unload()
        {
            On_NPC.OnGameEventClearedForTheFirstTime -= On_NPC_OnGameEventClearedForTheFirstTime;
        }

        private void On_NPC_OnGameEventClearedForTheFirstTime(On_NPC.orig_OnGameEventClearedForTheFirstTime orig, int gameEventId)
        {
            orig.Invoke(gameEventId);

            switch (gameEventId)
            {
                default:
                    break;
                case GameEventClearedID.DefeatedDestroyer:
                case GameEventClearedID.DefeatedTheTwins:
                case GameEventClearedID.DefeatedSkeletronPrime://TODO：增加多人同步
                    if (Main.netMode != NetmodeID.MultiplayerClient && Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                        KnowledgeSystem.CheckForUnlock(KeyKnowledgeID.Thunder1, Main.LocalPlayer.Center, Coralite.ThunderveinYellow);
                    break;
            }
        }
    }
}
