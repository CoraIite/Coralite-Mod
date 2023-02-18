using Coralite.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Weapons
{
    internal class BlazingBlade : ModItem
    {
        public override string Texture => AssetDirectory.BlazingItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("炽焰之刃");

            Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;

            Item.damage = 10;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;

            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.scale = 1.3f;
        }

        //public override void MeleeEffects(Player player, Rectangle hitbox)
        //{
        //    Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.FlameBurst, 1, 1, 100, Color.White, 1f);
        //}
    }
}
