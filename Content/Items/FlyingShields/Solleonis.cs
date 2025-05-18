using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Solleonis : BaseFlyingShieldItem<SolleonisGuard>
    {
        public Solleonis() : base(Item.sellPrice(0, 8), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<SolleonisProj>();
            Item.knockBack = 8;
            Item.shootSpeed = 18;
            Item.damage = 80;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Leonids>()
                .AddIngredient(ItemID.FragmentSolar, 10)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class SolleonisProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Solleonis";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 26;
            backTime = 22;
            backSpeed = 20;
            trailCachesLength = 8;
            trailWidth = 20 / 2;
        }

        public override void OnShootDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
            Projectile.SpawnTrailDust(14f, DustID.SolarFlare, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));

            if (Timer > flyingTime * 0.3f && Timer % (flyingTime / 3) == 0)
            {
                //射流星
                Projectile.NewProjectileFromThis<SolleonisMeteor>(Projectile.Center
                    , (Projectile.extraUpdates + 1) * Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(0.8f, 1.2f),
                    (int)(Projectile.damage * 0.78f), Projectile.knockBack);
            }
        }

        public override void OnBackDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
            Projectile.SpawnTrailDust(14f, DustID.SolarFlare, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
        }

        public override Color GetColor(float factor)
        {
            return new Color(255, 174, 33, 0) * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Color c = Color.White;
            c.A = 0;

            for (int i = trailCachesLength - 1; i > 4; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                c * 0.6f * (i * 1 / 10f), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale * (1 - (i * 0.05f)), 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.3f, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale * 1.15f, 0, 0);
        }
    }

    public class SolleonisGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Solleonis";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 54;
            Projectile.height = 68;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.3f;
        }

        public override void OnGuard()
        {
            base.OnGuard();
            int num4 = Projectile.NewProjectileFromThis(Owner.Center, Vector2.Zero, 608, Projectile.damage, 15f);
            Main.projectile[num4].netUpdate = true;
            Main.projectile[num4].Kill();
        }
    }

    public class SolleonisMeteor : BaseHeldProj
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        ref float State => ref Projectile.ai[0];
        ref float Target => ref Projectile.ai[1];

        public int trailCachesLength = 8;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 22;
        }

        public override void Initialize()
        {
            Target = -1;
            Projectile.InitOldPosCache(trailCachesLength);
            Projectile.InitOldRotCache(trailCachesLength);
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());

            switch (State)
            {
                default:
                case 0://刚生成没多久
                    {
                        if (Projectile.timeLeft < 6)
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 400, n => n.CanBeChasedBy(), out NPC target))
                            {
                                Target = target.whoAmI;
                                State = 1;
                                Projectile.timeLeft = 200;
                            }
                    }
                    break;
                case 1://追踪
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            State = 2;
                            Projectile.timeLeft = 10;
                            break;
                        }

                        if (!target.CanBeChasedBy())
                        {
                            State++;
                            Projectile.timeLeft = 10;
                        }
                        float length = Projectile.velocity.Length();
                        float factor = Coralite.Instance.X2Smoother.Smoother(Projectile.timeLeft / 200f);
                        Projectile.velocity = Projectile.rotation.AngleLerp((target.Center - Projectile.Center).ToRotation(), 1 - factor)
                            .ToRotationVector2() * length;
                    }
                    break;
                case 2://跟丢目标
                    {
                        //啥也不干
                    }
                    break;
            }

            Projectile.UpdateOldPosCache(addVelocity: true);
            Projectile.UpdateOldRotCache();

            Projectile.SpawnTrailDust(8f, DustID.SolarFlare, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(0.6f, 0.8f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileFromThis(target.Center, Vector2.Zero, ProjectileID.SolarWhipSwordExplosion
                , (int)(Projectile.damage * 0.5f), 10f, 0f, 0.85f + (Main.rand.NextFloat() * 1.15f));

            for (int i = 0; i < 3; i++)
            {
                int type = Utils.SelectRandom(Main.rand, 6, 259, 158);
                int num142 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, type, 2.5f * Projectile.direction, -2.5f);
                Main.dust[num142].alpha = 200;
                Dust dust2 = Main.dust[num142];
                //dust2.velocity *= 2.4f;
                dust2.scale += Main.rand.NextFloat();
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = (i * MathHelper.PiOver4).ToRotationVector2();
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.SolarFlare
                        , dir * (i % 2 == 0 ? 1 : 0.5f) * (2.5f + (j * 1f)), Scale: 1.6f - (j * 0.15f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Hit_Item10, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTrails();
            Texture2D mainTex = Projectile.GetTexture();
            Color c = Color.White;
            c.A = 0;
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            SpriteEffects effects = SpriteEffects.None;
            if (Projectile.direction < 0)
                effects = SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, Projectile.rotation, origin, 0.7f, effects, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.15f, Projectile.rotation, origin, 0.85f, effects, 0);
            return false;
        }

        public virtual void DrawTrails()
        {
            Texture2D Texture = CoraliteAssets.Trail.CircleA.Value;

            List<ColoredVertex> bars = new();
            List<ColoredVertex> bars2 = new();
            Color c = new Color(255, 174, 33) * 0.9f;
            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + (normal * 14 * ((factor / 2) + 0.5f));
                Vector2 Bottom = Center - (normal * 14 * ((factor / 2) + 0.5f));

                Vector2 Top2 = Center + (normal * 6);
                Vector2 Bottom2 = Center - (normal * 6);

                var Color = c * factor;

                bars.Add(new(Top, Color, new Vector3(factor, 0, 0)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 0)));
                bars2.Add(new(Top2, Color, new Vector3(factor, 0, 0)));
                bars2.Add(new(Bottom2, Color, new Vector3(factor, 1, 0)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            BlendState b = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = b;
        }
    }
}
