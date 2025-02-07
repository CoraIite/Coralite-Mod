using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
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
    public class ZumurudRing : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightRed4, Item.sellPrice(0, 7));
            Item.SetWeaponValues(36, 4);
            Item.useTime = Item.useAnimation = 24;
            Item.mana = 14;

            Item.shoot = ModContent.ProjectileType<ZumurudRingProj>();
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
                    (proj.ModProjectile as ZumurudRingProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.4f, 1f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.45f);
                effect.Parameters["addC"].SetValue(0.35f);
                effect.Parameters["highlightC"].SetValue(ZumurudProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(ZumurudProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(ZumurudProj.darkC.ToVector4());
            }, 0.2f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f, 1.75f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.45f);
                effect.Parameters["addC"].SetValue(0.35f);
                effect.Parameters["highlightC"].SetValue(ZumurudProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(ZumurudProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(ZumurudProj.darkC.ToVector4());
            }, extraSize: new Point(40, 4));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, ZumurudProj.brightC, ZumurudProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Zumurud>()
                .AddIngredient(ItemID.OrichalcumBar, 8)
                .AddIngredient(ItemID.Emerald)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient<Zumurud>()
                .AddIngredient(ItemID.MythrilBar, 8)
                .AddIngredient(ItemID.Emerald)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class ZumurudRingProj : BaseGemWeaponProj<ZumurudRing>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, ZumurudProj.brightC, ZumurudProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.5f, 0.2f));
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
                Projectile.rotation = Helper.Lerp(0, MathHelper.Pi, Coralite.Instance.SqrtSmoother.Smoother(1 - (AttackTime / Owner.itemTimeMax)));

                if (AttackTime == 1 && Projectile.IsOwnedByLocalPlayer())
                {
                    Projectile.NewProjectileFromThis<ZumurudProj>(Projectile.Center,
                        (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 9.5f, Owner.GetWeaponDamage(Item), Projectile.knockBack);

                    Helper.PlayPitched("Crystal/CrystalBling", 0.4f, -0.35f, Projectile.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        ZumurudProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
            else
            {
                Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.2f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(ZumurudProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class ZumurudProj : ModProjectile, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "OctagonProj1";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public ref float Hit => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Rot2 => ref Projectile.ai[2];

        public static Color highlightC = new(206, 248, 239);
        public static Color brightC = new(49, 230, 127);
        public static Color darkC = new(19, 112, 60);

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.InitOldPosCache(16);
                Projectile.InitOldRotCache(16);
                Projectile.localAI[0] = 1;
            }

            if (Hit == 1)
            {
                Projectile.velocity *= 0.98f;
                Rot2 += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 10;

                Timer++;
                if (Timer > 120)
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out _))
                    {
                        float angle = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileFromThis<SmallZumurudProj>(Projectile.Center, (angle + (MathHelper.PiOver2 * i)).ToRotationVector2() * 3
                                , (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4, Main.rand.NextFloat(-0.3f, 0.3f));
                        }
                    }

                    Projectile.Kill();
                }
            }

            if (Projectile.timeLeft % 4 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(6))
                Projectile.SpawnTrailDust(8f, DustID.GreenTorch, Main.rand.NextFloat(0.2f, 0.4f));

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit = 1;
            Projectile.velocity = Projectile.velocity.RotateByRandom(MathHelper.Pi - 0.5f, MathHelper.Pi + 0.5f) * Main.rand.NextFloat(0.6f, 0.8f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Hit = 1;

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.8f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.oldPos.Length < 16)
                return false;

            Texture2D Texture = CoraliteAssets.Trail.EdgeA.Value;
            List<CustomVertexInfo> bars = new();

            for (int i = 0; i < 16; i++)
            {
                float factor = i / 16f;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + (normal * 10);
                Vector2 Bottom = Center - Main.screenPosition - (normal * 10);

                var color = Color.Lerp(brightC, darkC, factor);//.MultiplyRGB(lightColor);
                color.A /= 2;
                bars.Add(new(Top, color, new Vector3(factor, 0, 1)));
                bars.Add(new(Bottom, color, new Vector3(factor, 1, 1)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            return false;
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            rand.X += 0.15f;

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.4f, 1f)
                , ((float)Main.timeForVisualEffects * (Main.gamePaused ? 0.02f : 0.01f)) + (Projectile.whoAmI / 3f)
                , highlightC, brightC, darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();
                    spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, addC: 0.35f);
        }
    }

    public class SmallZumurudProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "SmallZumurud";

        public ref float ExRot => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0:
                    {
                        Timer++;
                        Projectile.rotation += ExRot;
                        ExRot *= 0.99f;
                        Projectile.velocity *= 0.98f;
                        if (Timer > 60)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                            {
                                State = 1;
                                Projectile.velocity = (target.Center + (target.velocity * 7) - Projectile.Center).SafeNormalize(Vector2.Zero) * 9.5f;
                                for (int i = 0; i < 4; i++)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Helper.NextVec2Dir(0.5f, 2), Scale: Main.rand.NextFloat(1f, 1.5f));
                                    d.noGravity = true;
                                }

                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 dir = Helper.NextVec2Dir();
                                    ZumurudProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 2f));
                                }
                            }
                            else
                                Projectile.Kill();
                        }
                    }
                    break;
                case 1:
                    {
                        Projectile.rotation += ExRot;
                        if (Projectile.timeLeft % 6 == 0)
                            ZumurudProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(4, 4), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                        if (Main.rand.NextBool(10))
                            Projectile.SpawnTrailDust(8f, DustID.GreenTorch, Main.rand.NextFloat(0.2f, 0.4f));
                    }
                    break;
            }
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
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    ZumurudProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(ZumurudProj.darkC, 0.3f, 0.3f / 12, 0, 12, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }
}
