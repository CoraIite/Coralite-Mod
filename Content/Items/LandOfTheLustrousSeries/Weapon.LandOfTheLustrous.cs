﻿using Coralite.Core;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
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
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 3));
            Item.SetWeaponValues(214, 4, 4);
            Item.useTime = Item.useAnimation = 40;
            Item.mana = 18;

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
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as LandOfTheLustrousProj).StartAttack();
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
            SpawnParticleOnTooltipNormaly(line, Main.DiscoColor, Color.White);
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
                //.AddIngredient<SapphireHairpin>()
                .AddIngredient<TopazMirror>()
                //.AddIngredient<TopazMirror>()
                .AddTile<PhantomCrystalBallTile>()
                .Register();
        }
    }

    public class LandOfTheLustrousProj : BaseGemWeaponProj<LandOfTheLustrous>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "LandOfTheLustrous";

        public ref float ShootAngle => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 5);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, PearlProj.brightC, PearlProj.darkC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.8f));
        }

        public override void Move()
        {
            Vector2 idlePos = Owner.Center + new Vector2(0, -32);

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.8f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.8f);
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                Projectile.rotation = MathF.Sin((1 - AttackTime / Owner.itemTimeMax) * MathHelper.TwoPi) * 0.3f;
                if ((int)AttackTime == 1)
                {
                    ShootAngle++;
                    float factor = MathF.Sin(ShootAngle * 0.7f);
                    float angle = factor * 0.6f - 1.57f;
                    float speed = 7f + Math.Abs(factor) * 7f;

                    Projectile.NewProjectileFromThis<PearlProj>(Projectile.Center
                        , angle.ToRotationVector2() * speed, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, Main.rand.Next(4));

                    Helper.PlayPitched("Crystal/CrystalShoot", 0.1f, 0.4f, Projectile.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        PearlProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                    }
                }

                AttackTime--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    Main.hslToRgb(Main.GlobalTimeWrappedHourly + i * 0.1f, 0.9f, 0.9f) * (0.5f - i * 0.5f / 5), Projectile.oldRot[i], mainTex.Size() / 2, Projectile.scale, 0, 0);

            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    public class LustrousProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float TextureType => ref Projectile.ai[0];
        public bool Shiny => Projectile.ai[1] == 1;
        public ref float OwnerIndex => ref Projectile.ai[2];
        public ref float State => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float Target => ref Projectile.localAI[2];

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        private Trail trail;

        public static Asset<Texture2D> SmallPinkDiamond;
        public static Asset<Texture2D> LaserTex;

        public static int FlyTime = 30;

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
            //锆石,
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

        }

        public override bool? CanDamage() => State > 0;

        public override void AI()
        {
            if (trail == null && Shiny)
            {
                Target = -1;
                const int maxPoint = 14;
                trail ??= new Trail(Main.graphics.GraphicsDevice, maxPoint, new NoTip()
                    , factor => Helper.Lerp(2, 8, factor),
                      factor =>
                      {
                          return Color.Lerp(new Color(0, 0, 0, 0), Color.White * 0.65f, factor.X);
                      });

                Projectile.InitOldPosCache(maxPoint);
            }

            switch (State)
            {
                default:
                case 0:
                    {
                        if (!OwnerIndex.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                            return;

                        //从中心向外
                        Projectile.Center = owner.Center + (Timer + 2) * Projectile.velocity;

                        Timer++;
                        Projectile.velocity *= 0.99f;

                        if (Timer > FlyTime)
                        {
                            if (Shiny)
                            {
                                State++;
                                Projectile.velocity = (Projectile.Center - owner.Center).SafeNormalize(Vector2.Zero) * 8;
                                Projectile.extraUpdates = 3;
                                Projectile.timeLeft = 600 * Projectile.extraUpdates;
                                Projectile.tileCollide = true;
                            }
                            else
                            {
                                State = 2;
                                Projectile.Center = Main.MouseWorld + new Vector2(Main.rand.Next(-500, 500), -1200);
                                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 16;
                                Projectile.extraUpdates = 2;
                                Projectile.timeLeft = 100 * Projectile.MaxUpdates;

                                //生成粒子
                            }
                            Timer = 0;
                        }
                    }
                    break;
                case 1://追踪鼠标位置
                    {
                        Timer++;
                        if (Target > 0 && Timer % 20 == 0)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 1200, n => n.CanBeChasedBy(), out NPC t))
                            {
                                Target = t.whoAmI;
                                Projectile.timeLeft = 600;
                            }
                        }

                        Vector2 targetCenter = Main.MouseWorld;

                        if (Target.GetNPCOwner(out NPC target))
                        {
                            targetCenter = Vector2.SmoothStep(targetCenter, target.Center, Math.Clamp(Timer / 600, 0, 1));
                        }

                        float num481 = 12f;
                        Vector2 center = Projectile.Center;
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
                case 2://瞬移并向上发射
                    {

                    }
                    break;
            }

            Projectile.UpdateOldPosCache();
            trail.Positions = Projectile.oldPos;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
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
                    Main.instance.LoadItem(ItemID.PinkPearl);
                    return new GemDrawData(TextureAssets.Item[ItemID.Emerald].Value
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
                        , SapphireProj.highlightC, RubyProj.brightC, RubyProj.darkC)
                    {
                        scale = new Vector2(0.8f),
                        lightRange = 0.1f,
                        lightLimit = 0.35f,
                        addC = 0f,
                    };
                case GemType.Tourmaline:
                    Main.instance.LoadItem((int)TextureType);
                    return new GemDrawData(TextureAssets.Item[(int)TextureType].Value, Color.White, Color.Gray, Color.DarkGray);
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
                default:
                    return new GemDrawData(Projectile.GetTexture()
                        , Color.White, Color.Gray, Color.DarkGray);
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void OnKill(int timeLeft)
        {

        }

        public void DrawPrimitives()
        {
            if (!Shiny || trail == null)
                return;

            Effect effect = Filters.Scene["CrystalTrail"].GetShader().Shader;

            Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

            var data = GetDrawData();

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

            var data = GetDrawData();
            Texture2D mainTex = data.tex;

            if (!Shiny)
            {
                var origin = mainTex.Size() / 2;
                if (Projectile.oldPos.Length > 10)
                    for (int i = 0; i < 14; i++)
                    {
                        Color c = data.brightC;
                        c.A = (byte)((0.5f - i * 0.5f / 14) * 255);
                        spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                            c, Projectile.oldRot[i], origin, Projectile.scale * (1 - i * 1 / 14f), 0, 0);
                    }
            }
            else if (State == 0)
            {
                float factor = Timer / FlyTime;

                Helper.DrawPrettyStarSparkle(1, 0, Projectile.Center - Main.screenPosition, data.highlightC, data.brightC,
                    factor, 0, 0.4f, 0.6f, 0, Projectile.rotation, new Vector2(2, 1), Vector2.One);
            }

            spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation,
                mainTex.Size() / 2, Projectile.scale, 0, 0);
        }
    }
}