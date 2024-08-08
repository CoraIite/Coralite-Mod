using Terraria;
using Terraria.ID;

namespace Coralite.Core.Prefabs.Items
{
    public abstract class BasePotion(int buffTime, int value, int rare, string texturePath, bool pathHasName = false) : ModItem
    {
        private readonly string TexturePath = texturePath;
        private readonly bool PathHasName = pathHasName;

        public abstract int BuffType { get; }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetDefaults()
        {
            Item.UseSound = CoraliteSoundID.Drink_Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation =   Item.useTime = 17;
            Item.maxStack = Item.CommonMaxStack;

            Item.useTurn = true;
            Item.consumable = true;
            Item.buffType = BuffType;
            Item.buffTime = buffTime;
            Item.value = value;
            Item.rare = rare;

        }
    }
}