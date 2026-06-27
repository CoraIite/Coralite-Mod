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
        //权限解锁由 PremissionProj 仪式弹幕在动画结束时写入；该弹幕的 AI 在服务端也会运行，
        //因此保持默认拒绝客户端请求（服务端权威触发），避免被无前置条件篡改。
    }
}
