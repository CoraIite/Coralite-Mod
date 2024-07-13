using Coralite.Content.Items.Donator;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class LandOfTheLustrous : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 33, 10, 8));
            Item.SetWeaponValues(235, 4, 12);
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 23;

            Item.shoot = ModContent.ProjectileType<LandOfTheLustrousProj>();
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
                Projectile p = Main.projectile.First(p => p.active && p.owner == player.whoAmI && p.type == type);
                if (p != null)
                {
                    (p.ModProjectile as LandOfTheLustrousProj).StartAttack();
                    Helper.PlayPitched("Crystal/CrystalShoot", 0.1f, 0f, player.Center);
                    Helper.PlayPitched("Crystal/GemShoot", 0.3f, 0f, player.Center);
                    Helper.PlayPitched("Crystal/CrystalStrike", 0.2f, 0f, player.Center);
                }
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                float factor1 = Main.GlobalTimeWrappedHourly * 0.5f - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.5f);
                float factor2 = Main.GlobalTimeWrappedHourly * 0.45f - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.45f);
                effect.Parameters["scale"].SetValue(new Vector2(0.5f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.75f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(Color.White.ToVector4());
                effect.Parameters["brightC"].SetValue(Main.hslToRgb(factor1, 1, 0.9f).ToVector4());
                effect.Parameters["darkC"].SetValue(Main.hslToRgb(factor2, 0.8f, 0.3f).ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, Main.DiscoColor, Color.Gray);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PyropeCrown>()
                .AddIngredient<AmethystNecklace>()
                .AddIngredient<AquamarineBracelet>()
                .AddIngredient<PinkDiamondRose>()
                .AddIngredient<ZumurudRing>()
                .AddIngredient<PearlBrooch>()
                .AddIngredient<RubyScepter>()
                .AddIngredient<PeridotTalisman>()
                .AddIngredient<SapphireHairpin>()
                .AddIngredient<TourmalineMonoclastic>()
                .AddIngredient<TopazMirror>()
                .AddIngredient<ZirconGrail>()
                .AddTile<PhantomCrystalBallTile>()
                .Register();
        }
    }

    public class LandOfTheLustrousProj : BaseGemWeaponProj<LandOfTheLustrous>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public int itemType;

        public static Asset<Texture2D> HaloTex;
        public ref float Scale => ref Projectile.localAI[2];

        private float selfRot;

        private List<LandOfTheLustrousData> Draws = new List<LandOfTheLustrousData>();

        public class LandOfTheLustrousData(float rot)
        {
            public bool active = true;
            public float rot = rot;
            public float scale = 0.2f;
            public float alpha;

            private int timer;

            public void Update()
            {
                if (timer < 10)
                    alpha += 0.1f;

                if (timer > 60)
                {
                    alpha -= 0.05f;
                    scale += 0.03f;
                }
                else
                    scale = 0.2f + 0.8f * Coralite.Instance.SqrtSmoother.Smoother(timer, 60);

                if (timer > 80)
                    active = false;

                rot += 0.04f;
                timer++;
            }
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                HaloTex = ModContent.Request<Texture2D>(AssetDirectory.LandOfTheLustrousSeriesItems + "LandOfTheLustrousHalo");
            }
        }

        public override void Unload()
        {
            HaloTex = null;
        }

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 5);
            Main.projFrames[Type] = 52;
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(32, 64);
                Color c = Main.rand.NextFromList(Color.White, Main.DiscoColor*1.5f);
                var p = Particle.NewParticle<HexagramParticle>(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length),
                     Vector2.UnitX, c, Scale: Main.rand.NextFloat(0.1f, 0.12f));
                p.follow = () => Projectile.position - Projectile.oldPos[1];
                p.Rotation = -1.57f;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(1.2f));

            Projectile.UpdateFrameNormally(3, Main.projFrames[Projectile.type]-1);
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(-Owner.direction * 40, -60);

            if (AttackTime == 0)
            {
                float factor = Main.GlobalTimeWrappedHourly;
                float sin = MathF.Sin(factor);
                float cos = MathF.Cos(factor);
                float a = 14;
                float sin2Plus1 = 1 + sin * sin;

                idlePos += new Vector2(a * cos / sin2Plus1, a * sin * cos / sin2Plus1);

                float factor2 = Math.Clamp(MathF.Abs(Owner.velocity.X) / 3, 0, 1);
                float targetRotation = Helper.Lerp(0, OwnerDirection * 0.6f + MathF.Sin(Main.GlobalTimeWrappedHourly * 4) * 0.2f, factor2);

                idlePos += Owner.velocity * 4 * factor2;
                selfRot = selfRot.AngleLerp(targetRotation, 0.1f);
            }
            else
                selfRot = selfRot.AngleLerp(0, 0.2f);

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.4f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.4f);
            Projectile.rotation += 0.01f;

            if (Draws.Count > 0)
            {
                for (int i = 0; i < Draws.Count; i++)
                {
                    Draws[i].Update();
                }
                Draws.RemoveAll(d => !d.active);
            }
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                Projectile.rotation += 0.06f;
                int per = Owner.itemTimeMax / 3;
                if ((int)AttackTime % per == 0 && AttackTime > 0)
                {
                    float angle = Projectile.rotation;
                    int time = 3 - (int)(AttackTime / per);

                    int recordItemType = itemType;
                    for (int i = 0; i < 10; i++)
                    {
                        itemType = CoraliteWorld.chaosWorld ? itemType = Main.rand.Next(1, ItemLoader.ItemCount)
                           : -Main.rand.Next(1, (int)LustrousProj.GemType.CrystallineMagike + 1);
                        if (itemType != recordItemType)
                            break;
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        float speed = i switch
                        {
                            0 => 3.4f,
                            1 => 2.4f,
                            _ => 1.6f
                        };

                        speed -= time * 0.1f;

                        Projectile.NewProjectileFromThis<LustrousProj>(Projectile.Center
                            , angle.ToRotationVector2() * speed, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack
                            , itemType, i == 0 ? 1 : 0, Projectile.whoAmI);

                        angle += 0.3f;
                    }

                    Vector2 dir = Projectile.rotation.ToRotationVector2();
                    for (int i = 0; i < 4; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(1f, 5f)
                            , Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                        LustrousProj.SpawnTriangleParticle(Projectile.Center
                            , dir.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(3f, 5f));
                    }
                }

                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            base.StartAttack();
            Draws.Add(new LandOfTheLustrousData(Projectile.rotation + 1));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Texture2D haloTex = HaloTex.Value;
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var frame = mainTex.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2;

            SpriteEffects eff = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, frame,
                    Main.hslToRgb(Main.GlobalTimeWrappedHourly + i * 0.1f, 0.9f, 0.9f) * (0.5f - i * 0.5f / 5), selfRot, origin, Projectile.scale - i * 0.05f, eff, 0); ;

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frame, lightColor, selfRot,
                origin, Projectile.scale, eff, 0);

            float factor1 = MathF.Sin(Main.GlobalTimeWrappedHourly * 2);
            float factor2 = MathF.Sin((int)Main.timeForVisualEffects * 0.05f);
            Color c2 = Main.hslToRgb(Main.GlobalTimeWrappedHourly, 0.6f, 0.6f) * (0.1f + factor1 * 0.1f);
            c2.A = 0;

            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(mainTex, Projectile.Center + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2() * factor2 * 12 - Main.screenPosition, frame,
                   c2, selfRot, origin, Projectile.scale * (1.05f + factor2 * 0.2f), eff, 0);
            }

            for (int i = 0; i < Draws.Count; i++)
            {
                var data = Draws[i];
                Main.spriteBatch.Draw(haloTex, Projectile.Center - Main.screenPosition, null, Color.White * data.alpha, data.rot, haloTex.Size() / 2
                    , data.scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(haloTex, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * (data.alpha / 3), data.rot - 0.05f, haloTex.Size() / 2
                    , data.scale * 1.04f, SpriteEffects.None, 0f);
            }

            //Main.spriteBatch.Draw(haloTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, haloTex.Size() / 2
            //    , 1 + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f, SpriteEffects.None, 0f);

            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入贴图类型，ai1为1时表示为闪光弹幕，ai2传入主人
    /// </summary>
    public class LustrousProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TextureType => ref Projectile.ai[0];
        public bool Shiny => Projectile.ai[1] == 1;
        public ref float OwnerIndex => ref Projectile.ai[2];
        public ref float State => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float Target => ref Projectile.localAI[2];

        private bool init = true;

        public GemDrawData data;

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        private Trail trail;

        public static Asset<Texture2D> SmallPinkDiamond;
        public static Asset<Texture2D> LaserTex;

        public int FlyTime;
        public float alpha;

        public struct GemDrawData(Texture2D tex, Color highlightC, Color brightC, Color darkC)
        {
            public Texture2D tex = tex;
            public Vector2 scale = Vector2.One;
            public float lightRange = 0.2f;
            public float lightLimit = 0.35f;
            public float addC = 0.75f;
            public Color highlightC = highlightC;
            public Color brightC = brightC;
            public Color darkC = darkC;
        }

        public enum GemType
        {
            /// <summary> 红榴石 </summary>
            Pyrope = 1,
            /// <summary> 紫晶 </summary>
            Amethyst,
            /// <summary> 海蓝宝石 </summary>
            Aquamarine,
            /// <summary> 琥珀 </summary>
            Amber,
            /// <summary> 粉钻 </summary>
            PinkDiamond,
            /// <summary> 钻石 </summary>
            Diamond,
            /// <summary> 祖母绿 </summary>
            Zumurud,
            /// <summary> 绿宝石 </summary>
            Emerald,
            /// <summary> 白珍珠 </summary>
            WhitePearl,
            /// <summary> 黑珍珠 </summary>
            BlackPearl,
            /// <summary> 粉珍珠 </summary>
            PinkPearl,
            /// <summary> 红宝石 </summary>
            Ruby,
            /// <summary> 橄榄石 </summary>
            Peridot,
            /// <summary> 蓝宝石 </summary>
            Sapphire,
            /// <summary> 碧玺 </summary>
            Tourmaline,
            /// <summary> 托帕石 </summary>
            Topaz,
            /// <summary> 锆石 </summary>
            Zircon,
            /// <summary> 魔力水晶 </summary>
            MagicCrystal,
            /// <summary> 蕴魔水晶 </summary>
            CrystallineMagike,
        }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SmallPinkDiamond = ModContent.Request<Texture2D>(AssetDirectory.LandOfTheLustrousSeriesItems + "SmallPinkDiamond");
            LaserTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaser");
        }

        public override void Unload()
        {
            SmallPinkDiamond = null;
            LaserTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = Projectile.height = 22;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => State > 0;
        public override bool? CanDamage() => State > 0;

        public static void SpawnTriangleParticle(Vector2 pos, Vector2 velocity)
        {
            float factor1 = Main.GlobalTimeWrappedHourly * 0.5f - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.5f);
            float factor2 = Main.GlobalTimeWrappedHourly * 0.45f - MathF.Truncate(Main.GlobalTimeWrappedHourly * 0.45f);

            Color c1 = Color.White;
            c1.A = 125;
            Color c2 = Main.hslToRgb(factor1, 1, 0.9f);
            Color c4 = c2;
            c2.A = 125;
            Color c3 = Main.hslToRgb(factor2, 0.8f, 0.3f);
            c3.A = 100;
            Color c = Main.rand.NextFromList(c4, c1, c2, c3);
            CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.3f));
        }

        public void SpawnTriangleParticleSelf(Vector2 pos, Vector2 velocity)
        {
            Color c1 = data.highlightC;
            c1.A = 125;
            Color c2 = data.brightC;
            c2.A = 125;
            Color c3 = data.darkC;
            c3.A = 100;
            Color c = Main.rand.NextFromList(data.highlightC, data.brightC, c1, c2, c3);
            CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.3f));
        }

        public override void AI()
        {
            Initialize();

            switch (State)
            {
                default:
                case 0:
                    {
                        if (!OwnerIndex.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                            return;

                        //从中心向外
                        Projectile.Center = owner.Center + (FlyTime - Timer + 2) * Projectile.velocity;
                        Timer--;

                        if (Timer > 0)
                            alpha = Coralite.Instance.X2Smoother.Smoother(1 - Timer / FlyTime);

                        if (Timer < 0)
                        {
                            State++;
                            Projectile.velocity = (Projectile.Center - owner.Center).RotatedBy(-1.57f).SafeNormalize(Vector2.Zero) * 12;
                            Projectile.extraUpdates = 1;
                            Projectile.timeLeft = 110 * Projectile.extraUpdates;
                            Projectile.tileCollide = true;
                            FlyTime = 0;
                            Timer = 0;
                        }
                    }
                    break;
                case 1://追踪鼠标位置
                    {
                        Timer++;
                        if (Timer % 20 == 0 && Target < 0)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 1200, n => n.CanBeChasedBy(), out NPC t))
                            {
                                Target = t.whoAmI;
                                Projectile.timeLeft += 10 * Projectile.MaxUpdates;
                            }
                        }

                        Vector2 targetCenter = Main.MouseWorld;

                        if (Target.GetNPCOwner(out NPC target))
                        {
                            targetCenter = Vector2.SmoothStep(targetCenter, target.Center, Math.Clamp(Timer / 120, 0, 1));
                        }
                        else
                            Target = -1;

                        float slowTime = Timer < 15 ? Coralite.Instance.X2Smoother.Smoother(Timer / 15) : 1;
                        slowTime = Helper.Lerp(130, 28, slowTime);

                        float num481 = 24f;
                        Vector2 center = Projectile.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        if (length < 100f)
                            num481 = 16f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = (Projectile.velocity.X * slowTime + dir.X) / (slowTime + 1);
                        Projectile.velocity.Y = (Projectile.velocity.Y * slowTime + dir.Y) / (slowTime + 1);

                        if (Projectile.timeLeft % 12 == 0)
                            SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                        if (Main.rand.NextBool(15))
                            Projectile.SpawnTrailDust(8f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.6f, 1.4f), newColor: data.brightC);

                        if (Vector2.Distance(Projectile.Center, targetCenter) < 60 && Timer > 90)
                            TurnToFade();
                    }
                    break;
                case 2:
                    {
                        Timer--;
                        alpha = Timer / 25;
                        if (Projectile.timeLeft % 12 == 0)
                            SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                        if (Main.rand.NextBool(15))
                            Projectile.SpawnTrailDust(8f, DustID.PortalBoltTrail, Main.rand.NextFloat(0.6f, 1.4f), newColor: data.brightC);
                    }
                    break;
                case 3: break;
            }

            Projectile.UpdateOldPosCache(addVelocity: false);
            Projectile.UpdateOldRotCache();
            if (trail != null)
                trail.Positions = Projectile.oldPos;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.UpdateFrameNormally(8, 19);
        }

        public void Initialize()
        {
            if (init)
            {
                int maxPoint = Shiny ? 10 : 5;

                Target = -1;
                data = GetDrawData();
                Projectile.InitOldPosCache(maxPoint);
                Projectile.InitOldRotCache(maxPoint);
                if (OwnerIndex.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                {
                    Timer = 108 / Projectile.velocity.Length();
                    FlyTime = (int)Timer;
                }

                init = false;
                if (trail == null && Shiny)
                    trail ??= new Trail(Main.graphics.GraphicsDevice, maxPoint, new TriangularTip(20)
                        , TrailWidth, TrailColor);
            }
        }

        public static float TrailWidth(float f) => Helper.Lerp(2, 14, f);
        public Color TrailColor(Vector2 f)
        {
            return Color.Lerp(Color.Transparent, Color.White * 0.65f, f.X) * alpha;
        }

        public void TurnToFade()
        {
            State = 2;
            Timer = 25;
            Projectile.timeLeft = 25;
            Projectile.tileCollide = false;
        }

        public GemDrawData GetDrawData()
        {
            if (TextureType > 0)
            {
                Main.instance.LoadItem((int)TextureType);
                return new GemDrawData(TextureAssets.Item[(int)TextureType].Value, Color.White, Color.Gray, Color.DarkGray);
            }

            switch ((GemType)(-TextureType))
            {
                case GemType.Pyrope:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<Pyrope>()].Value
                        , PyropeProj.highlightC, PyropeProj.brightC, PyropeProj.darkC);
                case GemType.Amethyst:
                    Main.instance.LoadItem(ItemID.Amethyst);
                    return new GemDrawData(TextureAssets.Item[ItemID.Amethyst].Value
                        , AmethystLaser.highlightC, AmethystLaser.brightC, AmethystLaser.darkC)
                    {
                        scale = new Vector2(0.8f),
                        lightRange = 0.1f,
                        lightLimit = 0.35f,
                        addC = 0f,
                    };
                case GemType.Aquamarine:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<Aquamarine>()].Value
                        , AquamarineProj.highlightC, AquamarineProj.brightC, AquamarineProj.darkC)
                    {
                        scale = new Vector2(0.7f),
                        lightRange = 0.1f,
                        lightLimit = 0.65f,
                        addC = 0.5f,
                    };
                case GemType.Amber:
                    Main.instance.LoadItem(ItemID.Amber);
                    return new GemDrawData(TextureAssets.Item[ItemID.Amber].Value
                        , AmberProj.highlightC, AmberProj.brightC, AmberProj.darkC)
                    {
                        lightRange = 0.1f,
                        lightLimit = 0.75f,
                        addC = 0.75f,
                    };
                case GemType.PinkDiamond:
                    return new GemDrawData(SmallPinkDiamond.Value
                        , PinkDiamondProj.highlightC, PinkDiamondProj.brightC, PinkDiamondProj.darkC)
                    {
                        scale = new Vector2(1.7f),
                        lightRange = 0.15f,
                        lightLimit = 0.45f,
                        addC = 0.55f,
                    };
                case GemType.Diamond:
                    Main.instance.LoadItem(ItemID.Diamond);
                    return new GemDrawData(TextureAssets.Item[ItemID.Diamond].Value
                        , Color.White, new Color(218, 185, 210), new Color(0, 39, 89))
                    {
                        scale = new Vector2(1.7f),
                        lightRange = 0.15f,
                        lightLimit = 0.45f,
                        addC = 0.55f,
                    };
                case GemType.Zumurud:
                    return new GemDrawData(TextureAssets.Projectile[ModContent.ProjectileType<SmallZumurudProj>()].Value
                        , ZumurudProj.highlightC, ZumurudProj.brightC, ZumurudProj.darkC)
                    {
                        scale = new Vector2(0.4f, 1f),
                        addC = 0.35f,
                    };
                case GemType.Emerald:
                    Main.instance.LoadItem(ItemID.Emerald);
                    return new GemDrawData(TextureAssets.Item[ItemID.Emerald].Value
                        , ZumurudProj.highlightC, ZumurudProj.brightC, ZumurudProj.darkC)
                    {
                        scale = new Vector2(0.4f, 1f),
                        addC = 0.35f,
                    };
                case GemType.WhitePearl:
                    Main.instance.LoadItem(ItemID.WhitePearl);
                    return new GemDrawData(TextureAssets.Item[ItemID.WhitePearl].Value
                        , PearlProj.highlightC, PearlProj.brightC, PearlProj.darkC);
                case GemType.BlackPearl:
                    Main.instance.LoadItem(ItemID.BlackPearl);
                    return new GemDrawData(TextureAssets.Item[ItemID.BlackPearl].Value
                        , PearlProj.highlightC, PearlProj.brightC, PearlProj.darkC);
                case GemType.PinkPearl:
                    Main.instance.LoadItem(ItemID.PinkPearl);
                    return new GemDrawData(TextureAssets.Item[ItemID.PinkPearl].Value
                        , PearlProj.highlightC, PearlProj.brightC, PearlProj.darkC);
                case GemType.Ruby:
                    Main.instance.LoadItem(ItemID.Ruby);
                    return new GemDrawData(TextureAssets.Item[ItemID.Ruby].Value
                        , RubyProj.highlightC, RubyProj.brightC, RubyProj.darkC)
                    {
                        scale = new Vector2(0.8f),
                        lightRange = 0.1f,
                        lightLimit = 0.35f,
                        addC = 0f,
                    };
                case GemType.Peridot:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<Peridot>()].Value
                        , PeridotProj.highlightC, PeridotProj.brightC, PeridotProj.darkC)
                    {
                        scale = new Vector2(0.7f),
                        lightRange = 0.1f,
                        lightLimit = 0.25f,
                        addC = 0.15f,
                    };
                case GemType.Sapphire:
                    Main.instance.LoadItem(ItemID.Sapphire);
                    return new GemDrawData(TextureAssets.Item[ItemID.Sapphire].Value
                        , SapphireProj.highlightC, SapphireProj.brightC, SapphireProj.darkC)
                    {
                        scale = new Vector2(0.8f),
                        lightRange = 0.1f,
                        lightLimit = 0.35f,
                        addC = 0f,
                    };
                case GemType.Tourmaline:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<Tourmaline>()].Value
                        , TourmalineProj.highlightC, TourmalineProj.brightC, TourmalineProj.darkC)
                    {
                        scale = new Vector2(1.4f),
                        lightRange = 0.15f,
                        lightLimit = 0.55f,
                        addC = 0.55f,
                    };
                case GemType.Topaz:
                    Main.instance.LoadItem(ItemID.Topaz);
                    return new GemDrawData(TextureAssets.Item[ItemID.Topaz].Value
                        , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC)
                    {
                        scale = new Vector2(0.7f),
                        lightRange = 0.1f,
                        lightLimit = 0.15f,
                        addC = 0.7f,
                    };
                case GemType.Zircon:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<Zircon>()].Value
                        , ZirconProj.highlightC, ZirconProj.brightC, ZirconProj.darkC)
                    {
                        scale = new Vector2(1.2f),
                        lightRange = 0.15f,
                        lightLimit = 0.45f,
                        addC = 0.75f,
                    };

                case GemType.MagicCrystal:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<MagicCrystal>()].Value
                        , Color.White, Coralite.Instance.MagicCrystalPink, Color.DarkMagenta);
                case GemType.CrystallineMagike:
                    return new GemDrawData(TextureAssets.Item[ModContent.ItemType<CrystallineMagike>()].Value
                        , Color.White, Coralite.Instance.CrystallineMagikePurple, Color.DarkBlue);
                default:
                    return new GemDrawData(Projectile.GetTexture()
                        , Color.White, Color.Gray, Color.DarkGray);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State != 3)
            {
                State = 3;
                Projectile.penetrate = -1;
                Projectile.timeLeft = 2;
                Projectile.StartAttack();
                Projectile.Resize(80, 80);
                Projectile.velocity *= 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            if (State == 1)
            {
                FlyTime++;
                if (FlyTime > 10)
                    TurnToFade();
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Shiny)
                SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 3; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Helper.NextVec2Dir(4, 8), newColor: data.brightC, Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                float length = Main.rand.NextFloat(0, 6);
                Color c = Main.rand.NextFromList(Color.White, Main.DiscoColor );
                var p = Particle.NewParticle<HexagramParticle>(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length),
                     Vector2.UnitX*Main.rand.NextFloat(1,2), c, Scale: Main.rand.NextFloat(0.1f, 0.14f));
                p.Rotation = -1.57f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    SpawnTriangleParticleSelf(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State == 0)
            {
                float sccle = Shiny ? 1 : 0.4f;
                float factor = 1 - Timer / FlyTime;

                Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, data.highlightC * 0.5f, data.brightC * 0.5f,
                    factor, 0, 0.65f, 0.8f, 1, Projectile.rotation, new Vector2(1.5f, 4) * sccle, Vector2.One);
                for (int i = 0; i < 2; i++)
                    Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, data.highlightC, data.brightC,
                        factor, 0, 0.65f, 0.8f, 1, Projectile.rotation, new Vector2(1, 2) * sccle, Vector2.One / 2);
            }

            return false;
        }

        public void DrawPrimitives()
        {
            if (!Shiny || trail == null)
                return;

            Effect effect = Filters.Scene["CrystalTrail"].GetShader().Shader;
            Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

            effect.Parameters["noiseTexture"].SetValue(noiseTex);
            effect.Parameters["TrailTexture"].SetValue(LaserTex.Value);
            effect.Parameters["transformMatrix"].SetValue(Helper.GetTransfromMaxrix());
            effect.Parameters["basePos"].SetValue((Projectile.Center - Main.screenPosition + rand) * Main.GameZoomTarget);
            effect.Parameters["scale"].SetValue(data.scale / Main.GameZoomTarget);
            effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
            effect.Parameters["lightRange"].SetValue(data.lightRange);
            effect.Parameters["lightLimit"].SetValue(data.lightLimit);
            effect.Parameters["addC"].SetValue(data.addC);
            effect.Parameters["highlightC"].SetValue(data.highlightC.ToVector4());
            effect.Parameters["brightC"].SetValue(data.brightC.ToVector4());
            effect.Parameters["darkC"].SetValue(data.darkC.ToVector4());

            trail.Render(effect);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            rand -= Projectile.velocity / 10;

            Texture2D mainTex = data.tex;

            if (mainTex == null)
                return;
            var origin = mainTex.Size() / 2;

            if (!Shiny)
            {
                if (State > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Color c1 = data.brightC;
                        c1.A = (byte)(i * 0.5f / 5 * 255);
                        spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                            c1, Projectile.oldRot[i], origin, Projectile.scale * (0.75f + i * 0.25f / 5), 0, 0);
                    }
                }
            }

            Color c = Color.White;
            c.A = (byte)(30 * alpha);
            Vector2 pos = Projectile.Center - Main.screenPosition;
            float scale = Projectile.scale * 1.2f;
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(mainTex, pos + (Main.GlobalTimeWrappedHourly + i * MathHelper.TwoPi / 3).ToRotationVector2() * 2, null, c, Projectile.rotation,
                    origin, scale, 0, 0);
            }

            c.A = (byte)(255 * alpha);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation,
                origin, Projectile.scale, 0, 0);
        }
    }
}
