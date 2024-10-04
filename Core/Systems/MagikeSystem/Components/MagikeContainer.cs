using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeContainer : Component, IUIShowable
    {
        public sealed override int ID => MagikeComponentID.MagikeContainer;

        /// <summary> 当前内部的魔能量 </summary>
        public int Magike { get; set; }

        /// <summary> 自身魔能基础容量，可以通过升级来变化 </summary>
        public int MagikeMaxBase { get; protected set; }
        /// <summary> 额外魔能量，通过扩展膜附加的魔能容量 </summary>
        public float MagikeMaxBonus { get; set; } = 1f;

        /// <summary> 当前的魔能上限 </summary>
        public int MagikeMax { get => Math.Clamp((int)(MagikeMaxBase * MagikeMaxBonus), 0, int.MaxValue); }

        /// <summary> 当前内部的反魔能量 </summary>
        public int AntiMagike { get; set; }

        /// <summary> 自身反魔能基础容量，可以通过升级来变化 </summary>
        public int AntiMagikeMaxBase { get; protected set; }
        /// <summary> 额外反魔能量，通过扩展膜附加的魔能容量 </summary>
        public int AntiMagikeMaxExtra { get; set; }

        /// <summary> 当前的反魔能上限 </summary>
        public int AntiMagikeMax { get => Math.Clamp(AntiMagikeMaxBase + AntiMagikeMaxExtra, 0, int.MaxValue); }

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

        #region 反魔能相关

        public void LimitAntiMagikeAmount() => AntiMagike = Math.Clamp(AntiMagike, 0, AntiMagikeMax);

        /// <summary>
        /// 反魔能过载时调用<br></br>
        /// 包括摧毁物块，炸掉附近物块和生成反魔能污染
        /// </summary>
        public virtual void AntiMagikeOverflow()
        {
            for (int i = 0; i < Entity.ComponentsCache.Count; i++)
                if (Entity.ComponentsCache[i] is IAnnihilateable annihilateable)
                    annihilateable.OnAnnihilate();

            Point16 p = (Entity as MagikeTileEntity).Position;
            WorldGen.KillTile(p.X, p.Y, noItem: true);

            //TODO: 爆炸与反魔能污染

        }

        /// <summary>
        /// 直接向魔能容器内添加魔能
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void AddAntiMagike(int amount)
        {
            AntiMagike += amount;
            CheckAntiMagike();
        }

        /// <summary>
        /// 直接减少，请一定在执行这个操作前检测能否减少
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual void ReduceAntiMagike(int amount)
        {
            AntiMagike -= amount;
            CheckAntiMagike();
        }

        /// <summary>
        /// 检测当前的反魔能量，如果超过上限就TM爆了
        /// </summary>
        public virtual void CheckAntiMagike()
        {
            if (AntiMagike >= AntiMagikeMax)
                AntiMagikeOverflow();
        }

        #endregion

        #region UI

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.MagikeContainerName, parent);

            //添加魔能量显示条，在左边
            ContainerBar bar = new(this);
            bar.Top.Set(title.Height.Pixels + 8, 0);
            parent.Append(bar);

            AntiContainerBar bar2 = new(this);
            bar2.Top.Set(bar.Top.Pixels + bar.Height.Pixels + 24, 0);
            parent.Append(bar2);

            //其他的文本信息在右侧
            UIList list =
            [
                //魔能量
                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeAmount), parent),
                this.NewTextBar(c =>
                {
                    string colorCode = c.GetMagikeContainerMaxColorCode();
                    return  $"  ▶ {c.Magike} / [c/{colorCode}:{c.MagikeMax}]";
                }, parent),

                //魔能上限
                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerMagikeMax), parent),
                this.NewTextBar(c =>
                    $"  ▶ {c.MagikeMax} ({c.MagikeMaxBase} * {Math.Abs(c.MagikeMaxBonus)})", parent),

                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerAntiMagikeAmount), parent),
                this.NewTextBar(c =>
                {
                    string colorCode = c.GetMagikeContainerMaxColorCode();
                    return $"  ▶ {c.AntiMagike} / [c/{colorCode}:{c.AntiMagikeMax}]";
                }, parent),

                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.ContainerAntiMagikeMax), parent),
                this.NewTextBar(c =>
                    $"  ▶ {c.AntiMagikeMax} ({c.AntiMagikeMaxBase} {(c.AntiMagikeMaxExtra >= 0 ? "+" : "-")} {Math.Abs(c.AntiMagikeMaxExtra)})", parent),
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, bar.Width.Pixels + 6);

            list.QuickInvisibleScrollbar();

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
            tag.Add(preName + nameof(MagikeMaxBonus), MagikeMaxBonus);

            tag.Add(preName + nameof(AntiMagike), AntiMagike);
            tag.Add(preName + nameof(AntiMagikeMaxBase), AntiMagikeMaxBase);
            tag.Add(preName + nameof(AntiMagikeMaxExtra), AntiMagikeMaxExtra);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            Magike = tag.GetInt(preName + nameof(Magike));
            MagikeMaxBase = tag.GetInt(preName + nameof(MagikeMaxBase));
            MagikeMaxBonus = tag.GetFloat(preName + nameof(MagikeMaxBonus));

            if (MagikeMaxBonus == 0)
                MagikeMaxBonus = 1;

            AntiMagike = tag.GetInt(preName + nameof(AntiMagike));
            AntiMagikeMaxBase = tag.GetInt(preName + nameof(AntiMagikeMaxBase));
            AntiMagikeMaxExtra = tag.GetInt(preName + nameof(AntiMagikeMaxExtra));
        }
    }

    public class ContainerBar : UIElement
    {
        protected MagikeContainer container;

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
                   (int)(tex.Height * (percent + (0.04f * MathF.Sin((((float)Main.timeForVisualEffects) * 0.1f) + (i * 0.3f)))))
                    , 0, tex.Height);

                Rectangle frameBox2 = new(frameBox.Width + (i * 2), tex.Height - currentHeight, 2, currentHeight);
                var origin = new Vector2(0, frameBox2.Height);
                spriteBatch.Draw(tex, drawPos + new Vector2(i * 2, 0), frameBox2, Color.White, 0, origin, 1f, 0, 0f);
            }

            string percentText = MathF.Round(100 * percent, 1) + "%";

            Utils.DrawBorderString(spriteBatch, percentText, center, Color.White, 0.75f, anchorx: 0.5f, anchory: 0.5f);
        }
    }

    public class AntiContainerBar(MagikeContainer container) : ContainerBar(container)
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.MagikeContainerBar.Value;

            var frameBox = tex.Frame(2, 1);

            var dimensions = GetDimensions();
            Vector2 pos = dimensions.Position() + new Vector2((dimensions.Width - frameBox.Width) / 2, 0);
            Vector2 center = dimensions.Center();

            //绘制底层
            spriteBatch.Draw(tex, pos, frameBox, Color.White);
            float percent = (float)container.AntiMagike / container.AntiMagikeMax;
            Color c = Color.Lerp(Color.Green, Color.Red, percent);

            Vector2 drawPos = pos + new Vector2(0, frameBox.Height);
            for (int i = 0; i < frameBox.Width / 2; i++)
            {
                int currentHeight = Math.Clamp(
                   (int)(tex.Height * (percent + (0.04f * MathF.Sin((((float)Main.timeForVisualEffects) * 0.1f) + (i * 0.3f)))))
                    , 0, tex.Height);

                Rectangle frameBox2 = new(i * 2, tex.Height - currentHeight, 2, currentHeight);
                var origin = new Vector2(0, frameBox2.Height);
                spriteBatch.Draw(tex, drawPos + new Vector2(i * 2, 0), frameBox2, c, 0, origin, 1f, 0, 0f);
            }

            string percentText = MathF.Round(100 * percent, 1) + "%";

            Utils.DrawBorderString(spriteBatch, percentText, center, c, 0.75f, anchorx: 0.5f, anchory: 0.5f);
        }
    }
}
