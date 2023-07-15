using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public class BaseAccessory:ModItem
    {
        public override string Texture => AssetDirectory.Accessories+Name;

        private readonly int Rare;
        private readonly int Value;

        public BaseAccessory(int rare,int value)
        {
            Rare = rare;
            Value = value;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.DefaultToAccessory();
            Item.rare = Rare;
            Item.value = Value;
        }
    }
}
