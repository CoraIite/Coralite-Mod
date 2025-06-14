using Coralite.Content.Dusts;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Lantern
{
    public class GlowMushroomLantern : ModItem
    {
        public override string Texture => AssetDirectory.Lantern + Name;

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<GlowMushroomLanternFire>(), 20, 12, true);
            Item.SetWeaponValues(15, 3.5f);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(silver: 50));

            Item.mana = 12;
            Item.useTime = Item.useAnimation = 30;
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.UseSound = CoraliteSoundID.FireHitDD2_BetsysWrathImpact;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos = player.Center + new Vector2(player.direction * 22, -4);

            Projectile.NewProjectile(source, pos, velocity, type, damage, knockback, player.whoAmI);

            for (int i = 0; i < 5; i++)
            {
                Color c = Main.rand.NextFromList(new Color(25, 142, 196), new Color(20, 78, 129)) * 0.5f;
                PRTLoader.NewParticle<Fog>(pos + Main.rand.NextVector2Circular(12, 12)
                    , Helper.NextVec2Dir(0.25f, 0.6f), c, Main.rand.NextFloat(0.3f, 0.7f));

                Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(4, 4)
                    , DustID.MushroomTorch, Helper.NextVec2Dir(1, 2.5f), Scale: Main.rand.NextFloat(1, 1.5f));

                d.noGravity = true;

                d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(4, 4)
                    , DustID.GlowingMushroom, Helper.NextVec2Dir(1, 1.5f), Scale: Main.rand.NextFloat(1, 1.5f));

                d.noGravity = true;
            }
            return false;
        }

        public override void HoldStyle(Player player, Rectangle heldItemFrame)
        {
            player.itemLocation.Y += 14;
        }

        public override void HoldItem(Player player)
        {
            Lighting.AddLight(player.Center + new Vector2(player.direction * 8, 4), TorchID.Mushroom);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient(ItemID.MushroomTorch,15)
                .AddIngredient(ItemID.SilverBar,5)
                .AddTile(TileID.WorkBenches)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.WandofSparking)
                .AddIngredient(ItemID.MushroomTorch,15)
                .AddIngredient(ItemID.TungstenBar,5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class GlowMushroomLanternFire : ModProjectile
    {
        public override string Texture => AssetDirectory.Lantern + Name;

        public ref float Timer => ref Projectile.ai[0];

        public readonly int maxTime = 45;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity= true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = Projectile.height = 14;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            //下坠
            if (Projectile.velocity.Y < 4)
                Projectile.velocity.Y += 0.07f;

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(ModContent.DustType<PixelPoint>(), Main.rand.NextFloat(0, 0.4f), newColor: Color.DarkBlue
                    , Scale: Main.rand.NextFloat(0.75f, 1.25f));

            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(4f, DustID.MushroomTorch, Main.rand.NextFloat(-0.1f, 0.4f)
                    , Scale: Main.rand.NextFloat(1f, 1.75f));

            if (Main.rand.NextBool(4))
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.MushroomTorch
                    , -Vector2.UnitY.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.75f, 1.25f));
            }

            Timer++;
            if (Timer > maxTime)
                Projectile.Kill();

            Projectile.rotation += 0.2f;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                int damage = Projectile.damage / 3;

                for (int i = -1; i < 2; i++)
                {
                    Projectile.NewProjectileFromThis<GlowMushroomLanternGrow>(Projectile.Center
                        , -Vector2.UnitY.RotateByRandom(i * 1f - 0.75f, i + 1f + 0.75f) * Main.rand.NextFloat(2,6.5f), damage, Projectile.knockBack);
                }
            }

            //生成粒子
            if (!VaultUtils.isServer)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                        , DustID.MushroomTorch, Helper.NextVec2Dir(2, 3.5f), Scale:Main.rand.NextFloat(1.5f,2f));

                    d.noGravity = true;

                    d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                        , DustID.GlowingMushroom, Helper.NextVec2Dir(1, 1.5f));

                    d.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float factor = Helper.SqrtEase(1 - Timer / maxTime);

            Projectile.DrawShadowTrails(new Color(140, 140, 140, 10) * factor, 1, 1 / 10f
                , 0, 10, 1, 1 / 10f, 0, -1);

            Projectile.QuickDraw(new Color(230, 230, 230, 10) * factor, 0);
            return false;
        }
    }

    public class GlowMushroomLanternGrow : ModProjectile
    {
        public override string Texture => AssetDirectory.Lantern + Name;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Timer == 0 && !VaultUtils.isServer)
            {
                Projectile.InitOldPosCache(14);
                Projectile.InitOldRotCache(14);
            }

            if (Timer < 15)
            {
                float angle = Projectile.velocity.ToRotation();

                angle = angle.AngleLerp(-MathHelper.PiOver2, 0.1f);
                Projectile.velocity = angle.ToRotationVector2() * Projectile.velocity.Length();

                if (Main.rand.NextBool())
                    Projectile.SpawnTrailDust(8f, DustID.MushroomTorch, Main.rand.NextFloat(-0.1f, 0.4f)
                        , Scale: Main.rand.NextFloat(1f, 1.75f));
            }
            else if (Timer < 15 + 30)
            {
                Projectile.velocity *= 0.9f;
            }
            else
                Projectile.Kill();

            Timer++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (!VaultUtils.isServer)
            {
                Projectile.UpdateOldPosCache();
                Projectile.UpdateOldRotCache();
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VaultUtils.isServer)
                return;

            for (int i = 0; i < 5; i++)
            {
                Color c = Main.rand.NextFromList(new Color(25,142,196), new Color(20, 78, 129)) * 0.5f;
                PRTLoader.NewParticle<Fog>(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                    , Helper.NextVec2Dir(0.25f, 0.6f), c, Main.rand.NextFloat(0.3f, 0.7f));

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                    , DustID.MushroomTorch, Helper.NextVec2Dir(2, 2.5f), Scale: Main.rand.NextFloat(1, 1.5f));

                d.noGravity = true;

                d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4)
                    , DustID.GlowingMushroom, Helper.NextVec2Dir(1, 1.5f), Scale: Main.rand.NextFloat(1, 1.5f));

                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = CoraliteAssets.Misc.White32x32.Value;

            List<ColoredVertex> bars = new();

            if (Projectile.oldPos.Length==14)
            {
                for (int i = 0; i < 14; i++)
                {
                    float factor = (float)i / 14;
                    Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                    Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                    Vector2 Top = Center + (normal * 2f);
                    Vector2 Bottom = Center - (normal * 2f);

                    var color = /*Color.Lerp(new Color(20, 78, 129), , factor)*/ new Color(23, 231, 255) * factor*0.75f;
                    bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                    bars.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
                }

                Main.graphics.GraphicsDevice.Textures[0] = Texture;
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            }

            Projectile.QuickDraw(lightColor with { A=125}, 0);

            return false;
        }
    }
}
