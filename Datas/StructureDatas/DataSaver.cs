using Coralite.Content.WorldGeneration.MagikeShrineDatas;
using Coralite.Core;
using Terraria;
using Terraria.ID;

namespace Coralite.Datas.StructureDatas
{
#if DEBUG
    public class DataSaver : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool CanUseItem(Player player)
        {
            ApparatusShrine1.DoSave<ApparatusShrine1>();
            return base.CanUseItem(player);
        }
    }
#endif
}
