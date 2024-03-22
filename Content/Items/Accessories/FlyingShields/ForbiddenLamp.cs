using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Accessories.FlyingShields
{
    public class ForbiddenLamp : BaseAccessory, IFlyingShieldAccessory, IDashable
    {
        public ForbiddenLamp() : base(ItemRarityID.Pink, Item.sellPrice(0, 1, 50, 0))
        {
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Generic;
            Item.damage = 53;
        }

        public bool isDashing;
        public int SpawnEXProjCount;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.FlyingShieldAccessories?.Add(this);
            }

            if (isDashing)
            {
                player.noKnockback = true;
            }
        }

        public void OnDashing(BaseFlyingShieldGuard projectile)
        {
            projectile.OnGuard_DamageReduce(projectile.damageReduce);
            for (int i = 0; i < 2; i++)
            {
                projectile.Projectile.SpawnTrailDust(DustID.Wraith, Main.rand.NextFloat(0.3f, 0.5f),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
            }

            if (projectile.Timer > projectile.dashTime / 3)
            {
                projectile.Owner.velocity.X = (projectile.dashDir.ToRotationVector2() * projectile.dashSpeed).X;
            }
        }

        public void OnDashHit(BaseFlyingShieldGuard projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SpawnEXProjCount < 3)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Boom_Item14, projectile.Projectile.Center);
                int damage = projectile.Owner.GetWeaponDamage(Item);
                int damage2 = (int)(damage * 0.95f);
                if (SpawnEXProjCount == 0)
                {
                    projectile.Projectile.NewProjectileFromThis<ForbiddenLampExplosion>(projectile.Projectile.Center
                        , Vector2.Zero, damage, 0);
                    for (int i = 0; i < 3; i++)
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0, 35 + i * 10);
                }
                else
                {
                    projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                        , (-1.57f + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                        , damage2, 0,35);
                    if (Main.rand.NextBool())
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0,45);
                    if (Main.rand.NextBool(4))
                        projectile.Projectile.NewProjectileFromThis<ForbiddenDroplet>(projectile.Projectile.Center
                            , (-1.57f + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(10, 13)
                            , damage2, 0,55);
                }

                SpawnEXProjCount++;
            }
        }

        public void OnDashOver(BaseFlyingShieldGuard projectile)
        {
            projectile.Owner.velocity *= 0.7f;
            isDashing = false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        Player.direction = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 0 : 3.141f;
                        break;
                    }
                default:
                    return false;
            }

            if (Player.TryGetModPlayer(out CoralitePlayer cp)
                && cp.TryGetFlyingShieldGuardProj(out BaseFlyingShieldGuard flyingShieldGuard)
                && flyingShieldGuard.CanDash())
            {
                SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Player.Center);

                flyingShieldGuard.TurnToDashing(this, 20, dashDirection, 12f);
                Player.GetModPlayer<CoralitePlayer>().DashDelay = 75;

                SpawnEXProjCount = 0;
                isDashing = true;
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LavaLamp>()
                .AddIngredient(ItemID.AncientBattleArmorMaterial, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ForbiddenLampExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 160;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                 Dust.NewDustPerfect(pos, DustID.Wraith, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3),
                    Scale: Main.rand.NextFloat(1, 1.3f));
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Wraith, (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(1, 2f));
                d.noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(Projectile.width / 4, Projectile.height / 4);
                Dust d = Dust.NewDustPerfect(pos, DustID.Wraith, (Projectile.Center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(1, 2f));
                d.noGravity = true;
            }

            for (int i = 0; i <3; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
               Dust.NewDustPerfect(pos, DustID.GoldFlame, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 3),
                    Scale: Main.rand.NextFloat(1, 2f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }
    }

    public class ForbiddenDroplet : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] < Projectile.ai[0])
            {
                if (Projectile.velocity.Y < 12)
                    Projectile.velocity.Y += 0.3f;
            }
            else if ((int)Projectile.ai[1] == (int)Projectile.ai[0])
            {
                if (Helper.TryFindClosestEnemy(Projectile.Center, 500, n => n.CanBeChasedBy(), out NPC target))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                        Dust d = Dust.NewDustPerfect(pos, DustID.Wraith, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4),
                            Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }
                    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 12;
                    Projectile.extraUpdates = 1;
                }
                else
                    Projectile.velocity = new Vector2(0, 14);
            }
            else if (Projectile.ai[1]>80)
            {
                if (Projectile.velocity.Y < 12)
                    Projectile.velocity.Y += 0.15f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(DustID.Wraith, Main.rand.NextFloat(0.1f, 0.5f), Alpha: 50, Scale: Main.rand.NextFloat(0.8f, 1.1f));
            Projectile.SpawnTrailDust(DustID.GoldFlame, Main.rand.NextFloat(0.1f, 0.5f), Scale: Main.rand.NextFloat(1f, 1.3f));
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Wraith, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(1f, 1.3f));
                d.noGravity = true;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 3, Projectile.height / 3);
                Dust d = Dust.NewDustPerfect(pos, DustID.Enchanted_Gold, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1, 2),
                    Scale: Main.rand.NextFloat(1, 1.2f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D extraTex = TextureAssets.Extra[98].Value;
            Main.instance.LoadProjectile(931);

            Texture2D mainTex = TextureAssets.Projectile[931].Value;
            Vector2 center = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color baseColor = Color.Gold;
            Vector2 extraOrigin = extraTex.Size() / 2f;
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            //残影
            for (int i = 1; i < 10; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    baseColor * (0.5f - i * 0.5f / 10), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale, 0, 0);

            //额外星星
            Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, center,Color.Orange, Color.OrangeRed, 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, new Vector2(0.6f, 1.2f), Vector2.One);

            //主帖图
            Main.EntitySpriteDraw(extraTex, center, null, baseColor, Projectile.rotation, extraOrigin, Projectile.scale * 0.9f, 0);
            Main.EntitySpriteDraw(mainTex, center, null, Color.LightYellow, Projectile.rotation, mainTex.Size() / 2, Projectile.scale * 0.9f, 0);
            Main.EntitySpriteDraw(mainTex, center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale * 0.9f, 0);

            return false;
        }
    }
}
