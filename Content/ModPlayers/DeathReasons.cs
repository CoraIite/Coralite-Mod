using Terraria.DataStructures;
using Terraria.Localization;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        public static LocalizedText ThunderElectrifiedDeath;

        public void LoadDeathReasons()
        {
            ThunderElectrifiedDeath = this.GetLocalization("ThunderElectrifiedDeath", () => "被雷劈死了");
        }

        public void UnloadDeathReasons()
        {
            ThunderElectrifiedDeath = null;
        }


        public void ThunderElectrifiedDeathReason(ref PlayerDeathReason damageSource)
        {
            if (thunderElectrified)
                damageSource.SourceCustomReason = Player.name + ThunderElectrifiedDeath.Value;
        }
    }
}
