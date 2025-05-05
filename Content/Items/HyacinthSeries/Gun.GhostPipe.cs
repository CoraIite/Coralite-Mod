using Coralite.Content.Items.ThyphionSeries;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.HyacinthSeries
{
    public class GhostPipe : ModItem
    {
        public override string Texture => AssetDirectory.HyacinthSeriesItems + Name;

        private int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(24, 2);
            Item.DefaultToRangedWeapon(ProjectileType<QueenOfNightSpilitProj>(), AmmoID.Bullet, 15, 11f, true);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = CoraliteSoundID.Gun_Item11;

            Item.useTurn = false;
            Item.noUseGraphic = true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(0, -4);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, velocity, ProjectileType<GhostPipeHeldProj>(), 0, knockback, player.whoAmI);

            shootCount++;
            if (shootCount > 3)
                shootCount = 0;
            else
                type = ProjectileType<GhostPipeBullet>();

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlintlockPistol)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.CursedFlame, 15)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.HyacinthSeriesItems)]
    public class GhostPipeHeldProj : BaseGunHeldProj
    {
        public GhostPipeHeldProj() : base(0.1f, 24, -6, AssetDirectory.HyacinthSeriesItems) { }

        public static ATex GhostPipeFire { get; private set; }

        protected override float HeldPositionY => -2;

        private int FrameX;

        public override void InitializeGun()
        {
            FrameX = Main.rand.Next(4);
        }

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, Color.Lime.ToVector3() / 3);
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
            {
                Projectile.frame++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 2)
                return false;

            Texture2D effect = GhostPipeFire.Value;
            Rectangle frameBox = effect.Frame(4, 3, FrameX, Projectile.frame);
            //SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 20 + n.ToRotationVector2() * 4 - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale * 0.8f, 0, 0f);
            return false;
        }
    }

    public class GhostPipeBullet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public static ATex GhostPipeGradient { get; private set; }

        public Trail trail;
        private bool init = true;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public int trailCount = 10;
        public int trailWidth = 8;
        public float trailAlpha = 1;

        public ref float FadeTimer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * Projectile.MaxUpdates * 4;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
            => FadeTimer < 1;

        public override void AI()
        {
            Initialize();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Timer++;

            UpdateOldPos();

            if (FadeTimer > 0)
            {
                FadeTimer++;

                Projectile.velocity *= 0.85f;
                trailAlpha = 1 - FadeTimer / 25;

                if (FadeTimer > 25)
                    Projectile.Kill();

                return;
            }

            SpawnDust();
        }

        public void Initialize()
        {
            if (init)
            {
                init = false;

                if (!VaultUtils.isServer)
                {
                    Projectile.InitOldPosCache(trailCount);
                    trail = new Trail(Main.instance.GraphicsDevice, trailCount + 4, new EmptyMeshGenerator()
                        , f => trailWidth * trailAlpha, f => new Color(255, 255, 255, 170));//=> Color.Lerp(Color.Transparent, Color.White,f.X));
                }
            }
        }

        private void UpdateOldPos()
        {
            if (Timer % 2 == 0)
            {
                if (!VaultUtils.isServer)
                {
                    Projectile.UpdateOldPosCache();

                    Vector2[] pos2 = new Vector2[trailCount + 4];

                    //延长一下拖尾数组，因为使用的贴图比较特别
                    for (int i = 0; i < Projectile.oldPos.Length; i++)
                        pos2[i] = Projectile.oldPos[i] + Projectile.velocity;

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    int exLength =  4 ;

                    for (int i = 1; i < 5; i++)
                        pos2[trailCount + i - 1] = Projectile.oldPos[^1] + dir * i * exLength + Projectile.velocity;

                    trail.TrailPositions = pos2;
                }
            }
        }

        private void SpawnDust()
        {
            if (Main.rand.NextBool(3))
            {
                int width = 8 ;

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(width, width), DustID.CursedTorch
                    , Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f), 75, Scale: Main.rand.NextFloat(1, 1.4f));
                d.noGravity = true;
            }
        }

        public static Color RandomColor()
        {
            return Main.rand.Next(3) switch
            {
                0 => new Color(190, 170, 251),
                1 => new Color(134, 229, 251),
                _ => new Color(125, 190, 255)
            };
        }

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["TurbulenceArrow"].GetShader().Shader;

            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.08f);
            effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            effect.Parameters["udissolveS"].SetValue(1f);
            effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.LightShot.Value);
            effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
            effect.Parameters["uGradient"].SetValue(GhostPipeGradient.Value);
            effect.Parameters["uDissolve"].SetValue(TurbulenceArrow.TurbulenceFlow.Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            trail?.DrawTrail(effect);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            trail?.DrawTrail(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
