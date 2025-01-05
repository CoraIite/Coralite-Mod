using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class WhiteGardenia:ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public int shootCount;
        public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Glow");
            GlowMaskID = (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(150, 3f);
            Item.DefaultToRangedWeapon(ProjectileID.PurificationPowder, AmmoID.Bullet, 5, 11f);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Red;

            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.glowMask = GlowMaskID;
        }

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer==player.whoAmI)
            {

            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            return false;
        }
    }

    public class WhiteGardeniaHeldProj
    {

    }

    public class WhiteGardeniaFloat
    {

    }

    public class WhiteGardeniaAimProj
    {

    }
}
