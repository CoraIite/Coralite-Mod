using Coralite.Content.DamageClasses;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.Glistent
{
    public class GlistentJar : BaseFairyCatcher
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override int CatchPower => 8;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<GlistentJarProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(20, 4);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 1));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(6)
                .AddIngredient(ItemID.ClayBlock, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [VaultLoaden(AssetDirectory.GlistentItems)]
    public class GlistentJarProj : BaseJarProj
    {
        public override string Texture => AssetDirectory.GlistentItems + "GlistentJar";

        [VaultLoaden("{@classPath}" + "GlistentJar_Highlight")]
        public static ATex HighlightTex { get; private set; }

        public override void InitFields()
        {
            MaxChannelTime = 50;
            MaxFlyTime = 15;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 32;
        }

        public override void Load()
        {
            for (int i = 0; i < 3; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, Texture + "Proj_Gore" + i);
        }

        public override void SpawnDustOnFlying(bool outofTime)
        {
            Projectile.SpawnTrailDust(DustID.GemEmerald, Main.rand.NextFloat(0.1f, 0.3f));
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect())
                    , DustID.GemEmerald, Helper.NextVec2Dir(1f, 2.5f), 150, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;
            }

            this.SpawnGore(3);
            Helper.PlayPitched(CoraliteSoundID.GlassBroken_Shatter, Projectile.Center, pitchAdjust: -0.2f);

            if (Catch == 0 && FullCharge)//完全蓄力后生成爆炸圈弹幕
            {
                Projectile.NewProjectileFromThis<GlistentJarExplode>(Projectile.Center, Vector2.Zero
                    , Projectile.damage / 2, Projectile.knockBack / 2, 1);
            }
        }

        public override void DrawJar(Vector2 pos, Color lightColor, SpriteEffects eff, Texture2D tex)
        {
            base.DrawJar(pos, lightColor, eff, tex);
            HighlightTex.Value.QuickCenteredDraw(Main.spriteBatch, pos, Color.White, Projectile.rotation
                , Projectile.scale, eff);
        }
    }

    public class GlistentJarDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Debuff";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(npc.getRect())
                    , DustID.GemEmerald, Helper.NextVec2Dir(0.5f, 1f));
                d.noGravity = true;
            }
        }
    }

    public class GlistentJarExplode : ModProjectile
    {
        public override string Texture => AssetDirectory.DefaultItem;

        /// <summary>
        /// 未1时命中敌怪造成debuff
        /// </summary>
        public ref float HasDebuff => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.width = Projectile.height = 150;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Timer > 20)
                Projectile.Kill();

            Color darkGreen = new Color(0, 68, 18);
            Color lightGreen = new Color(182, 255, 171);

            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3() * 0.75f);

            if (Timer == 0)
            {
                float dir = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 7; i++)
                {
                    Vector2 dir2 = (dir + i / 7f * MathHelper.TwoPi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2();
                    FlowLineThin.Spawn(Projectile.Center + dir2 * Main.rand.NextFloat(2, 20)
                        , dir2 * Main.rand.NextFloat(2.5f, 5), 7, 20, Main.rand.NextFloat(-0.15f, 0.15f)
                        , Main.rand.NextFromList(Color.LimeGreen, lightGreen));

                    PRTLoader.NewParticle<TwistFogDark>(Projectile.Center, Helper.NextVec2Dir(2f, 3f)
                        , Main.rand.NextFromList(Color.DarkGreen, darkGreen), Main.rand.NextFloat(0.3f, 0.6f));
                    PRTLoader.NewParticle<TwistFog>(Projectile.Center, Helper.NextVec2Dir(2f, 3f)
                        , Main.rand.NextFromList(lightGreen, Color.LimeGreen), Main.rand.NextFloat(0.3f, 0.6f));
                }
            }

            if (Timer == 8)
            {
                float dir = Main.rand.NextFloat(MathHelper.TwoPi);

                for (int i = 0; i < 10; i++)
                {
                    Vector2 pos = Projectile.Center + (dir + Main.rand.NextFloat(-0.1f, 0.1f)).ToRotationVector2() * 70;
                    PRTLoader.NewParticle<TwistFogDark>(pos, (dir + Main.rand.NextFloat(MathHelper.Pi - 0.25f, MathHelper.Pi + 0.25f)).ToRotationVector2() * Main.rand.NextFloat(0.5f, 1)
                        , Main.rand.NextFromList(Color.DarkGreen, darkGreen) * 0.5f, Main.rand.NextFloat(0.2f, 0.4f));

                    pos = Projectile.Center + (dir + Main.rand.NextFloat(-0.1f, 0.1f)).ToRotationVector2() * 70;
                    PRTLoader.NewParticle<TwistFog>(pos, (dir + Main.rand.NextFloat(MathHelper.Pi - 0.25f, MathHelper.Pi + 0.25f)).ToRotationVector2() * Main.rand.NextFloat(0.5f, 1)
                        , Main.rand.NextFromList(lightGreen, Color.LimeGreen) * 0.5f, Main.rand.NextFloat(0.4f, 0.6f));

                    dir += 1 / 10f * MathHelper.TwoPi;
                }
            }

            float factor = Timer / 20;
            float scale = Helper.HeavyEase(factor);

            Vector2 dir3 = Helper.NextVec2Dir();
            Dust d = Dust.NewDustPerfect(Projectile.Center + dir3 * scale * 60, DustID.GemEmerald
                , dir3 * Main.rand.NextFloat(1, 2f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
            d.noGravity = true;

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
            if (HasDebuff == 1)
                target.AddBuff(ModContent.BuffType<GlistentJarDebuff>(), 60 * 10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Halo.CircleSPA.Value;
            Texture2D tex2 = CoraliteAssets.Halo.HighlightCircleSPA.Value;

            float factor = Timer / 20;
            float f2 = (1 - Helper.BezierEase(factor));

            Color dc = Color.DarkGreen * Helper.SinEase(factor);
            Color dc2 = new Color(0, 34, 9) * Helper.SinEase(factor);
            Color lc = Color.Green * f2;

            float scale = Helper.HeavyEase(factor) * 0.8f;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            tex2.QuickCenteredDraw(Main.spriteBatch, pos, dc2 * 0.55f, 0, scale * 0.85f);
            tex2.QuickCenteredDraw(Main.spriteBatch, pos, dc * 0.65f, 0, scale * 0.6f);

            tex.QuickCenteredDraw(Main.spriteBatch, pos, lc, 0, scale);
            tex = CoraliteAssets.Halo.HighlightCircle.Value;

            tex.QuickCenteredDraw(Main.spriteBatch, pos, (lc with { A = 0 } * 0.5f), 0, scale * 0.8f);

            return false;
        }
    }
}
