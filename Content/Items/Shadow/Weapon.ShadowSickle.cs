using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Coralite.Content.Items.Shadow
{
    public class ShadowSickle : ModItem
    {
        public override string Texture => AssetDirectory.ShadowItems + Name;

        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 40;
            Item.damage = 24;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.knockBack = 0f;
            
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Rapier;

            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;

            Item.shoot = ModContent.ProjectileType<ShadowSickleProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<ShadowCrystal>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return rand.NextFromList(PrefixID.Keen, PrefixID.Superior, PrefixID.Forceful, PrefixID.Broken,
                PrefixID.Damaged, PrefixID.Shoddy, PrefixID.Hurtful, PrefixID.Strong, PrefixID.Unpleasant,
                PrefixID.Weak, PrefixID.Ruthless, PrefixID.Godly, PrefixID.Demonic, PrefixID.Zealous, PrefixID.Quick,
                PrefixID.Deadly, PrefixID.Agile, PrefixID.Nimble, PrefixID.Murderous, PrefixID.Slow, PrefixID.Sluggish,
                PrefixID.Lazy, PrefixID.Annoying, PrefixID.Nasty, PrefixID.Dangerous, PrefixID.Savage, PrefixID.Sharp,
                PrefixID.Pointy, PrefixID.Tiny, PrefixID.Terrible, PrefixID.Dull, PrefixID.Unhappy, PrefixID.Bulky, PrefixID.Shameful,
                PrefixID.Heavy, PrefixID.Light, PrefixID.Legendary);
        }

        public override bool AllowPrefix(int pre) => true;

        public override bool MeleePrefix() => true;
    }
}