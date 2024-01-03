using Coralite.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class PoisonousSickle:ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public int useCount;
        public int oldCombo;
        private int holdItemCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 194;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.knockBack = 4f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = RarityType<EpicRarity>();
            //Item.shoot = ProjectileType<RuneSongSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.expert = true;
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

    }
}
