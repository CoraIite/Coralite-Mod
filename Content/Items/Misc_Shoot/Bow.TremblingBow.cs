using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Misc_Shoot
{
    public class TremblingBow : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Shoot + Name;

        public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Glow");
            GlowMaskID= (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 14;
            Item.shootSpeed = 7f;
            Item.knockBack = 5;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Green;
            Item.useTime = Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 0, 50);

            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = false;

            Item.glowMask = GlowMaskID;


            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundStyle st = CoraliteSoundID.ElectricExplosion_Item94;
            st.Volume -= 0.6f;
            SoundEngine.PlaySound(st, player.Center);

            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            float speed = 9f;
            int damage2 = (int)(damage * 0.4f);
            for (int i = -1; i < 2; i += 2)
            {
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                    , position + dir.RotatedBy(i * 1f) * 48, dir.RotatedBy(-i * 0.5f) * speed, ModContent.ProjectileType<TremblingElectric>()
                    , damage2, knockback, player.whoAmI, i);
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-1, 0);
        }
    }

    public class TremblingElectric : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];

        public ref float Dir => ref Projectile.ai[0];

        public ref float ThunderWidth => ref Projectile.localAI[1];
        public ref float ThunderAlpha => ref Projectile.localAI[2];
        public ref float Timer => ref Projectile.localAI[0];

        protected ThunderTrail[] thunderTrails;

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 2;
            Projectile.width = Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public virtual float ThunderWidthFunc_Sin(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth;
        }

        public virtual Color ThunderColorFunc(float factor)
        {
            return new Color(103, 255, 255, 0) * ThunderAlpha;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3() / 2);
            if (thunderTrails == null)
            {
                Projectile.InitOldPosCache(10);
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody2");
                for (int i = 0; i < 3; i++)
                {
                    thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc);
                    thunderTrails[i].CanDraw = i == 0;
                    thunderTrails[i].SetRange((0, 7));
                    thunderTrails[i].SetExpandWidth(2);
                    thunderTrails[i].BasePositions =
                    [
                        Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }

            RotateAI();
            if (Timer % 2 == 0)
                Projectile.UpdateOldPosCache();

            if (Main.rand.NextBool(3))
            {
                Projectile.SpawnTrailDust(DustID.Electric, Main.rand.NextFloat(0.2f, 0.6f), Scale: Main.rand.NextFloat(0.4f, 0.8f));
            }
        }

        public void RotateAI()
        {
            do
            {
                if (Timer < 12)
                {
                    ThunderWidth = Helper.Lerp(0, 14, Timer / 12f);
                    ThunderAlpha = Helper.Lerp(0, 1, Timer / 12f);
                    break;
                }

                if (Timer == 12)
                    Projectile.velocity = Projectile.velocity.RotatedBy(Dir * 1f);

                if (Timer < 20)
                    break;

                if (Timer == 20)
                    Projectile.velocity = Projectile.velocity.RotatedBy(-Dir * 0.6f);

                if (Timer < 40)
                    break;

                if (Timer < 52)
                {
                    UpdateTrails();

                    ThunderAlpha = Helper.Lerp(1, 0, (Timer - 40) / 12f);
                    ThunderWidth = Helper.Lerp(14, 0, (Timer - 40) / 12f);

                    break;
                }

                Projectile.Kill();
            } while (false);

            Timer++;
            UpdateTrails();
        }

        public void UpdateTrails()
        {
            for (int i = 0; i < 3; i++)
            {
                var trail = thunderTrails[i];

                if (i == 0)
                {
                    trail.BasePositions = Projectile.oldPos;
                    trail.RandomThunder();
                    continue;
                }

                if (Timer % 4 == 0)
                    trail.CanDraw = Main.rand.NextBool();
                if (trail.CanDraw)
                {
                    trail.BasePositions = Projectile.oldPos;
                    trail.RandomThunder();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SpawnKillDust(oldVelocity);

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnKillDust(Projectile.velocity);
        }

        public void SpawnKillDust(Vector2 velocity)
        {
            Vector2 dir = velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                    , DustID.Electric, dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(0.5f, 4f),
                    Scale: Main.rand.NextFloat(0.4f, 0.7f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
                foreach (var trail in thunderTrails)
                    if (trail.CanDraw)
                        trail.DrawThunder(Main.instance.GraphicsDevice);

            return false;
        }
    }
}
