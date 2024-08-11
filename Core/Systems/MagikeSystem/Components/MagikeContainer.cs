using Coralite.Content.CustomHooks;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Helpers;
using Microsoft.Build.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.ComponentModel;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeContainer : MagikeComponent
    {
        public sealed override int ID => MagikeComponentID.MagikeContainer;

        /// <summary> 当前内部的魔能量 </summary>
        public int Magike { get; set; }

        /// <summary> 自身魔能基础容量，可以通过升级来变化 </summary>
        public int MagikeMaxBase { get; protected set; }
        /// <summary> 额外魔能量，通过扩展膜附加的魔能容量 </summary>
        public int MagikeMaxExtra { get; set; }

        /// <summary> 当前的魔能上限 </summary>
        public int MagikeMax { get => MagikeMaxBase + MagikeMaxExtra; }

        /// <summary> 有魔能就为<see langword="true"/> </summary>
        public virtual bool HasMagike => Magike > 0;
        /// <summary> 魔能满了后为true </summary>
        public virtual bool FullMagike => Magike >= MagikeMax;

        public override void Update(IEntity entity) { }

        #region 魔能操作相关

        public void LimitMagikeAmount() => Magike = Math.Clamp(Magike, 0, MagikeMax);

        /// <summary>
        /// 直接向魔能容器内添加魔能
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void AddMagike(int amount)
        {
            Magike += amount;
            LimitMagikeAmount();
        }

        /// <summary>
        /// 直接减少，请一定在执行这个操作前检测能否减少
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void ReduceMagike(int amount)
        {
            Magike -= amount;
            LimitMagikeAmount();
        }

        /// <summary>
        /// 一键充满
        /// </summary>
        public void FullChargeMagike()
        {
            Magike = MagikeMax;
        }

        /// <summary>
        /// 一键清空
        /// </summary>
        public void ClearMagike()
        {
            Magike = 0;
        }

        /// <summary>
        /// 将传入的数值限制在能接受的魔能数量
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool LimitReceiveOverflow(ref int amount)
        {
            if (Magike + amount > MagikeMax)
            {
                amount = MagikeMax - Magike;
                return true;
            }

            return false;
        }

        #endregion

        #region UI

        public override void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = new ComponentUIElementText<MagikeContainer>(c =>
                 MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeContainerName), this, parent, new Vector2(1.3f));
            parent.Append(title);

            //添加魔能量显示条，在左边
            ContainerBar bar = new ContainerBar(this);
            bar.Top.Set(title.Height.Pixels + 8, 0);
            parent.Append(bar);

            //其他的文本信息在右侧
            UIList list = new UIList();
            list.Width.Set(0, 1);
            list.Height.Set(0, 1);
            list.Left.Set(bar.Width.Pixels + 6, 0);
            list.Top.Set(title.Height.Pixels + 8, 0);
            list.OverflowHidden = false;

            AddText(list, c =>
            {
                string colorCode = c.GetMagikeContainerMaxColorCode();

                return string.Concat(MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeAmount)
                    , $"\n  - {c.Magike} / [c/{colorCode}:{c.MagikeMax}]");
            }, parent);

            AddText(list, c =>
                string.Concat(MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeMax)
                , $"\n  - {c.MagikeMax} ({c.MagikeMaxBase} {(c.MagikeMaxExtra >= 0 ? "+" : "-")} {Math.Abs(c.MagikeMaxExtra)})"), parent);

            parent.Append(list);
        }

        public void AddText(UIList list, Func<MagikeContainer, string> textFunc, UIElement parent)
        {
            list.Add(new ComponentUIElementText<MagikeContainer>(textFunc, this, parent));
        }

        #endregion

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(Magike), Magike);
            tag.Add(preName + nameof(MagikeMaxBase), MagikeMaxBase);
            tag.Add(preName + nameof(MagikeMaxExtra), MagikeMaxExtra);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            Magike = tag.GetInt(preName + nameof(Magike));
            MagikeMaxBase = tag.GetInt(preName + nameof(MagikeMaxBase));
            MagikeMaxExtra = tag.GetInt(preName + nameof(MagikeMaxExtra));
        }
    }

    public class ContainerBar : UIElement
    {
        private MagikeContainer container;

        public ContainerBar(MagikeContainer container)
        {
            Texture2D tex = MagikeSystem.MagikeContainerBar.Value;
            Vector2 size = tex.Frame(2, 1).Size();

            Width.Set(size.X + 10, 0);
            Height.Set(size.Y, 0);
            this.container = container;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.MagikeContainerBar.Value;

            var frameBox = tex.Frame(2, 1);

            var dimensions = GetDimensions();
            Vector2 pos = dimensions.Position() + new Vector2((dimensions.Width - frameBox.Width) / 2, 0);
            Vector2 center = dimensions.Center();

            //绘制底层
            spriteBatch.Draw(tex, pos, frameBox, Color.White);

            float percent = (float)container.Magike / container.MagikeMax;

            Vector2 drawPos = pos + new Vector2(0, frameBox.Height);
            for (int i = 0; i < frameBox.Width / 2; i++)
            {
                int currentHeight = Math.Clamp(
                   (int)(tex.Height * (percent + 0.04f * MathF.Sin(((float)Main.timeForVisualEffects) * 0.1f + i * 0.3f)))
                    , 0, tex.Height);

                Rectangle frameBox2 = new Rectangle(frameBox.Width + i * 2, tex.Height - currentHeight, 2, currentHeight);
                var origin = new Vector2(0, frameBox2.Height);
                spriteBatch.Draw(tex, drawPos + new Vector2(i * 2, 0), frameBox2, Color.White, 0, origin, 1f, 0, 0f);
            }

            string percentText = MathF.Round(100 * percent, 1) + "%";

            Utils.DrawBorderString(spriteBatch, percentText, center, Color.White, 0.75f, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
