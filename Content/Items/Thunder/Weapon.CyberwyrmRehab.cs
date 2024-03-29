using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class CyberwyrmRehab : ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 7;
            Item.crit = 10;
            Item.mana = 16;
            Item.shootSpeed = 1;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ProjectileType<CyberwyrmRehabProj>();

            Item.noMelee = true;
            Item.useTurn = false;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int shootCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.active && n.CanBeChasedBy() && Vector2.Distance(player.Center, n.Center) < 700 && Collision.CanHit(player, n))
                {
                    Projectile.NewProjectile(source, position, n.Center, type,
                        damage, knockback, player.whoAmI, player.itemTimeMax * 0.6f, i);
                    shootCount++;
                    if (shootCount > 2)
                    {
                        break;
                    }
                }
            }

            if (shootCount > 0)
            {
                SoundEngine.PlaySound(CoraliteSoundID.NoUse_Electric_Item93, player.Center);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ZapCrystal>(2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    /// <summary>
    /// 使用ai0传入冲刺时间，ai1传入目标
    /// </summary>
    public class CyberwyrmRehabProj : BaseThunderProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float DashTime => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];

        public ref float Timer => ref Projectile.localAI[0];

        public Player Owner => Main.player[Projectile.owner];

        const int DelayTime = 30;

        protected ThunderTrail[] thunderTrails;

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (Timer > DashTime + DelayTime / 2)
                return false;

            return null;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.velocity, Projectile.Center, Projectile.width, ref a);
        }

        public override float ThunderWidthFunc_Sin(float factor)
        {
            return ThunderWidth;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.whoAmI != (int)Target)
                return false;
            return base.CanHitNPC(target);
        }

        public override void AI()
        {
            if (!Target.GetNPCOwner(out NPC target, Projectile.Kill))
                return;
            Projectile.velocity = target.Center;
            Lighting.AddLight(Projectile.Center, Coralite.Instance.ThunderveinYellow.ToVector3());
            if (thunderTrails == null)
            {
                Projectile.Resize((int)32, 40);
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc2_Orange);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc_Sin, ThunderColorFunc_Yellow);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((0, 8));
                    thunderTrails[i].SetExpandWidth(2);
                    thunderTrails[i].BasePositions = new Vector2[3]
                    {
                        Projectile.Center,Projectile.Center,Projectile.Center
                    };
                }
            }

            if (Timer < DashTime)
            {
                if (Owner.ItemTimeIsZero)
                    Timer = DashTime;

                SpawnDusts();
                Projectile.Center = Owner.Center + (Owner.itemRotation + (Owner.direction > 0 ? 0 : 3.141f)).ToRotationVector2() * 50;

                UpdateTrails();

                ThunderWidth = 14;
                ThunderAlpha = Timer / DashTime;
            }
            else if ((int)Timer == (int)DashTime)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                Projectile.Center = Owner.Center + (Owner.itemRotation + (Owner.direction > 0 ? 0 : 3.141f)).ToRotationVector2() * 50;
                SpawnDusts();

                UpdateTrails();

                float factor = (Timer - DashTime) / (DelayTime);
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 14 + sinFactor * 10;
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 6 + sinFactor * 12));
                    trail.SetExpandWidth((1 - factor) * 6);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > DashTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }

        public void UpdateTrails()
        {
            Vector2 pos2 = Projectile.velocity;
            List<Vector2> pos = new List<Vector2>
                {
                    Projectile.velocity
                };
            if (Vector2.Distance(Projectile.velocity, Projectile.Center) < 32)
                pos.Add(Projectile.Center);
            else
                for (int i = 0; i < 40; i++)
                {
                    pos2 = pos2.MoveTowards(Projectile.Center, 32);
                    if (Vector2.Distance(pos2, Projectile.Center) < 32)
                    {
                        pos.Add(Projectile.Center);
                        break;
                    }
                    else
                        pos.Add(pos2);
                }

            foreach (var trail in thunderTrails)
                trail.BasePositions = pos.ToArray();

            if (Timer % 5 == 0)
            {
                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
        }

        public void SpawnDusts()
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 pos = Vector2.Lerp(Projectile.velocity, Projectile.Center, Main.rand.NextFloat(0.1f, 0.9f))
                    + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.width / 2);
                if (Main.rand.NextBool())
                    Particle.NewParticle(pos, Vector2.Zero, CoraliteContent.ParticleType<ElectricParticle>(), Scale: Main.rand.NextFloat(0.6f, 1f));
                else
                    Dust.NewDustPerfect(pos, DustType<LightningShineBall>(), Vector2.Zero, newColor: ThunderveinDragon.ThunderveinYellowAlpha, Scale: Main.rand.NextFloat(0.1f, 0.2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (thunderTrails != null)
                foreach (var trail in thunderTrails)
                    trail?.DrawThunder(Main.instance.GraphicsDevice);
            return false;
        }
    }
}
