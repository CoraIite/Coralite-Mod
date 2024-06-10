using Coralite.Content.Items.Materials;
using Coralite.Content.NPCs.Magike;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class PeridotTalisman : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 9));
            Item.SetWeaponValues(70, 4);
            Item.useTime = Item.useAnimation = 38;
            Item.mana = 20;

            Item.shoot = ModContent.ProjectileType<PeridotTalismanProj>();
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
                    (proj.ModProjectile as PeridotTalismanProj).StartAttack();
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
                effect.Parameters["highlightC"].SetValue(PeridotProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(PeridotProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(PeridotProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, PeridotProj.brightC, PeridotProj.darkC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Peridot>()
                .AddIngredient(ItemID.Ectoplasm, 12)
                .AddIngredient<RegrowthTentacle>()
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class PeridotTalismanProj : BaseGemWeaponProj<PeridotTalisman>
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
                Color c = Main.rand.NextFromList(Color.White, PeridotProj.brightC, PeridotProj.darkC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.7f, 0));
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(-Owner.direction * 32, 0);

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

                    Projectile.NewProjectileFromThis<PeridotProj>(Projectile.Center,
                           Helper.NextVec2Dir(4, 10), Owner.GetWeaponDamage(Owner.HeldItem)
                           , Projectile.knockBack);

                    Helper.PlayPitched("Crystal/GemShoot", 0.3f, -0.8f, Projectile.Center);
                    SoundEngine.PlaySound(CoraliteSoundID.SpiritFlame_Item117, Projectile.Center);

                    for (int i = 0; i < 6; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        PeridotProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.2f);

                AttackTime--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(ZumurudProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class PeridotProj : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "Peridot";

        public static Color highlightC = new Color(140, 238, 255);
        public static Color brightC = new Color(181, 243, 0);
        public static Color darkC = new Color(70, 126, 0);

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public Vector2 TargetPos
        {
            get => new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            set
            {
                Projectile.localAI[0] = value.X;
                Projectile.localAI[1] = value.Y;
            }
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 18;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
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
            if (State == 0)
            {
                Timer++;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Timer > 15)
                {
                    State = 1;
                    TargetPos = Main.MouseWorld;
                }
            }
            else
            {
                if (Projectile.timeLeft % 3 == 0)
                    SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                if (Main.rand.NextBool(5))
                    Projectile.SpawnTrailDust(8f, DustID.CursedTorch, Main.rand.NextFloat(0.2f, 0.4f));

                float num481 = 20f;
                Vector2 center = Projectile.Center;
                Vector2 targetCenter = TargetPos;
                Vector2 dir = targetCenter - center;
                float length = dir.Length();
                if (length < 100f)
                    num481 = 10f;

                length = num481 / length;
                dir *= length;
                Projectile.velocity.X = (Projectile.velocity.X * 19f + dir.X) / 20f;
                Projectile.velocity.Y = (Projectile.velocity.Y * 19f + dir.Y) / 20f;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Vector2.Distance(Projectile.Center, TargetPos) < 24)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int x = 2;
                if (Helper.TryFindClosestEnemy(Projectile.Center, 200, n => n.CanBeChasedBy(), out _))
                    x = 8;

                float angle = Main.rand.NextFloat(6.282f);

                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = angle.ToRotationVector2();
                    if (Main.rand.NextBool(x, 10))
                    {
                        int index = Projectile.NewProjectileFromThis<PeridotSpilit>(Projectile.Center,dir* Main.rand.NextFloat(3, 5f),
                             Projectile.damage, Projectile.knockBack, 0);

                        Main.projectile[index].penetrate = -1;
                    }
                    else
                    {
                        Projectile.NewProjectileFromThis<PeridotSpilit>(Projectile.Center, dir * Main.rand.NextFloat(3, 4),
                            Projectile.damage, Projectile.knockBack, 1);
                    }

                    angle += Main.rand.NextFloat(MathHelper.TwoPi / 3 - 0.3f, MathHelper.TwoPi / 3 + 0.3f);
                }

                SoundStyle st = CoraliteSoundID.Crystal_Item101;
                st.Pitch =-0.4f;
                SoundEngine.PlaySound(st, Projectile.Center);
                Helper.PlayPitched("Crystal/CrystalStrike", 0.4f, -0.2f, Projectile.Center);
            }

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.NextVec2Dir(2, 8), Scale: Main.rand.NextFloat(1f, 2f));
                    d.noGravity = true;
                }
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Green, Helper.NextVec2Dir(1, 5));
                }
            }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 6f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D exTex = TextureAssets.Extra[98].Value;

            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var origin = exTex.Size() / 2;

            for (int i = 0; i < 12; i++)
                Main.spriteBatch.Draw(exTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    Color.Lerp(brightC, darkC, i / 12f) * (0.4f - i * 0.4f / 12), Projectile.oldRot[i] + 1.57f, origin, 1, 0, 0);

            Projectile.QuickDraw(lightColor, 0);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D glowTex = CrystalLaser.GlowTex.Value;
            Texture2D starTex = CrystalLaser.StarTex.Value;
            var pos = Projectile.Center - Main.screenPosition;
            Color c = brightC;
            c.A = 75;

            spriteBatch.Draw(glowTex, pos, null, c, 0, glowTex.Size() / 2, 0.55f, 0, 0);
            spriteBatch.Draw(starTex, pos, null, c, Main.GlobalTimeWrappedHourly, starTex.Size() / 2, 0.3f, 0, 0);
        }
    }

    public class PeridotSpilit : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public static Asset<Texture2D> SecondTex;
        public ref float TexType => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        private float Alpha = 1;
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SecondTex = ModContent.Request<Texture2D>(AssetDirectory.LandOfTheLustrousSeriesItems + Name + "2");
            }
        }

        public override void Unload()
        {
            SecondTex = null;
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 6);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = Projectile.height = 24;
            //Projectile.penetrate = 2;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            switch (TexType)
            {
                default://飞一段时间消失
                    {
                        Projectile.UpdateFrameNormally(4, 4);
                        Timer++;
                        Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);

                        if (Timer > 40)
                        {
                            Alpha -= 0.05f;
                            Projectile.velocity *= 0.95f;
                            if (Alpha < 0.05f)
                            {
                                Projectile.Kill();
                            }
                        }
                    }
                    break;
                case 1://幽魂追踪
                    {
                        Projectile.UpdateFrameNormally(4, 3);

                        const int ChaseTime = 10;
                        const int MaxTime = ChaseTime + 60;

                        Projectile.velocity = Projectile.velocity.RotatedBy(0.01f);

                        if (Timer > ChaseTime)//开始追踪阶段
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                            {
                                Target = target.whoAmI;
                                TexType = 2;
                                Projectile.timeLeft = 600;
                                Projectile.extraUpdates = 1;
                                Alpha = 1;
                            }

                            Alpha -= 1 / 60f;
                        }

                        Projectile.velocity *= 0.97f;
                        if (Timer > MaxTime)
                            Projectile.Kill();

                        Timer++;
                    }
                    break;
                case 2:
                    {
                        Projectile.UpdateFrameNormally(4, 3);

                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            TexType = 3;

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
                        Projectile.velocity.X = (Projectile.velocity.X * 19f + dir.X) / 20f;
                        Projectile.velocity.Y = (Projectile.velocity.Y * 19f + dir.Y) / 20f;
                    }
                    break;
                case 3:
                    Projectile.UpdateFrameNormally(4, 3);
                    break;
            }

            if (Projectile.timeLeft % 5 == 0)
                PeridotProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(5))
                Projectile.SpawnTrailDust(8f, DustID.CursedTorch, Main.rand.NextFloat(0.2f, 0.4f));

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 4; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    PeridotProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TexType == 0 ? SecondTex.Value : Projectile.GetTexture();
            var frame = mainTex.Frame(1, TexType == 0 ? 4 : 3, 0, Projectile.frame);
            float exrot = TexType == 0 ? 0 : -1.57f;

            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 6; i++)
            {
                Color c = PeridotProj.brightC * (Alpha / 2f - i * Alpha / 2f / 6f);
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frame,
                    c, Projectile.oldRot[i] + exrot, frame.Size() / 2, 1 * (1 + i * 0.1f), 0, 0);
            }

            rand-= Projectile.velocity/10;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 3f
                , PeridotProj.highlightC, PeridotProj.brightC, PeridotProj.darkC, () =>
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.Center, frame, Color.White * Alpha, Projectile.rotation + exrot, frame.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.25f, 0.15f);

            return false;
        }
    }
}
