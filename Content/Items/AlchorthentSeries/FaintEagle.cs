using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class FaintEagle : BaseAlchorthentItem
    {
        public override void SetOtherDefaults()
        {
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 30;
            Item.shoot = 10;

            Item.SetWeaponValues(15, 4);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 0, 50));
        }

        public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<FaintEagleHeldProj>(), damage, knockback, player.whoAmI, 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 3)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddIngredient(ItemID.CopperBar, 6)
                .AddTile(TileID.Campfire)
                .AddCondition(Condition.NearWater)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.StoneBlock, 3)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddIngredient(ItemID.TinBar, 6)
                .AddTile(TileID.Campfire)
                .AddCondition(Condition.NearWater)
                .Register();
        }
    }

    public class FaintEagleHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = Projectile.height = 16;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.dead || Owner.HeldItem.type != ModContent.ItemType<FaintEagle>())
            {
                Projectile.Kill();
                return;
            }


            Projectile.UpdateFrameNormally(3, 7);
            SetHeld();

            Projectile.rotation = ToMouseA;
            Owner.itemRotation = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            Projectile.Center = Owner.Center + UnitToMouseV * 32;

            if (State == 1)
                SpecialAttack();
            else
                NormalState();
        }

        public void SpecialAttack()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Owner.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                Projectile.timeLeft = 2;
                Owner.itemTime = Owner.itemTime = 2;
            }

            Timer++;

            if (Timer > 20 && Timer % 4 == 0)
            {
                //生成火焰弹幕
                Projectile.NewProjectileFromThis<FaintEagleFire>(Projectile.Center + UnitToMouseV * 10, UnitToMouseV.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(12, 14), Projectile.damage / 2, Projectile.knockBack, 0,Main.rand.Next(2));
            }


        }

        public void NormalState()
        {
            if (Timer == 0)
            {
                Timer++;
                Projectile.timeLeft = Owner.itemTimeMax;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 8), lightColor, MathHelper.PiOver4);

            return false;
        }

    }

    [VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
    public class FaintEagleFire : ModProjectile
    {
        public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

        public ref float Frame => ref Projectile.ai[0];
        public ref float Flip => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[1];
        public bool Heated
        {
            get => Projectile.ai[2] == 1;
            set
            {
                if (value)
                    Projectile.ai[2] = 1;
                else
                    Projectile.ai[2] = 0;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
            Projectile.width = Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(2, 16);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOwnedByLocalPlayer() && Projectile.frame > 15)
                Projectile.Kill();

            Timer++;
            HeatEagle();
        }

        public void HeatEagle()
        {
            if (Heated)
                return;

            if (Timer % 2 == 0)//找火鹰弹幕并给它加能量
            {

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frameBox = new Rectangle(0, Projectile.frame, 1, 15);
            SpriteEffects effect = Flip == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Projectile.DrawFramedShadowTrails(Color.White with { A = 0 }, 0.15f, 0.15f / 6, 1, 6, 1, Projectile.scale, frameBox, effect, MathHelper.Pi);

            //绘制一层更大的在后面
            Projectile.QuickFrameDraw(frameBox, (Color.White * 0.15f )with { A=0}, MathHelper.Pi, 1.2f, effect);
            Projectile.QuickFrameDraw(frameBox, Color.White*0.8f, MathHelper.Pi, effect);

            return false;
        }
    }
}
