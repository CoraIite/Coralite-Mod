using Coralite.Content.ModPlayers;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    public class FullmoonFlower : BaseGemWeapon
    {
        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.75f);
                effect.Parameters["highlightC"].SetValue(Hecatolite.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(Hecatolite.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(Hecatolite.darkC.ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.75f);
                effect.Parameters["highlightC"].SetValue(Hecatolite.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(Hecatolite.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(Hecatolite.darkC.ToVector4());
            }, extraSize: new Point(35, 2));
        }

        public override void SetDefs()
        {
            Item.maxStack = 1;
            Item.DefaultToAccessory();
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(0, 2);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.aggro -= 400;
            player.manaFlower = true;
            player.manaMagnet = true;
            player.maxRegenDelay = player.manaRegenDelay = 60;
            player.buffImmune[BuffID.ManaSickness] = true;
            player.starCloakItem = Item;
            player.starCloakItem_manaCloakOverrideItem = Item;

            player.manaCost += 0.25f;
            player.GetDamage(DamageClass.Magic) -= 0.05f;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return Helpers.Helper.CanBeEquipedWith<FullmoonFlower>(equippedItem, incomingItem, ItemID.ManaFlower, ItemID.PutridScent, ItemID.CelestialMagnet, ItemID.StarCloak, ItemID.ManaCloak, ItemID.MagnetFlower, ItemID.ArcaneFlower);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Hecatolite>()
                .AddIngredient(ItemID.ManaFlower)
                .AddIngredient(ItemID.PutridScent)
                .AddIngredient(ItemID.CelestialMagnet)
                .AddIngredient(ItemID.StarCloak)
                .AddIngredient(ItemID.SpectreBar,5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<Hecatolite>()
                .AddIngredient(ItemID.ManaCloak)
                .AddIngredient(ItemID.PutridScent)
                .AddIngredient(ItemID.CelestialMagnet)
                .AddIngredient(ItemID.SpectreBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<Hecatolite>()
                .AddIngredient(ItemID.MagnetFlower)
                .AddIngredient(ItemID.PutridScent)
                .AddIngredient(ItemID.StarCloak)
                .AddIngredient(ItemID.SpectreBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient<Hecatolite>()
                .AddIngredient(ItemID.ArcaneFlower)
                .AddIngredient(ItemID.CelestialMagnet)
                .AddIngredient(ItemID.StarCloak)
                .AddIngredient(ItemID.SpectreBar, 5)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Accessory))
                wr.Add(pre, 1);

            float w = 0.5f;
            if (Main.LocalPlayer.GetModPlayer<CoralitePlayer>().HasEffect(nameof(EightsquareHand)))
                w = 3f;

            wr.Add(ModContent.PrefixType<VibrantAccessory>(), w);

            for (int i = 0; i < 50; i++)
                prefix = wr.Get();

            return prefix;
        }
    }
}
