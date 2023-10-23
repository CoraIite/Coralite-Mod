using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class QueensWreath:ModItem,INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 24;
        }

    }
}
