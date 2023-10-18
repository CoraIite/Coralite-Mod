using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Core;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class Dreamcatcher : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 140;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.reuseDelay = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 15;
            Item.knockBack = 3;

            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = RarityType<NightmareRarity>();
            Item.shoot = ProjectileType<NightmareRaven>();
            Item.UseSound = CoraliteSoundID.TerraprismaSummon_Item82;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;
        }

        public override void HoldItem(Player player)
        {
            Lighting.AddLight(player.Center + player.velocity + new Vector2(player.direction * 24, 16), TorchID.Red);

            //临时工
            //if (player.whoAmI == Main.myPlayer && Main.mouseRight)
            //{
            //    foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
            //    {
            //        proj.ai[2] = 6;
            //        NightmareRaven pro = (NightmareRaven)proj.ModProjectile;
            //        pro.drawColor = NightmarePlantera.nightmareRed;
            //    }
            //}
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            float rot = player.velocity.X / 15;
            if (MathF.Abs(rot) > 0.6f)
                rot = Math.Sign(rot) * 0.6f;

            player.itemRotation = rot;
            Vector2 offset = ((player.itemRotation - MathHelper.PiOver2).ToRotationVector2() * 68);
            player.itemLocation += new Vector2(player.direction * -6 - offset.X, 44 - (68 + offset.Y) * 2);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center + player.velocity + new Vector2(player.direction * 24, 16);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type == Item.shoot && p.owner == player.whoAmI))
            {
                proj.localAI[2] = 0;
            }
            return false;
        }
    }

    public class NightmareRavenBuff:ModBuff
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<NightmareRaven>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<NightmareRaven>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }

    }
}
