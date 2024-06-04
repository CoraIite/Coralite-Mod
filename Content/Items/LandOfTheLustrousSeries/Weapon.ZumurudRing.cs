using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Pink5, Item.sellPrice(0, 7));
            Item.SetWeaponValues(40, 4);
            Item.useTime = Item.useAnimation = 20;
            Item.mana = 8;

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
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, ZumurudProj.brightC, ZumurudProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Zumurud>()
                .AddIngredient(ItemID.BorealWood, 12)
                .AddIngredient(ItemID.FlowerPacketPink)
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

        public override void Initialize()
        {
            TargetPos = Owner.Center;
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

        }

        public override void Attack()
        {

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

        public static Asset<Texture2D> trailTex;

        public static Color highlightC = new Color(206, 248, 239);
        public static Color brightC = new Color(49, 230, 127);
        public static Color darkC = new Color(19, 112, 60);

        public override void Load()
        {
            if (Main.dedServ)
                return;

            trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "EdgeTrail");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            trailTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]==0)
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
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 1200, n => Collision.CanHit(Projectile, n), out _))
                    {
                        float angle = Main.rand.NextFloat(6.282f);
                        for (int i = 0; i < 4; i++)
                        {
                            Projectile.NewProjectileFromThis<SmallZumurudProj>(Projectile.Center, (angle + MathHelper.PiOver4 * i).ToRotationVector2() * 6
                                , Projectile.damage / 4, Projectile.knockBack / 4, Main.rand.NextFloat(-0.2f, 0.2f));
                        }
                    }

                    Projectile.Kill();
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();
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

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = trailTex.Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

            for (int i = 0; i < 16; i++)
            {
                float factor =i  / 16f;
                Vector2 Center = Projectile.oldPos[i];
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center - Main.screenPosition + normal * 10;
                Vector2 Bottom = Center - Main.screenPosition - normal * 10;

                var color = Color.Lerp(brightC,darkC,factor);//.MultiplyRGB(lightColor);
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

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, Vector2.One
                , (float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f) + Projectile.whoAmI / 3f
                , highlightC, brightC, darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();
                    spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                });
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
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
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
                        ExRot *= 0.96f;
                        if (Timer>60)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 1200, n => Collision.CanHit(Projectile, n), out NPC target))
                            {
                                State = 1;

                            }
                            else
                                Projectile.Kill();
                        }
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

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(ZumurudProj.darkC, 0.3f, 0.3f / 8, 0, 8, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }
}
