using Coralite.Helpers;

namespace Coralite.Core.Systems.MagikeSystem.BaseItems
{
    public abstract class MagikeChargeableItem(int magikeMax, int value, int rare, int magikeAmount = -1, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false) : ModItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

        public sealed override void SetDefaults()
        {
            SetDefs();
            Item.value = value;
            Item.rare = rare;
            Item.maxStack = 1;
            Item.GetMagikeItem().MagikeMax = magikeMax;
            Item.GetMagikeItem().magikeAmount = magikeAmount;
        }

        public virtual void SetDefs() { }
    }
}
