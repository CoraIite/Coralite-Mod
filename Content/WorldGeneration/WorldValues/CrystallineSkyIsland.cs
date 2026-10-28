using Coralite.Core.Systems.WorldValueSystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.WorldGeneration.WorldValues
{
    /// <summary>
    /// 蕴魔空岛祭坛：光明之魂
    /// </summary>
    public class CrystallineSkyIsland_SoulOfLightFlag : WorldFlag
    {
        // 客户端发解锁请求；服务端在 WorldValueSystem 中复验并 ConsumeItem。
        public override bool AcceptClientChangeRequest => true;

        public override bool TryAuthorizeClientUnlock(Player player)
            => player.ConsumeItem(ItemID.SoulofLight, includeVoidBag: true);
    }

    /// <summary>
    /// 蕴魔空岛祭坛：暗影之魂
    /// </summary>
    public class CrystallineSkyIsland_SoulOfNightFlag : WorldFlag
    {
        public override bool AcceptClientChangeRequest => true;

        public override bool TryAuthorizeClientUnlock(Player player)
            => player.ConsumeItem(ItemID.SoulofNight, includeVoidBag: true);
    }

    /// <summary>
    /// 蕴魔空岛祭坛：进入权限
    /// </summary>
    public class CrystallineSkyIsland_PermissionFlag : WorldFlag
    {
    }
}
