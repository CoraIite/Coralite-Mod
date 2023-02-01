using Coralite.Content.Projectiles.RedJadeProjectiles;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJadeItems
{
    public class RedJadeShrine : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("赤玉祭坛");

            Tooltip.SetDefault("召唤小赤玉灵为你作战");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 17;
            Item.useTime = 35;
            Item.useAnimation = 18;
            Item.knockBack = 4f;
            Item.maxStack = 1;

            Item.autoReuse = true;
            Item.useTurn = false;

            //Item.shoot = ModContent.ProjectileType<RedJadeStrike>();
            Item.UseSound = SoundID.Item1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }
    }
}
