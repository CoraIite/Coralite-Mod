using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseMaterial(int maxStack, int value, int rare, string texturePath, bool pathHasName = false) : ModItem
    {
        public override string Texture => string.IsNullOrEmpty(texturePath) ? base.Texture : texturePath + (pathHasName ? string.Empty : Name);

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
            Item.maxStack = maxStack;
            Item.value = value;
            Item.rare = rare;
        }
    }
}
