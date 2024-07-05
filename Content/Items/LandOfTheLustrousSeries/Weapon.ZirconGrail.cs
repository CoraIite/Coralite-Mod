using Coralite.Content.Dusts;
using Coralite.Content.NPCs.Magike;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class ZirconGrail : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 24));
            Item.SetWeaponValues(155, 4, 6);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 12;

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
                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            AttackTime = Owner.itemTimeMax;


            Helper.PlayPitched("Crystal/CrystalShoot", 0.4f, 0, Projectile.Center);
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
                origin, Projectile.scale , 0, 0);
            return false;
        }
    }

    public class ZirconProj:ModProjectile,IDrawAdditive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "ExtraLaserFlow";

        public ref float BaseLength => ref Projectile.ai[0];
        public ref float LaserRotation => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float LaserHeight => ref Projectile.localAI[0];

        public Vector2 endPoint;
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(255, 179, 22);
        public static Color darkC = new Color(117, 55, 29);

        public const int TotalAttackTime = 60 + delayTime;
        public const int delayTime = 15;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.timeLeft = 400;
            Projectile.width = Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            LaserRotation = LaserRotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.04f);

            GetEndPoint(160);
            LaserAI();

            Projectile.UpdateFrameNormally(8, 19);
        }

        public void GetEndPoint(int count)
        {
            for (int k = 0; k < count; k++)
            {
                if (k == count - 1)
                {
                    Vector2 posCheck2 = Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * k * 16;
                    endPoint = posCheck2;
                    break;
                }

                if (k * 16 < BaseLength)
                    continue;

                Vector2 posCheck = Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * k * 16;

                if (Helper.PointInTile(posCheck))
                {
                    endPoint = posCheck;
                    break;
                }
            }
        }

        public virtual void LaserAI()
        {
            int width = (int)(Projectile.Center - endPoint).Length();
            Vector2 dir = Vector2.UnitX.RotatedBy(LaserRotation);
            Color color = RubyProj.brightC;

            do
            {
                float height = 16 * LaserHeight;
                float min = width / 140f;
                float max = width / 120f;

                for (int i = 0; i < width; i += 16)
                {
                    Lighting.AddLight(Projectile.position + Vector2.UnitX.RotatedBy(LaserRotation) * i, color.ToVector3() * height * 0.030f);
                    if (Main.rand.NextBool(50))
                    {
                        RubyProj.SpawnTriangleParticle(Projectile.Center + dir * i + Main.rand.NextVector2Circular(8, 8)
                            , dir * Main.rand.NextFloat(min, max));
                    }
                }

                if (Timer > delayTime)
                {
                    LaserHeight = Helper.Lerp(0, 1, 1 - (Timer - delayTime) / (TotalAttackTime - delayTime));

                    SpawnLaserParticle();
                    break;
                }

                if (Timer == delayTime / 2)
                {
                    for (int i = 0; i < width - 128; i += 24)
                    {
                        Vector2 pos = Projectile.Center + dir * i + Main.rand.NextVector2Circular(8, 8);
                        if (Main.rand.NextBool(4))
                        {
                            if (Main.rand.NextBool())
                                Dust.NewDustPerfect(pos, ModContent.DustType<GlowBall>(),
                                    dir * Main.rand.NextFloat(width / 160f), 0, color, 0.35f);
                            else
                                RubyProj.SpawnTriangleParticle(pos, dir * Main.rand.NextFloat(width / 160f));
                        }
                    }
                }

                LaserHeight -= 1f / delayTime;

            } while (false);

            Timer--;
            if (Timer < 1)
                Projectile.Kill();
        }

        public void SpawnLaserParticle()
        {
            if (VisualEffectSystem.HitEffect_Lightning && Main.rand.NextBool(7))
            {
                RubyProj.SpawnTriangleParticle(endPoint, Helper.NextVec2Dir(0.5f, 2f));
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Timer > delayTime)
            {
                float a = 0;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPoint, 16, ref a);
            }

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public virtual void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D laserTex = Projectile.GetTexture();
            Texture2D flowTex = CrystalLaser.LaserBodyTex.Value;

            rand += LaserRotation.ToRotationVector2() * 3;
            Color color = RubyProj.darkC;

            float height = LaserHeight * laserTex.Height / 5f;
            int width = (int)(Projectile.Center - endPoint).Length();   //这个就是激光长度

            Vector2 startPos = Projectile.Center;
            Vector2 endPos = endPoint - Main.screenPosition;

            var laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height));
            var flowTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.55f));

            var laserSource = new Rectangle((int)(Projectile.timeLeft / 20f * laserTex.Width), 0, laserTex.Width, laserTex.Height);
            var flowSource = new Rectangle((int)(Projectile.timeLeft / 45f * flowTex.Width), 0, flowTex.Width, flowTex.Height);

            var origin = new Vector2(0, laserTex.Height / 2);
            var origin2 = new Vector2(0, flowTex.Height / 2);

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, Vector2.One * 0.8f
                , (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 3f
                , RubyProj.highlightC, RubyProj.brightC, RubyProj.darkC, () =>
                {
                    //绘制流动效果
                    spriteBatch.Draw(laserTex, laserTarget, laserSource, Color.White, LaserRotation, origin, 0, 0);
                    spriteBatch.Draw(flowTex, flowTarget, flowSource, Color.White * 0.2f, LaserRotation, origin2, 0, 0);
                }, sb =>
                {
                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.35f, 0f);

            //绘制主体光束
            Texture2D bodyTex = CrystalLaser.LaserBodyTex.Value;

            color = RubyProj.brightC;

            startPos = Projectile.Center - Main.screenPosition;

            laserTarget = new Rectangle((int)startPos.X, (int)startPos.Y, width, (int)(height * 0.65f));
            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);
            spriteBatch.Draw(bodyTex, laserTarget, laserSource, color, LaserRotation, new Vector2(0, bodyTex.Height / 2), 0, 0);

            //绘制用于遮盖首尾的亮光
            Texture2D glowTex = CrystalLaser.GlowTex.Value;
            Texture2D starTex = CrystalLaser.StarTex.Value;

            float factor = Timer / 10;
            float fadeFactor = Math.Abs(factor - MathF.Truncate(factor));
            float rot = ((int)factor) * MathHelper.TwoPi / 3;

            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(glowTex, endPos, null, color * (height * 0.2f), LaserRotation + i * 0.785f, glowTex.Size() / 2, height * 0.05f * new Vector2(0.5f, 0.1f), 0, 0);
            }

            spriteBatch.Draw(glowTex, startPos, null, color * (height * 0.06f), 0, glowTex.Size() / 2, 0.5f, 0, 0);

            for (int i = 0; i < 3; i++)
            {
                Helper.DrawPrettyStarSparkle(1, 0, startPos, color, Color.White
                , fadeFactor, 0, 0.25f, 0.75f, 1, rot, new Vector2(4f, 2f), Vector2.One);
            }
        }
    }
}
