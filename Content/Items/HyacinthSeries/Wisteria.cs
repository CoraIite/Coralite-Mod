using Coralite.Content.Items.Glistent;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class Wisteria : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(16, 2, 6);
            Item.DefaultToRangedWeapon(ProjectileType<WisteriaPetal>(), AmmoID.None, 11, 12.5f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = CoraliteSoundID.Grass;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<WisteriaHeldProj>(), 0, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.05f, 0.05f)
                , type, damage, knockback, player.whoAmI, Main.rand.NextFromList(-1, 1));

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LeafStone>(12)
                .AddIngredient(ItemID.Amethyst)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class WisteriaHeldProj : BaseGunHeldProj
    {
        public WisteriaHeldProj() : base(0.2f, 15, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex WisteriaFire { get; private set; }

        protected override float HeldPositionY => -2;

        private int FrameX;

        public override void InitializeGun()
        {
            FrameX = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = WisteriaFire.Value;
            Rectangle frameBox = effect.Frame(4, 4, FrameX, Projectile.frame);
            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 20 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.White
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale*0.8f, 0, 0f);
            return false;
        }
    }

    public class WisteriaPetal:ModProjectile
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems+Name;

        private ref float Dir => ref Projectile.ai[0];
        private ref float EXTime => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 65;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(3, 6);
            Timer++;
            if (Timer > 24)
            {
                float factor = 0.3f * Coralite.Instance.BezierEaseSmoother.Smoother(Math.Clamp((Timer - 24) / 40, 0, 1));
                Projectile.velocity = Projectile.velocity.RotatedBy(Dir * factor);
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.02f * MathF.Sin(-Dir * MathHelper.PiOver2 + Timer * 0.2f));
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.rand.NextBool())
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleCrystalShard, Helper.NextVec2Dir(0.5f, 1)
                    , Alpha: 100, Scale: Main.rand.NextFloat(0.5f, 1f));
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PurpleCrystalShard, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * Main.rand.NextFloat(0.15f, 0.6f)
                    , Alpha: 100, Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }

            Vector2 toCenter = Projectile.Size / 2;
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.oldPos[i] + toCenter, DustID.PurpleCrystalShard, Helper.NextVec2Dir(0.5f, 1)
                    , Alpha: 100, Scale: Main.rand.NextFloat(0.8f, 1f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 7, 0, Projectile.frame);

            Projectile.DrawShadowTrails(lightColor, 0.2f, 0.2f / 12, 1, 12, 1
                , Projectile.scale, frameBox, 0);

            Projectile.QuickDraw(frameBox, lightColor, 0);

            return false;
        }
    }
}
