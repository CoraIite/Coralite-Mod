using Terraria.Localization;

namespace Coralite.Core.Systems.BossSystems
{
    public class BossBarSystem : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Systems";

        public static LocalizedText DontTakeDamage;

        public override void Load()
        {
            DontTakeDamage = this.GetLocalization("DontTakeDamage", () => "不可攻击！");
        }
    }
}
