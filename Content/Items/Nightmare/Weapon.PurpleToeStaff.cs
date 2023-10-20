using Coralite.Core;
using System.ComponentModel.DataAnnotations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class PurpleToeStaff : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 145;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.reuseDelay = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 15;
            Item.knockBack = 3;

            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = RarityType<NightmareRarity>();
            Item.shoot = ProjectileType<NightmareRaven>();
            Item.UseSound = CoraliteSoundID.TerraprismaSummon_Item82;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
        }

    }

    public class PurpleToeProj : ModProjectile, INightmareMinion
    {
        public override string Texture => AssetDirectory.NightmareItems + "PurpleToe";

        public void GetPower(int howMany)
        {

        }
    }
}
