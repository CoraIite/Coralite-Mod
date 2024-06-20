using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class SapphireHairpin : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Yellow8, Item.sellPrice(0, 13));
            Item.SetWeaponValues(83, 4);
            Item.useTime = Item.useAnimation = 45;
            Item.mana = 30;

            Item.shoot = ModContent.ProjectileType<SapphireHairpinProj>();
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
                    (proj.ModProjectile as SapphireHairpinProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.8f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(SapphireProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SapphireProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SapphireProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, SapphireProj.brightC, SapphireProj.darkC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Sapphire)
                .AddIngredient(ItemID.MartianConduitPlating, 30)
                .AddIngredient(ItemID.FallenStar)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class SapphireHairpinProj : BaseGemWeaponProj<SapphireHairpin>
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
                Color c = Main.rand.NextFromList(Color.White, SapphireProj.brightC, SapphireProj.darkC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.7f));
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

                if (dir.Length() < 80)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 80;
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

                    Projectile.NewProjectileFromThis<SapphireProj>(Projectile.Center,
                           dir2 * Main.rand.NextFloat(5, 6), Owner.GetWeaponDamage(Owner.HeldItem)
                           , Projectile.knockBack);

                    Helper.PlayPitched("Crystal/GemShoot", 0.2f, -0.8f, Projectile.Center);
                    SoundEngine.PlaySound(CoraliteSoundID.CrystalSerpent_Item109, Projectile.Center);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        SapphireProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(SapphireProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);

            if (AttackTime > 0)
            {
                Vector2 pos = Projectile.Center - Main.screenPosition;
                float factor = AttackTime / Owner.itemTimeMax;
                Helper.DrawPrettyStarSparkle(1, 0, pos, SapphireProj.highlightC, SapphireProj.brightC
                    , factor, 0, 0.5f, 0.5f, 1
                    , 0, new Vector2(2f, 0.8f), Vector2.One / 3);
                Helper.DrawPrettyStarSparkle(1, 0, pos, SapphireProj.highlightC, SapphireProj.brightC
                    , factor, 0, 0.5f, 0.5f, 1
                    , 0, new Vector2(3f, 1f), Vector2.One / 3);
            }
            return false;
        }
    }

    public class SapphireProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(80, 150, 255);
        public static Color darkC = new Color(0, 0, 255);

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 18;
            Projectile.timeLeft = 1200;

            Projectile.extraUpdates = 2;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            switch (State)
            {
                default: Projectile.Kill(); break;
                case 0://穿墙射击
                    {
                        Timer++;

                        Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        float factor2 = Timer / (50 * Projectile.MaxUpdates);
                        float baseSpeed = Helper.Lerp(3.5f, 0.2f,factor2 );
                        for (int i = 0; i < 3; i++)
                        {
                            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly+Timer * 0.05f + i * MathHelper.PiOver2);
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Blue
                                , -dir.RotatedBy(factor) * Main.rand.NextFloat(baseSpeed, baseSpeed + 0.05f), Scale: Main.rand.NextFloat(0.1f, 0.7f));
                            d.noGravity = true;
                        }

                        Projectile.localAI[0] += (1 - factor2) * 0.6f;

                        if (Timer > 50 * Projectile.MaxUpdates)
                        {
                            State = 1;
                            Timer = 0;
                            Projectile.velocity *= 0.7f;
                        }

                        if (Projectile.timeLeft % 6 == 0)
                            SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                        if (Main.rand.NextBool(6))
                            Projectile.SpawnTrailDust(8f, DustID.BlueTorch, Main.rand.NextFloat(0.2f, 0.4f));

                        Timer++;
                    }
                    break;
                case 1://查找目标
                    {
                        Timer++;
                        Projectile.velocity *= 0.97f;

                        if (Timer > 30 && Timer % 10 == 0 && Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy(), out NPC target))
                        {
                            Target = target.whoAmI;
                            State = 2;
                            Timer = 0;
                            return;
                        }

                        if (Timer > 50 * Projectile.MaxUpdates)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
                case 2://追踪
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Projectile.Kill();
                            return;
                        }

                        float num481 = 8f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = target.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        float length2 = length;
                        if (length < 100f)
                            num481 = 6f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = (Projectile.velocity.X * 29f + dir.X) / 30f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 29f + dir.Y) / 30f;

                        Timer++;
                        if (Timer > 120 * Projectile.MaxUpdates || length2 < 32)
                        {
                            State = 3;
                            Projectile.velocity *= 0.2f;
                            Projectile.timeLeft = 120 * Projectile.MaxUpdates;
                            Timer = 0;

                            //生成彗星
                            Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFromList(-650, 650) + Main.rand.Next(-150, 150), -800);
                            Projectile.NewProjectileFromThis<SapphireComet>(pos, (Projectile.Center + new Vector2(0, -400) - pos).SafeNormalize(Vector2.Zero) * 8
                                , Main.player[Projectile.owner].GetWeaponDamage(Main.player[Projectile.owner].HeldItem), Projectile.knockBack, Projectile.whoAmI);
                        }
                    }
                    break;
                case 3://引导落星
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Projectile.Kill();
                            return;
                        }

                        float num481 = 1f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = target.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        float length2 = length;
                        if (length < 100f)
                            num481 = 2f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = (Projectile.velocity.X * 19f + dir.X) / 20f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 19f + dir.Y) / 20f;
                    }
                    break;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.5f));
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, Helper.NextVec2Dir(2, 8), Scale: Main.rand.NextFloat(1f, 2f));
                    d.noGravity = true;
                }
                for (int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Blue, Helper.NextVec2Dir(1, 5));
                }
            }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 6f));
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
            if (Projectile.damage < 5)
                Projectile.damage = 5;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadItem(ItemID.Sapphire);
            Texture2D mainTex = TextureAssets.Item[ItemID.Sapphire].Value;
            var origin1 = mainTex.Size() / 2;

            Vector2 pos = Projectile.Center - Main.screenPosition;
            Helper.DrawPrettyStarSparkle(1, 0, pos, highlightC, brightC
                , 0.5f, 0, 0.3f, 0.7f, 1
                , 0, new Vector2(2f, 0.8f), Vector2.One / 3);
            Helper.DrawPrettyStarSparkle(1, 0, pos, highlightC, brightC
                , MathF.Sin((int)Main.timeForVisualEffects * 0.4f) / 2 + 0.5f, 0, 0.3f, 0.7f, 1
                , 0, new Vector2(3f, 1f), Vector2.One / 3);

            rand -= Projectile.velocity / 10;
            Color c = lightColor;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 3f
                , highlightC, brightC, darkC, () =>
                {
                    Texture2D exTex = TextureAssets.Extra[98].Value;

                    Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);
                    var origin = exTex.Size() / 2;

                    for (int i = 0; i < 12; i++)
                        Main.spriteBatch.Draw(exTex, Projectile.oldPos[i] + toCenter, null,
                            Color.White * (0.4f - i * 0.4f / 12), Projectile.oldRot[i] + 1.57f, origin, 0.6f, 0, 0);

                    Main.spriteBatch.Draw(mainTex, Projectile.Center, null, c, Projectile.rotation + Projectile.localAI[0], origin1, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);

            return false;
        }
    }

    public class SapphireComet : ModProjectile, IDrawPrimitive, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        private VertexStrip _vertexStrip = new VertexStrip();
        private Trail trail;

        public ref float Owner => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];

        public static Asset<Texture2D> TrailTex;

        private Vector2[] oldPos2;

        public override void Load()
        {
            if (!Main.dedServ)
                TrailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "FadeTrail");
        }

        public override void Unload()
        {
            TrailTex = null;
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 40);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 1200;

            Projectile.penetrate = 3;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            const int TrailCount = 24;
            if (trail == null)
            {
                oldPos2 = new Vector2[TrailCount];
                _vertexStrip = new VertexStrip();
                for (int i = 0; i < TrailCount; i++)
                    oldPos2[i] = Projectile.Center;
                trail = new Trail(Main.graphics.GraphicsDevice, TrailCount, new NoTip(), factor => 54,
                     factor =>
                     {
                         return Color.Lerp(SapphireProj.darkC, SapphireProj.brightC, factor.X);
                     });
            }

            switch (State)
            {
                default:
                    {
                        if (!Owner.GetProjectileOwner(out Projectile owner))
                        {
                            Projectile.Kill();
                            return;
                        }

                        float num481 = 10f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = owner.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        float length2 = length;
                        if (length < 100f)
                            num481 = 13f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = (Projectile.velocity.X * 39f + dir.X) / 40f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 39f + dir.Y) / 40f;

                        if (length2 < 32)
                        {
                            owner.Kill();
                            State = 1;
                            Projectile.timeLeft = 60 * 2 * Projectile.MaxUpdates;
                            Projectile.tileCollide = true;
                            Projectile.StartAttack();

                            SoundStyle st = CoraliteSoundID.MeteorImpact_Item89;
                            st.Pitch =0.5f;
                            SoundEngine.PlaySound(st, Projectile.Center);
                            st = CoraliteSoundID.Crystal_Item101;
                            st.Pitch =-0.2f;
                            SoundEngine.PlaySound(st, Projectile.Center);
                            Helper.PlayPitched("Crystal/CrystalStrike", 0.2f, -0.2f, Projectile.Center);

                            float angle = Main.rand.NextFloat(6.282f);
                            float velocity = Main.rand.NextFloat(8, 11);
                            for (int i = 0; i < 6; i++)
                            {
                                Projectile.NewProjectileFromThis<SapphireSmallProj>(Projectile.Center
                                    , (angle + MathHelper.TwoPi / 6 * i).ToRotationVector2() * velocity, (int)(Projectile.damage *0.75f), Projectile.knockBack);
                            }
                        }
                    }
                    break;
                case 1:
                    {
                    }
                    break;
            }

            if (Projectile.timeLeft % 4 == 0)
                SapphireProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(8f, Main.rand.NextBool() ? DustID.UnusedWhiteBluePurple : DustID.BlueTorch, Main.rand.NextFloat(0.2f, 0.4f));

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.4f, 1f));

            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < TrailCount - 1; i++)
                oldPos2[i] = oldPos2[i + 1];
            oldPos2[^1] = Projectile.Center + Projectile.velocity;
            trail.Positions = oldPos2;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dir2 = Helper.NextVec2Dir();
                    SapphireProj.SpawnTriangleParticle(Projectile.Center + dir2 * Main.rand.NextFloat(6, 12), dir2 * Main.rand.NextFloat(0.5f, 3f));
                }

            if (!VisualEffectSystem.HitEffect_Dusts)
                return;

                for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.UnusedWhiteBluePurple, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 8),
                    Scale: Main.rand.NextFloat(1f, 2.5f));
               Dust d= Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.PurpleTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 4),
                    Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
            }

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.MushroomTorch, dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                        DustID.Firework_Blue, Helper.NextVec2Dir(1 + i * 2, 2 + i * 2),
                        Scale: Main.rand.NextFloat(0.3f, 0.7f));
                    d.noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explosion();
            return true;
        }

        public void Explosion()
        {
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.localAI[0] = 1;
            Projectile.StartAttack();
            Projectile.Resize(128, 128);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.Violet, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 2;
            return result; 
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(24f, 48f, progressOnStrip);

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPrimitives()
        {
            if (trail == null || Projectile.localAI[0] > 0)
                return;

            Effect effect = Filters.Scene["SimpleTrailNoHL"].GetShader().Shader;

            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
            effect.Parameters["sampleTexture"].SetValue(TrailTex.Value);
            trail?.Render(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition + Projectile.Size / 2);
            _vertexStrip.DrawTrail();

            Texture2D mainTex = TextureAssets.Extra[57].Value;
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            for (int i = 0; i < 4; i++)
                spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.PiOver2).ToRotationVector2() * 3
                    , null, new Color(255, 255, 255, 50), 0, origin, 0.7f, 0, 0);

            spriteBatch.Draw(mainTex, pos, null, Color.White, 0, origin, 0.7f, 0, 0);
        }
    }

    public class SapphireSmallProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 20;
            Projectile.timeLeft = 100;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;
            float factor = (60f - Projectile.ai[1]) / 60f;
            if (Projectile.ai[1] > 60f)
                Projectile.Kill();

            Projectile.velocity *= 0.98f;
            if (Projectile.ai[1] < 10)
            {
                if (Projectile.ai[1] == 9)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(2f);
                }
            }
            else if (Projectile.ai[1] % 20 == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(1.8);
            }

            for (int i = 0; i < 2; i++)
            {
                int num249 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1.1f);
                Dust dust2 = Main.dust[num249];
                dust2.position = (dust2.position + Projectile.Center) / 2f;
                dust2.noGravity = true;
                dust2.velocity *= 0.3f;
                dust2.scale *= factor;
            }

            for (int o = 0; o < 1; o++)
            {
                int num249 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomTorch, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 0.6f);
                Dust dust2 = Main.dust[num249];
                dust2.position = (dust2.position + Projectile.Center * 5f) / 6f;
                dust2.velocity *= 0.4f;
                dust2.noGravity = true;
                dust2.fadeIn = 0.9f * factor;
                dust2.scale *= factor;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -1;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -1;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.6f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int num187 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height
                    , DustID.BlueTorch, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 0, default(Color), 0.5f);
                Dust dust2;
                if (Main.rand.NextBool(3))
                {
                    Main.dust[num187].fadeIn = 0.75f + (float)Main.rand.Next(-10, 11) * 0.01f;
                    Main.dust[num187].scale = 0.25f + (float)Main.rand.Next(-10, 11) * 0.005f;
                    dust2 = Main.dust[num187];
                    dust2.type++;
                }
                else
                {
                    Main.dust[num187].scale = 1f + (float)Main.rand.Next(-10, 11) * 0.01f;
                }

                Main.dust[num187].noGravity = true;
                dust2 = Main.dust[num187];
                dust2.velocity *= 1.25f;
                dust2 = Main.dust[num187];
                dust2.velocity -= Projectile.oldVelocity / 10f;
            }
        }
    }
}
