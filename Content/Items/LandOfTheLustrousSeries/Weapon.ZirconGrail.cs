using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class ZirconGrail : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 24));
            Item.SetWeaponValues(110, 4, 6);
            Item.useTime = Item.useAnimation = 37;
            Item.mana = 18;

            Item.shoot = ModContent.ProjectileType<ZirconGrailProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as ZirconGrailProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.2f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.45f);
                effect.Parameters["addC"].SetValue(0.75f);
                effect.Parameters["highlightC"].SetValue(ZirconProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(ZirconProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(ZirconProj.darkC.ToVector4());
            }, 0.4f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, ZirconProj.brightC, ZirconProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Zircon>()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.SteampunkCup)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class ZirconGrailProj : BaseGemWeaponProj<ZirconGrail>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "ZirconGrail";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, ZirconProj.brightC, ZirconProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(30, 50);
                cs.shineRange = 12;
            }
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center;
            for (int i = 0; i < 16; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
            {
                Tile idleTile = Framing.GetTileSafely(idlePos.ToTileCoordinates());
                if (idleTile.HasTile && Main.tileSolid[idleTile.TileType] && !Main.tileSolidTop[idleTile.TileType])
                {
                    idlePos -= new Vector2(0, -4);
                    break;
                }
                else
                    idlePos += new Vector2(0, -4);
            }

            if (AttackTime > 0)
            {
                Vector2 dir = (Main.MouseWorld - Projectile.Center);
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
            Lighting.AddLight(Projectile.Center, ZirconProj.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.Next(-20, 20), -20),
                    DustID.Torch, -Vector2.UnitY * Main.rand.NextFloat(3, 6));
                d.noGravity = true;

                if (AttackTime == 1)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int p = Projectile.NewProjectileFromThis<ZirconProj>(Projectile.Center + new Vector2(0, -25), -Vector2.UnitY.RotateByRandom(-0.2f, 0.2f) * (12 + i * 2.5f)
                             , Owner.GetWeaponDamage(Owner.HeldItem), 5);
                        Main.projectile[p].localNPCHitCooldown += i;
                    }
                }

                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            AttackTime = Owner.itemTimeMax;

            Helper.PlayPitched("Crystal/GemShoot", 0.4f, 0, Projectile.Center);
            SoundEngine.PlaySound(CoraliteSoundID.FireFork_Item73, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 4; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    new Color(251, 100, 152) * (0.3f - i * 0.3f / 4), Projectile.oldRot[i] + 0, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class ZirconProj : ModProjectile, IDrawAdditive, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(255, 179, 22);
        public static Color darkC = new Color(117, 55, 29);

        ref float State => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        ref float FlyingTime => ref Projectile.localAI[2];

        private ParticleGroup fireParticles;
        private Trail trail;
        private readonly int trailPoint = 16;

        private int chaseTime;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 26;
            Projectile.penetrate = 7;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = 3;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override bool? CanDamage()
        {
            if (State > 1)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;
            State = 2;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            chaseTime = 0;
            if (Projectile.penetrate < 3)
            {
                State = 2;
                Projectile.velocity *= 0;
            }
        }

        public override void AI()
        {
            if (fireParticles == null)
            {
                fireParticles = new ParticleGroup();
                Projectile.InitOldPosCache(trailPoint);
                Projectile.localAI[1] = Main.rand.NextFloat(-0.01f, 0.01f);
                FlyingTime = 20 * 5;
            }

            trail ??= new Trail(Main.instance.GraphicsDevice, trailPoint, new NoTip(), factor =>
            {
                if (factor < 0.8f)
                    return Helper.Lerp(6, 8, factor / 0.8f);

                return Helper.Lerp(12, 0, (factor - 0.8f) / 0.2f);
            }, ColorFunc1);

            switch (State)
            {
                default:
                case 0://下落
                    {
                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                        {
                            float selfAngle = Projectile.velocity.ToRotation();
                            float targetAngle = (target.Center - Projectile.Center).ToRotation();

                            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.2f + 0.8f * Math.Clamp((chaseTime - 30) / 30, 0, 1f)).ToRotationVector2() * Projectile.velocity.Length();
                        }

                        SpawnDusts(1 - 0.3f * Timer / FlyingTime);

                        Timer++;
                        chaseTime++;

                        if (Timer > FlyingTime)
                        {
                            State = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        Projectile.localAI[0] += Projectile.localAI[1];
                        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
                        Projectile.velocity *= 0.97f;

                        SpawnDusts(0.7f);

                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        Timer++;
                        if (Timer > 15)
                        {
                            Projectile.velocity *= 0;
                            State++;
                        }
                    }
                    break;
                case 2://他紫砂了
                    {
                        if (!fireParticles.Any())
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }

            Projectile.UpdateOldPosCache();
            trail.Positions = Projectile.oldPos;
            fireParticles.UpdateParticles();
        }

        public static Color ColorFunc1(Vector2 factor)
        {
            if (factor.X < 0.7f)
            {
                return Color.Lerp(new Color(0, 0, 0, 0), brightC, factor.X / 0.7f);
            }

            return Color.Lerp(brightC, highlightC, (factor.X - 0.7f) / 0.3f);
        }

        public void SpawnDusts(float factor)
        {
            Color c;
            int type;
            type = DustID.OrangeTorch;
            c = Main.rand.Next(3) switch
            {
                1 => darkC,
                _ => brightC,
            };

            Projectile.SpawnTrailDust(type, Main.rand.NextFloat(0.2f, 0.6f), Scale: Main.rand.NextFloat(1f, 2f));

            float angle = Projectile.velocity.AngleFrom(Projectile.oldVelocity);
            float rate = MathHelper.Clamp(0.4f - Math.Abs(angle) / 5, 0, 0.4f);
            if (Main.rand.NextBool())
            {
                var p = fireParticles.NewParticle<FireParticle>(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                     (Projectile.velocity * factor * Main.rand.NextFloat(rate * 0.7f, rate * 1.3f + 0.001f)).RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f))
                    , c, Main.rand.NextFloat(0.2f, 0.6f));
                p.MaxFrameCount = 2;
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            fireParticles?.DrawParticles(Main.spriteBatch);
        }

        public void DrawPrimitives()
        {
            if (State == 2 || trail == null)
                return;

            Effect effect = Filters.Scene["Flow2"].GetShader().Shader;

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 5);
            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
            effect.Parameters["uTextImage"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaserFlow").Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            trail.Render(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

}
