using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Helpers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class RubyScepter : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 9));
            Item.SetWeaponValues(55, 4);
            Item.useTime = Item.useAnimation = 24;
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
                effect.Parameters["scale"].SetValue(new Vector2(0.6f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.15f);
                effect.Parameters["lightLimit"].SetValue(0.25f);
                effect.Parameters["addC"].SetValue(0.55f);
                effect.Parameters["highlightC"].SetValue(RubyProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(RubyProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(RubyProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, RubyProj.brightC, RubyProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.Emerald)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    public class RubyScepterProj : BaseGemWeaponProj<RubyScepter>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "RubyScepter";

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
                Projectile.rotation = Helper.Lerp(0, MathHelper.Pi, Coralite.Instance.SqrtSmoother.Smoother(1 - AttackTime / Owner.itemTimeMax));

                if (AttackTime == 1 && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectileFromThis<ZumurudProj>(Projectile.Center,
                        (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 9.5f, Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);

                    Helper.PlayPitched("Crystal/CrystalBling", 0.4f, -0.35f, Projectile.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir = Helper.NextVec2Dir();
                        ZumurudProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
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

    public class RubyLaser
    {

    }

    public class RubyProj:ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems+ "EquilateralHexagonProj1";

        public static Color highlightC = new Color(255, 164, 163);
        public static Color brightC = new Color(238, 51, 53);
        public static Color darkC = new Color(73, 10, 0);

    }
}
