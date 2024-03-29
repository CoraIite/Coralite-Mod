namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary>
        /// 御剑念力条UI的透明度
        /// </summary>
        public float yujianUIAlpha;
        /// <summary>
        /// 是否拥有御剑弹幕
        /// </summary>
        public bool ownedYujianProj;
        /// <summary>
        /// 念力值
        /// </summary>
        public float nianli;
        /// <summary>
        /// 念力上限
        /// </summary>
        public float nianliMax = BaseNianliMax;
        /// <summary>
        /// 念力恢复值
        /// </summary>
        public float nianliRegain = BaseNianliRegain;

        public const float BaseNianliMax = 300f;
        public const float BaseNianliRegain = 0.5f;

    }
}
