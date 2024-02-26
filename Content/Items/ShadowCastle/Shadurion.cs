using Coralite.Core;
using Coralite.Core.Systems.SlashBladeSystem;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ShadowCastle
{
    public class Shadurion : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public int shadowShootCount;

        public ComboManager comboManager;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            //Item.shoot = ProjectileType<ShaduraSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.SetWeaponValues(100, 4, 0);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

    }
}
