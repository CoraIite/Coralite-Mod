using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Core.Prefabs.Items
{
    public class BaseDoorItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        protected BaseDoorItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 28;
            Item.maxStack = 99;
            Item.value = Value;
            Item.rare = Rare;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = CreateTile;
        }

    }

    public class BaseBedItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseBedItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 99;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.value = Value;
            Item.rare = Rare;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = CreateTile;
        }
    }

    public class BaseWorkBenchItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseWorkBenchItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 14;
            Item.maxStack = 99;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.value = Value;
            Item.rare = Rare;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = CreateTile;
        }
    }

    public class BaseTableItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseTableItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(CreateTile);
            Item.value = Value;
            Item.maxStack = 99;
            Item.width = 38;
            Item.height = 24;
            Item.rare = Rare;
        }
    }

    public class BaseChairItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseChairItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(CreateTile);
            Item.value = Value;
            Item.maxStack = 99;
            Item.width = 12;
            Item.height = 30;
            Item.rare = Rare;
        }
    }

    public class BaseDroplightItem:ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseDroplightItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.value = Value;
            Item.rare = Rare;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = CreateTile;
        }
    }

    public class BaseCandleItem : ModItem
    {
        private readonly int Value;
        private readonly int Rare;
        private readonly int CreateTile;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        public BaseCandleItem(int value, int rare, int createTile, string texturePath, bool pathHasName = false)
        {
            Value = value;
            Rare = rare;
            CreateTile = createTile;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.maxStack = 999;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.value = Value;
            Item.rare = Rare;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = CreateTile;
        }
    }
}
