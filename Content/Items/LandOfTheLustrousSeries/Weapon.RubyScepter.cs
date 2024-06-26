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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class RubyScepter : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 9));
            Item.SetWeaponValues(60, 4);
            Item.useTime = Item.useAnimation = 34;
            Item.mana = 17;

            Item.shoot = ModContent.ProjectileType<RubyScepterProj>();
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
                    (proj.ModProjectile as RubyScepterProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.6f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.25f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(RubyProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(RubyProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(RubyProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, RubyProj.brightC, RubyProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.AvengerEmblem)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class RubyScepterProj : BaseGemWeaponProj<RubyScepter>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "RubyScepter";

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, RubyProj.brightC, RubyProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.6f, 0.1f, 0.1f));
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(Owner.direction * 32, 0);

            for (int i = 0; i < 8; i++)//检测头顶4个方块并尝试找到没有物块阻挡的那个
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

            if (AttackTime != 0)
            {
                Vector2 dir = Main.MouseWorld - Projectile.Center;

                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Owner.velocity.X / 40;
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                if (AttackTime == 1 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 dir2 = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);

                    Projectile.NewProjectileFromThis<RubyLaser>(Projectile.Center,
                           Vector2.Zero, Owner.GetWeaponDamage(Owner.HeldItem)
                           , Projectile.knockBack, Projectile.whoAmI, dir2.ToRotation(), RubyLaser.TotalAttackTime);

                    int howMany = Main.rand.NextFromList(1, 1, 1, 2, 2, 3);

                    for (int i = 0; i < howMany; i++)
                    {
                        Projectile.NewProjectileFromThis<RubyProj>(Projectile.Center,
                            dir2.RotatedBy((i % 2 == 0 ? -0.53f : 0.35f) + Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.NextFloat(3f, 13f)
                            , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, ai1: (Main.MouseWorld - Projectile.Center).ToRotation(), ai2: 35 + i * 35);
                    }

                    Helper.PlayPitched("Crystal/CrystalStrike", 0.4f, -0.2f, Projectile.Center);
                    SoundEngine.PlaySound(CoraliteSoundID.LaserGun_Item158, Projectile.Center);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        RubyProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.2f);

                AttackTime--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(RubyProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1, -0.785f, -1);
            Projectile.QuickDraw(lightColor, -0.785f);
            return false;
        }
    }

    /// <summary>
    /// 使用ai1传入角度,ai0传入主人
    /// </summary>
    public class RubyLaser : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "ExtraLaserFlow";

        public ref float Owner => ref Projectile.ai[0];
        public ref float LaserRotation => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float LaserHeight => ref Projectile.localAI[0];

        public Vector2 endPoint;
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public const int TotalAttackTime = 15 + delayTime;
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
            if (!CheckOwner(out Projectile owner))
                return;

            Projectile.Center = owner.Center + new Vector2(0, -16);
            LaserRotation = LaserRotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.04f);

            GetEndPoint(160);
            LaserAI();

            Projectile.UpdateFrameNormally(8, 19);
        }

        public bool CheckOwner(out Projectile projOwner)
        {
            projOwner = null;
            if (!Main.projectile.IndexInRange((int)Owner))
            {
                Projectile.Kill();
                return false;
            }

            projOwner = Main.projectile[(int)Owner];
            if (!projOwner.active || projOwner.owner != Projectile.owner || projOwner.type != ModContent.ProjectileType<RubyScepterProj>())
            {
                Projectile.Kill();
                return false;
            }

            return true;
        }

        public void GetEndPoint(int count)
        {
            for (int k = 0; k < count; k++)
            {
                Vector2 posCheck = Projectile.Center + Vector2.UnitX.RotatedBy(LaserRotation) * k * 8;

                if (Helper.PointInTile(posCheck) || k == count - 1)
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
            Projectile.damage = (int)(Projectile.damage * 0.85f);
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

    /// <summary>
    /// 使用ai1传入角度,ai2传入等待时间
    /// </summary>
    public class RubyProj : RubyLaser
    {
        public ref float State => ref Projectile.ai[0];

        public static Color highlightC = new Color(255, 164, 163);
        public static Color brightC = new Color(238, 51, 53);
        public static Color darkC = new Color(73, 10, 0);

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

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 20;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (State == 0)
            {
                Projectile.velocity *= 0.95f;
                Projectile.tileCollide = true;
                Timer--;
                Projectile.rotation += Projectile.velocity.X / 8;
                LaserRotation = LaserRotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.1f);

                if (Timer < 1)
                {
                    Player owner = Main.player[Projectile.owner];
                    if (owner.CheckMana(12, true))
                    {
                        Helper.PlayPitched("Crystal/CrystalStrike", 0.1f, 0.2f, Projectile.Center);

                        State = 1;
                        Timer = TotalAttackTime;
                        Projectile.tileCollide = false;
                        GetEndPoint(60);
                    }
                    else
                        Projectile.Kill();
                }
            }
            else
            {
                LaserRotation = LaserRotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.02f);
                GetEndPoint(60);
                LaserAI();
            }

            Projectile.UpdateFrameNormally(8, 19);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == 0)
                return false;
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.8f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadItem(ItemID.Ruby);
            Texture2D mainTex = TextureAssets.Item[ItemID.Ruby].Value;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation,
                mainTex.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }

        public override void DrawAdditive(SpriteBatch spriteBatch)
        {
            if (State != 0)
                base.DrawAdditive(spriteBatch);
        }
    }
}
