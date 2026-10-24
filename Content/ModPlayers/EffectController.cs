using Coralite.Core.Loaders;
using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.ModPlayers
{
    public partial class CoralitePlayer
    {
        #region 开关类效果

        /// <summary>
        /// 各种效果的bool
        /// </summary>
        public bool[] Effects { get; set; } = new bool[PlayerEffectLoader.EffectCount];

        /// <summary>
        /// 重置所有特殊效果
        /// </summary>
        public void ResetEffectFlags()
        {
            //防止出现各种各样的奇葩问题
            if (Effects.Length != PlayerEffectLoader.EffectCount)
                Effects = new bool[PlayerEffectLoader.EffectCount];

            for (int i = 0; i < Effects.Length; i++)
                Effects[i] = false;
        }

        /// <summary>
        /// 玩家是否有某个效果，建议使用<see cref="nameof"/>来获取字符串
        /// </summary>
        /// <param name="effectName"></param>
        /// <returns></returns>
        public bool HasEffect(string effectName)
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
        public bool AddEffect(string effectName)
        {
            if (PlayerEffectLoader.Effects.TryGetValue(effectName, out var index))
            {
                Effects[index] = true;
                return true;
            }

            return false;
        }

        #endregion

        #region 接口调用类效果

        private List<object> _effectHandlers;

        /// <summary>
        /// 当前帧参与接口调用的效果对象，由各系统按接口类型筛选
        /// </summary>
        public List<object> EffectHandlers => _effectHandlers ??= [];

        /// <summary>
        /// 注册一个效果对象，需在每帧更新时重新加入
        /// </summary>
        /// <param name="handler"></param>
        public void AddEffectHandler(object handler)
        {
            EffectHandlers.Add(handler);
        }

        public void ResetEffectHandlers()
        {
            _effectHandlers?.Clear();
        }

        #endregion

    }
}
