using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.RedJades
{
    public class RedJadeSpear : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public int useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 18;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 3f;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ProjectileType<RedJadeSpearSpurt>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount > 3 && Main.rand.NextBool(useCount, 9))
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 1);
                    useCount = 0;
                    return false;
                }

                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
            }

            useCount++;
            return false;
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<RedJade>(8)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    public class RedJadeSpearSpurt : BaseSwingProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "RedJadeSpear";

        public RedJadeSpearSpurt() : base(new Vector2(68, 66).ToRotation() - 0.05f, trailLength: 6) { }

        public ref float Combo => ref Projectile.ai[0];

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 48;
            Projectile.width = 40;
            Projectile.height = 95;
            trailTopWidth = -8;
            minTime = 0;
            onHitFreeze = 12;
            useShadowTrail = true;
        }

        protected override void Initializer()
        {
            if (Main.myPlayer == Projectile.owner)
                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            startAngle = Main.rand.NextFloat(-0.2f, 0.2f);
            totalAngle = 0.01f;
            Projectile.extraUpdates = 3;

            switch (Combo)
            {
                default:
                case 0:
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 35;
                    Smoother = Coralite.Instance.BezierEaseSmoother;
                    distanceToOwner = -47;
                    SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Projectile.Center);
                    break;
                case 1:
                    minTime = 60;
                    distanceToOwner = -20;
                    maxTime = (int)(Owner.itemTimeMax * 0.4f) + 35 + minTime;
                    break;
            }

            base.Initializer();
        }

        protected override void BeforeSlash()
        {
            if (Combo == 1)
            {
                float factor = Timer / minTime;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 pos = Projectile.Center + Helper.NextVec2Dir(44 - factor * 30, 46 - factor * 30);
                    Dust d = Dust.NewDustPerfect(pos, DustID.GemRuby, (Projectile.Center - pos) / 25);
                    d.noGravity = true;
                }

                distanceToOwner = MathHelper.Lerp(-20, -47, factor);
                Slasher();
                if (Timer==minTime-1)
                    SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
            }

            base.BeforeSlash();
        }

        protected override void OnSlash()
        {
            float factor = (Timer-minTime) / (maxTime-minTime);
            if (Timer % 3 == 0)
            {
                Vector2 pos = Projectile.Center + RotateVec2 * Main.rand.NextFloat(20, 70) * Projectile.scale
                    +Main.rand.NextVector2Circular(16,16);
                if (Main.rand.NextBool())
                {
                   Dust d= Dust.NewDustPerfect(pos, DustID.RedTorch, RotateVec2 * Main.rand.NextFloat(1f, 3f));
                    d.noGravity = true;
                }
            }

            if (factor < 0.5f)
                distanceToOwner = -47 + 70 * Smoother.Smoother(factor * 2);
            else
                distanceToOwner = -47 + 70 * Smoother.Smoother((1 - factor) * 2);

            if (Combo == 1)
            {
                if ((Timer - minTime) % ((maxTime - minTime) / 4) == 0)
                {
                    float x = (Timer - minTime) / ((maxTime - minTime) / 4);
                    Vector2 pos = Owner.Center + RotateVec2 * 64 * x + Main.rand.NextVector2Circular(16, 16);

                    int type = ProjectileType<RedJadeBoom>();
                    if (x > 3)
                        type = ProjectileType<RedJadeBigBoom>();

                    Projectile.NewProjectileFromThis(pos, Vector2.Zero, type, Projectile.damage, Projectile.knockBack);
                }
            }

            base.OnSlash();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (onHitTimer == 0)
            {
                onHitTimer = 1;
                Owner.immuneTime += 10;
                if (Main.netMode == NetmodeID.Server)
                    return;

                float strength = 2;

                if (VisualEffectSystem.HitEffect_ScreenShaking)
                {
                    PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, RotateVec2, strength, 6, 6, 1000);
                    Main.instance.CameraModifiers.Add(modifier);
                }

                Dust dust;
                float offset = Projectile.localAI[1] + Main.rand.NextFloat(0, Projectile.width * Projectile.scale - Projectile.localAI[1]);
                Vector2 pos = Bottom + RotateVec2 * offset;

                if (VisualEffectSystem.HitEffect_Dusts)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 dir = -RotateVec2.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        dust = Dust.NewDustPerfect(pos, DustID.RedTorch, dir * Main.rand.NextFloat(1f, 5f), 50, Coralite.Instance.ThunderveinYellow, Scale: Main.rand.NextFloat(1f, 1.5f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        protected override void DrawShadowTrail(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
        {
            if (Timer<6)
                return;

            SpriteEffects effect = CheckEffect();
            for (int i = 1; i < 6; i += 1)
            {
                Vector2 Center = GetCenter(i);
                Center += oldRotate[i].ToRotationVector2() * (oldLength[i]/2 + oldDistanceToOwner[i]);
                Main.spriteBatch.Draw(mainTex, Center - Main.screenPosition, null,
                    Coralite.Instance.RedJadeRed * (0.3f - i * 0.3f / 6), oldRotate[i] + extraRot, origin, Projectile.scale, effect, 0);
            }
        }
    }

}
