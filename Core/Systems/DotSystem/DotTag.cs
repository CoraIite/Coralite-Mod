using Coralite.Core.Loaders;
using Terraria.Localization;

namespace Coralite.Core.Systems.DotSystem;

public abstract class DotTag : ModType, ILocalizedModType
{
    public int Type { get; internal set; }

    /// <summary>
    /// 标签的本地化名称
    /// </summary>
    public LocalizedText DisplayName { get; private set; }

    public virtual Color TagColor => Color.White;
    public virtual Color CritColor => TagColor;

    public string LocalizationCategory => "Dots.DotTags";

    public override void Load()
    {
        DisplayName = this.GetLocalization(nameof(Name));
    }

    protected override void Register()
    {
        ModTypeLookup<DotTag>.Register(this);

        DotLoader.dotTags ??= [];
        DotLoader.dotTags.Add(this);

        Type = DotLoader.ReserveDotTagID();
    }
}
