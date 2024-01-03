using Coralite.Content.Items.CoreKeeper.Bases;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.CoreKeeper
{
    public class RuneParchment : ModItem
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(0, 1, 0, 0);
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
            return p.HasItem(ModContent.ItemType<BrokenHandle>())
                && p.HasItem(ModContent.ItemType<ClearGemstone>())
                && p.HasItem(ModContent.ItemType<ChippedBlade>())
                && p.CountItem(ModContent.ItemType<AncientGemstone>(), 11) >= 10
                && p.CountItem(ItemID.IronBar, 51) >= 50;
        }

        private void Mp_OnCraft(Player p)
        {
            p.ConsumeItem(ModContent.ItemType<BrokenHandle>());
            p.ConsumeItem(ModContent.ItemType<ClearGemstone>());
            p.ConsumeItem(ModContent.ItemType<ChippedBlade>());
            for (int i = 0; i < 10; i++)
                p.ConsumeItem(ModContent.ItemType<AncientGemstone>());
            for (int i = 0; i < 50; i++)
                p.ConsumeItem(ItemID.IronBar);

            Item.SetDefaults(ModContent.ItemType<RuneSong>());
            Main.NewText(this.GetLocalization("OnCraft", () => "得到了") + Item.Name + " !", new Color(243, 180, 65, 255));
            //PopupText.NewText(new AdvancedPopupRequest() { Text = Item.Name
            //    , Color = new Color(243, 180, 65, 255)
            //    , DurationInFrames = 2 * 60 }, p.Top+new Vector2(0,20));
            Helpers.Helper.PlayPitched("CoreKeeper/CraftFinish", 1f, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!NPC.downedPlantBoss)
            tooltips.Add(new TooltipLine(Mod, "Coralite:CraftLock", this.GetLocalization("CraftLock", () => "该物品已被一个强大的丛林生物诅咒，击败它以解放上古的力量").Value));
        }
    }
}

