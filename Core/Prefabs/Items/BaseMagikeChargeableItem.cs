using Coralite.Helpers;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseMagikeChargeableItem : ModItem
    {
        private readonly int magikeMax;
        private readonly int Value;
        private readonly int Rare;
        private readonly int magikeAmount;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseMagikeChargeableItem(int magikeMax, int value, int rare, int magikeAmount = -1, string texturePath = AssetDirectory.MagikeItems, bool pathHasName = false)
        {
            this.magikeMax = magikeMax;
            Value = value;
            Rare = rare;
            this.magikeAmount = magikeAmount;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public sealed override void SetDefaults()
        {
            SetDefs();
            Item.value = Value;
            Item.rare = Rare;
            Item.maxStack = 1;
            Item.GetMagikeItem().magikeMax = magikeMax;
            Item.GetMagikeItem().magikeAmount = magikeAmount;
        }

        public virtual void SetDefs() { }
    }
}
