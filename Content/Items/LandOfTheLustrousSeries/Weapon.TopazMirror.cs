using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Utilities;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public class TopazMirror : BaseGemWeapon
    {
        public override void SetDefs()
        {
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Cyan9, Item.sellPrice(0, 20));
            Item.SetWeaponValues(145, 4, 4);
            Item.useTime = Item.useAnimation = 26;
            Item.mana = 19;

            Item.shoot = ModContent.ProjectileType<TopazMirrorProj>();
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
                    (proj.ModProjectile as TopazMirrorProj).StartAttack();
            }

            return false;
        }

        public override void DrawGemName(DrawableTooltipLine line)
        {
            DrawGemNameNormally(line, effect =>
            {
                effect.Parameters["scale"].SetValue(new Vector2(0.4f) / Main.GameZoomTarget);
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.015f);
                effect.Parameters["lightRange"].SetValue(0.1f);
                effect.Parameters["lightLimit"].SetValue(0.15f);
                effect.Parameters["addC"].SetValue(0.7f);
                effect.Parameters["highlightC"].SetValue(TopazProj.highlightC.ToVector4());
                effect.Parameters["brightC"].SetValue(TopazProj.brightC.ToVector4());
                effect.Parameters["darkC"].SetValue(TopazProj.darkC.ToVector4());
            }, 0.2f);
        }

        public override void SpawnParticle(DrawableTooltipLine line)
        {
            SpawnParticleOnTooltipNormaly(line, TopazProj.brightC, TopazProj.highlightC);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Topaz)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.MagicMirror)
                .AddTile<PhantomCrystalBallTile>()
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.Topaz)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.IceMirror)
                .AddTile<PhantomCrystalBallTile>()
                .Register();
        }
    }

    public class TopazMirrorProj : BaseGemWeaponProj<TopazMirror>
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "TopazMirror";

        public int attackType;
        public int recordTime;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void BeforeMove()
        {
            if ((int)Main.timeForVisualEffects % 30 == 0 && Main.rand.NextBool(2))
            {
                float length = Main.rand.NextFloat(24, 32);
                Color c = Main.rand.NextFromList(Color.White, TopazProj.brightC, TopazProj.highlightC);
                var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                     , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
                cs.follow = () => Projectile.position - Projectile.oldPos[1];
                cs.TrailCount = 3;
                cs.fadeTime = Main.rand.Next(40, 70);
                cs.shineRange = 12;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.3f, 0));
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

            if (AttackTime > 0)
            {
                AttackTime--;
            }

            TargetPos = Vector2.Lerp(TargetPos, idlePos, 0.3f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
            Projectile.rotation = Owner.velocity.X / 40;
        }

        public override void StartAttack()
        {
            attackType--;

            if (Main.rand.NextBool(5))
                attackType = -1;

            if (attackType < 0)
            {
                var wr = new WeightedRandom<int>();
                wr.Add((int)TopazProj.AttackType.ShootSword, 1);
                wr.Add((int)TopazProj.AttackType.SpawnSpike, 1.5);
                wr.Add((int)TopazProj.AttackType.Meteor, 2);
                wr.Add((int)TopazProj.AttackType.Boulder, 2.5);
                wr.Add((int)TopazProj.AttackType.Trident, 3);
                attackType = wr.Get();
                //attackType = (int)TopazProj.AttackType.Trident;
            }

            int time = Owner.itemTimeMax;

            Helper.PlayPitched("Crystal/GemShoot", 0.4f, 0f, Projectile.Center);
            Helper.PlayPitched("Crystal/CrystalStrike", 0.2f, -0.6f, Projectile.Center);
            SoundEngine.PlaySound(CoraliteSoundID.MagicStaff_Item8, Projectile.Center);

            switch (attackType)
            {
                default:
                    break;
                case (int)TopazProj.AttackType.ShootSword:
                    time = (int)(time * 1.5f);
                    break;
                case (int)TopazProj.AttackType.Boulder:
                    time = (int)(time * 0.8f);
                    break;
            }

            Projectile.NewProjectileFromThis<TopazProj>(Projectile.Center, Vector2.Zero, 0, Projectile.knockBack
                , Projectile.whoAmI, attackType, time);

            AttackTime = time;
            Owner.itemTime = Owner.itemAnimation = time;
            recordTime = time;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(TopazProj.darkC, 0.3f, 0.3f / 4, 0, 4, 1, 0, -1);
            Projectile.QuickDraw(lightColor, 0);

            if (AttackTime > 0)
            {
                Vector2 pos = Projectile.Center + new Vector2(0, -12) - Main.screenPosition;
                float factor = AttackTime / recordTime;
                Helper.DrawPrettyStarSparkle(1, 0, pos, TopazProj.highlightC, TopazProj.brightC
                    , factor, 0, 0.9f, 0.9f, 1
                    , MathHelper.PiOver4, new Vector2(1.5f), Vector2.One / 3);
                Helper.DrawPrettyStarSparkle(1, 0, pos, TopazProj.highlightC, TopazProj.brightC
                    , factor, 0, 0.9f, 0.9f, 1
                    , MathHelper.PiOver4, new Vector2(2f), Vector2.One / 3);
            }
            return false;
        }
    }

    public class TopazProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float POwner => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];

        public ref float RecodeTime => ref Projectile.localAI[0];

        private float alpha;
        private float scale;
        private float WeaponTexLength;
        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

        public enum AttackType
        {
            /// <summary>
            /// 裂天剑
            /// </summary>
            ShootSword,
            /// <summary>
            /// 血荆棘
            /// </summary>
            SpawnSpike,
            /// <summary>
            /// 陨石法杖
            /// </summary>
            Meteor,
            /// <summary>
            /// 弹力巨石
            /// </summary>
            Boulder,
            /// <summary>
            /// 三叉戟
            /// </summary>
            Trident,
        }

        public static Color highlightC = new(255, 234, 186);
        public static Color brightC = new(255, 132, 0);
        public static Color darkC = new(136, 47, 0);

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            if (!POwner.GetProjectileOwner(out Projectile owner, Projectile.Kill))
                return;

            if (RecodeTime == 0)
                RecodeTime = Timer;

            Projectile.Center = owner.Center;

            Timer--;

            Attack();

            if (Timer < 0)
                Projectile.Kill();
        }

        public void Attack()
        {
            switch ((int)State)
            {
                default:
                    Projectile.Kill(); return;
                case (int)AttackType.ShootSword:
                    ShootSword();
                    break;
                case (int)AttackType.SpawnSpike:
                    SpawnSpike();
                    break;
                case (int)AttackType.Meteor:
                    Meteor();
                    break;
                case (int)AttackType.Boulder:
                    Boulder();
                    break;
                case (int)AttackType.Trident:
                    Trident();
                    break;
            }

            float factor = 1 - Timer / RecodeTime;

            do
            {
                if (factor < 0.2f)
                {
                    float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor / 0.2f);
                    alpha = Helper.Lerp(0, 1, sqrtFactor);
                    WeaponTexLength = Helper.Lerp(0, 80, sqrtFactor);
                    scale = Helper.Lerp(0, 0.3f, sqrtFactor);
                    break;
                }

                float factor2 = (factor - 0.2f) / 0.7f;
                alpha = Helper.Lerp(1, 0, factor2);
                WeaponTexLength = Helper.Lerp(80, 96, factor2);
            } while (false);

            Projectile.rotation += 0.01f;
        }

        public void ShootSword()
        {
            if ((int)Timer % (Owner.itemTimeMax * 3 / 8) == 0)
            {
                int count = (int)Timer / (Owner.itemTimeMax * 3 / 8);
                Vector2 pos = Projectile.Center + (count * MathHelper.PiOver2).ToRotationVector2() * 48;
                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
                int index = Projectile.NewProjectileFromThis<TopazFlySword>(pos, dir * 15
                     , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);
                Main.projectile[index].rotation = dir.ToRotation() + 0.785f;
            }
        }

        public void SpawnSpike()
        {
            if ((int)Timer % (Owner.itemTimeMax / 4) == 0)
            {
                Vector2 pos = Main.MouseWorld;

                bool hasTile = false;
                for (int i = 0; i < 30; i++)
                {
                    Point pos2 = Main.MouseWorld.ToTileCoordinates() + new Point(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                    if (Framing.GetTileSafely(pos2).HasUnactuatedTile)
                    {
                        hasTile = true;
                        pos = pos2.ToWorldCoordinates();
                        break;
                    }
                }

                if (!hasTile)
                {
                    pos += Helper.NextVec2Dir(100, 180);
                }

                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
                int index = Projectile.NewProjectileFromThis<TopazSpike>(pos, dir
                      , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, 0, Main.rand.NextFloat() * 0.5f + 0.6f);
                Main.projectile[index].rotation = dir.ToRotation();
                Main.projectile[index].scale = 0.01f;
            }
        }

        public void Meteor()
        {
            if ((int)Timer % (Owner.itemTimeMax * 3 / 4) == 0)
            {
                Vector2 pos = Main.MouseWorld + new Vector2(Main.rand.Next(-500, 500), -1200);
                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
                int index = Projectile.NewProjectileFromThis<TopazMeteor>(pos, dir * 10
                     , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);
                Main.projectile[index].rotation = dir.ToRotation();
            }
        }

        public void Boulder()
        {
            if ((int)Timer == 1)
            {
                Vector2 pos = Projectile.Center;
                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
                int index = Projectile.NewProjectileFromThis<TopazBoulder>(pos, dir * 10
                     , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);
                Main.projectile[index].rotation = dir.ToRotation();
            }
        }

        public void Trident()
        {
            if ((int)Timer == 1)
            {
                Vector2 pos = Projectile.Center;
                Vector2 dir = (Main.MouseWorld - pos).SafeNormalize(Vector2.Zero);
                int index = Projectile.NewProjectileFromThis<TopazTrident>(pos, dir * 12
                     , Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack);
                Main.projectile[index].rotation = dir.ToRotation() + 0.785f;
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

        public Texture2D GetTexture(out float extraRot)
        {
            extraRot = 0;
            switch ((int)State)
            {
                default:
                    return Projectile.GetTexture();
                case (int)AttackType.ShootSword:
                    extraRot = -0.785f;
                    Main.instance.LoadItem(ItemID.SkyFracture);
                    return TextureAssets.Item[ItemID.SkyFracture].Value;
                case (int)AttackType.SpawnSpike:
                    Main.instance.LoadItem(ItemID.SharpTears);
                    return TextureAssets.Item[ItemID.SharpTears].Value;
                case (int)AttackType.Meteor:
                    extraRot = -0.785f;
                    Main.instance.LoadItem(ItemID.MeteorStaff);
                    return TextureAssets.Item[ItemID.MeteorStaff].Value;
                case (int)AttackType.Boulder:
                    extraRot = -0.785f;
                    Main.instance.LoadItem(ItemID.StaffofEarth);
                    return TextureAssets.Item[ItemID.StaffofEarth].Value;
                case (int)AttackType.Trident:
                    extraRot = -0.785f;
                    Main.instance.LoadItem(ItemID.UnholyTrident);
                    return TextureAssets.Item[ItemID.UnholyTrident].Value;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = GetTexture(out float exRot);
            var weaponOrigin = tex.Size() / 2;
            Texture2D circleTex = TextureAssets.Projectile[ProjectileID.CultistRitual].Value;
            var circleOrigin = circleTex.Size() / 2;
            Texture2D circleTex2 = TextureAssets.Extra[34].Value;
            var circleOrigin2 = circleTex2.Size() / 2;

            rand.Y += 0.2f;

            Color c = Color.White * alpha;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , highlightC, brightC, darkC, () =>
                {
                    Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(0, -WeaponTexLength), null,
                         c, exRot, weaponOrigin, 1, 0, 0);
                    Main.spriteBatch.Draw(circleTex, Projectile.Center, null,
                         c * 0.5f, Projectile.rotation, circleOrigin, scale, 0, 0);
                    Main.spriteBatch.Draw(circleTex2, Projectile.Center, null,
                         c * 0.2f, Projectile.rotation + Main.GlobalTimeWrappedHourly, circleOrigin2, scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);

            return false;
        }
    }

    public abstract class BaseTopazProj : ModProjectile
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);
    }

    public class TopazFlySword : BaseTopazProj
    {
        public ref float Timer => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 4);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 30;

            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }

        #region AI

        public override void AI()
        {
            Default();

            if (Projectile.timeLeft % 5 == 0)
                TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(7))
                Projectile.SpawnTrailDust(8f, DustID.YellowTorch, Main.rand.NextFloat(0.2f, 0.4f));

            Timer++;
        }

        protected void Default()
        {
            if (Timer == 2)
            {
                Projectile.hide = false;
                Projectile.penetrate = 1;

                SpawnFractureDust(3f, 1f);

                Projectile.netUpdate = true;
                Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;  //pi / 4
                return;
            }

            if (Timer == 7)
            {
                SpawnFractureDust(1.6f, 1f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
        }

        protected void SpawnFractureDust(float widthDarker, float widthLighter)
        {
            float r = Projectile.velocity.ToRotation();
            for (int i = 0; i < 12; i++)
            {
                r += MathHelper.TwoPi / 12;
                Vector2 dir = r.ToRotationVector2() * Helper.EllipticalEase(2.1f + MathHelper.TwoPi / 12 * i, 1f, 3f) * 0.5f;

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, dir * widthDarker, 0, default, 1.8f);
                dust.noGravity = true;
            }
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2;

            Vector2 toCenter = Projectile.Size / 2;
            for (int i = 0; i < 4; i++)     //这里是绘制类似于影子拖尾的东西，简单讲就是随机位置画几个透明度低的自己
            {
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, null,
                      TopazProj.highlightC * (0.5f - i * 0.5f / 4), Projectile.oldRot[i], origin, Projectile.scale + i * 0.2f, SpriteEffects.None, 0);
            }

            rand -= Projectile.velocity / 20;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Main.spriteBatch.Draw(Projectile.GetTexture(), Projectile.Center, null,
                         Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    TopazProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }

            if (Projectile.ai[0] != 1)
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }
    }

    public class TopazSpike : BaseTopazProj
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 30;

            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Opacity = 0;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint16 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 200f * Projectile.scale, 22f * Projectile.scale, ref collisionPoint16))
                return true;

            return false;
        }

        public override void AI()
        {
            int maxValue = 3;

            Projectile.ai[0] += 1f;
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.frame = Main.rand.Next(maxValue);
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = Projectile.velocity * MathHelper.Lerp(0.2f, 0.7f, Main.rand.NextFloat());
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.YellowTorch, velocity);
                    dust.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust.noGravity = true;

                    TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), velocity);
                }

                for (int j = 0; j < 6; j++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), DustID.YellowTorch, Main.rand.NextVector2Circular(2f, 2f) + Projectile.velocity * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust2.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust2.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust2.fadeIn = 1f;
                    dust2.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            }

            if (Projectile.ai[0] < 20)
            {
                Projectile.Opacity += 0.1f;
                Projectile.scale = Projectile.Opacity * Projectile.ai[1];
                for (int k = 0; k < 2; k++)
                {
                    Dust dust3 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), DustID.YellowTorch, Projectile.velocity * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat()));
                    dust3.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust3.velocity *= 0.5f;
                    dust3.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust3.noGravity = true;
                }
            }

            if (Projectile.ai[0] >= 30)
            {
                Projectile.Opacity -= 0.2f;
                for (int l = 0; l < 2; l++)
                {
                    Vector2 velocity = Projectile.velocity * MathHelper.Lerp(0.2f, 0.5f, Main.rand.NextFloat());

                    Dust dust4 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), DustID.YellowTorch, velocity);
                    dust4.velocity += Main.rand.NextVector2Circular(0.5f, 0.5f);
                    dust4.velocity *= 0.5f;
                    dust4.scale = 0.8f + Main.rand.NextFloat() * 0.5f;
                    dust4.noGravity = true;

                    TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(24f, 24f), velocity);
                }
            }

            if (Projectile.ai[0] >= 35)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.1f, 0.1f) * Projectile.scale);
        }

        public override void OnKill(int timeLeft)
        {
            for (float i = 0f; i < 1f; i += 0.075f)
            {
                Dust dust8 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f) * Projectile.scale + Projectile.velocity.SafeNormalize(Vector2.UnitY) * i * 200f * Projectile.scale, DustID.OrangeTorch, Main.rand.NextVector2Circular(3f, 3f));
                dust8.velocity.Y += -0.3f;
                dust8.velocity += Projectile.velocity * 0.2f;
                dust8.scale = 1f;
                dust8.alpha = 100;
                dust8.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle value24 = mainTex.Frame(1, 3, 0, Projectile.frame);
            Vector2 origin11 = new(16f, value24.Height / 2);
            Color alpha4 = lightColor;
            Vector2 scale2 = new(Projectile.scale);
            float lerpValue4 = Utils.GetLerpValue(35f, 35f - 5f, Projectile.ai[0], clamped: true);
            scale2.Y *= lerpValue4;
            Vector4 vector37 = lightColor.ToVector4();
            Vector4 vector38 = new Color(67, 17, 17).ToVector4();
            vector38 *= vector37;

            rand.Y += 0.2f;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Main.EntitySpriteDraw(mainTex, Projectile.Center + new Vector2(0f, Projectile.gfxOffY)
                        , value24, alpha4, Projectile.rotation, origin11, scale2, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);
            return false;
        }
    }

    public class TopazMeteor : BaseTopazProj
    {
        public ref float CanExplosion => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 30;

            Projectile.timeLeft = 800;
            Projectile.extraUpdates = 3;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (CanExplosion != 0)
                return;

            Projectile.rotation += 0.2f;

            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 normal = dir.RotatedBy(1.57f);

            float factor = MathF.Tan(MathF.Sin((float)Main.timeForVisualEffects / 3));

            for (int i = -1; i < 2; i += 2)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + normal * i * factor * 20, DustID.Firework_Yellow, dir.RotatedBy(i * 0.2f) * -6, Scale: Main.rand.NextFloat(0.2f, 0.6f));
                d.noGravity = true;
            }

            if (Projectile.timeLeft % 8 == 0)
                TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(8))
                Projectile.SpawnTrailDust(8f, DustID.OrangeTorch, Main.rand.NextFloat(0.2f, 0.4f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CanExplosion == 0)
            {
                CanExplosion = 1;

                Projectile.velocity *= 0;
                Projectile.timeLeft = 2;
                Projectile.Resize(120, 120);
                Projectile.penetrate = -1;
                Projectile.StartAttack();
                Projectile.velocity *= 0;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dir2 = Helper.NextVec2Dir();
                    TopazProj.SpawnTriangleParticle(Projectile.Center + dir2 * Main.rand.NextFloat(6, 12), dir2 * Main.rand.NextFloat(0.5f, 3f));
                }

            if (!VisualEffectSystem.HitEffect_Dusts)
                return;

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.OrangeTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 8),
                    Scale: Main.rand.NextFloat(1f, 2.5f));
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                     DustID.YellowTorch, Helper.NextVec2Dir() * Main.rand.NextFloat(2, 4),
                     Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = true;
            }

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.YellowTorch, dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1, 5),
                    Scale: Main.rand.NextFloat(1.5f, 2f));
                d.noGravity = true;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                        DustID.Firework_Yellow, Helper.NextVec2Dir(1 + i * 2, 2 + i * 2),
                        Scale: Main.rand.NextFloat(0.3f, 0.7f));
                    d.noGravity = true;
                }
            }

            SoundEngine.PlaySound(CoraliteSoundID.MeteorImpact_Item89, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(TopazProj.brightC, 0.4f, 0.4f / 8, 1, 8, 1);

            rand -= Projectile.velocity / 20;

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Texture2D mainTex = Projectile.GetTexture();

                    Main.spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation,
                        mainTex.Size() / 2, Projectile.scale, 0, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);

            return false;
        }
    }

    public class TopazBoulder : BaseTopazProj
    {
        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
            AIType = ProjectileID.BoulderStaffOfEarth;

            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 30;

            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 3;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void PostAI()
        {
            if (Projectile.timeLeft % 4 == 0)
                TopazProj.SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
            if (Main.rand.NextBool(4))
                Projectile.SpawnTrailDust(8f, DustID.OrangeTorch, Main.rand.NextFloat(0.2f, 0.4f));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.9f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.3f;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    TopazProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }

            SoundEngine.PlaySound(CoraliteSoundID.StoneBurst_Item70, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rand -= Projectile.velocity / 20;

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2;

            Projectile.DrawShadowTrails(TopazProj.brightC, 0.4f, 0.4f / 12, 1, 12, 1);

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.Center, null,
                         Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);
            return false;
        }
    }

    public class TopazTrident : BaseTopazProj
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = Projectile.height = 30;

            Projectile.extraUpdates = 8;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;

            Projectile.penetrate = 6;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 0.785f;
            Projectile.SpawnTrailDust(8f, DustID.Firework_Yellow, Main.rand.NextFloat(0.2f, 0.4f), Scale: Main.rand.NextFloat(0.1f, 0.6f));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowTorch, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

            if (VisualEffectSystem.HitEffect_SpecialParticles)
                for (int i = 0; i < 3; i++)
                {
                    Vector2 dir = Helper.NextVec2Dir();
                    TopazProj.SpawnTriangleParticle(Projectile.Center + dir * Main.rand.NextFloat(6, 12), dir * Main.rand.NextFloat(1f, 3f));
                }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
            {
                Vector2 dir2 = (Projectile.rotation - 0.785f).ToRotationVector2();

                for (int i = 0; i < 12; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center + dir2 * i * 8, DustID.Firework_Yellow
                        , dir2 * 4, Scale: 1f - i * 0.9f / 12);
                    d.noGravity = true;
                }

                for (int i = 1; i < 3; i++)
                    for (int j = -1; j < 2; j += 2)
                    {
                        Vector2 v = dir2.RotatedBy(j * i * 0.6f);
                        for (int k = 0; k < 8; k++)
                        {
                            Dust d = Dust.NewDustPerfect(Projectile.Center + v * (3 - i) * k * 4, DustID.Firework_Yellow
                                 , v * 3, Scale: 0.8f - k * 0.7f / 8);
                            d.noGravity = true;
                        }

                    }
            }

            SoundEngine.PlaySound(CoraliteSoundID.Slash_Item71, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            rand -= Projectile.velocity / 20;

            Texture2D mainTex = Projectile.GetTexture();
            Vector2 origin = mainTex.Size() / 2;

            Projectile.DrawShadowTrails(TopazProj.brightC, 0.4f, 0.4f / 12, 1, 12, 1);

            Helper.DrawCrystal(Main.spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
                , (float)Main.timeForVisualEffects * 0.02f
                , TopazProj.highlightC, TopazProj.brightC, TopazProj.darkC, () =>
                {
                    Main.spriteBatch.Draw(mainTex, Projectile.Center, null,
                         Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }, sb =>
                {
                    sb.End();
                    sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }, 0.1f, 0.15f, 0.7f);

            return false;
        }
    }
}
