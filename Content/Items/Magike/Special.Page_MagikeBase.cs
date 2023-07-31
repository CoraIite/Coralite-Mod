using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Magike
{
    public class Page_MagikeBase: ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;
        }

        public override bool CanUseItem(Player player)
        {
            MagikeSystem.learnedMagikeBase = true;
            MagikeHelper.SpawnDustOnGenerate(3, 3, player.Center.ToPoint16(), Coralite.Instance.MagicCrystalPink);

            return true;
        }
    }
}
