using Coralite.Helpers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BasePlantMaterial : ModItem
    {
        private readonly string _Name;
        private readonly string _Tooltip;
        private readonly int Maxstack;
        private readonly int Value;
        private readonly int Rare;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        protected BasePlantMaterial(string name, string tooltip, int maxstack, int value, int rare, string texturePath = AssetDirectory.Plants, bool pathHasName = false)
        {
            _Name = name;
            _Tooltip = tooltip;
            Maxstack = maxstack;
            Value = value;
            Rare = rare;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault(_Name);
            // Tooltip.SetDefault(_Tooltip);
        }

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
            Item.maxStack = Maxstack;
            Item.value = Value;
            Item.rare = Rare;

            Item.GetBotanicalItem().botanicalItem = true;
        }
    }
}
