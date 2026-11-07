using Coralite.Content.GlobalItems;
using Coralite.Content.GlobalProjectiles;
using Coralite.Content.Items.DessertSeries.CommonDots;
using Coralite.Core;
using Coralite.Core.Systems.DotSystem;
using Coralite.Core.Systems.DotSystem.BasicDots;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.DessertSeries
{
    public class Tanghulu : ModItem, IDotWeapon
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ProjectileType<TanghuluProj>(), 30, 8f, true);
            Item.SetWeaponValues(30, 6);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 7;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(gold: 3));
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            CoraliteGlobalItem.SetEdibleDamage(Item);

            Item.reuseDelay = 10;
            Item.noUseGraphic = true;
        }

        public DotTipInfo[] GetDotTips()
            => [
                new(GetInstance<RockSugarDot>().Type, 1.5f),
                new(GetInstance<HawthornSourDot>().Type, 3f),
                ];

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AdamantiteBar, 12)
                .AddIngredient(ItemID.PinkIceBlock, 8)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class TanghuluProj : ModProjectile
    {
        private const int MaxHits = 4;

        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public ref float HitCount => ref Projectile.ai[0];
        public ref float Alpha => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            CoraliteGlobalProjectile.SetEdibleDamage(Projectile);

            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 60 * 3;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Alpha < 1)
            {
                Alpha += 0.04f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.ai[2]++;
            if (Projectile.ai[2]>50*Projectile.MaxUpdates)
            {
                Projectile.velocity.X *= 0.99f;
                if (Projectile.velocity.Y < 10)
                {
                    Projectile.velocity.Y += 0.125f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Dot.AddDot<RockSugarDot>(target, (int)(damageDone * 1.5f), Projectile);

            Projectile.damage = (int)(Projectile.damage * 0.9f);

            Projectile.NewProjectileFromThis<HawthornProj>(Projectile.Center, new Vector2(target.velocity.X, -8f), Projectile.damage, Projectile.knockBack, Projectile.owner);

            HitCount++;
            Projectile.netUpdate = true;
            if (HitCount >= MaxHits)
                Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //SpawnRestHawthorn();
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SpawnRestHawthorn();
        }

        public void SpawnRestHawthorn()
        {
            int remainingHits = MaxHits - (int)HitCount;
            float rot = Main.rand.NextFloat(MathHelper.TwoPi);
            Vector2 dir = -Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < remainingHits; i++)
            {
                Vector2 velocity = rot.ToRotationVector2() * 6f;
                Projectile.NewProjectileFromThis<HawthornProj>(Projectile.Center + dir * 18 * Projectile.scale * i, velocity, Projectile.damage, Projectile.knockBack,30);

                rot += MathHelper.TwoPi / remainingHits;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTextureValue();
            Rectangle box = tex.Frame(1, 4, 0, (int)HitCount);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(box.Width * 0.84f, box.Height * 0.16f);

            Main.spriteBatch.Draw(tex, pos, box, lightColor*Alpha, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale, 0, 0);

            return false;
        }
    }

    public class HawthornProj : ModProjectile
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;
        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            CoraliteGlobalProjectile.SetEdibleDamage(Projectile);

            Projectile.width = Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60*6;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override bool? CanDamage()
        {
            if (Timer < 30)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void AI()
        {
            Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 40;
            Timer++;
            if (Projectile.velocity.Y<10)
            {
                Projectile.velocity.Y += 0.25f;
            }

            if (Timer>30)
            {
                Projectile.velocity.X *= 0.99f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.TileReflect(oldVelocity, 0.1f, 0.25f);
            if (Projectile.timeLeft>30)
            {
                Projectile.timeLeft -= 30;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 0)
            {
                Dot.AddDot<HawthornSourDot>(target, damageDone * 3, Projectile);
                Projectile.ai[1] = 1;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), DustID.RedTorch, Helper.NextVec2Dir(0.1f, 1.5f), Scale: Main.rand.NextFloat(1, 3f));

                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public class HawthornSourDot : BasicSour
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override int BaseMaxDotLayer => 2;

        protected override int BasicDamageInterval => 30;
        protected override int BaseArmorPenetration => 12;
    }
}
