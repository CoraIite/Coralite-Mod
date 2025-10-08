using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class TourmalineMonoclastic : BaseGemWeapon
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ToolTipDamageMultiplier[Type] = 0.6f;
        }

        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.StrongRed10, Item.sellPrice(0, 18));
            Item.SetWeaponValues(175, 4, 6);
            Item.useTime = Item.useAnimation = 35;
            Item.mana = 12;

            Item.shoot = ModContent.ProjectileType<TourmalineMonoclasticProj>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 1)
                Projectile.NewProjectile(source, position, Vector2.Zero, type, 0, knockback, player.whoAmI);
            else
            {
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == type))
                    (proj.ModProjectile as TourmalineMonoclasticProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.4f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.55f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(TourmalineProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TourmalineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(120, 16, 40).ToVector4());
            }, 0.4f,
            effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(1.2f / Main.GameZoomTarget));
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.55f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(TourmalineProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TourmalineProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(TourmalineProj.darkC.ToVector4());
            }, extraSize: new Point(45, 4));
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, TourmalineProj.brightC, TourmalineProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tourmaline>()
                .AddIngredient(ItemID.FragmentNebula, 5)
                .AddIngredient(ItemID.Lens)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class TourmalineMonoclasticProj : BaseGemWeaponProj<TourmalineMonoclastic>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override bool CanFire => AttackTime > 0;

        public Vector2 scale = Vector2.One;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(16, 32);
                Color c = Main.rand.NextFromList(Color.White, TourmalineProj.brightC, TourmalineProj.highlightC);
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
            Vector2 idlePos = Owner.Center;
            for (int i = 0; i < 16; i++)//检测头顶2个方块并尝试找到没有物块阻挡的那个
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
                Vector2 dir = InMousePos - Projectile.Center;
                if (dir.Length() < 48)
                    idlePos += dir;
                else
                    idlePos += dir.SafeNormalize(Vector2.Zero) * 48;
                Projectile.netUpdate = true;
            }

            TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Projectile.rotation.AngleLerp(Owner.velocity.X / 20, 0.2f);
            Lighting.AddLight(Projectile.Center, TourmalineProj.brightC.ToVector3());
        }

        public override void Attack()
        {
            if (AttackTime > 0)
            {
                scale.Y = Helper.Lerp(0.2f, 1f, Helper.SqrtEase(1 - (AttackTime / Owner.itemTimeMax)));
                AttackTime--;
            }
        }

        public override void StartAttack()
        {
            AttackTime = Owner.itemTimeMax;

            List<NPC> targets = Main.npc.Where(n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n) && Vector2.DistanceSquared(Projectile.Center, n.Center) < 1000 * 1000).ToList();

            targets.Sort((t1, t2) => t1.DistanceSQ(Projectile.Center).CompareTo(t2.DistanceSQ(Projectile.Center)));

            int howMany = targets.Count;
            if (howMany < 1)
                return;

            if (howMany > 5)
                howMany = 5;

            int damage = Owner.GetWeaponDamage(Item);
            damage = (int)Helper.Lerp(damage * 1.15f, damage * 0.6f, (howMany - 1) / 4f);

            for (int i = 0; i < howMany; i++)
            {
                Projectile.NewProjectileFromThis<TourmalineProj>(Projectile.Center, Vector2.Zero, damage, Projectile.knockBack
                    , Projectile.whoAmI, targets[i].whoAmI, AttackTime);
            }

            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CrimsonTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                TourmalineProj.SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
            }

            Helper.PlayPitched("Crystal/CrystalShoot", 0.4f, 0, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var origin = mainTex.Size() / 2;
            Vector2 toCenter = new(Projectile.width / 2, Projectile.height / 2);

            for (int i = 0; i < 4; i++)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                    new Color(251, 100, 152) * (0.3f - (i * 0.3f / 4)), Projectile.oldRot[i] + 0, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation,
                origin, Projectile.scale * scale, 0, 0);
            return false;
        }
    }

    public class TourmalineProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public static Color highlightC = Color.White;
        public static Color brightC = new(255, 83, 113);
        public static Color darkC = new(75, 7, 28);

        public ref float Owner => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float RecordTimer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 22;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (!Owner.GetProjectileOwner(out Projectile owner, Projectile.Kill)
                || !Target.GetNPCOwner(out NPC target, Projectile.Kill))
                return;

            Projectile.Center = owner.Center;
            Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
            //追踪一小段时间后生成斩击弹幕

            if (Timer < 0)
            {
                //生成斩击弹幕
                Projectile.NewProjectileFromThis<TourmalineSlash>(target.Center + (Vector2.UnitY * ((10 * 15) + (20 * Main.rand.Next(-4, 4)))), -Vector2.UnitY * 15,
                    Projectile.damage, Projectile.knockBack);

                Projectile.Kill();
            }

            float factor = 1 - (Timer / RecordTimer);
            float length = Vector2.Distance(owner.Center, target.Center);
            Dust d = Dust.NewDustPerfect(Vector2.Lerp(owner.Center, target.Center, Main.rand.NextFloat(factor / 5, factor / 2))
                , DustID.CrimsonTorch, (target.Center - owner.Center).SafeNormalize(Vector2.Zero).RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2, 4)
                , Scale: Main.rand.NextFloat(1, 1.5f));
            d.noGravity = true;

            if (RecordTimer == 0)
            {
                RecordTimer = Timer;
            }

            Timer--;
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
            if (!Owner.GetProjectileOwner(out Projectile owner)
                || !Target.GetNPCOwner(out NPC target))
                return false;

            Vector2 dir = target.Center - owner.Center;
            float distance = dir.Length();

            Texture2D mainTex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            var origin = new Vector2(mainTex.Width / 2, mainTex.Height);
            var pos = Projectile.Center - Main.screenPosition;

            distance = Math.Clamp(distance, 0, 350);
            float factor = distance / mainTex.Width;
            float factor2 = 1 - (Timer / RecordTimer);

            Vector2 scale = new Vector2(0.1f * factor, factor) * Helper.SqrtEase(factor2);

            Color c1 = darkC * 0.6f;
            c1.A = 0;
            Color c2 = highlightC * 0.6f;
            c1.A = 0;
            Color c3 = brightC * 0.2f;
            c1.A = 0;

            Main.spriteBatch.Draw(mainTex, pos, null, c3 * factor2, Projectile.rotation + 1.57f, origin, new Vector2(0.4f * factor, factor), 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, c1, Projectile.rotation + 1.57f, origin, scale * 1.2f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, brightC, Projectile.rotation + 1.57f, origin, scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, c2, Projectile.rotation + 1.57f, origin, scale * 0.8f, 0, 0);

            return false;
        }
    }

    public class TourmalineSlash : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 24;
            Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            int num23 = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(8, 8), 0, 0, DustID.RainbowTorch, 0f, 0f, 0, TourmalineProj.brightC);
            Main.dust[num23].velocity = -Vector2.UnitY;
            Main.dust[num23].noGravity = true;
            Main.dust[num23].scale = Main.rand.NextFloat(0.5f, 1.5f);

            if (Timer > 20)
            {
                Projectile.Kill();
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float factor = Timer / 20;

            Vector2 fatness = new(0.5f + (factor * 1.5f), 0.8f + (MathF.Sin(factor * MathHelper.Pi) * 0.2f));
            float scale = 2f + (8f * factor);
            var pos = Projectile.Center - Main.screenPosition;
            //两侧
            Helper.DrawPrettyLine(1, 0, pos, TourmalineProj.brightC, TourmalineProj.darkC,
                factor, 0, 0.4f, 0.6f, 1, (MathHelper.Pi / 6) + MathHelper.PiOver2, 4.5f, fatness);
            Helper.DrawPrettyLine(1, 0, pos, TourmalineProj.brightC, TourmalineProj.darkC,
                factor, 0, 0.4f, 0.6f, 1, (-MathHelper.Pi / 6) + MathHelper.PiOver2, 4.5f, fatness);
            //竖直
            Helper.DrawPrettyLine(1, 0, pos, TourmalineProj.brightC, TourmalineProj.darkC,
                factor, 0, 0.4f, 0.6f, 1, MathHelper.PiOver2, scale, fatness);
            Helper.DrawPrettyLine(1, 0, pos, TourmalineProj.highlightC, TourmalineProj.brightC,
                factor, 0, 0.4f, 0.6f, 1, MathHelper.PiOver2, scale, fatness);
            Helper.DrawPrettyLine(1, 0, pos, TourmalineProj.highlightC, TourmalineProj.brightC,
                factor, 0, 0.4f, 0.6f, 1, MathHelper.PiOver2, scale, fatness);

            return false;
        }
    }
}
