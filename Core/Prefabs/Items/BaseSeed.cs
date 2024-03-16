using Coralite.Helpers;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Prefabs.Items
{
    public class BaseSeed : ModItem
    {
        private readonly int MaxStack;
        private readonly int Value;
        private readonly int Rare;
        private readonly int DominantGrowTime;
        private readonly int RecessiveGrowTime;
        private readonly int DominantLevel;
        private readonly int RecessiveLevel;
        private readonly int PlantTileType;
        private readonly string TexturePath;
        private readonly bool PathHasName;

        protected BaseSeed(int maxStack, int value, int rare, int DGrowTime, int RGrowTime, int DLevel, int RLevel, int plantTileType, string texturePath = AssetDirectory.Seeds, bool pathHasName = false)
        {
            MaxStack = maxStack;
            Value = value;
            Rare = rare;
            DominantGrowTime = DGrowTime;
            RecessiveGrowTime = RGrowTime;
            DominantLevel = DLevel;
            RecessiveLevel = RLevel;
            PlantTileType = plantTileType;
            TexturePath = texturePath;
            PathHasName = pathHasName;
        }

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.width = 16;
            Item.height = 16;
            Item.placeStyle = 0;
            Item.maxStack = MaxStack;
            Item.value = Value;
            Item.rare = Rare;
            Item.GetBotanicalItem().botanicalItem = true;
            Item.GetBotanicalItem().DominantGrowTime = DominantGrowTime;
            Item.GetBotanicalItem().RecessiveGrowTime = RecessiveGrowTime;
            Item.GetBotanicalItem().DominantLevel = DominantLevel;
            Item.GetBotanicalItem().RecessiveLevel = RecessiveLevel;
            Item.createTile = PlantTileType;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("DominantGrowTime", Item.GetBotanicalItem().DominantGrowTime);
            tag.Add("RecessiveGrowTime", Item.GetBotanicalItem().RecessiveGrowTime);
            tag.Add("DominantLevel", Item.GetBotanicalItem().DominantLevel);
            tag.Add("RecessiveLevel", Item.GetBotanicalItem().RecessiveLevel);
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            Item.GetBotanicalItem().DominantGrowTime = tag.GetInt("DominantGrowTime");
            Item.GetBotanicalItem().RecessiveGrowTime = tag.GetInt("RecessiveGrowTime");
            Item.GetBotanicalItem().DominantLevel = tag.GetInt("DominantLevel");
            Item.GetBotanicalItem().RecessiveLevel = tag.GetInt("RecessiveLevel");
            base.LoadData(tag);
        }
    }
}
