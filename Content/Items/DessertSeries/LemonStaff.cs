using Coralite.Content.GlobalItems;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.DotSystem;
using Coralite.Core.Systems.DotSystem.BasicDots;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.DessertSeries
{
    public class LemonStaff : ModItem, IDotWeapon
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<LemonSlice>(), 32, 10, true);
            Item.SetWeaponValues(12, 4);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.mana = 6;
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(silver: 50));
            Item.UseSound = CoraliteSoundID.MagicStaff_Item8;

            CoraliteGlobalItem.SetEdibleDamage(Item);
        }

        public DotTipInfo[] GetDotTips()
            => [new(ModContent.GetInstance<LemonadeDot>().Type, 2f)];

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero) * 46;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Lemon)
                .AddRecipeGroup(RecipeGroupID.Wood, 12)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class LemonSlice : ModProjectile
    {
        public override string Texture => AssetDirectory.DessertSeriesItem + Name;

        public ref float Timer => ref Projectile.ai[0];
        public ref float Scale => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 32;
        }

        public override bool ShouldUpdatePosition()
        {
            return State == 2;
        }

        public override bool? CanDamage()
        {
            if (State!=2)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            switch (State)
            {
                default:
                case 0://初始化
                    {
                        Projectile.position += owner.position - owner.oldPosition;

                        Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                        Timer = owner.itemAnimationMax * 2 / 3;
                        if (Timer < 1)
                        {
                            Timer = 1;
                        }

                        State = 1;
                    }
                    break;
                case 1:
                    {
                        Projectile.position += owner.position - owner.oldPosition;

                        float f = 1 - Timer / (Main.player[Projectile.owner].itemAnimationMax * 2 / 3);

                        Scale = Helper.HeavyEase(f);
                        Projectile.rotation += 0.4f * Helper.SqrtEase(1 - f);

                        Timer--;
                        if (Timer < 0)
                        {
                            Scale = 1;
                            Timer = 0;
                            State = 2;
                            Projectile.tileCollide = true;

                            if (Projectile.IsOwnedByLocalPlayer())
                            {
                                Projectile.netUpdate = true;
                                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();
                            }

                            Projectile.InitOldPosCache(10, false);
                        }
                    }
                    break;
                case 2:
                    {
                        Timer++;
                        if (Timer > 30)
                        {
                            Projectile.velocity.X *= 0.99f;
                            if (Projectile.velocity.Y <10)
                            {
                                Projectile.velocity.Y += 0.3f;
                            }
                        }

                        Projectile.rotation += (Projectile.velocity.Length() / 70f) * MathF.Sign(Projectile.velocity.X);

                        if (Main.rand.NextBool())
                        Projectile.SpawnTrailDust(Main.rand.NextFromList( DustID.YellowTorch,DustID.YellowStarDust), Main.rand.NextFloat(0.2f, 0.4f), Scale: Main.rand.NextFloat(1, 1.9f));
                    }
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Dot.AddDot<LemonadeDot>(target, damageDone * 2, Projectile.owner, Projectile.CritChance);

            Vector2 dir = -Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 8; i++)
            {
                LiquidTrailParticle p = LiquidTrailParticle.Spawn(Projectile.Center, dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(1, 5), new Color(255, 247, 0)*Main.rand.NextFloat(0.7f,1f), Main.rand.NextFloat(0.05f, 0.1f), Color.Transparent);

                p?.velYAdd = 0.3f;

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 6), Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            Timer = 30;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Timer = 30;

            Vector2 dir = -oldVelocity.SafeNormalize(Vector2.Zero);

            for (int i = 0; i < 6; i++)
            {
                LiquidTrailParticle p = LiquidTrailParticle.Spawn(Projectile.Center, dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(1, 5), new Color(255, 247, 0) * Main.rand.NextFloat(0.7f, 1f), Main.rand.NextFloat(0.05f, 0.1f), Color.Transparent);

                p?.velYAdd = 0.3f;

                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 6), Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 1)
            {
                return true;
            }

            Projectile.TileReflect(oldVelocity, 0.5f, 0.8f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsOwnedByLocalPlayer()&&!VaultUtils.isServer)
            {
                var p = DrawShadowParticle.Spawn(Projectile.Center, Projectile.rotation, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.GetTexture(), new Rectangle(0, 0, 1, 1), Projectile.scale);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 2)
                Projectile.DrawShadowTrails(lightColor, 0.6f, 0.6f / 10, 1, 10, 2, 0.4f / 10, 0, Scale * Projectile.scale);

            Projectile.QuickDraw(lightColor, Scale * Projectile.scale, 0f);

            return false;
        }
    }

    public class LemonadeDot : BasicSour
    {
        public override string Texture => AssetDirectory.DessertSeriesItem+Name;
        public override int BaseMaxDotLayer => 4;
    }
}
