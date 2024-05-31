using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseMaterial(int maxStack, int value, int rare, string texturePath, bool pathHasName = false) : ModItem
    {
        private readonly int MaxStack = maxStack;
        private readonly int Value = value;
        private readonly int Rare = rare;
        private readonly string TexturePath = texturePath;
        private readonly bool PathHasName = pathHasName;

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.width = 16;
            Item.height = 16;
            Item.placeStyle = 0;
            Item.maxStack = MaxStack;
            Item.value = Value;
            Item.rare = Rare;
        }
    }
}
