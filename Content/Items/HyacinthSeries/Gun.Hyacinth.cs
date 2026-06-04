using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.FlowerGunChapter;
using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Nightmare;
using Coralite.Content.Items.Thunder;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Hyacinth : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<FlowerGunKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<FlowerGunCollect>();

        public float shootAngle;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(208, 3);
            Item.DefaultToRangedWeapon(ProjectileType<HyacinthHeldProj>(), AmmoID.Bullet, 14, 11f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 40));

            Item.useStyle = ItemUseStyleID.Rapier;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            List<int> textureList = new();
            for (int i = 1; i < (int)HyacinthBullet.GunType.Count; i++)
                textureList.Add(-i);

            for (int i = 0; i < 3; i++)     //生成环绕的幻影枪弹幕
            {
                float angle = shootAngle + (i * MathHelper.TwoPi / 3);
                int textureType = Main.rand.NextFromList(textureList.ToArray());
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center + (angle.ToRotationVector2() * 40), Vector2.Zero, ProjectileType<HyacinthPhantomGun>()
                    , (int)(damage * 0.75f), knockback, player.whoAmI, CoraliteWorld.chaosWorld ? Main.rand.Next(ItemLoader.ItemCount) : textureType, i);
                textureList.Remove(textureType);
            }

            shootAngle += MathHelper.TwoPi / 12;
            shootAngle %= MathHelper.TwoPi;

            //生成手持弹幕以及红色子弹
            Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One) * 16, ProjectileType<HyacinthRedBullet>(), damage, knockback, player.whoAmI);
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<HyacinthHeldProj>(), damage, knockback, player.whoAmI);

            SoundStyle style = CoraliteSoundID.Gun_Item11;
            //style.Pitch = -0.8f;
            SoundEngine.PlaySound(style, player.Center);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Wisteria>()
                .AddIngredient<Datura>()
                .AddIngredient<Aloe>()
                .AddIngredient<ThunderDukeVine>()
                .AddIngredient<EternalBloom>()
                .AddIngredient(ItemID.Xenopopper)
                .AddIngredient<QueenOfNight>()
                .AddIngredient(ItemID.VortexBeater)
                .AddIngredient(ItemID.SDMG)
                .AddIngredient<StarsBreath>()
                .AddIngredient<Lycoris>()
                .AddTile<AncientFurnaceTile>()
                .Register();

            CreateRecipe()
                .AddIngredient<Wisteria>()
                .AddIngredient<GhostPipe>()
                .AddIngredient<Aloe>()
                .AddIngredient<ThunderDukeVine>()
                .AddIngredient<EternalBloom>()
                .AddIngredient(ItemID.Xenopopper)
                .AddIngredient<QueenOfNight>()
                .AddIngredient(ItemID.VortexBeater)
                .AddIngredient(ItemID.SDMG)
                .AddIngredient<StarsBreath>()
                .AddIngredient<Lycoris>()
                .AddTile<AncientFurnaceTile>()
                .Register();
        }
    }
}