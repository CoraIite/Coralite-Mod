using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.RedJade;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.KeySystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeBlade : ModItem, IConsultableItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<RedJadeKnowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<RedJadeItemPage>();

        public int shootCount;
        /// <summary> 浣跨敤澶氬皯娆″悗杩涜寮哄寲澶х垎鐐哥殑灏勫嚮 </summary>
        public int useBigBoom = 8;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(18, 4f);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 0, 50, 0));

            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ProjectileType<RedJadeBladeHeldProj>();
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (shootCount >= useBigBoom) //浣跨敤寮哄姏鎸ヨ垶
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 3);

                shootCount = 0;
                useBigBoom = Main.rand.Next(6, 9);
            }
            else
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, shootCount % 3);
                Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 10f,
                    ProjectileType<RedJadeStrike>(), (int)(damage * 0.75f), knockback, player.whoAmI, Main.rand.Next(3));
            }

            shootCount++;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<RedJade>(14)
            .AddTile<Tiles.RedJades.MagicCraftStation>()
            .Register();
        }
    }

    public class RedJadeBladeHeldProj : BaseSwingProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeBlade";

        public ref float Combo => ref Projectile.ai[0];

        public override void SetSwingProperty()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 22;
            Projectile.width = 34;
            Projectile.height = 68;
            Projectile.extraUpdates = 1;

            distanceToOwner = 6;
            minTime = 0;
            onHitFreeze = 4;
        }

        protected override void InitializeSwing()
        {
            if (Projectile.IsOwnedByLocalPlayer())
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
            switch (Combo)
            {
                default:
                case 0:
                case 1:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = 2.2f;
                    totalAngle = 4.6f;
                    Smoother = Coralite.Instance.NoSmootherInstance;

                    break;
                case 2:
                    maxTime = Owner.itemTimeMax * 2;
                    startAngle = -1.2f;
                    totalAngle = -4.2f;
                    Smoother = Coralite.Instance.NoSmootherInstance;

                    break;
                case 3: //寮哄寲鎸ヨ垶
                    minTime = 14;
                    maxTime = 18 + (Owner.itemTimeMax * 2);
                    startAngle = 2.2f;
                    totalAngle = 4.8f;
                    Smoother = Coralite.Instance.HeavySmootherInstance;
                    SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
                    break;
            }

            base.InitializeSwing();
        }

        protected override float GetStartAngle() => Owner.direction > 0 ? 0f : MathHelper.Pi;

        protected override void BeforeSlash()
        {
            _Rotation += -Owner.direction * 0.03f;
            Slasher();
        }

        protected override void SpawnDustOnSlash()
        {
            if (Projectile.IsOwnedByLocalPlayer() && Timer == maxTime / 2 && Combo == 3)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center + ((Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero) * 64), Vector2.Zero,
                    ProjectileType<RedJadeBigBoom>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
            }
        }

        protected override void AfterSlash()
        {
            Slasher();
            if (Timer > maxTime + 6)
                Projectile.Kill();
        }

        protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            base.DrawSelf(mainTex, origin, lightColor, extraRot);
            if (Timer < minTime)
            {
                float factor = Timer / minTime;
                Helper.DrawPrettyStarSparkle(1, SpriteEffects.None, Projectile.Center - Main.screenPosition + (RotateVec2 * 20), new Color(255, 255, 255, 0) * 0.8f,
                    Coralite.RedJadeRed, factor, 0, 0.4f, 0.6f, 1f, -Owner.direction * factor * 1f, new Vector2(2, 1f), Vector2.One);
            }
        }
    }

    public class RedJadeStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.RedJadeProjectiles + Name;

        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;

            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 55;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public void Initialize()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (Projectile.ai[0] == 0)//鐢ㄤ簬鍚屾杈撳叆鐨刟i0锛岃繖涓猘i0鏄敤浜庢帶鍒跺脊骞曟槸鍚﹁兘鐖嗙偢鐨?                Projectile.scale = 1.5f;

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            if (Projectile.velocity.Y < 14)
                Projectile.velocity.Y += 0.04f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Main.netMode != NetmodeID.Server)
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5), DustID.GemRuby, -Projectile.velocity * 0.4f, 0, default, 0.7f);
                    dust.noGravity = true;
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == 0 && Projectile.IsOwnedByLocalPlayer())
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<RedJadeBoom>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                return;
            }

            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GemRuby, Main.rand.NextVector2Circular(3, 3), 0, default, Main.rand.NextFloat(1f, 1.3f));
                    dust.noGravity = true;
                }

        }
    }

    public class RedJadeBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 5;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public void Initialize()
        {
            Helper.RedJadeExplosion(Projectile.Center);
        }

        public override bool PreAI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override bool? CanHitNPC(NPC target)
        {
            if (Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) && Vector2.Distance(Projectile.Center, target.Center) < 64)
                return null;

            return false;
        }
    }

    public class RedJadeBigBoom : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 10;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

        }

        public void Initialize()
        {
            Helper.RedJadeBigBoom(Projectile.Center);
        }

        public override bool PreAI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor) => false;

        public override bool? CanHitNPC(NPC target)
        {
            if (target.friendly)
            {
                return null;
            }

            return Collision.CanHitLine(Projectile.Center, 1, 1, target.Center, 1, 1) && Vector2.Distance(Projectile.Center, target.Center) < 200;
        }
    }
}
