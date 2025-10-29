using Coralite.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    public class PhantomSpark : ModItem,IDashable
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public int useCount;
        public int oldCombo;
        //private int holdItemCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.SetWeaponValues(216, 5, 12);
            Item.useTime = 26;
            Item.useAnimation = 26;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = RarityType<LegendaryRarity>();
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

        public bool Dash(Player Player, int DashDir)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlyphParchment>()
                .AddIngredient<ChannelingGemstone>()
                .AddIngredient<FracturedLimbs>()
                .AddIngredient<EnergyString>()
                .AddIngredient(ItemID.HallowedBar, 100)
                .AddIngredient<AncientGemstone>(20)
                .AddCondition(CoraliteConditions.UseRuneParchment)
                .Register();
        }
    }
}
