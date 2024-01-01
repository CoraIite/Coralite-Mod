using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class BrokenHandle : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public int useCount;
        public int oldCombo;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 112;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 2f;

            Item.holdStyle = ItemHoldStyleID.HoldGolfClub;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = RarityType<RareRarity>();
            //Item.shoot = ProjectileType<RuneSoneSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Helper.PlayPitched("CoreKeeper/swordLegendaryAttack", 0.5f, 0, player.Center);
            if (Main.myPlayer == player.whoAmI)
            {
                int combo = Main.rand.Next(2);
                if (combo == oldCombo)
                {
                    useCount++;
                    if (useCount > 1)
                    {
                        useCount = 0;
                        combo = combo switch
                        {
                            0 => 1,
                            _ => 0,
                        };

                    }
                }
                Projectile.NewProjectile(source, player.Center, Vector2.Zero,
                    type, damage, knockback, player.whoAmI, combo);

                oldCombo = combo;
            }

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Damage", null);
            if (tooltip != null)
            {
                bool addLeadingSpace = Item.DamageType is not VanillaDamageClass;
                string tip = (addLeadingSpace ? " " : "") + Item.DamageType.DisplayName;

                tooltip.Text = string.Concat(((int)(Item.damage * 0.903f)).ToString()
                    , "-", ((int)(Item.damage * 1.098f)).ToString(), tip);
            }
        }

        public override bool AllowPrefix(int pre)
        {
            return true;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

    }
}
