using Coralite.Content.Dusts;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class CrystalSword : BaseMagikeChargeableItem
    {
        public CrystalSword() : base(150, Item.sellPrice(0, 0, 10, 0)
            , ModContent.RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeSeries1Item)
        { }

        public override void SetDefs()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 60;
            Item.useAnimation = 25;
            Item.shoot = ModContent.ProjectileType<CrystalSwordProj>();
            Item.damage = 15;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Generic;
            Item.UseSound = CoraliteSoundID.Swing_Item1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (Item.TryCosumeMagike(5) || player.TryCosumeMagike(5))
                {
                    Projectile.NewProjectile(source, position, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 12,
                        type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }
    }

    public class CrystalSwordProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        private VertexStrip _vertexStrip = new VertexStrip();

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 18);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Crystal_Item101, Projectile.Center);
        }

        public override void AI()
        {
            if (Main.rand.NextBool())
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4, 4), ModContent.DustType<GlowBall>(),
                    -Projectile.velocity * 0.1f, 0, Coralite.Instance.MagicCrystalPink, 0.15f);
            }

            int num18 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 100, new Color(162, 42, 131), 1f);
            Main.dust[num18].velocity *= 0.1f;
            Main.dust[num18].velocity += Projectile.velocity * 0.2f;
            Main.dust[num18].position.X = Projectile.Center.X + 4f + Main.rand.Next(-4, 4);
            Main.dust[num18].position.Y = Projectile.Center.Y + Main.rand.Next(-4, 4);
            Main.dust[num18].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Teleporter, Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-3, 3), 100, Coralite.Instance.MagicCrystalPink, 1f);
                //dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["RainbowRod"];
            miscShaderData.UseSaturation(-1.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2f);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            //ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, Projectile.oldPos[0]+new Vector2(8,8) - Main.screenPosition,
            //    Color.White * 0.8f, Coralite.Instance.MagicCrystalPink, 0.5f, 0f, 0.5f, 0.5f, 0f, 0f,
            //    new Vector2(0.5f, 0.5f), Vector2.One*0.5f);
            return false;
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, new Color(162, 42, 131), Utils.GetLerpValue(-0.2f, 0.5f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A = 64;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(4f, 16f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);
    }
}
