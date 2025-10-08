namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseFairyEVBonusItem : ModItem
    {
        public override string Texture => AssetDirectory.FairyEVBonus + Name;

        /// <summary>
        /// 能为哪种属性提供努力值
        /// </summary>
        public abstract FairyIV.IVType BonusType { get; }
        /// <summary>
        /// 每次提供多少努力值
        /// </summary>
        public abstract int BonusValue { get; }
    }
}
