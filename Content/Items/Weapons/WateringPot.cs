using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.Items.Weapons
{
    public class WateringPot : ModItem
    {
        public override string Texture => AssetDirectory.Weapons_Melee + Name;

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 34;
            Item.useTime = 10;
            Item.damage = 15;
            Item.shootSpeed = 10f;
            Item.knockBack = 4f;
            Item.useAnimation = 10;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item19;
            Item.shoot = ModContent.ProjectileType<WateringPotProj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player Player)
        {
            for (int k = 0; k <= Main.maxProjectiles; k++)
                if (Main.projectile[k].active && Main.projectile[k].owner == Player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<WateringPotProj>())
                    return false;

            return base.CanUseItem(Player);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MagliteDust>(8)
                .AddIngredient(ItemID.BottledWater, 2)
                .AddRecipeGroup(RecipeGroupID.IronBar, 6)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class WateringPotProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Weapons_Melee + "WateringPot";

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.timeLeft = 1200;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];

            switch (State)
            {
                default:
                case 0:     //默认射出
                    Projectile.rotation += 0.3f;

                    if (Projectile.timeLeft % 8 == 0)
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);

                    if (Timer > 35)
                        ChangeState(2);
                    break;
                case 1:         //旋转喷水 (我去还会喷水，这小朋友看了顶不顶得住啊)
                    Projectile.velocity *= 0.65f;
                    Projectile.rotation += 0.35f;

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Vector2 dir = (Projectile.rotation - 0.3f).ToRotationVector2();
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + dir * 8, DustID.Water, dir * 2f, 0, default, Main.rand.NextFloat(1.5f, 2f));
                        dust.noGravity = true;

                        if (Timer % 15 == 0)
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
                    }

                    if (Timer > 40)
                        ChangeState(2);
                    break;
                case 2:         //收回
                    Projectile.rotation += 0.3f;
                    if (Vector2.Distance(projOwner.Center, Projectile.Center) < 24)
                        Projectile.Kill();

                    else if (Vector2.Distance(projOwner.Center, Projectile.Center) < 200)
                        Projectile.velocity += Vector2.Normalize(projOwner.Center - Projectile.Center) * 4;
                    else
                        Projectile.velocity += Vector2.Normalize(projOwner.Center - Projectile.Center);

                    if (Projectile.velocity.Length() > 12)
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 12;

                    if (Projectile.timeLeft % 8 == 0)
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);

                    break;
            }

            Timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ChangeState(2, true);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (State == 0)
                ChangeState(1, true);
        }

        public void ChangeState(int state, bool bounce = false)
        {
            if (State == 0)
                Timer = 0;

            State = state;

            if (bounce && State == 1)
                Projectile.velocity = -Projectile.velocity;

            Projectile.netUpdate = true;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == 1)
            {
                float _ = 1;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (Projectile.rotation - 0.3f).ToRotationVector2() * 30, 12, ref _);
            }

            return null;
        }
    }
}
