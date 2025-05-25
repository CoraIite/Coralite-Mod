using Coralite.Core.Loaders;
using Terraria;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        /// <summary>
        /// 各种效果的bool
        /// </summary>
        public bool[] Effects { get; set; } = new bool[PlayerEffectLoader.EffectCount];

        /// <summary>
        /// 重置所有特殊效果
        /// </summary>
        public void ResetCoraliteEffects()
        {
            //防止出现各种各样的奇葩问题
            if (Effects.Length != PlayerEffectLoader.EffectCount)
                Effects = new bool[Effects.Length];

            for (int i = 0; i < Effects.Length; i++)
                Effects[i] = false;
        }

        /// <summary>
        /// 玩家是否有某个效果，建议使用<see cref="nameof"/>来获取字符串
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public bool HasEffect(string effectName) //=> Effects.Contains(effectName);
        {
            if (PlayerEffectLoader.Effects.TryGetValue(effectName, out var index))
                return Effects.IndexInRange(index) && Effects[index];

            return false;
        }

        /// <summary>
        /// 为玩家添加某个效果，建议使用<see cref="nameof"/>来获取字符串
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public bool AddEffect(string effectName) //=> Effects.Add(effectName);
        {
            if (PlayerEffectLoader.Effects.TryGetValue(effectName, out var index))
            {
                Effects[index] = true;
                return true;
            }

            return false;
        }
    }
}
