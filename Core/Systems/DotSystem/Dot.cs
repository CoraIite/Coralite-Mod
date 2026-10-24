using Coralite.Content.CustomHooks;
using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
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
    /// 是否已请求在下一次NPC更新时移除
    /// </summary>
    public bool NotActive { get; private set; }

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
        var instance = (Dot)System.Activator.CreateInstance(GetType(), true);
        instance.Type = Type;
        return instance;
    }

    /// <summary>
    /// 每成功添加一层dot后调用，传入该层的伤害信息
    /// </summary>
    public virtual void OnAdd(DotInfo info) { }

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
                existingDot.NotActive = false;
                dot = existingDot;
                break;
            }
        }

        bool isNew = dot == null;
        if (isNew)
            dot = template.NewInstance();

        //重置层数上限
        dot.ExtraDotLayer = 0;

        for (int i = 0; i < numAdd; i++)
        {
            DotInfo info = new DotInfo(player, damage, crit);
            dot.DotInfos.Enqueue(info);
            dot.OnAdd(info);
        }

        int maxLayer = System.Math.Max(1, dot.MaxDotLayer);
        while (dot.DotInfos.Count > maxLayer)
            dot.DotInfos.Dequeue();

        if (isNew)
            globalNPC.Dots.Add(dot);

        if (!Main.dedServ)
            Overlays.Scene.Activate(nameof(DotBarDrawOverlay));

        return true;
    }

    /// <summary>
    /// 标记移除，由DotGlobalNPC在下一次更新开始时移除
    /// </summary>
    public void RemoveDot() => NotActive = true;

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
                npc.SimpleStrikeNPC(GetDamage(info), 1, Main.rand.NextBool(info.critChance, 100), 0, DamageType, false, 0, true);
        }

        if (CurrentLayer <= 0)
            RemoveDot();
    }

    /// <summary>
    /// 获取这个dot所造成的具体伤害，默认从传入的info获取伤害，也可以遍历<see cref="DotInfos"/>从所有dot层数中获取
    /// </summary>
    /// <param name="info"></param>
    public virtual int GetDamage(DotInfo info)
        => info.damage;

    #region 网络同步

    #endregion

    #region 绘制dot

    public const int MaxBarWidth = 120;

    public virtual void DrawDotBar(Vector2 center)
    {
        int maxLayer = MaxDotLayer;
        if (maxLayer <= 0)
            return;

        float width = (float)80 / maxLayer;
        Vector2 left = center - new Vector2(80 / 2f, 0);
        left.X += width / 2f;

        Texture2D tex = CoraliteAssets.Misc.White32x32.Value;
        Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
        Vector2 scale = new Vector2(System.Math.Max(2f, width - 4f) / 32f, 8 / 32f)*Main.UIScale;
        Vector2 position = left;

        Color backColor = new Color(10, 10, 10);
        backColor *= 0.6f;

        for (int i = 0; i < maxLayer; i++)
        {
            Main.spriteBatch.Draw(tex, position + new Vector2(-2, 0), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(2, 0), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, -2), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, position + new Vector2(0, 2), null, backColor, 0, origin, scale, SpriteEffects.None, 0);
            position.X += width;
        }

        Color color = Color.White;
        if (DotSystem.DotTags != null && Type >= 0 && Type < DotSystem.DotTags.Length
            && DotSystem.DotTags[Type] is { Length: > 0 } tags)
            color = DotLoader.GetDotTag(tags[0])?.TagColor ?? Color.White;

        position = left;
        int layer = System.Math.Min(CurrentLayer, maxLayer);
        for (int i = 0; i < layer; i++)
        {
            Main.spriteBatch.Draw(tex, position, null, color, 0, origin, scale, SpriteEffects.None, 0);
            position.X += width;
        }

        //绘制icon
        Texture2D icon = DotSystem.DotAssets[Type].Value;
        Vector2 iconPosition = left + new Vector2(-12 - icon.Width / 2f, 0);
        Main.spriteBatch.Draw(icon, iconPosition, null, Color.White, 0, new Vector2(icon.Width / 2f, icon.Height / 2f), 1f, SpriteEffects.None, 0);

        //绘制数字
        Vector2 textPosition = center + new Vector2(80 / 2f + 8, 4);
        Utils.DrawBorderString(Main.spriteBatch, CurrentLayer.ToString(), textPosition, color, 1f, 0f, 0.5f);
    }

    public virtual void DrawDotExtraDescription()
    {

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
