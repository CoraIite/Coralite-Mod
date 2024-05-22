using Coralite.Content.Items.Thunder;
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

        public PlayerDeathReason DeathByLocalization(string key)
        {
            return PlayerDeathReason.ByCustomReason(Language.GetTextValue($"Mods.Coralite.DeathMessage.{key}", Player.name));
        }

        public void ThunderElectrifiedDeathReason(ref PlayerDeathReason damageSource)
        {
            if (HasEffect(nameof(ThunderElectrified)))
                //damageSource.SourceCustomReason = Player.name + ThunderElectrifiedDeath.Value;
                damageSource = DeathByLocalization("ThunderElectrifiedDeath");
        }
    }
}
