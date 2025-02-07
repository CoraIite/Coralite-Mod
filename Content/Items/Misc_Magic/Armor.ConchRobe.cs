using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Magic
{
    [AutoloadEquip(EquipType.Head)]
    public class ConchHat : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public static LocalizedText bonus;

        public override void Load()
        {
            bonus = this.GetLocalization("ArmorBonus");
        }

        public override void Unload()
        {
            bonus = null;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 20;
            player.GetCritChance(DamageClass.Magic) += 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<ConchRobe>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;

            if (player.statMana < player.statManaMax2 / 2 && player.ownedProjectileCounts[ProjectileType<ConchBubble>()] < 1)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ProjectileType<ConchBubble>()
                    , 0, 0, player.whoAmI);

                for (int i = 0; i < 14; i++)
                {
                    Vector2 dir = (i * MathHelper.TwoPi / 14).ToRotationVector2();
                    Dust d = Dust.NewDustPerfect(player.Center + dir * 9, DustID.Water, dir * Main.rand.NextFloat(1, 2), Scale: Main.rand.NextFloat(1, 2f));
                    d.noGravity = true;
                    d = Dust.NewDustPerfect(player.Center + dir * 18, DustID.Water, dir * Main.rand.NextFloat(3, 4), Scale: Main.rand.NextFloat(1, 2f));
                    d.noGravity = true;
                }
            }

            if (player.ownedProjectileCounts[ProjectileType<ConchBubble>()] > 0)
            {
                player.accRunSpeed = 6f;
                player.CancelAllBootRunVisualEffects();
                player.sailDash = true;
                player.statDefense += 3;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TulipShell, 2)
                .AddIngredient(ItemID.CoralstoneBlock, 8)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.LightningWhelkShell, 2)
                .AddIngredient(ItemID.CoralstoneBlock, 8)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.JunoniaShell)
                .AddIngredient(ItemID.CoralstoneBlock, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body, EquipType.Legs)]
    public class ConchRobe : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Magic + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.statManaMax2 += 20;
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(ConchRobe));
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Coral, 2)
                .AddIngredient(ItemID.CoralstoneBlock, 4)
                .AddIngredient(ItemID.PalmWood, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class ConchBubble : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 8;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.Center = Owner.Center;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Bubble_Item54, Projectile.Center);
            for (int i = 0; i < 14; i++)
            {
                Vector2 dir = (i * MathHelper.TwoPi / 14).ToRotationVector2();
                Dust.NewDustPerfect(Projectile.Center + dir * 9, DustID.Water, dir * Main.rand.NextFloat(1, 2), Scale: Main.rand.NextFloat(1, 2f));
                Dust.NewDustPerfect(Projectile.Center + dir * 18, DustID.Water, dir * Main.rand.NextFloat(3, 4), Scale: Main.rand.NextFloat(1, 2f));
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Npc[NPCID.DetonatingBubble].Value;
            Rectangle frameBox = mainTex.Frame(1, 2, 0, 1);

            float factor = MathF.Sin((int)Main.timeForVisualEffects * 0.1f) * 0.1f;

            Vector2 scale = new Vector2(1.2f + factor, 1.2f - factor);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor * 0.5f, 0, frameBox.Size() / 2, scale, 0, 0);

            return false;
        }
    }
}
