using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PinkDiamondRose : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 1));
            Item.SetWeaponValues(27, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 8;

            Item.shoot = ModContent.ProjectileType<AquamarineBraceletProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
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
    }

    public class PinkDiamondRoseProj : BaseGemWeaponProj<PinkDiamondRose>
    {
        private ParticleGroup group;
        private Vector2 offset;

        public override void BeforeMove()
        {
            group ??= new ParticleGroup();
            if ((int)Main.timeForVisualEffects % 20 == 0)
            {
                if (AttackTime < 1)
                {
                    Color c = Main.rand.Next(3) switch
                    {
                        0 => new Color(193, 89, 138, 100),
                        1 => new Color(125, 33, 80, 100),
                        _ => new Color(193, 89, 138, 100),
                    };
                    Vector2 pos = Projectile.Center +
                        (Projectile.rotation - 1.57f).ToRotationVector2() * Main.rand.NextFloat(24, 32);
                    group.Add(Particle.NewPawticleInstance<Fog>(pos
                        , Vector2.UnitY.RotateByRandom(MathHelper.Pi - 0.4f, MathHelper.Pi + 0.4f)
                        , c, Main.rand.NextFloat(0.2f, 0.3f)));

                    Dust d = Dust.NewDustPerfect(pos, DustID.PinkTorch, Helper.NextVec2Dir(1f, 3f));
                    d.noGravity = true;
                }

                if (Main.rand.NextBool(2))
                {
                    float length = Main.rand.NextFloat(48, 64);
                    Color c2 = Main.rand.NextFromList(Color.White, AmethystLaser.brightC, AmethystLaser.highlightC);
                    var cs = CrystalShine.New(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                         , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c2);
                    cs.follow = () => Projectile.position - Projectile.oldPos[1];
                    cs.TrailCount = 3;
                    cs.fadeTime = Main.rand.Next(40, 70);
                    cs.shineRange = 12;
                    group.Add(cs);
                }
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
                    offset *= 0.98f;
                }
                else
                {
                    Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.2f);
                    if (AttackTime == 1)
                    {
                        Vector2 dir = Projectile.rotation.ToRotationVector2();
                        offset = -dir * 64;

                        Projectile.NewProjectileFromThis<PinkDiamondProj>(Projectile.Center + dir * 12, dir * 12
                            , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                        for (int i = 0; i < 8; i++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }

                        for (int i = 0; i < 5; i++)
                            PinkDiamondProj.SpawnTriangleParticle(Projectile.Center + dir.RotateByRandom(-0.5f,0.5f) * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));

                    }
                }

                AttackTime--;
                return;
            }

            Projectile.rotation = Projectile.rotation.AngleLerp(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.4f, 0.6f);
        }
    }

    public class PinkDiamondProj : ModProjectile, IDrawNonPremultiplied
    {
        public static Color highlightC = Color.White;
        public static Color brightC = new Color(193, 89, 138);
        public static Color darkC = new Color(125, 33, 80);

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            
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
            CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.3f));
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {

            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            rand.X += 0.15f;

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f) + Projectile.whoAmI / 3f
                , highlightC, brightC, darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();
                    spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.65f, 0.5f);

        }
    }

    public class PinkDiamondExplosion
    {
        public List<PinkDiamondExplosionData> datas;

        public class PinkDiamondExplosionData
        {
            public bool active = true;
            public Vector2 position ;
            public Vector2 velocity;
            public float rotation;
            public float timer;
            public float alpha;
            public float scale;
            public float width;
            public float height;
            public int penetrate;

            public PinkDiamondExplosionData(Vector2 center, float width, float height, int penetrate, float scale)
            {
                this.width = width;
                this.height = height;
                this.penetrate = penetrate;
                this.scale=scale;
                OnSpawn();
                Center = center;
            }

            public void OnSpawn()
            {
                width = scale * width;
                height = scale * height;
            }

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


        }
    }
}
