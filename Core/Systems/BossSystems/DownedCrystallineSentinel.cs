using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.UI.NewKnowledgeUnlock;
using Coralite.Core.Network;
using Coralite.Core.Systems.WorldValueSystem;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public static void DownCrystallineSentinel()
        {
            DownedCrystallineSentinel d = ModContent.GetInstance<DownedCrystallineSentinel>();

            if (!d.Value)
            {
                if (VaultUtils.isSinglePlayer)
                    SlabText();
                else
                    SendUnlockSlabs();

                d.SetAndSync(true);
            }
        }

        public static void SendUnlockSlabs()
        {
            if (!VaultUtils.isServer)
                return;

            ModPacket p = Coralite.Instance.GetPacket();
            p.Write((byte)CoraliteNetWorkEnum.UnlockSlabs);
            p.Send(-1);
        }

        public static void SlabText()
        {
            if (VaultUtils.isServer)
                return;

            NewKnowledgeState.AddCustomTip(CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>(), Coralite.CrystallinePurple, MagikeInterstitial3Slab1.SlabUnlockName, MagikeInterstitial3Slab1.SlabUnlockText);
        }
    }

    public class DownedCrystallineSentinel : WorldFlag { }
}
