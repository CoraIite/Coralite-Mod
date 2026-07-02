using Coralite.Content.DamageClasses;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineShield : BaseFlyingShieldItem<CrystallineShieldGuard>
    {
        public CrystallineShield() : base(Item.sellPrice(0, 2, 50), ModContent.RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeSeries2Item)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 15;
            Item.shoot = ModContent.ProjectileType<CrystallineShieldProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 16;
            Item.damage = 54;
            Item.DamageType = MagikeDamage.Instance;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CrystallineEngram>()
                .AddTile<SkarnCutterTile>()
                .Register();
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            int i = 0;
            if (MagikeHelper.TryCosumeMagike(15, Item, player))
                i = 1;

            Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), velocity, type, damage, knockback, player.whoAmI, ai2:i);
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineShieldProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float SPAttack => ref Projectile.ai[2];
        public ref float SPProjIndex => ref Projectile.localAI[2];

        [VaultLoaden("{@classPath}" + "CrystallineSentinelGradientBlack")]
        public static ATex GradientTextureBlack { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 42;
            Projectile.DamageType = MagikeDamage.Instance;
        }

        public override bool PreUpdate()
        {
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() * 0.4f);

            return base.PreUpdate();
        }

        public override void SetOtherValues()
        {
            flyingTime = 24;
            backTime = 22;
            backSpeed = 17;
            trailCachesLength = 10;
            trailWidth = 30 / 2;

            if (Projectile.IsOwnedByLocalPlayer()&& SPAttack==1)
            {
                SPProjIndex = Projectile.NewProjectileFromThis<CrystallineShieldExpand>(Projectile.Center, Vector2.Zero, (int)(Projectile.damage*1.6f), Projectile.knockBack, ai2: Projectile.whoAmI);
            }
        }

        public override void TurnToBack()
        {
            if (Projectile.IsOwnedByLocalPlayer() && SPAttack == 1 && SPProjIndex.GetProjectileOwner<CrystallineShieldExpand>(out Projectile p))
                (p.ModProjectile as CrystallineShieldExpand).TurnToFade();

            base.TurnToBack();
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            var pos = Projectile.Center - Main.screenPosition;

            Rectangle frameBox = mainTex.Frame(2, 1, 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, Projectile.rotation - 1.57f + extraRotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
        }

        public override void DrawTrails(Color lightColor)
        {
            CoraliteSystem.InitBars();

            int length = trailCachesLength;
            for (int i = 0; i < length; i++)
            {
                float factor = i / (float)length;

                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                float width = WidthFunction(factor);
                Vector2 top = Projectile.oldPos[i] + normal * width;
                Vector2 bottom = Projectile.oldPos[i] - normal * width;
                Color color = ColorFunction(factor);

                CoraliteSystem.Vertexes.Add(new(top, color, new Vector3(factor, 0, 0)));
                CoraliteSystem.Vertexes.Add(new(bottom, color, new Vector3(factor, 1, 0)));
            }

            if (CoraliteSystem.Vertexes.Count > 2)
            {
                Effect effect = ShaderLoader.GetShader("TurbulenceArrow");

                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());

                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
                effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
                effect.Parameters["udissolveS"].SetValue(0.5f);
                effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Laser.Body.Value);
                effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.AirFlow2.Value);
                effect.Parameters["uGradient"].SetValue(GradientTextureBlack.Value);
                effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.VanillaFlowA.Value);
                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    var arr = CoraliteSystem.Vertexes.ToArray();
                    Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, arr, 0, CoraliteSystem.Vertexes.Count - 2);
                }
            }
        }

        public static float WidthFunction(float factor) => Utils.Remap(factor, 0, 1, 2, 20);

        public static Color ColorFunction(float factor) => Color.Lerp(Color.Black, Color.White, factor) * Utils.Remap(factor, 0, 1, 0, 1);
    }

    public class CrystallineShieldGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 44;
            Projectile.height = 54;
            Projectile.DamageType = MagikeDamage.Instance;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.15f;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(3, 2, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            frameBox = mainTex.Frame(3, 2, 0, 1);
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, lightColor, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 2, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            frameBox = mainTex.Frame(3, 2, 1, 1);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 3), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //最上
            frameBox = mainTex.Frame(3, 2, 2, 0);

            Main.spriteBatch.Draw(mainTex, pos + (dir * 15), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 20), frameBox, lightColor, rotation, origin2, scale, effect, 0);
            frameBox = mainTex.Frame(3, 2, 2, 1);


            Main.spriteBatch.Draw(mainTex, pos + (dir * 9), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    [VaultLoaden(AssetDirectory.CrystallineNPCs)]
    public class CrystallineShieldExpand : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle")]
        public static ATex ShieldParticle { get; set; }

        [VaultLoaden("{@classPath}" + "CrystallineSentinelShieldParticle_Glow")]
        public static ATex ShieldParticleGlow { get; set; }
        public ref float Timer => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float ProjOwner => ref Projectile.ai[2];

        public ref float ScaleTimer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = MagikeDamage.Instance;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            ProjOwner.GetProjectileOwner<CrystallineShieldProj>(out Projectile proj, () =>
            {
                if (State != 2)
                {
                    State = 2;
                    Timer = 0;
                    ProjOwner = -1;
                }
            });

            switch (State)
            {
                default:
                case 0://出现
                    {
                        if (proj != null)
                            Projectile.Center = proj.Center;

                        ScaleTimer++;
                        if (Timer > 12)
                        {
                            ScaleTimer = 12;
                            Timer = 0;
                            State = 1;
                        }
                    }
                    break;
                case 1://保持
                    {
                        if (proj != null)
                            Projectile.Center = proj.Center;

                        if (ScaleTimer < 12)
                        {
                            ScaleTimer++;
                        }
                    }
                    break;
                case 2://消失
                    {
                        if (ScaleTimer > 0)
                        {
                            ScaleTimer--;
                        }

                        if (ScaleTimer < 1 || Timer > 11)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }

            float scale = ScaleTimer / 12f * 2;
            {
                int width = (int)(80 * scale);
                Projectile.Resize(width, width);
            }

            if (Timer % 2 == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 position = Projectile.Center + Helper.NextVec2Dir(1, 3);
                    Vector2 vel = Projectile.DirectionTo(position).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(1, 2);
                    var prt = PRTLoader.NewParticle<CrystallineFlashParticle>(position - vel * 5, vel * 0.75f + Projectile.velocity * 0.25f);
                    prt.Scale /= 2f;
                }
            }

            Timer++;
        }

        public void TurnToFade()
        {
            Projectile.netUpdate = true;
            ScaleTimer = 12;
            Timer = 0;
            State = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.95f);
        }

        public override void OnKill(int timeLeft)
        {
            int prtCount = 5;
            for (int i = 0; i < prtCount; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 12);
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0, 3.5f);
                var prt = PRTLoader.NewParticle<CrystallineFragmentParticle>(pos, vel);
                prt.Scale = Main.rand.NextFloat(0.4f, 1f);
            }

            for (int i = 0; i < 3 * 3; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(16);
                Vector2 vel = Projectile.rotation.ToRotationVector2().RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 3);
                PRTLoader.NewParticle<CrystallineFlashParticle>(position, vel * 0.75f);
            }

            Helper.PlayPitchedVariants(AssetDirectory.Sounds.Crystalline + "Sentinel_Explosion", 0.2f, 0, 0, 2, Projectile.Center);
        }

        public override bool PreDraw(ref Color drawColor)
        {
            DrawShard();

            //Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, $"{CounterFactor}", NPC.position.X - Main.screenPosition.X, NPC.position.Y - Main.screenPosition.Y, Color.White, Color.Black, Vector2.Zero);
            return false;
        }

        public void DrawShard()
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            const float maxShardCount = 8f;
            float fadeinFactor = Utils.Remap(ScaleTimer, 0, 12, 0f, 1f);
            float visualSpeed = /*Utils.Remap(factor, 0f, 1f, 0.05f, 0.01f);*/0.07f;

            for (int i = 0; i < 8; i++)
            {
                float factor = (i + 1f) / (maxShardCount + 1);
                float rotSpeed = Utils.Remap(factor, 0f, 1f, 0.07f, 0.02f);
                float maxRadius = 20 + 70 * fadeinFactor;
                float radius = Helper.SqrtEase(factor) * maxRadius;
                float dir = (float)(-ScaleTimer * rotSpeed + i * 1214f - Main.timeForVisualEffects * visualSpeed);
                Vector2 targetPos = Projectile.Center + dir.ToRotationVector2() * radius - Main.screenPosition;

                int trailLength = (int)(MathHelper.TwoPi * radius / 16f);
                Vector2 scale = new(0.5f, 0.65f);
                float iTimeFactor = MathF.Sin((float)(Main.timeForVisualEffects * 0.04f + i * 91208)) * 0.5f + 0.5f;
                for (int j = 0; j < trailLength; j++)
                {
                    float trailFactor = j / (float)trailLength;
                    float trailDir = dir + MathHelper.Pi * 2f / 3f * trailFactor;
                    Vector2 trailPos = Projectile.Center + trailDir.ToRotationVector2() * radius - Main.screenPosition;
                    float alpha = Utils.Remap(trailFactor, 0, 1f, 1, 0f) * 0.35f * fadeinFactor * iTimeFactor;
                    Main.spriteBatch.Draw(star, trailPos, null, Color.Violet with { A = 20 } * alpha, trailDir, star.Size() / 2, scale, 0, 0);
                }

                var frameBox = ShieldParticle.Frame(1, 13, 0, (Projectile.whoAmI * 12901 + i * 109) % 13);

                float shieldScale = 0.75f;
                Main.spriteBatch.Draw(ShieldParticle.Value, targetPos, frameBox, Color.White * fadeinFactor * 0.5f , dir + Projectile.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
                Main.spriteBatch.Draw(ShieldParticleGlow.Value, targetPos, frameBox, Color.White * fadeinFactor , dir + Projectile.whoAmI * 634f, frameBox.Size() / 2, shieldScale, 0, 0);
            }
        }
    }
}
