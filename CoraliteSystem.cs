using Coralite.Content.Items.ThyphionSeries;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite
{
    /// <summary>
    /// 用于处理各类“重要事项”，比如版本更新时的进入世界跳字等<br></br>
    /// 在不同版本此类中的内容可能有较大程度的变化
    /// </summary>
    public class CoraliteSystem : ModSystem, ILocalizedModType
    {
        public string LocalizationCategory => "Important";

        //v0.2.1 用于提醒玩家魔能的故障
        public static LocalizedText OnEnterWorld { get; set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                OnEnterWorld = this.GetLocalization(nameof(OnEnterWorld));
            }
        }

        public override void Unload()
        {
            OnEnterWorld = null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            //不知道写哪所以写这里了
            if (Thyphion.Skin)
                tag.Add("ThyphionSkin", true);
        }

        public override void OnWorldLoad()
        {
            Thyphion.Skin = false;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("ThyphionSkin", out bool b))
                Thyphion.Skin = b;
        }
    }
}
