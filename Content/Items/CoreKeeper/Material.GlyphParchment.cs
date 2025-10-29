using Coralite.Content.Items.CoreKeeper.Bases;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.CoreKeeper
{
    public class GlyphParchment : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public static LocalizedText OnCraftText { get; private set; }
        public static LocalizedText CraftLock { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                OnCraftText = this.GetLocalization("OnCraft");
                CraftLock = this.GetLocalization(nameof(CraftLock));
            }
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 5);
            Item.rare = ModContent.RarityType<EpicRarity>();
            Item.shoot = ModContent.ProjectileType<CraftUIProj>();

            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            //Helpers.Helper.PlayPitched("CoreKeeper/open_chest_end", 1f, -0.1f);

            return player.velocity == Vector2.Zero && NPC.downedPlantBoss && CanCraft(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile p = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, 1, 0, player.whoAmI, 400);
                if (p.ModProjectile is CraftUIProj mp)
                {
                    mp.CheckCanCraft = CanCraft;
                    mp.OnCraft += Mp_OnCraft;
                }
            }
            return false;
        }

        public static bool CanCraft(Player p)
        {
            return p.HasItem(ModContent.ItemType<GlyphParchment>())
                && p.HasItem(ModContent.ItemType<ChannelingGemstone>())
                && p.HasItem(ModContent.ItemType<FracturedLimbs>())
                && p.HasItem(ModContent.ItemType<EnergyString>())
                && p.CountItem(ItemID.HallowedBar, 101) >= 100
                && p.CountItem(ModContent.ItemType<AncientGemstone>(), 21) >= 20;
        }

        private void Mp_OnCraft(Player p)
        {
            p.QuickSpawnItem(new EntitySource_ItemUse(p, Item), ModContent.ItemType<PhantomSpark>());

            p.ConsumeItem(ModContent.ItemType<GlyphParchment>());
            p.ConsumeItem(ModContent.ItemType<ChannelingGemstone>());
            p.ConsumeItem(ModContent.ItemType<FracturedLimbs>());
            p.ConsumeItem(ModContent.ItemType<EnergyString>());
            for (int i = 0; i < 20; i++)
                p.ConsumeItem(ModContent.ItemType<AncientGemstone>());
            for (int i = 0; i < 100; i++)
                p.ConsumeItem(ItemID.HallowedBar);

            Main.NewText(OnCraftText.Format(ContentSamples.ItemsByType[ModContent.ItemType<PhantomSpark>()].Name), new Color(243, 180, 65, 255));
            //PopupText.NewText(new AdvancedPopupRequest() { Text = Item.Name
            //    , Color = new Color(243, 180, 65, 255)
            //    , DurationInFrames = 2 * 60 }, p.Top+new Vector2(0,20));
            Helpers.Helper.PlayPitched("CoreKeeper/CraftFinish", 1f, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!NPC.downedPlantBoss)
                tooltips.Add(new TooltipLine(Mod, "Coralite:CraftLock", CraftLock.Value));
        }
    }
}

