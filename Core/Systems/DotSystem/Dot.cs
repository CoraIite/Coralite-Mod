using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Content.UI.NewKnowledgeUnlock;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.DotSystem.DotAccessories;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Systems.DotSystem;

public abstract class Dot : ModTexturedType,ILocalizedModType
{
    public int Type { get; internal set; }

    /// <summary> 附着的本地化名称 </summary>
    public LocalizedText DisplayName { get; private set; }
    /// <summary> 附着的本地化描述 </summary>
    public LocalizedText Description { get; private set; }

    /// <summary>
    /// 退出计时器，0表示正常，达到30时移除
    /// </summary>
    public int NotActive { get; internal set; }

    private Queue<DotInfo> _dotInfos;

    /// <summary>
    /// 存储所有的伤害
    /// </summary>
    public Queue<DotInfo> DotInfos
    {
        get
        {
            _dotInfos ??= [];
            return _dotInfos;
        }
    }

    public virtual int CurrentLayer
    {
        get => DotInfos.Count;
    }

    /// <summary>
    /// 这个dot的伤害类型
    /// </summary>
    public virtual DamageClass DamageType => DamageClass.Generic;

    /// <summary>
    /// 这个附着的基础层数
    /// </summary>
    public virtual int BaseMaxDotLayer => 1;

    /// <summary>
    /// 额外的附着层数上限
    /// </summary>
    public int ExtraDotLayer;

    /// <summary>
    /// 当前总层数上限，包括基础层数和额外层数
    /// </summary>
    public int MaxDotLayer => BaseMaxDotLayer + ExtraDotLayer;

    public string LocalizationCategory => "Dots";

    public override void Load()
    {
        DisplayName = this.GetLocalization(nameof(Name));
        Description = this.GetLocalization(nameof(Description));
    }

    protected sealed override void Register()
    {
        ModTypeLookup<Dot>.Register(this);

        DotLoader.dots ??= [];
        DotLoader.dots.Add(this);

        Type = DotLoader.ReserveDotID();
    }

    /// <summary>
    /// 加载这个dot具有的标签类型，在所有内容初始化完成后调用
    /// </summary>
    public virtual int[] LoadTags() => [];

    /// <summary>
    /// 更新dot
    /// </summary>
    /// <param name="npc"></param>
    public virtual void Update(NPC npc) { }

    /// <summary>
    /// 修改附着目标受到的伤害
    /// </summary>
    public virtual void ModifyNPCHit(NPC npc, ref NPC.HitModifiers modifiers) { }

    #region 增删dot部分

    /// <summary>
    /// 创建用于NPC的独立实例，并保留注册的类型ID
    /// </summary>
    public virtual Dot NewInstance()
    {
        var instance = (Dot)Activator.CreateInstance(GetType(), true);
        instance.Type = Type;
        instance.DisplayName = DisplayName;
        instance.Description = Description;
        return instance;
    }

    /// <summary>
    /// 每成功添加一层dot后调用，传入该层的伤害信息
    /// </summary>
    public virtual void OnAdd(DotInfo info) { }

    public static bool AddDot<T>(NPC npc, int damage, Projectile proj, int numAdd = 1) where T : Dot
        => AddDot<T>(npc, damage, proj.owner, proj.CritChance, numAdd);

    /// <summary>
    /// 向一个NPC加入dot
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="npc"></param>
    /// <param name="damage"></param>
    /// <param name="player"></param>
    /// <param name="numAdd"></param>
    /// <returns></returns>
    public static bool AddDot<T>(NPC npc, int damage, int player, int crit, int numAdd = 1) where T : Dot
    {
        if (npc == null || !npc.active || numAdd <= 0
            || !npc.TryGetGlobalNPC(out DotGlobalNPC globalNPC))
            return false;

        //获取dot
        T template = ModContent.GetInstance<T>();
        int type = template.Type;
        Dot dot = null;
        foreach (var existingDot in globalNPC.Dots)
        {
            if (existingDot.Type == type)
            {
                dot = existingDot;
                break;
            }
        }

        bool isNew = dot == null;
        if (isNew)
            dot = template.NewInstance();

        dot.NotActive = 0;

        //重置层数上限
        dot.ExtraDotLayer = 0;

        for (int i = 0; i < numAdd; i++)
        {
            DotInfo info = new DotInfo(player, damage, crit);
            dot.DotInfos.Enqueue(info);
            dot.OnAdd(info);
        }

        dot.OtherDotAdd(npc);

        int maxLayer = Math.Max(1, dot.MaxDotLayer);
        while (dot.DotInfos.Count > maxLayer)
            dot.DotInfos.Dequeue();

        if (isNew)
            globalNPC.Dots.Add(dot);

        if (!Main.dedServ)
            Overlays.Scene.Activate(nameof(DotBarDrawOverlay));

        return true;
    }

    /// <summary>
    /// 添加dot层后、限制层数前，通知NPC身上的其他dot，每次AddDot调用一次
    /// </summary>
    /// <param name="npc"></param>
    public virtual void OtherDotAdd(NPC npc)
    {
        if (!npc.TryGetGlobalNPC(out DotGlobalNPC globalNPC))
            return;

        foreach (Dot dot in globalNPC.Dots)
        {
            if (dot != this)
                dot.OnOtherDotAdd(this, npc);
        }
    }

    /// <summary>
    /// 同一个NPC上的其他dot添加层数后、限制层数前调用
    /// </summary>
    /// <param name="dot">正在添加层数的dot</param>
    /// <param name="npc"></param>
    public virtual void OnOtherDotAdd(Dot dot, NPC npc)
    {
    }

    /// <summary>
    /// 启动退出计时，由DotGlobalNPC在计时达到30时移除
    /// </summary>
    public void RemoveDot()
    {
        if (NotActive == 0)
            NotActive = 1;
    }

    #endregion


    /// <summary>
    /// 将dot伤害施加给这个NPC，默认效果移除最早添加的dot层，全部移除后将这个dot消除
    /// </summary>
    /// <param name="npc"></param>
    public void ApplyDot(NPC npc)
    {
        if (DotInfos.TryDequeue(out DotInfo info))
        {
            if (Main.myPlayer == info.player)
            {
                var hitModifyer = npc.GetIncomingStrikeModifiers(DamageType, 0);
                ModifyDamage(npc, ref hitModifyer, info);//这里可以自定义伤害
                ApplyOtherDots(npc,ref hitModifyer, info);

                var hit = hitModifyer.ToHitInfo(GetDamage(info), Main.rand.NextBool(Math.Clamp(info.critChance, 0, 100), 100), 0, false, 0);
                hit.HideCombatText = true;
                NewDamageText(npc, hit, info);

                npc.StrikeNPC(hit, fromNet: false, true);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendStrikeNPC(npc, hit);
            }
        }

        int maxLayer = Math.Max(1, MaxDotLayer);
        while (DotInfos.Count > maxLayer)
            DotInfos.Dequeue();

        if (CurrentLayer <= 0)
            RemoveDot();
    }

    /// <summary>
    /// 在这个dot造成伤害后通知NPC身上的其他dot
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="hitModifyer"></param>
    /// <param name="dotInfo"></param>
    public virtual void ApplyOtherDots(NPC npc,ref NPC.HitModifiers hitModifyer, DotInfo dotInfo)
    {
        if (!npc.TryGetGlobalNPC(out DotGlobalNPC globalNPC))
            return;

        foreach (Dot dot in globalNPC.Dots)
        {
            if (dot != this)
                dot.OnOtherDotApply(this, npc,ref hitModifyer, dotInfo);
        }
    }

    /// <summary>
    /// 当其他dot在同一个NPC上造成伤害时调用
    /// </summary>
    /// <param name="dot">正在造成伤害的DOT</param>
    /// <param name="npc"></param>
    /// <param name="hitModifyer"></param>
    /// <param name="dotInfo"></param>
    public virtual void OnOtherDotApply(Dot dot, NPC npc,ref NPC.HitModifiers hitModifyer, DotInfo dotInfo)
    {
    }

    /// <summary>
    /// 自定义伤害跳字
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="info"></param>
    public virtual void NewDamageText(NPC target,NPC.HitInfo hit,DotInfo info)
    {

    }

    /// <summary>
    /// 自定义伤害的各种东西
    /// </summary>
    /// <param name="target"></param>
    /// <param name="modifyer"></param>
    /// <param name="info"></param>
    public virtual void ModifyDamage(NPC target, ref NPC.HitModifiers modifyer, DotInfo info)
    {

    }

    /// <summary>
    /// 获取这个dot所造成的具体伤害，默认从传入的info获取伤害，也可以遍历<see cref="DotInfos"/>从所有dot层数中获取
    /// </summary>
    /// <param name="info"></param>
    public virtual int GetDamage(DotInfo info)
        => info.damage;


    /// <summary>
    /// 应用一名玩家提供的属性加成，不负责重置或叠加dot层数
    /// </summary>
    public virtual void ApplyPlayerBonuses(Player player)
    {
        if (player.TryGetModPlayer(out CoralitePlayer cp))
            foreach (var effect in cp.EffectHandlers)
                if (effect is IDessertBonus bonus)
                    bonus.ModifyDessertDot(this);
    }

    #region 网络同步

    #endregion

    #region 绘制dot

    public const int MaxBarWidth = 80;

    public virtual void DrawDotBar(ref Vector2 center)
    {
        int maxLayer = MaxDotLayer;
        if (maxLayer <= 0)
            return;

        float opacity = 1f - MathHelper.Clamp(NotActive / 30f, 0f, 1f);
        float width = (float)MaxBarWidth / maxLayer;
        Vector2 left = center - new Vector2(MaxBarWidth / 2f, 0);

        Texture2D tex = CoraliteAssets.Misc.White32x32.Value;
        Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);

        // 横向尺寸与分段位置使用相同坐标，总宽度固定为MaxBarWidth
        float borderWidth = Math.Min(2f, width / 4f);
        Vector2 scale = new Vector2((width - borderWidth * 2f) / tex.Width, 8f * Main.UIScale / tex.Height);
        Vector2 position = left;
        position.X += width / 2f;

        Color backColor = new Color(10, 10, 10);
        backColor *= 0.6f * opacity;

        for (int i = 0; i < maxLayer; i++)
        {
            Main.spriteBatch.Draw(tex, position + new Vector2(-borderWidth, 0), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(borderWidth, 0), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, -2), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, 2), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            position.X += width;
        }

        Color color = Color.White;
        if (DotSystem.DotTags[Type] is { Length: > 0 } tags)
            color = DotLoader.GetDotTag(tags[0])?.TagColor ?? Color.White;

        position = left;
        position.X += width / 2f;

        int layer = Math.Min(CurrentLayer, maxLayer);
        float fillBorderWidth = Math.Min(1f, width / 12f);
        float fillWidth = width - borderWidth * 2f;
        scale = new Vector2((fillWidth - fillBorderWidth * 2f) / tex.Width, 6f * Main.UIScale / tex.Height);
        Vector2 scale2 = new Vector2((fillWidth - fillBorderWidth * 4f) / tex.Width, 4f * Main.UIScale / tex.Height);
        float perAdd = MathHelper.TwoPi / maxLayer;

        for (int i = 0; i < layer; i++)
        {
            Color c2 = color * (0.7f + 0.15f * MathF.Sin(-Main.GlobalTimeWrappedHourly*3 + i * perAdd));
            Main.spriteBatch.Draw(tex, position + new Vector2(-fillBorderWidth, 0), null, c2 * opacity, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(fillBorderWidth, 0), null, c2 * opacity, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, -1), null, c2 * opacity, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, 1), null, c2 * opacity, 0, origin, scale, SpriteEffects.None, 0);

            //c2 *= 0.9f;
            c2.A = 255;

            Main.spriteBatch.Draw(tex, position, null, c2 * opacity, 0, origin, scale2, SpriteEffects.None, 0);
            position.X += width;
        }

        //绘制icon
        Texture2D icon = DotSystem.DotAssets[Type].Value;
        Vector2 iconPosition = left + new Vector2(-8 - icon.Width / 2f, 0);
        origin = icon.Size() / 2;
        Color color3 = Color.White * opacity;
        Color color2 = color3;
        color2 *= 0.3f;
        color2.A = 0;

        for (int i = 0; i < 4; i++)
        {
            Main.spriteBatch.Draw(icon, iconPosition + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 2).ToRotationVector2() * 2f, null, color2, 0, origin, 1f, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Draw(icon, iconPosition, null, color3, 0, origin, 1f, SpriteEffects.None, 0);


        //绘制数字
        Vector2 textPosition = center + new Vector2(MaxBarWidth / 2f + 8, 4);
        string text = CurrentLayer.ToString();
        var font = Terraria.GameContent.FontAssets.MouseText.Value;
        Vector2 textOrigin = font.MeasureString(text) * new Vector2(0f, 0.5f);
        Utils.DrawBorderStringFourWay(Main.spriteBatch, font, text, textPosition.X, textPosition.Y, color * opacity, Color.Black * opacity, textOrigin);

        center.Y += MathF.Max(icon.Height + 8, 30);
    }

    /// <summary>
    /// 获取面板属性行，子类可调用base后继续添加
    /// </summary>
    public virtual List<string> GetDotTipProperties()
        => [DotSystem.MaxLayers.Format(Math.Max(1, MaxDotLayer))];

    private const float DotTipSeparatorAdvance = 6f;
    private const float DotTipTextOffsetY = 4f;

    private readonly record struct DotTipText(string Text, float Scale, bool Centered, float MaxWidth, Vector2 Offset);

    private sealed class DotTipLayout
    {
        public List<DotTipText> Texts = [];
        public (float First, float Second, float Third) SeparatorOffsets;
        public Color LineColor;
        public Vector2 Size;
    }

    private DotTipLayout GetDotTipLayout(Item item, float damageMultiplier)
    {
        Player player = Main.LocalPlayer;
        int damage = player.GetWeaponDamage(item);
        Dot preview = NewInstance();
        preview.ExtraDotLayer = 0;
        preview.ApplyPlayerBonuses(player);

        var (tags, lineColor) = GetDotTipTags();
        var layout = new DotTipLayout { LineColor = lineColor };
        float width = UIConfig.MouseDotTipWidth;
        float height = 0f;

        AddText(DisplayName.Value, 1.1f, true, width);
        float firstLineY = AddSeparator();

        AddText(string.Concat((int )(damage* damageMultiplier), " (", damageMultiplier,"×",damage, ") ", preview.DamageType.DisplayName.Value.Trim()), 1f, true, width);
        AddText(tags, 1f, true, -1);
        float secondLineY = AddSeparator();

        foreach (string property in preview.GetDotTipProperties())
            AddText(property, 0.9f, false, width);
        float thirdLineY = AddSeparator();

        AddText(preview.Description.Value, 0.9f, false, width);

        layout.SeparatorOffsets = (firstLineY, secondLineY, thirdLineY);
        layout.Size = new Vector2(width, height);
        return layout;

        void AddText(string text, float scale, bool centered, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return;

            if (maxWidth>0)
            {
                maxWidth += scale * 30;
            }

            Vector2 offset = new Vector2(centered ? width / 2f : 0, height + DotTipTextOffsetY);
            layout.Texts.Add(new DotTipText(text, scale, centered, maxWidth, offset));
            height += Helper.GetStringSize(text, Vector2.One * scale, maxWidth).Y;
        }

        float AddSeparator()
        {
            float offset = height;
            height += DotTipSeparatorAdvance;
            return offset;
        }
    }

    private (string Text, Color LineColor) GetDotTipTags()
    {
        if (DotSystem.DotTags == null || Type < 0 || Type >= DotSystem.DotTags.Length
            || DotSystem.DotTags[Type] is not { Length: > 0 } tags)
            return (string.Empty, Color.White);

        List<string> tagNames = new List<string>(tags.Length);
        Color lineColor = Color.White;
        for (int i = 0; i < tags.Length; i++)
        {
            DotTag tag = DotLoader.GetDotTag(tags[i]);
            if (tag == null)
                continue;

            Color color = tag.TagColor;
            if (i == 0)
                lineColor = color;

            tagNames.Add($"[c/{color.R:X2}{color.G:X2}{color.B:X2}:{tag.DisplayName.Value}]");
        }

        return (string.Join(", ", tagNames), lineColor);
    }

    /// <summary>
    /// 获取dot说明内容的尺寸，宽度由UIConfig控制，高度包含全部文字、分隔线和间距
    /// </summary>
    public virtual Vector2 GetDotTipTotalSize(Item item, float damageMultiplier = 1f)
        => GetDotTipLayout(item, damageMultiplier).Size;

    /// <summary>
    /// 绘制dot名称、伤害、标签、属性和描述，不包含背景
    /// </summary>
    public virtual void DrawDotExtraDescription(Vector2 topLeft, Item item, float damageMultiplier = 1f)
    {
        DotTipLayout layout = GetDotTipLayout(item, damageMultiplier);
        foreach (DotTipText text in layout.Texts)
        {
            Helper.DrawText(Main.spriteBatch, text.Text, text.MaxWidth, topLeft + text.Offset
                , new Vector2(text.Centered ? 0.5f : 0, 0), Vector2.One * text.Scale
                , new Color(30, 30, 30), Color.White, out _);
        }

        DrawDotTipSeparators(topLeft, layout);
    }

    private static void DrawDotTipSeparators(Vector2 topLeft, DotTipLayout layout)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
        Effect effect = ShaderLoader.GetShader("SinLine");
        effect.Parameters["flowPercent"].SetValue(0.03f);
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
        effect.Parameters["uFlowTime"].SetValue(-(float)Main.timeForVisualEffects * 0.003f);

        //三条线共用同一个shader批次
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap
            , DepthStencilState.None, EffectLoader.OverflowHiddenRasterizerState, effect, Main.UIScaleMatrix);

        float halfWidth = layout.Size.X / 2f;
        float centerX = topLeft.X + halfWidth;
        NewKnowledgeBar.DrawLine(spriteBatch, new Vector2(centerX, topLeft.Y + layout.SeparatorOffsets.First), layout.LineColor, 1f, halfWidth);
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f + 5);
        effect.Parameters["uFlowTime"].SetValue(-(float)Main.timeForVisualEffects * 0.003f-0.3f);

        NewKnowledgeBar.DrawLine(spriteBatch, new Vector2(centerX, topLeft.Y + layout.SeparatorOffsets.Second), layout.LineColor, 1f, halfWidth);
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f + 10);
        effect.Parameters["uFlowTime"].SetValue(-(float)Main.timeForVisualEffects * 0.003f-0.6f);


        NewKnowledgeBar.DrawLine(spriteBatch, new Vector2(centerX, topLeft.Y + layout.SeparatorOffsets.Third), layout.LineColor, 1f, halfWidth);

        spriteBatch.End();
        spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp
            , DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
    }

    #endregion
}

public record DotInfo
{
    public readonly int player;
    public readonly int damage;
    /// <summary>
    /// 与给玩家增加暴击率相同，为整数的暴击几率
    /// </summary>
    public readonly int critChance;

    public DotInfo(int player, int damage, int critChance)
    {
        this.player = player;
        this.damage = damage;
        this.critChance = critChance;
    }
}
