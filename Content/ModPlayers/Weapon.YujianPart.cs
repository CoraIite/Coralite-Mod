using Coralite.Content.UI;
using Coralite.Core;
using System;
using Terraria.Audio;
using Terraria.ID;

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

        private void ResetYujianNianli()
        {
            nianliRegain = BaseNianliRegain;
            nianliMax = BaseNianliMax;
        }

        private void UpdateNianli()
        {
            if (ownedYujianProj)
            {
                bool justCompleteCharge = nianli < nianliMax;
                nianli += nianliRegain;
                nianli = Math.Clamp(nianli, 0f, nianliMax);
                if (nianli == nianliMax && justCompleteCharge)      //蓄力完成的时刻发出声音
                    SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4);
            }
            else
                nianli = 0f;
        }

        private void UpdateNianliUI()
        {
            //有御剑弹幕那就让透明度增加，没有御剑减小透明度直到为0
            if (ownedYujianProj)
            {
                if (yujianUIAlpha < 1f)
                {
                    yujianUIAlpha += 0.035f;
                    yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);
                    NianliChargingBar.visible = true;
                }
            }
            else
                NianliUIFade();
        }

        private void LimitNianli()
        {
            nianli = Math.Clamp(nianli, 0f, nianliMax);  //只是防止意外发生
        }

        private void NianliUIFade()
        {
            if (yujianUIAlpha > 0f)
            {
                yujianUIAlpha -= 0.035f;
                yujianUIAlpha = MathHelper.Clamp(yujianUIAlpha, 0f, 1f);

                if (yujianUIAlpha <= 0f)
                    NianliChargingBar.visible = false;
            }
        }
    }
}
