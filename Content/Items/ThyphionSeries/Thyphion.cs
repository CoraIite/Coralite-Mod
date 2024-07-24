using Coralite.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Thyphion : ModItem
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 235;
            Item.shootSpeed = 7f;
            Item.knockBack = 0;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Red;
            Item.useTime = Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 0, 10);

            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.channel = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = new StatModifier(0, 0, 1000, 0);
        }
    }

    public class ThyphionHeldProj
    {

    }

    public class ThyphionArrow
    {

    }

    public class ThyphionPhantomDash
    {
        public enum BowType
        {
            /// <summary> 晚霞 </summary>
            AfterGlow = 0,

            End
        }
    }
}
