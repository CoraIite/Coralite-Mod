using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Nightmare
{
    public class BloodJadeMagicWeapon : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = 1;
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.SetWeaponValues(166, 4, 4);
            Item.mana = 8;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 13;
            Item.UseSound = CoraliteSoundID.NoUse_BlowgunPlus_Item65;
        }

    }
}
