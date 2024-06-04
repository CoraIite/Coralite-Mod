using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PinkDiamondRose : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 5));
            Item.SetWeaponValues(30, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 12;

            Item.shoot = ModContent.ProjectileType<PinkDiamondRoseProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as PinkDiamondRoseProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.4f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.45f);
                effect.Parameters["addC"].SetValue(0.85f);
                effect.Parameters["highlightC"].SetValue(PinkDiamondProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(PinkDiamondProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(PinkDiamondProj.darkC.ToVector4());
            }, 0.1f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, PinkDiamondProj.brightC, PinkDiamondProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PinkDiamond>()
                .AddIngredient(ItemID.BorealWood, 12)
                .AddIngredient(ItemID.FlowerPacketPink)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PinkDiamondRoseProj : BaseGemWeaponProj<PinkDiamondRose>, IDrawAdditive
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "PinkDiamondRose";

        private ParticleGroup group;
        private Vector2 offset;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void Initialize()
        {
            TargetPos = Owner.Center;
        }

        public override void BeforeMove()
        {
            group ??= new ParticleGroup();
            if (AttackTime < 1 && Main.rand.NextBool(6))
            {
                Color c = Main.rand.Next(3) switch
                {
                    0 => new Color(193, 89, 138, 100),
                    1 => new Color(125, 33, 80, 100),
                    _ => new Color(193, 89, 138, 100),
                };
                Vector2 pos = Projectile.Center +
                    (Projectile.rotation - 1.57f).ToRotationVector2() * Main.rand.NextFloat(8, 16);
                group.Add(Particle.NewPawticleInstance<Fog>(pos
                    , Vector2.UnitY.RotateByRandom(MathHelper.Pi - 0.4f, MathHelper.Pi + 0.4f)
                    , c, Main.rand.NextFloat(0.5f, 0.6f)));

                Dust d = Dust.NewDustPerfect(pos, DustID.PinkTorch, Helper.NextVec2Dir(1f, 3f));
                d.noGravity = true;
            }

            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 24);
                Color c2 = Main.rand.NextFromList(Color.White, PinkDiamondProj.brightC, PinkDiamondProj.darkC);
                var cs = CrystalShine.New(
                    Projectile.Center + (Projectile.rotation - 1.57f).ToRotationVector2() * 12 + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c2);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
                group.Add(cs);
            }

            group.UpdateParticles();
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(-OwnerDirection * 32, 0);
            for (int i = 0; i < 12; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
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
                idlePos += (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 64;
                idlePos += offset;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);

            Lighting.AddLight(Projectile.Center, AmethystLaser.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                int halfTime = Owner.itemTimeMax / 2;
                if (AttackTime > halfTime)
                {
                    //蓄力旋转
                    Projectile.rotation += OwnerDirection * MathHelper.TwoPi / halfTime;
                    offset *= 0.9f;
                }
                else
                {
                    Projectile.rotation = Projectile.rotation.AngleTowards((Main.MouseWorld - Projectile.Center).ToRotation() + 1.57f, MathHelper.TwoPi / halfTime);
                    if (AttackTime == 1)
                    {
                        Vector2 dir = (Projectile.rotation - 1.57f).ToRotationVector2();
                        offset = -dir * 128;

                        Projectile.NewProjectileFromThis<PinkDiamondProj>(Projectile.Center + dir * 12, dir * 12
                            , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                        Helper.PlayPitched("Crystal/GemShoot", 0.4f, 0, Projectile.Center);

                        for (int i = 0; i < 8; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }

                        for (int i = 0; i < 5; i++)
                            PinkDiamondProj.SpawnTriangleParticle(Projectile.Center + dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
                return;
            }

            offset = Vector2.Zero;
            Projectile.rotation = Projectile.rotation.AngleLerp(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f, 0.6f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(new Color(251, 100, 152), 0.3f, 0.3f / 4, 0, 4, 1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            group?.DrawParticles(spriteBatch);
        }
    }

    public class PinkDiamondProj : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "DiamondProj1";

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(244, 144, 183); 
        public static Color darkC = new Color(153, 90, 123);

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.timeLeft % 2 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool())
            {
                Color c = Main.rand.Next(3) switch
                {
                    0 => new Color(193, 89, 138, 100),
                    1 => new Color(125, 33, 80, 100),
                    _ => new Color(193, 89, 138, 100),
                };
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                Particle.NewParticle<Fog>(pos
                    , Vector2.UnitY.RotateByRandom(MathHelper.Pi - 0.4f, MathHelper.Pi + 0.4f)
                    , c, Main.rand.NextFloat(0.5f, 0.6f));

                Projectile.SpawnTrailDust(8f, DustID.PinkTorch, Main.rand.NextFloat(0.2f, 0.4f));
            }
        }

        public static void SpawnTriangleParticle(Vector2 pos, Vector2 velocity)
        {
            Color c1 = highlightC;
            c1.A = 125;
            Color c2 = brightC;
            c2.A = 125;
            Color c3 = darkC;
            c3.A = 100;
            Color c = Main.rand.NextFromList(highlightC, brightC, c1, c2, c3);
            CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.2f));
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectileFromThis<PinkDiamondExplosion>(Projectile.Center, Vector2.Zero
                    , Projectile.damage, Projectile.knockBack, Main.rand.NextFloat(6.282f));

                if (VisualEffectSystem.HitEffect_Dusts)
                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                if (VisualEffectSystem.HitEffect_SpecialParticles)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        Color c = Main.rand.Next(3) switch
                        {
                            0 => new Color(193, 89, 138, 100),
                            1 => new Color(125, 33, 80, 100),
                            _ => new Color(193, 89, 138, 100),
                        };
                        Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                        Particle.NewParticle<Fog>(pos
                            , Vector2.UnitY.RotateByRandom(MathHelper.Pi - 0.4f, MathHelper.Pi + 0.4f)
                            , c, Main.rand.NextFloat(0.5f, 0.8f));
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            rand.X += 0.15f;

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(1.7f)
                , (float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f) + Projectile.whoAmI / 3f
                , highlightC, brightC, darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();
                    spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.15f, 0.45f, 0.55f);
        }
    }

    public class PinkDiamondExplosion : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public ref float BaseRot => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public List<PinkDiamondExplosionData> datas = new();
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public class PinkDiamondExplosionData
        {
            public bool active = true;
            public Vector2 position;
            public Vector2 velocity;
            public float rotation;
            public float timer;
            public float alpha;
            public float scale;
            public float width;
            public float height;

            public Vector2 Center
            {
                get
                {
                    return position + new Vector2(width / 2, height / 2);
                }
                set
                {
                    position = value - new Vector2(width / 2, height / 2);
                }
            }

            public PinkDiamondExplosionData(Vector2 center, float width, float height, float scale)
            {
                this.width = width;
                this.height = height;
                this.scale = scale;
                OnSpawn();
                Center = center;
            }

            public Rectangle GetRect() => new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height);

            public void OnSpawn()
            {
                width = scale * width;
                height = scale * height;
            }

            public void Update()
            {
                do
                {
                    if (timer < 8)
                    {
                        alpha += 1 / 8f;
                        velocity = rotation.ToRotationVector2() * 2 * timer / 8f;
                        break;
                    }

                    if (timer < 16)
                    {
                        alpha = 1;
                        break;
                    }

                    if (timer > 16)
                    {
                        velocity *= 0.98f;
                        alpha -= 1 / 10f;
                        if (alpha < 0.1f)
                            active = false;
                    }

                } while (false);

                if (timer % 6 == 0)
                    PinkDiamondProj.SpawnTriangleParticle(Center + Main.rand.NextVector2Circular(12, 12), velocity * Main.rand.NextFloat(0.2f, 0.4f));
                if (Main.rand.NextBool(15))
                {
                    Dust dust = Dust.NewDustPerfect(Center + Main.rand.NextVector2Circular(6, 6), DustID.PinkTorch, Vector2.Zero);
                    dust.noGravity = true;
                    dust.velocity = -velocity * Main.rand.NextFloat(0.2f, 0.4f);
                }

                timer++;
                velocity = Collision.TileCollision(position, velocity, (int)width, (int)height, true, true);
                velocity = velocity.RotatedBy(0.08f);
                rotation += 0.02f;
                position += velocity;
            }

            public void Draw(SpriteBatch spriteBatch, Texture2D mainTex, Vector2 origin)
            {
                Color c = Color.White * alpha;
                //spriteBatch.Draw(mainTex, Center, null, c * 0.5f, rotation + 1.57f, origin, scale * 1.2f, 0, 0);
                spriteBatch.Draw(mainTex, Center - velocity * 3, null, c * 0.3f, rotation + 1.57f, origin, scale * 0.8f, 0, 0);
                spriteBatch.Draw(mainTex, Center, null, c, rotation + 1.57f, origin, scale, 0, 0);
            }
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Timer < 20 && Timer % 2 == 0)
            {
                Player owner = Main.player[Projectile.owner];
                if (owner.CheckMana(1, true))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float factor = Timer / 20;
                        factor = Coralite.Instance.X2Smoother.Smoother(factor);

                        float rot = BaseRot + Timer * (MathHelper.TwoPi / 5 - factor * 0.3f);
                        Vector2 dir = rot.ToRotationVector2();
                        float length = 8 + factor * 70;

                        var data = new PinkDiamondExplosionData(Projectile.Center + dir * length, 30, 30, 0.5f + factor * 0.4f)
                        {
                            rotation = rot,
                        };

                        datas.Add(data);
                    }

                    owner.manaRegenDelay = 40;
                }
            }
            else if (datas.Count == 0)
            {
                Projectile.Kill();
                return;
            }

            Timer++;
            for (int i = datas.Count - 1; i >= 0; i--)
                datas[i].Update();

            datas.RemoveAll(data => !data.active);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = datas.Count - 1; i >= 0; i--)
            {
                if (targetHitbox.Intersects(datas[i].GetRect()))
                    return true;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rand.X += 0.15f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(1.7f)
                , (float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f) + Projectile.whoAmI / 3f
                , PinkDiamondProj.highlightC, PinkDiamondProj.brightC, PinkDiamondProj.darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();
                    Vector2 origin = mainTex.Size() / 2;
                    for (int i = datas.Count - 1; i >= 0; i--)
                        datas[i].Draw(spriteBatch, mainTex, origin);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.15f, 0.65f, 0.55f);

            return false;
        }
    }
}
