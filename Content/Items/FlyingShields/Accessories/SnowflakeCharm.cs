using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields.Accessories
{
    [AutoloadEquip(EquipType.Neck)]
    public class SnowflakeCharm : BaseFlyingShieldAccessory, IFlyingShieldAccessory
    {
        public SnowflakeCharm() : base(ItemRarityID.Green, Item.sellPrice(0, 0, 10))
        { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 20;
            Item.DamageType = DamageClass.Generic;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !(equippedItem.type == ModContent.ItemType<DemonsProtection>()//上位

                && incomingItem.type == ModContent.ItemType<SnowflakeCharm>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }
        }

        public void OnGuardInitialize(BaseFlyingShieldGuard projectile)
        {
            projectile.parryTime = 6;
            projectile.distanceAdder *= 1.1f;
        }

        public bool OnParry(BaseFlyingShieldGuard projectile)
        {
            Player Owner = projectile.Owner;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    Owner.AddImmuneTime(ImmunityCooldownID.General, 20);
                }

                int damage = (int)(projectile.Owner.GetWeaponDamage(Item) * (1.1f - (0.15f * cp.parryTime / 280f)));

                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, projectile.Projectile.Center);
                Helper.PlayPitched("Misc/ShieldGuard", 0.4f, 0f, projectile.Projectile.Center);

                Vector2 dir = projectile.Projectile.rotation.ToRotationVector2();
                int index = projectile.Projectile.NewProjectileFromThis<Snowflake>(Owner.Center, dir * 4,
                     damage, projectile.Projectile.knockBack, Owner.whoAmI);
                Main.projectile[index].scale = projectile.Projectile.scale;

                if (cp.parryTime < 280)
                    cp.parryTime += 100;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 20)
                .AddIngredient(ItemID.GoldBar, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.IceBlock, 20)
                .AddIngredient(ItemID.PlatinumBar, 8)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class Snowflake : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldAccessories + Name;

        ref float Timer => ref Projectile.ai[0];
        ref float State => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.width = Projectile.height = 16;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://向前旋转
                    {
                        if (Projectile.localAI[0] == 0)
                        {
                            Projectile.localAI[0] = 1;
                            Projectile.rotation = Main.rand.NextFloat(6.282f);
                            Projectile.scale = 0.3f;
                        }

                        Projectile.rotation += (1 - (Timer / 50)) * 0.3f;
                        Projectile.scale += 0.7f / 50;
                        Projectile.velocity *= 0.98f;

                        Timer++;
                        if (Timer > 50)
                        {
                            Timer = 0;
                            State++;
                            Projectile.velocity *= 0;
                        }
                    }
                    break;
                case 1://缩小并炸开
                    {
                        Timer++;
                        if (Timer < 3)
                            break;

                        Projectile.scale -= 1 / 10f;
                        if (Timer > 10)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                Vector2 velocity = (Projectile.rotation + 1.57f + (i * MathHelper.TwoPi / 6)).ToRotationVector2();
                                Projectile.NewProjectileFromThis<SnowflakeSpike>(Projectile.Center
                                    , velocity * Main.rand.NextFloat(5, 6f),
                                     (int)(Projectile.damage * 0.9f), Projectile.knockBack, ai1: Main.rand.NextFloat(8, 10));
                            }
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Color c = new Color(255, 255, 255, 0) * 0.1f;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition + ((Main.GlobalTimeWrappedHourly + (i * MathHelper.TwoPi / 3)).ToRotationVector2() * 3), null,
                    c, Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.05f, 0, 0);
            }
            Projectile.QuickDraw(lightColor, 0f);
            return false;
        }
    }

    public class SnowflakeSpike : BaseHeldProj, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Trails + "SlashFlatBlurHVMirror";

        public ref float Timer => ref Projectile.ai[0];
        public ref float TrailWidth => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Length => ref Projectile.localAI[1];

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 200;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.netImportant = true;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 35;
        }

        public override void Initialize()
        {
            Projectile.InitOldPosCache(16);

            Alpha = 1;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > 20)
                return false;

            return null;
        }

        public override void AI()
        {
            trail ??= new Trail(Main.graphics.GraphicsDevice, 16, new EmptyMeshGenerator(), WidthFunction, ColorFunction);

            Lighting.AddLight(Projectile.Center, Color.CadetBlue.ToVector3());

            const int ShootTime = 20;
            const int DelayTime = ShootTime + 15;

            if (Timer < ShootTime)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Circular(TrailWidth, TrailWidth) / 2), DustID.ApprenticeStorm,
                    Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.2f));
                if (Main.rand.NextBool())
                    dust.noGravity = true;
                if (Timer < 14)
                {
                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;

                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Vector2.Lerp(Projectile.oldPos[0], Projectile.oldPos[15], i / 15f);
                }
                else
                {
                    for (int i = 0; i < 15; i++)
                        Projectile.oldPos[i] = Projectile.oldPos[i + 1];

                    Projectile.oldPos[15] = Projectile.Center + Projectile.velocity;
                }
            }
            else
            {
                Alpha -= 1 / 15f;
                Projectile.velocity = Vector2.Zero;
            }


            trail.TrailPositions = Projectile.oldPos;

            Timer++;
            if (Timer > DelayTime)
            {
                Projectile.Kill();
            }
        }

        public float WidthFunction(float factor)
        {
            if (factor < 0.7f)
                return Helper.Lerp(0, TrailWidth, factor / 0.7f);
            return Helper.Lerp(TrailWidth, 0, (factor - 0.7f) / 0.3f);
        }

        public Color ColorFunction(Vector2 factor)
        {
            return Color.White;
        }

        public void DrawPrimitives()
        {
            if (trail == null || Timer < 0)
                return;

            Effect effect = Filters.Scene["AlphaNoHLGradientTrail"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Projectile.GetTexture());
            effect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.FlyingShieldAccessories + "SnowflakeSpikeGradient").Value);
            effect.Parameters["alpha"].SetValue(Alpha);

            trail.DrawTrail(effect);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, Projectile.oldPos[12] - Main.screenPosition,
                Color.White, Color.CadetBlue, Timer / 35, 0, 0.2f, 0.6f, 1, Projectile.rotation + 1.57f,
                new Vector2(0.1f, 2.4f), Vector2.One);
            return false;
        }
    }
}
