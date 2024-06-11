using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
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
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class SapphireHairpin : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.LightPurple6, Item.sellPrice(0, 13));
            Item.SetWeaponValues(80, 4);
            Item.useTime = Item.useAnimation = 28;
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
                effect.Parameters["scale"].SetValue(new Vector2(0.8f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(SapphireProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(new Color(80, 150, 255).ToVector4());
                effect.Parameters["darkC"].SetValue(new Color(0, 0, 255).ToVector4());
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
            Projectile.rotation += Owner.velocity.X / 40;
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

                Projectile.rotation += Owner.direction * 0.2f * (1 - AttackTime / Owner.itemTimeMax);

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

    public class SapphireProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        public static Color highlightC = Color.White;
        public static Color brightC = new Color(54, 190, 254);
        public static Color darkC = new Color(0, 0, 121);

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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
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
                        for (int i = 0; i < 4; i++)
                        {
                            float factor = MathF.Sin(Timer * 0.2f + i * MathHelper.PiOver2);
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Firework_Blue, -dir.RotatedBy(factor) * Main.rand.NextFloat(3.5f, 4f));
                            d.noGravity = true;
                        }

                        if (Timer > 50 * Projectile.MaxUpdates)
                        {
                            State = 1;
                            Timer = 0;
                        }

                        if (Projectile.timeLeft % 10 == 0)
                            SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
                        if (Main.rand.NextBool(10))
                            Projectile.SpawnTrailDust(8f, DustID.BlueTorch, Main.rand.NextFloat(0.2f, 0.4f));

                        Timer++;
                    }
                    break;
                case 1://查找目标
                    {
                        Timer++;
                        Projectile.velocity *= 0.99f;

                        if (Timer>30&&Timer % 10 == 0 && Helper.TryFindClosestEnemy(Projectile.Center, 1200, n => n.CanBeChasedBy(), out NPC target))
                        {
                            Target = target.whoAmI;
                            State = 2;
                            Timer = 0;
                            return;
                        }

                        if (Timer > 60 * Projectile.MaxUpdates)
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
                            Projectile.velocity *= 0.5f;
                            Projectile.timeLeft = 300 * 3;
                            Timer = 0;

                            //生成彗星
                        }
                    }
                    break;
                case 3://引导落星
                    {

                    }
                    break;
            }
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
            Helper.DrawPrettyStarSparkle(1, 0, pos, darkC, brightC
                , MathF.Sin((int)Main.timeForVisualEffects * 0.4f) / 2 + 0.5f, 0, 0.4f, 0.6f, 1
                , 0, new Vector2(7f, 3f), Vector2.One / 3);

            if (State < 2)//绘制发光拖尾
            {
                Texture2D exTex = TextureAssets.Extra[98].Value;

                Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);
                var origin = exTex.Size() / 2;

                for (int i = 0; i < 12; i++)
                    Main.spriteBatch.Draw(exTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                        Color.Lerp(brightC, darkC, i / 12f) * (0.4f - i * 0.4f / 12), Projectile.oldRot[i] + 1.57f, origin, 1, 0, 0);

            }
            else
            {

            }

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation, origin1, Projectile.scale, 0, 0);
            return false;
        }
    }

    public class SapphireComet : ModProjectile, IDrawPrimitive, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        private static VertexStrip _vertexStrip = new VertexStrip();
        private Trail trail;

        public ref float Owner => ref Projectile.ai[0];

        public static Asset<Texture2D> TrailTex;

        public override void Load()
        {
            if (!Main.dedServ)
            {

            }
        }

        public override void Unload()
        {

        }

        public override void SetDefaults()
        {
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {

        }

        public override void OnKill(int timeLeft)
        {

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public void Explosion()
        {
            Projectile.StartAttack();
            Projectile.Resize(128, 128);
        }

        private Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(Color.White, Color.Violet, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
            result.A /= 2;
            return result;
        }

        private float StripWidth(float progressOnStrip) => MathHelper.Lerp(26f, 32f, Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true)) * Utils.GetLerpValue(0f, 0.07f, progressOnStrip, clamped: true);

        public override bool PreDraw(ref Color lightColor) => false;


        public void DrawPrimitives()
        {
            if (trail == null)
                return;

            MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
            miscShaderData.UseSaturation(-2.8f);
            miscShaderData.UseOpacity(2f);
            miscShaderData.Apply();
            _vertexStrip.PrepareStripWithProceduralPadding(Projectile.oldPos, Projectile.oldRot, StripColors, StripWidth, -Main.screenPosition);
            _vertexStrip.DrawTrail();
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }
    }

    public class SapphireSmallProj
    {

    }
}
