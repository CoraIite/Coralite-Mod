using Coralite.Content.ModPlayers;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Content.Tiles.RedJades;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class SmokyRing : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(20, 4);

            Item.accessory = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as AquamarineBraceletProj).StartAttack();
            }

            return false;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Accessory))
                wr.Add(pre, 1);
            //foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Magic))
            //    wr.Add(pre, 1);
            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.AnyWeapon))
                wr.Add(pre, 1);

            float w = 0.5f;
            if (Main.LocalPlayer.GetModPlayer<CoralitePlayer>().HasEffect(nameof(EightsquareHand)))
                w = 3f;

            wr.Add(ModContent.PrefixType<VibrantAccessory>(), w);

            for (int i = 0; i < 50; i++)
                prefix = wr.Get();

            return prefix;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, extraSize: new Point(35, 2));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, SmokyCrystalProj.brightC, SmokyCrystalProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class SmokyRingProj
    {

    }

    public class SmokyCrystalProj
    {
        public static Color highlightC = new(235, 232, 208);
        public static Color brightC = new(190, 120, 33);
        public static Color darkC = new(108, 61, 48);
    }
}
