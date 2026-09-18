using Coralite.Core;

namespace Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera
{
    /// <summary>
    /// 出生演出预留的空弹幕，原本和旧 <c>Phase.P0_OnSpawnAnmi.cs</c> 放在一个文件里。<br/>
    /// 逻辑全空但已经是注册内容（类名决定本地化键与贴图路径），所以只搬家不删除。
    /// </summary>
    public class SpawnLight : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
        }

        public override void AI()
        {
        }
    }
}
