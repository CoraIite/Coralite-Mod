using Coralite.Content.Items.Icicle;
using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using InnoVault.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class AquamarineBracelet : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Orange3, Item.sellPrice(0, 3));
            Item.SetWeaponValues(20, 4);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 7;

            Item.shoot = ModContent.ProjectileType<AquamarineBraceletProj>();
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
                    (proj.ModProjectile as AquamarineBraceletProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue((AquamarineProj.brightC * 1.3f).ToVector4());
                effect.Parameters["brightC"].SetValue(AquamarineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(130, 130, 255).ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue((AquamarineProj.brightC * 1.3f).ToVector4());
                effect.Parameters["brightC"].SetValue(AquamarineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(50, 50, 160).ToVector4());
            }, extraSize: new Point(35, 2));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, AquamarineProj.brightC, AquamarineProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Aquamarine>()
                .AddIngredient(ItemID.ShadowScale, 12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient<Aquamarine>()
                .AddIngredient(ItemID.TissueSample, 12)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile<MagicCraftStation>()
                .Register();

            CreateRecipe()
                .AddIngredient<Aquamarine>()
                .AddIngredient<IcicleScale>(7)
                .AddIngredient(ItemID.WaterBucket)
                .AddTile<MagicCraftStation>()
                .Register();

            if (ModLoader.TryGetMod(CoraliteCrossMod.Thorium, out Mod thorium))
            {
                int thoriumAquamarine = thorium.Find<ModItem>("Aquamarine").Type;

                CreateRecipe()
                    .AddIngredient(thoriumAquamarine)
                    .AddIngredient(ItemID.ShadowScale, 12)
                    .AddIngredient(ItemID.WaterBucket)
                    .AddTile<MagicCraftStation>()
                    .Register();

                CreateRecipe()
                    .AddIngredient(thoriumAquamarine)
                    .AddIngredient(ItemID.TissueSample, 12)
                    .AddIngredient(ItemID.WaterBucket)
                    .AddTile<MagicCraftStation>()
                    .Register();

                CreateRecipe()
                    .AddIngredient(thoriumAquamarine)
                    .AddIngredient<IcicleScale>(7)
                    .AddIngredient(ItemID.WaterBucket)
                    .AddTile<MagicCraftStation>()
                    .Register();
            }
        }
    }

    public class AquamarineBraceletProj : BaseGemWeaponProj<AquamarineBracelet>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, AquamarineProj.brightC, AquamarineProj.highlightC);
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
            Vector2 idlePos = Owner.Center + new Vector2(DirSign * 48, 0);
            for (int i = 0; i < 8; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
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
                Vector2 dir = Main.MouseWorld - Projectile.Center;
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
            Lighting.AddLight(Projectile.Center, AquamarineProj.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                if (AttackTime % 7 == 0)
                {
                    Color c = Main.rand.NextFromList(AquamarineProj.brightC, AquamarineProj.highlightC, Main.DiscoColor);
                    LightLine ll = LightLine.Spwan(Projectile.Center + ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 10), Vector2.Zero, c,
                       null, Main.rand.NextFloat(0.1f, 0.4f), Main.rand.NextFloat(0.1f, 0.4f));
                    if (ll != null)
                    {
                        ll.fadeTime = Main.rand.Next(15, 25);
                        ll.center = () => Projectile.Center + ((Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 10);
                        ll.scaleY = Main.rand.NextFloat(0.2f, 0.6f);
                    }
                }

                Projectile.rotation = MathF.Sin((1 - (AttackTime / Owner.itemTimeMax)) * MathHelper.TwoPi) * 0.5f;
                if ((int)AttackTime % (Owner.itemTimeMax / 3) == 0 && Owner.CheckMana(Owner.HeldItem.mana, true))
                {
                    Owner.manaRegenDelay = 40;

                    float angle = (Main.rand.NextFromList(-1, 1) * 0.35f) + Main.rand.NextFloat(-0.5f, 0.5f);
                    Projectile.NewProjectileFromThis<AquamarineProj>(Projectile.Center
                        , Vector2.UnitY.RotatedBy(angle) * 8, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                    for (int i = 0; i < 4; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        AquamarineProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

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
            var origin = new Vector2(mainTex.Width / 2, 0);
            Vector2 toCenter = new(Projectile.width / 2, 0);

            for (int i = 0; i < 4; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    new Color(251, 100, 152) * (0.3f - (i * 0.3f / 4)), Projectile.oldRot[i] + 0, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Top - Main.screenPosition, null, lightColor, Projectile.rotation,
                origin, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class AquamarineProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "WaterDropProj1";

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float HitTileCount => ref Projectile.localAI[0];

        public static Color highlightC = new(168, 248, 249);
        public static Color brightC = new(71, 235, 250);
        public static Color darkC = new(24, 27, 81);

        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 18;
        }

        public override void AI()
        {
            if (trail == null)
            {
                const int maxPoint = 12;
                trail ??= new Trail(Main.graphics.GraphicsDevice, maxPoint, new EmptyMeshGenerator()
                    , factor => Helper.Lerp(2, 13, factor),
                      factor =>
                      {
                          return Color.Lerp(new Color(0, 0, 0, 0), Color.White * 0.65f, factor.X);
                      });

                Projectile.InitOldPosCache(maxPoint);
            }

            switch (State)
            {
                default:
                case 0://刚刚生成
                    Spawn();
                    break;
                case 1://追踪
                    Chase();
                    break;
                case 2:
                    Timer++;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    if (Timer>30)
                        Projectile.Kill();
                    break;
            }

            if (Projectile.timeLeft % 3 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(5))
                Projectile.SpawnTrailDust(8f, DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.4f));

            Projectile.UpdateFrameNormally(8, 19);
            Projectile.UpdateOldPosCache();
            trail.TrailPositions = Projectile.oldPos;
        }

        public void Spawn()
        {
            const int ChaseTime = 10;
            const int MaxTime = ChaseTime + 60;

            float angle = Projectile.velocity.ToRotation();
            float speed = Projectile.velocity.Length();

            angle = angle.AngleTowards(-1.57f, Coralite.Instance.X2Smoother.Smoother(Math.Clamp(Timer / 40, 0, 1)));

            if (Timer > ChaseTime)//开始追踪阶段
            {
                speed *= 0.97f;
                if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                {
                    Target = target.whoAmI;
                    State = 1;
                    Projectile.timeLeft = 600;
                    Projectile.extraUpdates = 1;
                }
            }

            Projectile.velocity = angle.ToRotationVector2() * speed;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer > MaxTime)
                Projectile.Kill();

            Timer++;
        }

        public void Chase()
        {
            if (!Target.GetNPCOwner(out NPC target))
            {
                Timer = 0;
                State = 2;

                return;
            }

            float num481 = 12f;
            Vector2 center = Projectile.Center;
            Vector2 targetCenter = target.Center;
            Vector2 dir = targetCenter - center;
            float length = dir.Length();
            if (length < 100f)
                num481 = 10f;

            length = num481 / length;
            dir *= length;
            Projectile.velocity.X = ((Projectile.velocity.X * 19f) + dir.X) / 20f;
            Projectile.velocity.Y = ((Projectile.velocity.Y * 19f) + dir.Y) / 20f;
            Projectile.rotation = Projectile.velocity.ToRotation();
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
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            HitTileCount++;
            if (HitTileCount > 3)
                Projectile.Kill();

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.8f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.8f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            Effect effect = Filters.Scene["CrystalTrail"].GetShader().Shader;

            Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

            effect.Parameters["noiseTexture"].SetValue(noiseTex);
            effect.Parameters["TrailTexture"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaser").Value);
            effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
            effect.Parameters["basePos"].SetValue((Projectile.Center - Main.screenPosition + rand) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * (Main.gamePaused ? 0.02f : 0.01f));
            effect.Parameters["lightRange"].SetValue(0.2f);
            effect.Parameters["lightLimit"].SetValue(0.25f);
            effect.Parameters["addC"].SetValue(0.65f);
            effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(darkC.ToVector4());

            trail.DrawTrail(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            rand.X += 0.15f;

            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , ((float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f)) + (Projectile.whoAmI / 3f)
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
}
