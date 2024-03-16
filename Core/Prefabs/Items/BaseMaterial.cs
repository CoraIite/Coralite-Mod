using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BaseMaterial : ModItem
    {
        private readonly int MaxStack;
        private readonly int Value;
        private readonly int Rare;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        protected BaseMaterial(int maxStack, int value, int rare, string texturePath, bool pathHasName = false)
        {
            MaxStack = maxStack;
            Value = value;
            Rare = rare;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

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
