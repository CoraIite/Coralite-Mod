using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.MagikeSystem.Components.Producers
{
    public abstract class MagikeActiveProducer : MagikeProducer, ITimerTriggerComponent
    {
        /// <summary> 基础生产时间 </summary>
        public int ProductionDelayBase { get => DelayBase; protected set => DelayBase = value; }
        /// <summary> 发送时间减少量（效率增幅量） </summary>
        public float ProductionDelayBonus { get => DelayBonus; set => DelayBonus = value; }

        /// <summary> 生产时间 </summary>
        public int ProductionDelay => Math.Clamp((int)(DelayBase * DelayBonus), -1, int.MaxValue);

        public int DelayBase { get; set; }
        public float DelayBonus { get; set; } = 1f;
        public int Timer { get; set; }

        public bool TimeResetable => true;

        public bool CheckTime()
        {
            return (this as ITimerTriggerComponent).UpdateTime();
        }

        public bool CanProduce_CheckMagike()
        {
            return !Entity.GetMagikeContainer().FullMagike;
        }

        /// <summary>
        /// 重写这个检测是否能生产
        /// </summary>
        public override bool CanProduce()
        {
            //魔能容量限制
            //其他特定的特殊条件
            return CanProduce_CheckMagike() && CanProduce_SpecialCheck();
        }

        public virtual bool CanProduce_SpecialCheck() => true;

        public sealed override void Update()
        {
            //生产时间限制
            if (ProductionDelayBase < 0 || !CheckTime())
                return;

            this.SendTimerComponentTime(this);

            if (!CanProduce())
                return;

            Produce();
        }

        #region UI部分

        //public virtual string ProductionDelayText(MagikeActiveProducer p)
        //{
        //    float timer = MathF.Round(p.Timer / 60f, 1);
        //    float delay = MathF.Round(p.ProductionDelay / 60f, 1);
        //    float delayBase = MathF.Round(p.ProductionDelayBase / 60f, 1);
        //    float DelayBonus = p.ProductionDelayBonus;

        //    return $"  ▶ {timer} / {MagikeHelper.BonusColoredText(delay.ToString(), DelayBonus, true)} ({delayBase} * {MagikeHelper.BonusColoredText(DelayBonus.ToString(), DelayBonus, true)})";
        //}

        public virtual string ThroughputText(MagikeActiveProducer p)
            => $"  ▶ {MagikeHelper.BonusColoredText(p.Throughput.ToString(), p.ThroughputBonus)} ({p.ThroughputBase} * {MagikeHelper.BonusColoredText(p.ThroughputBonus.ToString(), p.ThroughputBonus)})";

        #endregion

        #region 网络同步

        public override void SendData(ModPacket data)
        {
            data.Write(Timer);

            data.Write(ProductionDelayBase);
            data.Write(ProductionDelayBonus);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            Timer = reader.ReadInt32();

            ProductionDelayBase = reader.ReadInt32();
            ProductionDelayBonus = reader.ReadSingle();
        }

        #endregion

        #region 数据存取

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            tag.Add(preName + nameof(Timer), Timer);

            tag.Add(preName + nameof(ProductionDelayBase), ProductionDelayBase);
            tag.Add(preName + nameof(ProductionDelayBonus), ProductionDelayBonus);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);

            Timer = tag.GetInt(preName + nameof(Timer));

            ProductionDelayBase = tag.GetInt(preName + nameof(ProductionDelayBase));
            ProductionDelayBonus = tag.GetFloat(preName + nameof(ProductionDelayBonus));
        }

        #endregion
    }

    [VaultLoaden(AssetDirectory.MagikeUI)]
    public class ProduceBar : UIElement
    {
        public static ATex ProgressBar { get; private set; }

        protected MagikeActiveProducer producer;

        private const int LeftPaddling = 10;

        public ProduceBar(MagikeActiveProducer producer)
        {
            this.producer = producer;

            ResetSize();
        }

        public void ResetSize()
        {
            Vector2 timerSize = GetStringSize(producer.Timer);
            Vector2 sendDelaySize = GetStringSize(producer.ProductionDelay);

            float width = timerSize.X + 10;
            if (sendDelaySize.X + 10 > width)
                width = sendDelaySize.X + 10;
            if (ProgressBar.Width() + 10 > width)
                width = ProgressBar.Width() + 10;

            Width.Set(width + LeftPaddling, 0);
            Height.Set(timerSize.Y * 3f + ProgressBar.Height() / 2, 0);
        }

        private static Vector2 GetStringSize(int value)
        {
            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(value.ToString(), Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

            return ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, Vector2.One * 1.1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle size = GetDimensions().ToRectangle();

            float per = (size.Height - ProgressBar.Height() / 2) / 3f;
            int width = size.Width - LeftPaddling;
            Vector2 topLeft = size.TopLeft();
            Vector2 pos = topLeft + new Vector2(30 + width / 2, per / 2);

            //绘制时间
            int productionDelay = producer.ProductionDelay;
            Utils.DrawBorderString(spriteBatch, (productionDelay < 0 ? 0 : MathF.Round((1 - producer.Timer / (float)productionDelay) * 100)).ToString() + " %", pos + new Vector2(0, 4), Color.White
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);

            //绘制中间的进度条
            Texture2D barTex = ProgressBar.Value;

            Rectangle box = barTex.Frame(1, 2, 0, 1);

            pos += new Vector2(0, per / 2 + box.Height / 2);

            Vector2 barPos = pos - new Vector2(width / 2 - 4, 0);
            Vector2 origin = new Vector2(0, box.Height / 2);
            spriteBatch.Draw(barTex, barPos, box, Color.White, 0, origin
                , 1, 0, 0);

            int delay = productionDelay;
            if (productionDelay <= 0)
            {
                delay = 0;
            }
            else
            {
                box = barTex.Frame(1, 2);
                box.Width = (int)((1 - producer.Timer / (float)productionDelay) * box.Width);
                spriteBatch.Draw(barTex, barPos, box, Color.White, 0, origin
                    , 1, 0, 0);
            }

            pos += new Vector2(0, per / 2 + box.Height / 2);

            //绘制倒计时
            Color color = MagikeHelper.GetBonusColor(producer.ProductionDelayBonus, true);
            Utils.DrawBorderString(spriteBatch, MathF.Round(delay / 60f, 1).ToString() + " " + MagikeSystem.GetUIText(MagikeSystem.UITextID.Second), pos + new Vector2(0, 4), color
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);

            pos += new Vector2(0, per);

            //绘制倒计时加成
            Utils.DrawBorderString(spriteBatch, $"< × {producer.ProductionDelayBonus} >", pos + new Vector2(0, 4), color
                , 1, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
