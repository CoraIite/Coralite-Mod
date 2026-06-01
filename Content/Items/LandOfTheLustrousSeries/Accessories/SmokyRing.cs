using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Content.Prefixes.GemWeaponPrefixes;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries.Accessories
{
    [PlayerEffect]
    public class SmokyRing : BaseGemWeapon, IHookPlayerShoot
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Blue1, Item.sellPrice(0, 1));
            Item.SetWeaponValues(22, 4);

            Item.accessory = true;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            int prefix = 0;
            var wr = new WeightedRandom<int>(rand);

            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Accessory))
                wr.Add(pre, 1);
            //foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.Magic))
            //    wr.Add(pre, 1);
            foreach (int pre in Item.GetVanillaPrefixes(PrefixCategory.AnyWeapon))
                wr.Add(pre, 1);

            float w = 0.5f;
            if (Main.LocalPlayer.GetModPlayer<CoralitePlayer>().HasEffect(nameof(EightsquareHand)))
                w = 3f;

            wr.Add(ModContent.PrefixType<VibrantAccessory>(), w);

            for (int i = 0; i < 50; i++)
                prefix = wr.Get();

            return prefix;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.ShootHooks.Add(this);
                cp.AddEffect(nameof(SmokyRing));
            }
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(SmokyCrystalProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(SmokyCrystalProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(SmokyCrystalProj.darkC.ToVector4());
            }, extraSize: new Point(35, 2));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, SmokyCrystalProj.brightC, SmokyCrystalProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.PlatinumBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
            CreateRecipe()
                .AddIngredient<SmokyCrystal>()
                .AddIngredient(ItemID.GoldBar, 12)
                .AddTile<MagicCraftStation>()
                .Register();
        }

        public void PlayerShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!item.DamageType.CountsAsClass(DamageClass.Magic))
                return;

            int type2 = ModContent.ProjectileType<SmokyRingProj>();

            if (player.ownedProjectileCounts[type2] < 1)
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<SmokyRingProj>(), player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI, ai2: 1);

            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == player.whoAmI && p.type == type2)
                {
                    (p.ModProjectile as SmokyRingProj).StartAttack();
                    break;
                }
        }
    }

    [VaultLoaden(AssetDirectory.LandOfTheLustrousSeriesItems)]
    public class SmokyRingProj : BaseGemWeaponProj<SmokyRing>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public ref float TargetProj => ref Projectile.ai[1];
        public ref float AttackState => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[2];

        [VaultLoaden("{@classPath}" + "SmokyRingCircle")]
        public static ATex CircleTex { get; private set; }

        public override void InitializeGemWeapon()
        {
            TargetProj = -1;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 3);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
        }

        public override bool OwnerItemCheck()
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (!cp.HasEffect(nameof(SmokyRing)))
                {
                    Projectile.Kill();
                    return false;
                }

                return true;

            }

            return false;
        }

        public override void BeforeMove()
        {
            if (!VaultUtils.isServer && (int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(8, 24);
                Color c = Main.rand.NextFromList(SmokyCrystalProj.highlightC, SmokyCrystalProj.brightC, SmokyCrystalProj.darkC);
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
            Timer++;

            if (Timer % 20 == 0 && TargetProj == -1)//寻找宝石武器弹幕
            {
                foreach (var p in Main.ActiveProjectiles)
                    if (p.owner == Projectile.owner && p.whoAmI != Projectile.whoAmI && CoraliteSets.Projectiles.GemWeaponProj[p.type])
                    {
                        TargetProj = p.whoAmI;
                        break;
                    }
            }

            Vector2 idlePos;
            Vector2 rotVec2 = (Timer * 0.03f).ToRotationVector2() * 16 * 5;
            if (TargetProj.GetProjectileOwner(out Projectile proj, () => TargetProj = -1))
            {
                if (!CoraliteSets.Projectiles.GemWeaponProj[proj.type])
                    TargetProj = -1;

                idlePos = proj.Center;
            }
            else
                idlePos = Owner.Center;

            Projectile.rotation = (Projectile.Center - idlePos).ToRotation();

            idlePos += rotVec2;

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.5f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Lighting.AddLight(Projectile.Center, SmokyCrystalProj.brightC.ToVector3() / 2);
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                AttackTime--;

                if (AttackState == 0 && (AttackTime == 52 || AttackTime == 44))
                    ShootSmokyCrystal();
            }
        }

        public override void StartAttack()
        {
            if (AttackTime != 0)
                return;

            AttackTime = 60;
            AttackState++;
            if (AttackState > 3)
            {
                AttackState = 0;
                //射出烟水晶弹幕

                ShootSmokyCrystal();
            }
        }

        private void ShootSmokyCrystal()
        {
            Vector2 dir = new Vector2(0,-1);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Ambient_DarkBrown, dir.RotateByRandom(-0.4F, 0.4F) * Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            for (int i = 0; i < 3; i++)
            {
                SmokyCrystalProj.SpawnTriangleParticle(Projectile.Center + (dir.RotateByRandom(-0.4F, 0.4F) * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
            }

            Projectile.NewProjectileFromThis<SmokyCrystalProj>(Projectile.Center, dir * 12, Projectile.damage, Projectile.knockBack);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTextureValue();
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float rot = MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 0.1f ;

            const int maxFrameX = 4;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            CircleTex.Value.QuickCenteredDraw(spriteBatch, pos, Color.White with { A=180}, Projectile.rotation + MathHelper.PiOver2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (AttackTime > 30 && AttackState != 0)//绘制切换动画
            {
                float f = (AttackTime - 30) / 30f;

                //绘制本体的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 1, maxFrameX, 2), pos, 0, Color.White * (1 - f), rot);

                //绘制上一个状态的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState - 1, 1, maxFrameX, 2), pos, 0, Color.White * f, rot);

            }
            else if (AttackTime > 0 && AttackState != 0)
            {
                float f = AttackTime / 30f;

                //绘制本体
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 0, maxFrameX, 2), pos, 0, lightColor, rot);

                //绘制本体的高亮
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 1, maxFrameX, 2), pos, 0, Color.White * f, rot);

            }
            else
            {
                tex.QuickCenteredDraw(Main.spriteBatch, new Rectangle((int)AttackState, 0, maxFrameX, 2), pos, 0, lightColor, rot);
            }

            return false;
        }
    }

    public class SmokyCrystalProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public static Color highlightC = new(235, 232, 208);
        public static Color brightC = new(190, 120, 33);
        public static Color darkC = new(108, 61, 48);

        public ref float Target => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 10);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 230;
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

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Target = -1;
            }

            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, brightC.ToVector3() * 0.4f);

            if (Main.rand.NextBool(4))
            {
                Projectile.SpawnTrailDust(ModContent.DustType<SmokyCrystalDust>(), Main.rand.NextFloat(-0.3f, -0.1f),Main.rand.NextFloat(-0.3f,0.3f),0, newColor: Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Scale:Main.rand.NextFloat(0.7f, 1f));
            }

            if (Projectile.timeLeft % 4 == 0)
                SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));

            if (!Target.GetNPCOwner(out NPC npc, () => Target = -1))
            {
                if (Timer % 4 == 0)
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 900, n => n.CanBeChasedBy(), out NPC target))
                        Target = target.whoAmI;
                }

                return;
            }

            Projectile.ChaseGradually(npc.Center, 20, 19, 20);

            if (!Projectile.tileCollide)//不再墙壁内就检测碰墙
                if (!Helper.PointInTile(Projectile.Center))
                    Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Ambient_DarkBrown, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
                }

            Helper.PlayPitched(CoraliteSoundID.CrystalBroken_DD2_WitherBeastDeath, Projectile.Center);
            Projectile.NewProjectileFromThis<SmokySmokeProj>(Projectile.Center, Vector2.Zero, Projectile.damage, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D exTex = TextureAssets.Extra[ExtrasID.SharpTears].Value;

            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2) - Main.screenPosition;
            var origin = exTex.Size() / 2;

            for (int i = 0; i < 10; i++)
                Main.spriteBatch.Draw(exTex, Projectile.oldPos[i] + toCenter, null, Color.Lerp(highlightC, darkC, i / 12f) * (0.6f - (i * 0.6f / 12)), Projectile.oldRot[i] + 1.57f, origin, new Vector2(0.5f, 1), 0, 0);

            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }

    public class SmokySmokeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public PRTGroup group;
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 180;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Timer++;

            group ??= new PRTGroup();

            if (Timer == 1)
            {
                float dir = Main.rand.NextFloat(MathHelper.TwoPi);
                for (int i = 0; i < 5; i++)
                {
                    Vector2 dir2 = (dir + i / 7f * MathHelper.TwoPi + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2();
                    FlowLineThin.Spawn(Projectile.Center + dir2 * Main.rand.NextFloat(2, 20)
                        , dir2 * Main.rand.NextFloat(2.5f, 5), 11, 20, Main.rand.NextFloat(-0.15f, 0.15f)
                        , SmokyCrystalProj.highlightC);
                }

                for (int i = 0; i < 9; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokyCrystalDust>(), Helper.NextVec2Dir(2, 4), newColor: Color.White);
                }

                Vector2 center = Projectile.Center;
                //RedJades.RedGlowParticle2.Spawn(center, 0.55f, (SmokyCrystalProj.darkC * 0.4f) with { A = 80 }, (Color.DarkRed * 0.4f) with { A = 100 }, 0.1f);
                //RedJades.RedGlowParticle2.Spawn(center, 0.5f, SmokyCrystalProj.brightC * 0.4f, SmokyCrystalProj.brightC with { A = 150 }, 0.2f);

                RedJades.RedExplosionParticle2.Spawn(center, 0.6f, SmokyCrystalProj.brightC * 0.6f);
                RedJades.RedExplosionParticle2.Spawn(center, 0.4f, SmokyCrystalProj.brightC * 1.2f);
            }

            if (Timer < 18)
            {
                //for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    if (Main.rand.NextBool())
                        group.NewParticle<TwistFog>(Projectile.Center + Main.rand.NextFloat(4, 30) * dir, dir * Main.rand.NextFloat(1.5f, 3), Color.White * 0.6f, Main.rand.NextFloat(0.4f, 0.6f));
                    else
                        group.NewParticle<BigFog>(Projectile.Center + Main.rand.NextFloat(4, 30) * dir, dir * Main.rand.NextFloat(1.5f, 3), Color.White * 0.6f, Main.rand.NextFloat(0.4f, 0.6f));
                }
            }

            else if (Timer > 26 && !group.Any())
            {
                Projectile.Kill();
                return;
            }

            group.Update();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.damage>8)
                Projectile.damage--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rand.X += 0.15f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.9f)
                , ((float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f)) + (Projectile.whoAmI / 3f)
                , SmokyCrystalProj.highlightC, SmokyCrystalProj.brightC, SmokyCrystalProj.darkC, () =>
                {
                    group?.Draw(Main.spriteBatch);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.55f, 0.14f, 0.55f,hasWorld:false);

            return false;
        }
    }

    public class SmokyCrystalDust : ModDust
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame.X = Main.rand.Next(3);
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;

            if (dust.fadeIn > 7)
            {
                dust.color *= 0.9f;
                dust.velocity *= 0.97f;
            }

            if (dust.color.A < 10)
            {
                dust.active = false;
            }

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D.Value.QuickCenteredDraw(Main.spriteBatch, new Rectangle(dust.frame.X, 0, 3, 1), dust.position - Main.screenPosition, dust.color, dust.rotation + MathHelper.PiOver2, dust.scale);
            return false;
        }
    }
}
